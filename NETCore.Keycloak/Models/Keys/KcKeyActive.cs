using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Keys;

/// <summary>
/// Represents the active algorithms for Keycloak keys.
/// </summary>
public class KcKeyActive
{
    /// <summary>
    /// Gets or sets the active key ID for the HS256 algorithm.
    /// </summary>
    /// <value>
    /// A string representing the key ID for the HS256 algorithm.
    /// </value>
    [JsonProperty("HS256")]
    public string Hs256 { get; set; }

    /// <summary>
    /// Gets or sets the active key ID for the RS256 algorithm.
    /// </summary>
    /// <value>
    /// A string representing the key ID for the RS256 algorithm.
    /// </value>
    [JsonProperty("RS256")]
    public string Rs256 { get; set; }

    /// <summary>
    /// Gets or sets the active key ID for the AES algorithm.
    /// </summary>
    /// <value>
    /// A string representing the key ID for the AES algorithm.
    /// </value>
    [JsonProperty("AES")]
    public string Aes { get; set; }
}
