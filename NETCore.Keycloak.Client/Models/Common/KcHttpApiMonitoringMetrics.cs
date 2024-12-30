using System.Net;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents the metrics collected for monitoring HTTP API requests.
/// </summary>
public class KcHttpApiMonitoringMetrics
{
    /// <summary>
    /// Gets or sets the HTTP status code returned by the API.
    /// </summary>
    /// <remarks>
    /// This uses the <see cref="HttpStatusCode"/> enumeration to represent standard HTTP response codes.
    /// </remarks>
    [JsonProperty("statusCode")]
    public HttpStatusCode? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the URI of the HTTP request.
    /// </summary>
    [JsonProperty("url")]
    public Uri Url { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method (e.g., GET, POST) used in the request.
    /// </summary>
    [JsonProperty("httpMethod")]
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the time, in milliseconds, that the request took to complete.
    /// </summary>
    [JsonProperty("requestMilliseconds")]
    public double RequestMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets the error message, if any, associated with the request.
    /// </summary>
    [JsonProperty("error")]
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets the exception, if any, associated with the request.
    /// </summary>
    /// <remarks>
    /// This property stores additional details about any errors that occurred during the request processing.
    /// </remarks>
    [JsonProperty("requestException")]
    public Exception RequestException { get; set; }

    /// <summary>
    /// Maps an execution result of an HTTP request to an instance of <see cref="KcHttpApiMonitoringMetrics"/>.
    /// </summary>
    /// <param name="requestExecutionResult">The result of the HTTP request execution.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// An instance of <see cref="KcHttpApiMonitoringMetrics"/> containing the mapped data.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="requestExecutionResult"/> or its <c>MonitoringFallback</c> property is null.
    /// </exception>
    public static async Task<KcHttpApiMonitoringMetrics> MapFromHttpRequestExecutionResult(
        KcHttpRequestExecutionResult requestExecutionResult, CancellationToken cancellationToken = default)
    {
        // Validate that the request execution result is not null.
        ArgumentNullException.ThrowIfNull(requestExecutionResult);

        // Validate that the monitoring fallback property is not null.
        ArgumentNullException.ThrowIfNull(requestExecutionResult.MonitoringFallback);

        // Determine if a response message is available and map its details to metrics.
        var responseData = requestExecutionResult.ResponseMessage != null
            ? new KcHttpApiMonitoringMetrics
            {
                // Extract the URL from the response message's request.
                Url = requestExecutionResult.ResponseMessage.RequestMessage?.RequestUri,

                // Extract the HTTP method from the response message's request.
                HttpMethod = requestExecutionResult.ResponseMessage.RequestMessage?.Method,

                // Record the request execution time in milliseconds.
                RequestMilliseconds = requestExecutionResult.RequestMilliseconds,

                // Extract the HTTP status code from the response message.
                StatusCode = requestExecutionResult.ResponseMessage.StatusCode,

                // Retrieve the error message if the status code indicates failure.
                Error = !requestExecutionResult.ResponseMessage.IsSuccessStatusCode
                    ? await requestExecutionResult.ResponseMessage.Content
                        .ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
                    : null,

                // Capture any exception that occurred during the request.
                RequestException = requestExecutionResult.Exception
            }
            : new KcHttpApiMonitoringMetrics
            {
                // Handle the fallback case where no response message is available.
                RequestException = requestExecutionResult.Exception,

                // Extract the HTTP method from the fallback model.
                HttpMethod = requestExecutionResult.MonitoringFallback.HttpMethod,

                // Record the request execution time in milliseconds from the fallback model.
                RequestMilliseconds = requestExecutionResult.MonitoringFallback.RequestMilliseconds,

                // Extract the HTTP status code from the fallback model.
                StatusCode = requestExecutionResult.MonitoringFallback.StatusCode,

                // Extract the error from the exception if possible.
                Error = requestExecutionResult.Exception.Message,

                // Validate and extract the URL from the fallback model.
                Url = Uri.TryCreate(requestExecutionResult.MonitoringFallback.Url,
                    UriKind.Absolute,
                    out var uriResult)
                    ? uriResult
                    : null
            };

        // Return the constructed monitoring metrics.
        return responseData;
    }
}
