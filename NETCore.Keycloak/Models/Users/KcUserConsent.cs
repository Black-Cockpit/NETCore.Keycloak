using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Users;

/// <summary>
/// Keycloak user consent representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_userconsentrepresentation"/>
/// </summary>
public class KcUserConsent
{
    /// <summary>
    /// Client id
    /// </summary>
    [JsonProperty("clientId")]
    public string ClientId { get; set; }

    /// <summary>
    /// List of granted scopes
    /// </summary>
    [JsonProperty("grantedClientScopes")]
    public IEnumerable<string> GrantedClientScopes { get; set; }

    /// <summary>
    /// Date of creation of the consent.
    /// </summary>
    [JsonProperty("createdDate")]
    public long? CreatedDate { get; set; }

    /// <summary>
    /// Date of the consent last update
    /// </summary>
    [JsonProperty("lastUpdatedDate")]
    public long? LastUpdatedDate { get; set; }
}
