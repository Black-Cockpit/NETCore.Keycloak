using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Represents a resource configuration for a Keycloak client.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_resourcerepresentation"/>
/// </summary>
public class KcClientResource
{
    /// <summary>
    /// Gets or sets the unique identifier for the Keycloak resource.
    /// </summary>
    /// <value>
    /// A string representing the resource ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the attributes associated with the resource.
    /// </summary>
    /// <value>
    /// A dictionary of key-value pairs representing resource attributes.
    /// </value>
    [JsonProperty("attributes")]
    public IDictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Gets or sets a unique display name for this resource. Useful for identifying resources when querying.
    /// </summary>
    /// <value>
    /// A string representing the resource's display name.
    /// </value>
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets a URI pointing to an icon for this resource.
    /// </summary>
    /// <value>
    /// A string representing the icon URI.
    /// </value>
    [JsonProperty("icon_uri")]
    public string IconUri { get; set; }

    /// <summary>
    /// Gets or sets a unique name for this resource. This name can be used for querying specific resources.
    /// </summary>
    /// <value>
    /// A string representing the resource's unique name.
    /// </value>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether access to this resource can be managed by its owner.
    /// </summary>
    /// <value>
    /// A string indicating if the resource is owner-managed.
    /// </value>
    [JsonProperty("ownerManagedAccess")]
    public string OwnerManagedAccess { get; set; }

    /// <summary>
    /// Gets or sets the list of scopes associated with this resource.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcClientResourceScope"/> representing resource scopes.
    /// </value>
    [JsonProperty("scopes")]
    public IEnumerable<KcClientResourceScope> Scopes { get; set; }

    /// <summary>
    /// Gets or sets the type of this resource. This can be used to group similar resources.
    /// </summary>
    /// <value>
    /// A string representing the resource type.
    /// </value>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the set of URIs protected by this resource.
    /// </summary>
    /// <value>
    /// A collection of strings representing the protected URIs.
    /// </value>
    [JsonProperty("uris")]
    public IEnumerable<string> Uris { get; set; }
}
