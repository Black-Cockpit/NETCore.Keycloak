using NETCore.Keycloak.Client.Exceptions;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Auth;

/// <summary>
/// Represents the credentials required for a Keycloak client.
/// </summary>
public class KcClientCredentials
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// A string representing the unique client identifier.
    /// </value>
    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>
    /// A string representing the client's secret key.
    /// </value>
    [JsonProperty("client_secret")]
    public string Secret { get; set; }

    /// <summary>
    /// Gets or sets client authentication for token and revocation requests.
    /// Defaults to form authentication to preserve existing confidential and public client behavior.
    /// </summary>
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public KcClientAuthenticationMethod AuthenticationMethod { get; set; }

    /// <summary>
    /// Validates the Keycloak client credentials.
    /// </summary>
    /// <exception cref="KcException">
    /// Thrown when <see cref="ClientId"/> is blank or <see cref="AuthenticationMethod"/> is unknown.
    /// </exception>
    public void Validate()
    {
        if ( !Enum.IsDefined(AuthenticationMethod) )
        {
            throw new KcException($"{nameof(AuthenticationMethod)} is invalid");
        }

        if ( string.IsNullOrWhiteSpace(ClientId) )
        {
            throw new KcException($"{nameof(ClientId)} is required");
        }
    }
}
