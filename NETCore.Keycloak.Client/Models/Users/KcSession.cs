using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Users;

/// <summary>
/// Keycloak session representation
/// </summary>
public class KcSession
{
    /// <summary>
    /// Keycloak session id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// Keycloak user id
    /// </summary>
    [JsonProperty("userId")]
    public string UserId { get; set; }

    /// <summary>
    /// Session source IP address
    /// </summary>
    [JsonProperty("ipAddress")]
    public string IpAddress { get; set; }

    /// <summary>
    /// Session start timestamp
    /// </summary>
    [JsonProperty("start")]
    public long? Start { get; set; }

    /// <summary>
    /// Session last access
    /// </summary>
    [JsonProperty("lastAccess")]
    public long? LastAccess { get; set; }

    /// <summary>
    /// Indicates if remember me was selected
    /// </summary>
    [JsonProperty("rememberMe")]
    public bool RememberMe { get; set; }

    /// <summary>
    /// Session clients
    /// </summary>
    [JsonProperty("clients")]
    public IDictionary<string, string> Clients { get; set; }
}
