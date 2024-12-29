using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models;

/// <summary>
/// Represents a base class for Keycloak API responses, encapsulating common properties for handling API results.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public abstract class KcBaseResponse<T>
{
    /// <summary>
    /// Gets or sets the response data returned by the Keycloak API.
    /// </summary>
    /// <value>
    /// An instance of type <typeparamref name="T"/> containing the response data,
    /// or <c>null</c> if the request failed or no data is available.
    /// </value>
    [JsonProperty("response")]
    public T Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the API request resulted in an error.
    /// </summary>
    /// <value>
    /// <c>true</c> if the request failed or encountered an error; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("isError")]
    public bool IsError { get; set; }

    /// <summary>
    /// Gets or sets the exception that occurred during the request execution, if applicable.
    /// </summary>
    /// <value>
    /// An <see cref="Exception"/> instance representing the error that occurred during the request,
    /// or <c>null</c> if no exception was encountered.
    /// </value>
    [JsonProperty("exception")]
    public Exception Exception { get; set; }

    /// <summary>
    /// Gets or sets the error message for the request, if the request failed.
    /// </summary>
    /// <value>
    /// A string containing the error message provided by the Keycloak API or generated during the request execution,
    /// or <c>null</c> if the request was successful.
    /// </value>
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; }
}
