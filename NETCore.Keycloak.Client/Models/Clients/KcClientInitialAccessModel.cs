using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Represents the initial access configuration for a Keycloak client.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_clientinitialaccesspresentation"/>
/// </summary>
public class KcClientInitialAccessModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the initial access configuration.
    /// </summary>
    /// <value>
    /// A string representing the Keycloak ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the remaining number of clients that can be created using this initial access configuration.
    /// </summary>
    /// <value>
    /// An integer representing the remaining count, or <c>null</c> if not specified.
    /// </value>
    [JsonProperty("remainingCount")]
    public int? RemainingCount { get; set; }

    /// <summary>
    /// Gets or sets the UNIX timestamp representing when the initial access was created.
    /// </summary>
    /// <value>
    /// An integer representing the timestamp, or <c>null</c> if not specified.
    /// </value>
    [JsonProperty("timestamp")]
    public int? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the token associated with the initial access configuration.
    /// </summary>
    /// <value>
    /// A string representing the initial access token.
    /// </value>
    [JsonProperty("token")]
    public string Token { get; set; }
}
