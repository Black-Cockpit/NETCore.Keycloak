using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Keys;

/// <summary>
/// Represents metadata for a Keycloak key.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_keysmetadatarepresentation-keymetadatarepresentation"/>
/// </summary>
public class KcKey
{
    /// <summary>
    /// Gets or sets the algorithm used by the key.
    /// </summary>
    /// <value>
    /// A string representing the key's algorithm (e.g., "RS256").
    /// </value>
    [JsonProperty("algorithm")]
    public string Algorithm { get; set; }

    /// <summary>
    /// Gets or sets the certificate associated with the key.
    /// </summary>
    /// <value>
    /// A string containing the certificate in PEM format.
    /// </value>
    [JsonProperty("certificate")]
    public string Certificate { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the key.
    /// </summary>
    /// <value>
    /// A string representing the key ID (kid).
    /// </value>
    [JsonProperty("kid")]
    public string Kid { get; set; }

    /// <summary>
    /// Gets or sets the provider ID associated with the key.
    /// </summary>
    /// <value>
    /// A string representing the provider ID.
    /// </value>
    [JsonProperty("providerId")]
    public string ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the priority of the key's provider.
    /// </summary>
    /// <value>
    /// An integer representing the provider's priority.
    /// </value>
    [JsonProperty("providerPriority")]
    public int? ProviderPriority { get; set; }

    /// <summary>
    /// Gets or sets the public key.
    /// </summary>
    /// <value>
    /// A string containing the public key in PEM format.
    /// </value>
    [JsonProperty("publicKey")]
    public string PublicKey { get; set; }

    /// <summary>
    /// Gets or sets the status of the key.
    /// </summary>
    /// <value>
    /// A string representing the key's status (e.g., "ACTIVE").
    /// </value>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the type of the key.
    /// </summary>
    /// <value>
    /// A string representing the key's type (e.g., "RSA").
    /// </value>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the intended usage of the key.
    /// </summary>
    /// <value>
    /// A string representing the key's usage (e.g., "sig" for signature or "enc" for encryption).
    /// </value>
    [JsonProperty("use")]
    public string Use { get; set; }
}
