using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Roles;

/// <summary>
/// Keycloak composite role representation.
/// A composite role is a role that can be associated with other roles.
/// For example a superuser composite role could be associated with the sales-admin and order-entry-admin roles.
/// If a user is mapped to the superuser role they also inherit the sales-admin and order-entry-admin roles.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_rolerepresentation-composites"/>
/// </summary>
public class KcRoleComposite
{
    /// <summary>
    /// List of keycloak client names
    /// </summary>
    [JsonProperty("client")]
    public IDictionary<string, string> Client { get; set; }

    /// <summary>
    /// List of keycloak realm names
    /// </summary>
    [JsonProperty("realm")]
    public IEnumerable<string> Realm { get; set; }
}
