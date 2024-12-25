using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Roles;

/// <summary>
/// Represents a client role mapping in Keycloak.
/// </summary>
public class KcClientRoleMapping
{
    /// <summary>
    /// Gets or sets the unique identifier for the role mapping.
    /// </summary>
    /// <value>
    /// A string representing the role mapping ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the client associated with the role mapping.
    /// </summary>
    /// <value>
    /// A string representing the client name.
    /// </value>
    [JsonProperty("client")]
    public string Client { get; set; }

    /// <summary>
    /// Gets or sets the roles mapped to the client.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcRole"/> representing the mapped roles.
    /// </value>
    [JsonProperty("mappings")]
    public IEnumerable<KcRole> Mappings { get; set; }
}
