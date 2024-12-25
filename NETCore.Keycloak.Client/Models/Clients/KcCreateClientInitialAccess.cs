using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Represents the parameters for creating an initial access token for Keycloak clients.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_clientinitialaccesscreatepresentation"/>
/// </summary>
public class KcCreateClientInitialAccess
{
    /// <summary>
    /// Gets or sets the number of clients that can be created using the token.
    /// Default value is 1.
    /// </summary>
    /// <value>
    /// An integer representing the number of clients that can be created.
    /// </value>
    [JsonProperty("count")]
    public int? Count { get; set; } = 1;

    /// <summary>
    /// Gets or sets the expiration time for the token, in seconds since the Unix epoch.
    /// Default value is one hour from the current time.
    /// </summary>
    /// <value>
    /// An integer representing the token's expiration time.
    /// </value>
    [JsonProperty("expiration")]
    public int? Expiration { get; set; } =
        ( int ) DateTime.Now.AddHours(1).Subtract(DateTime.UnixEpoch).TotalSeconds;
}
