using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Tokens;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak authentication client
/// </summary>
public interface IKcAuth
{
    /// <summary>
    /// Get client credentials token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="clientCredentials">Client credentials <see cref="KcClientCredentials"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcIdentityProviderToken>> GetClientCredentialsTokenAsync(string realm,
        KcClientCredentials clientCredentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get resource owner password access token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="clientCredentials">Client credentials <see cref="KcClientCredentials"/></param>
    /// <param name="userLogin">User credentials <see cref="KcUserLogin"/></param>
    /// <param name="scope">OIDC scope</param>
    /// <param name="resource">Specify the API Resource Identifier </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcIdentityProviderToken>> GetResourceOwnerPasswordTokenAsync(string realm,
        KcClientCredentials clientCredentials, KcUserLogin userLogin, string scope = null,
        string resource = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate password
    /// The call is tricky, it uses the password resource owner grant type to get a token.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="clientCredentials">Client credentials <see cref="KcClientCredentials"/></param>
    /// <param name="userLogin">User credentials <see cref="KcUserLogin"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<bool>> ValidatePasswordAsync(string realm,
        KcClientCredentials clientCredentials, KcUserLogin userLogin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="clientCredentials">Client credentials <see cref="KcClientCredentials"/></param>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcIdentityProviderToken>> RefreshAccessTokenAsync(string realm,
        KcClientCredentials clientCredentials, string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke access token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="clientCredentials">Client credentials <see cref="KcClientCredentials"/></param>
    /// <param name="accessToken">Access token</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<bool>> RevokeAccessTokenAsync(string realm,
        KcClientCredentials clientCredentials, string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="clientCredentials">Client credentials <see cref="KcClientCredentials"/></param>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<bool>> RevokeRefreshTokenAsync(string realm,
        KcClientCredentials clientCredentials, string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get request party token (RPT)
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Bearer access token</param>
    /// <param name="audience">The client identifier of the resource server to which the client is seeking access.</param>
    /// <param name="permissions">
    /// A string representing a set of one or more resources and scopes the client is seeking access.
    /// <see href="https://github.com/keycloak/keycloak-documentation/blob/main/authorization_services/topics/service-authorization-obtaining-permission.adoc"/>
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcIdentityProviderToken>> GetRequestPartyTokenAsync(string realm,
        string accessToken, string audience, IEnumerable<string> permissions = null,
        CancellationToken cancellationToken = default);
}
