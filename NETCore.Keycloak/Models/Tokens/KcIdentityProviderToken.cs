using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Tokens;

/// <summary>
/// Represents an identity provider token in Keycloak.
/// </summary>
public class KcIdentityProviderToken
{
    /// <summary>
    /// Gets or sets a value indicating whether the token has been upgraded.
    /// </summary>
    /// <value>
    /// <c>true</c> if the token is upgraded; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("upgraded")]
    public bool? Upgraded { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    /// <value>
    /// A string representing the access token.
    /// </value>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the type of the access token.
    /// </summary>
    /// <value>
    /// A string representing the token type.
    /// </value>
    [JsonProperty("token_type")]
    public string RequestingPartyToken { get; set; }

    /// <summary>
    /// Gets or sets the token's expiry timestamp in seconds.
    /// </summary>
    /// <value>
    /// An integer representing the token's expiration time.
    /// </value>
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    /// <value>
    /// A string representing the refresh token.
    /// </value>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token's expiry time in seconds.
    /// </summary>
    /// <value>
    /// An integer representing the refresh token's expiration time.
    /// </value>
    [JsonProperty("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the scope of the token.
    /// </summary>
    /// <value>
    /// A string representing the token's scope.
    /// </value>
    [JsonProperty("scope")]
    public string Scope { get; set; }

    /// <summary>
    /// Gets or sets the session state associated with the token.
    /// </summary>
    /// <value>
    /// A string representing the session state.
    /// </value>
    [JsonProperty("session_state")]
    public string SessionState { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the token was created.
    /// </summary>
    /// <value>
    /// A long representing the token's creation time in seconds since the Unix epoch.
    /// </value>
    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the "not-before" policy associated with the token.
    /// </summary>
    /// <value>
    /// A nullable long representing the "not-before" policy timestamp.
    /// </value>
    [JsonProperty("not-before-policy")]
    public long? NotBeforePolicy { get; set; }

    /// <summary>
    /// Decodes the access token and retrieves the session ID (`sid`) claim if it exists.
    /// </summary>
    /// <returns>
    /// A string representing the session ID, or <c>null</c> if the claim is not found or the token is invalid.
    /// </returns>
    public string GetSessionId()
    {
        if ( string.IsNullOrWhiteSpace(AccessToken) )
        {
            return null;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(AccessToken);

            return token.Claims.FirstOrDefault(claim => claim.Type == "sid")?.Value;
        }
        catch ( Exception )
        {
            return null;
        }
    }
}
