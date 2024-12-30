using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NETCore.Keycloak.Client.HttpClients;

/// <summary>
/// Base class for handling Keycloak HTTP client operations.
/// </summary>
public abstract class KcHttpClientBase
{
    /// <summary>
    /// Logger instance for logging operations.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// The base URL for Keycloak API.
    /// </summary>
    protected string BaseUrl { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcHttpClientBase"/> class.
    /// </summary>
    /// <param name="logger">Logger instance to log information and errors.</param>
    /// <param name="baseUrl">Keycloak base URL. Must not be null or empty.</param>
    /// <exception cref="KcException">Thrown if the <paramref name="baseUrl"/> is null or empty.</exception>
    protected KcHttpClientBase(ILogger logger, string baseUrl)
    {
        if ( string.IsNullOrWhiteSpace(baseUrl) )
        {
            throw new KcException($"{nameof(baseUrl)} is required");
        }

        // Ensure the base URL does not end with a trailing slash.
        BaseUrl = baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? baseUrl.Remove(baseUrl.Length - 1, 1)
            : baseUrl;

        Logger = logger;
    }

    /// <summary>
    /// Validates the realm name and access token for a Keycloak operation.
    /// </summary>
    /// <param name="realm">The Keycloak realm name.</param>
    /// <param name="accessToken">The access token for the realm administrator.</param>
    /// <exception cref="KcException">Thrown if either <paramref name="realm"/> or <paramref name="accessToken"/> is null or empty.</exception>
    protected static void ValidateAccess(string realm, string accessToken)
    {
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new KcException($"{nameof(realm)} is required");
        }

        if ( string.IsNullOrWhiteSpace(accessToken) )
        {
            throw new KcException($"{nameof(accessToken)} is required");
        }
    }

    /// <summary>
    /// Handles the response from a Keycloak HTTP request.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="result">The HTTP response message.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A <see cref="KcResponse{T}"/> object containing the response data or error message.</returns>
    protected static async Task<KcResponse<T>> HandleAsync<T>(
        KcHttpRequestExecutionResult result, CancellationToken cancellationToken = default) =>
        result?.ResponseMessage?.IsSuccessStatusCode != true
            ? new KcResponse<T>
            {
                IsError = true,
                ErrorMessage = result?.ResponseMessage != null
                    ? await result.ResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
                    : null,
                MonitoringMetrics = await KcHttpApiMonitoringMetrics
                    .MapFromHttpRequestExecutionResult(result, cancellationToken)
                    .ConfigureAwait(false)
            }
            : result.ResponseMessage.StatusCode == HttpStatusCode.NoContent
                ? new KcResponse<T>()
                : new KcResponse<T>
                {
                    Response = JsonConvert.DeserializeObject<T>(
                        await result.ResponseMessage.Content.ReadAsStringAsync(cancellationToken)
                            .ConfigureAwait(false)),
                    MonitoringMetrics = await KcHttpApiMonitoringMetrics
                        .MapFromHttpRequestExecutionResult(result, cancellationToken)
                        .ConfigureAwait(false)
                };

    /// <summary>
    /// Executes an HTTP request and captures execution metrics, including fallback data in case of exceptions.
    /// </summary>
    /// <param name="request">The function representing the HTTP request to be executed.</param>
    /// <param name="monitoringFallbackModel">
    /// The monitoring fallback model used to track execution metrics and errors.
    /// <see cref="KcHttpMonitoringFallbackModel"/>
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="KcHttpMonitoringFallbackModel"/>
    /// with the result of the HTTP request and its associated metrics.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="request"/> or <paramref name="monitoringFallbackModel"/> is null.
    /// </exception>
    protected static async Task<KcHttpRequestExecutionResult> ExecuteRequest(
        Func<Task<HttpResponseMessage>> request,
        KcHttpMonitoringFallbackModel monitoringFallbackModel)
    {
        // Ensure the request function is not null
        ArgumentNullException.ThrowIfNull(request);

        // Ensure the monitoring fallback model is not null
        ArgumentNullException.ThrowIfNull(monitoringFallbackModel);

        // Start a timer to measure request execution time
        var timer = Stopwatch.StartNew();

        try
        {
            // Execute the HTTP request
            var response = await request.Invoke().ConfigureAwait(false);

            // Stop the timer after the request completes successfully
            timer.Stop();

            // Record the elapsed time in the monitoring fallback model
            monitoringFallbackModel.RequestMilliseconds = timer.ElapsedMilliseconds;

            // Return the execution result with the captured metrics and response data
            return new KcHttpRequestExecutionResult
            {
                RequestMilliseconds = timer.ElapsedMilliseconds,
                ResponseMessage = response,
                RequestMessage = response.RequestMessage,
                MonitoringFallback = monitoringFallbackModel
            };
        }
        catch ( Exception e )
        {
            // Stop the timer if an exception occurs
            timer.Stop();

            // Record the elapsed time in the monitoring fallback model
            monitoringFallbackModel.RequestMilliseconds = timer.ElapsedMilliseconds;

            // Return the execution result with the exception details and fallback metrics
            return new KcHttpRequestExecutionResult
            {
                RequestMilliseconds = timer.ElapsedMilliseconds,
                Exception = e,
                MonitoringFallback = monitoringFallbackModel
            };
        }
    }

