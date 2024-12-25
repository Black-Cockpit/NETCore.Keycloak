using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Roles;

/// <summary>
/// Represents a mapping of roles in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_mappingsrepresentation"/>
/// </summary>
public class KcRoleMapping
{
    /// <summary>
    /// Gets or sets the client role mappings.
    /// </summary>
    /// <value>
    /// A dictionary where the key is the client name and the value is a <see cref="KcClientRoleMapping"/> representing the client's role mappings.
    /// </value>
    [JsonProperty("clientMappings")]
    public IDictionary<string, KcClientRoleMapping> ClientMappings { get; set; }

    /// <summary>
    /// Gets or sets the realm role mappings.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcRole"/> representing the realm role mappings.
    /// </value>
    [JsonProperty("realmMappings")]
    public IEnumerable<KcRole> RealmMappings { get; set; }
}
