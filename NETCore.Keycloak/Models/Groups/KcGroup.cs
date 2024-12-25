using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Groups;

/// <summary>
/// Represents a group in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_grouprepresentation"/>
/// </summary>
public class KcGroup
{
    /// <summary>
    /// Gets or sets the access settings for the group.
    /// </summary>
    /// <value>
    /// An object representing the group's access permissions.
    /// </value>
    [JsonProperty("access")]
    public object Access { get; set; }

    /// <summary>
    /// Gets or sets the attributes associated with the group.
    /// </summary>
    /// <value>
    /// A dictionary containing key-value pairs of group attributes.
    /// </value>
    [JsonProperty("attributes")]
    public IDictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Gets or sets the client roles associated with the group.
    /// </summary>
    /// <value>
    /// A dictionary where the key is the client identifier and the value is a collection of role names.
    /// </value>
    [JsonProperty("clientRoles")]
    public IDictionary<string, IEnumerable<string>> ClientRoles { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    /// <value>
    /// A string representing the group ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    /// <value>
    /// A string representing the group name.
    /// </value>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the path of the group within the hierarchy.
    /// </summary>
    /// <value>
    /// A string representing the group's path.
    /// </value>
    [JsonProperty("path")]
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the realm roles associated with the group.
    /// </summary>
    /// <value>
    /// A collection of strings representing the realm roles.
    /// </value>
    [JsonProperty("realmRoles")]
    public IEnumerable<string> RealmRoles { get; set; }

    /// <summary>
    /// Gets or sets the subgroups within this group.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcGroup"/> representing the subgroups.
    /// </value>
    [JsonProperty("subGroups")]
    public IEnumerable<KcGroup> Subgroups { get; set; }
}
