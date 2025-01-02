using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Roles;

/// <summary>
/// Represents a role in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_rolerepresentation"/>
/// </summary>
public class KcRole
{
    /// <summary>
    /// Gets or sets the attributes associated with the role.
    /// </summary>
    /// <value>
    /// A dictionary containing key-value pairs of role attributes.
    /// </value>
    [JsonProperty("attributes")]
    public IDictionary<string, IEnumerable<string>> Attributes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the role is a client role.
    /// </summary>
    /// <value>
    /// <c>true</c> if the role is a client role; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("clientRole")]
    public bool? ClientRole { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the role is composite.
    /// </summary>
    /// <value>
    /// <c>true</c> if the role is composite; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("composite")]
    public bool? Composite { get; set; }

    /// <summary>
    /// Gets or sets the composite roles associated with this role.
    /// </summary>
    /// <value>
    /// An instance of <see cref="KcRoleComposite"/> representing the composite roles.
    /// </value>
    [JsonProperty("composites")]
    public KcRoleComposite Composites { get; set; }

    /// <summary>
    /// Gets or sets the container ID for the role.
    /// </summary>
    /// <value>
    /// A string representing the container ID.
    /// </value>
    [JsonProperty("containerId")]
    public string ContainerId { get; set; }

    /// <summary>
    /// Gets or sets the description of the role.
    /// </summary>
    /// <value>
    /// A string representing the role's description.
    /// </value>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the role.
    /// </summary>
    /// <value>
    /// A string representing the role ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    /// <value>
    /// A string representing the role's name.
    /// </value>
    [JsonProperty("name")]
    public string Name { get; set; }
}
