using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Common;

/// <summary>
/// Represents the management permission settings in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_managementpermissionreference"/>
/// </summary>
public class KcPermissionManagement
{
    /// <summary>
    /// Gets or sets a value indicating whether management permissions are enabled.
    /// </summary>
    /// <value>
    /// <c>true</c> if management permissions are enabled; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets the name of the resource associated with the management permissions.
    /// </summary>
    /// <value>
    /// A string representing the resource name.
    /// </value>
    [JsonProperty("resource")]
    public string Resource { get; set; }

    /// <summary>
    /// Gets or sets the scope permissions for the resource.
    /// </summary>
    /// <value>
    /// An object representing the scope permissions.
    /// </value>
    [JsonProperty("scopePermissions")]
    public object ScopePermissions { get; set; }
}
