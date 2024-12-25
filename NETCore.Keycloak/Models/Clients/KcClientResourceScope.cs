using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Clients;

/// <summary>
/// Keycloak scope representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_scoperepresentation"/>
/// </summary>
public class KcClientResourceScope
{
    /// <summary>
    /// A unique name for this scope. The name can be used to uniquely identify a scope, useful when querying for a specific scope.
    /// </summary>
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// A URI pointing to an icon.
    /// </summary>
    [JsonProperty("iconUri")]
    public string IconUri { get; set; }

    /// <summary>
    /// Keycloak resource scope id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// A unique name for this scope. The name can be used to uniquely identify a scope, useful when querying for a specific scope.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// List of scope policies <see cref="KcClientResourcePolicy"/>
    /// </summary>
    [JsonProperty("policies")]
    public IEnumerable<KcClientResourcePolicy> Policies { get; set; }

    /// <summary>
    /// List of scope resources <see cref="KcClientResource"/>
    /// </summary>
    [JsonProperty("resources")]
    public IEnumerable<KcClientResource> Resources { get; set; }
}
