using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Tokens;

/// <summary>
/// Represents the certificate configuration in a Keycloak access token.
/// </summary>
public class KcAccessTokenCertConf
{
    /// <summary>
    /// Gets or sets the certificate thumbprint using the SHA-256 algorithm.
    /// </summary>
    /// <value>
    /// A string representing the SHA-256 thumbprint of the certificate.
    /// </value>
    [JsonProperty("x5t#S256")]
    public string X5Ts256 { get; set; }
}