    /// <summary>
    /// Processes an HTTP request and returns a response of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the expected response data.</typeparam>
    /// <param name="url">The URL for the HTTP request.</param>
    /// <param name="method">The HTTP method (e.g., GET, POST, PUT, DELETE) to use for the request.</param>
    /// <param name="accessToken">Optional: The access token to include in the request headers for authentication.</param>
    /// <param name="errorMessage">The error message to log if the request fails.</param>
    /// <param name="content">Optional: The content to include in the request body, if applicable.</param>
    /// <param name="contentType">The content type header for the request. Defaults to "application/json".</param>
    /// <param name="cancellationToken">Optional: A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing the response data or error details.
    /// </returns>
    /// <remarks>
    /// This method handles the execution of an HTTP request, including logging errors and managing exceptions.
    /// It creates the request, sends it using an <see cref="HttpClient"/>, and processes the response.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="url"/> or <paramref name="method"/> is null or empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    protected async Task<KcResponse<T>> ProcessRequestAsync<T>(
        string url,
        HttpMethod method,
        string accessToken,
        string errorMessage,
        object content = null,
        string contentType = "application/json",
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Execute the HTTP request and process the response.
            using var response = await ExecuteRequest(async () =>
            {
                // Create the HTTP request based on the specified method and content.
                using var request = content == null
                    ? CreateRequest(method, url, accessToken,
                        contentType: contentType) // For requests without a body (e.g., GET).
                    : CreateRequest(method, url, accessToken,
                        GetBody(content), contentType: contentType); // For requests with a body (e.g., POST, PUT).

                // Create a new HttpClient instance for sending the request.
                using var client = new HttpClient();

                // Send the request asynchronously and return the response.
                return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = url, // Log the URL for monitoring purposes.
                HttpMethod = method // Log the HTTP method for monitoring purposes.
            }).ConfigureAwait(false);

            // Handle the response and deserialize it to the specified type.
            return await HandleAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            // Log the error details if a logger instance is available.
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, errorMessage, e);
            }

            // Return a response object indicating an error occurred.
            return new KcResponse<T>
            {
                IsError = true, // Mark the response as an error.
                Exception = e // Attach the exception for debugging purposes.
            };
        }
    }

    /// <summary>
    /// Validates that a required string parameter is not null, empty, or whitespace.
    /// </summary>
    /// <param name="paramName">The name of the parameter being validated, used in the exception message.</param>
    /// <param name="value">The value of the string parameter to validate.</param>
    /// <exception cref="KcException">Thrown when the string parameter is null, empty, or contains only whitespace.</exception>
    protected static void ValidateRequiredString(string paramName, string value)
    {
        if ( string.IsNullOrWhiteSpace(value) )
        {
            throw new KcException($"{paramName} is required");
        }
    }

    /// <summary>
    /// Validates that a required object parameter is not null.
    /// </summary>
    /// <param name="paramName">The name of the parameter being validated, used in the exception message.</param>
    /// <param name="value">The object parameter to validate.</param>
    /// <exception cref="KcException">Thrown when the object parameter is null.</exception>
    protected static void ValidateNotNull(string paramName, object value)
    {
        if ( value == null )
        {
            throw new KcException($"{paramName} is required");
        }
    }

    /// <summary>
    /// Creates an HTTP request message for Keycloak operations.
    /// </summary>
    /// <param name="method">The HTTP method (e.g., GET, POST).</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <param name="accessToken">The access token for authorization.</param>
    /// <param name="content">Optional request content.</param>
    /// <param name="contentType">The content type header for the request. Defaults to "application/json".</param>
    /// <returns>An <see cref="HttpRequestMessage"/> configured with the specified parameters.</returns>
    private static HttpRequestMessage CreateRequest(
        HttpMethod method,
        string endpoint,
        string accessToken,
        HttpContent content = null,
        string contentType = "application/json")
    {
        // Initialize the HTTP request message with the provided method, endpoint, and content.
        var request = new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(endpoint),
            Content = content ?? new StringContent(string.Empty) // Use empty content if none is provided.
        };

        // Configure the content type header if the content exists.
        if ( request.Content.Headers.ContentType != null )
        {
            request.Content.Headers.ContentType.MediaType = contentType; // Set the media type for the content.
            request.Content.Headers.ContentType.CharSet = null; // Clear the charset to ensure compatibility.
        }

        // Add the Accept header to indicate the expected response content type.
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

        // Add the Authorization header with the access token if it is not null or empty.
        if ( !string.IsNullOrWhiteSpace(accessToken) )
        {
            _ = request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
        }

        // Return the configured HTTP request message.
        return request;
    }

    /// <summary>
    /// Builds the body for an HTTP request.
    /// </summary>
    /// <param name="o">The object to serialize as the request body.</param>
    /// <returns>A <see cref="StringContent"/> containing the serialized object.</returns>
    private static StringContent GetBody(object o) =>
        new(JsonConvert.SerializeObject(o, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            }
        }));
}
