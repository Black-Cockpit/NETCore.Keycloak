using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Users;

/// <summary>
/// Keycloak federated identity representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_federatedidentityrepresentation"/>
/// </summary>
public class KcFederatedIdentity
{
    /// <summary>
    /// An identity provider derives from a specific protocol used to authenticate and send authentication and authorization information to users.
    /// It can be:
    /// - A social provider such as Facebook, Google, or Twitter.
    /// - A business partner whose users need to access your services.
    /// - A cloud-based identity service you want to integrate.
    /// </summary>
    [JsonProperty("identityProvider")]
    public string IdentityProvider { get; set; }

    /// <summary>
    /// User id
    /// </summary>
    [JsonProperty("userId")]
    public string UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    [JsonProperty("userName")]
    public string UserName { get; set; }
}
