using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Keys;

/// <summary>
/// Represents the metadata for Keycloak keys.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_keysmetadatarepresentation"/>
/// </summary>
public class KcKeyMetadata
{
    /// <summary>
    /// Gets or sets the active algorithms and their associated keys.
    /// </summary>
    /// <value>
    /// An instance of <see cref="KcKeyActive"/> representing the active keys for specific algorithms.
    /// </value>
    [JsonProperty("active")]
    public KcKeyActive Active { get; set; }

    /// <summary>
    /// Gets or sets the list of keys in the Keycloak metadata.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcKey"/> representing the Keycloak keys.
    /// </value>
    [JsonProperty("keys")]
    public IEnumerable<KcKey> Keys { get; set; }
}
