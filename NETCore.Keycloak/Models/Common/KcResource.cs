using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Common;

/// <summary>
/// Represents a resource in Keycloak.
/// </summary>
public class KcResource
{
    /// <summary>
    /// Gets or sets the unique identifier of the resource.
    /// </summary>
    /// <value>
    /// A string representing the resource ID.
    /// </value>
    [JsonProperty("rsid")]
    public string RsId { get; set; }

    /// <summary>
    /// Gets or sets the name of the resource.
    /// </summary>
    /// <value>
    /// A string representing the resource name.
    /// </value>
    [JsonProperty("rsname")]
    public string RsName { get; set; }

    /// <summary>
    /// Gets or sets the scopes associated with the resource.
    /// </summary>
    /// <value>
    /// A collection of strings representing the resource scopes.
    /// </value>
    [JsonProperty("scopes")]
    public IEnumerable<string> Scopes { get; set; }
}
