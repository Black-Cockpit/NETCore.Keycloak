using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents a permission in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_permission"/>
/// </summary>
public class KcPermission : KcResource
{
    /// <summary>
    /// Gets or sets the claims associated with the permission.
    /// </summary>
    /// <value>
    /// A dictionary containing key-value pairs representing the permission claims.
    /// </value>
    [JsonProperty("claims")]
    public IDictionary<string, object> Claims { get; set; }
}
