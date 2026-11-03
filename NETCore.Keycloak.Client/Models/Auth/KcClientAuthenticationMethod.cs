namespace NETCore.Keycloak.Client.Models.Auth;

/// <summary>
/// Defines client authentication for OpenID Connect token and revocation requests.
/// </summary>
public enum KcClientAuthenticationMethod
{
    /// <summary>
    /// Sends credentials using client_secret_post. Public clients may omit the secret.
    /// </summary>
    ClientSecretPost = 0,

    /// <summary>
    /// Sends form-encoded credentials in the HTTP Basic header using client_secret_basic.
    /// </summary>
    ClientSecretBasic = 1
}
