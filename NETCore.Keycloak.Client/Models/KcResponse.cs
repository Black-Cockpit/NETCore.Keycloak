using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models;

/// <summary>
/// Represents a generic Keycloak request response.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class KcResponse<T>
{
    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    /// <value>
    /// An instance of type <typeparamref name="T"/> containing the response data.
    /// </value>
    public T Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the request resulted in an error.
    /// </summary>
    /// <value>
    /// <c>true</c> if the request failed; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("isError")]
    public bool IsError { get; set; }

    /// <summary>
    /// Gets or sets the exception that occurred during the request, if any.
    /// </summary>
    /// <value>
    /// An <see cref="Exception"/> representing the error that occurred during the request.
    /// </value>
    [JsonProperty("exception")]
    public Exception Exception { get; set; }

    /// <summary>
    /// Gets or sets the error message for the failed request.
    /// </summary>
    /// <value>
    /// A string containing the error message.
    /// </value>
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; }
}
