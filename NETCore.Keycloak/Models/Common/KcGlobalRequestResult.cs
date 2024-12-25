using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Common;

/// <summary>
/// Represents the result of a global request in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_globalrequestresult"/>
/// </summary>
public class KcGlobalRequestResult
{
    /// <summary>
    /// Gets or sets the list of failed request identifiers.
    /// </summary>
    /// <value>
    /// A collection of strings representing the identifiers of failed requests.
    /// </value>
    [JsonProperty("failedRequests")]
    public IEnumerable<string> FailedRequests { get; set; }

    /// <summary>
    /// Gets or sets the list of successful request identifiers.
    /// </summary>
    /// <value>
    /// A collection of strings representing the identifiers of successful requests.
    /// </value>
    [JsonProperty("successRequests")]
    public IEnumerable<string> SuccessRequests { get; set; }
}
