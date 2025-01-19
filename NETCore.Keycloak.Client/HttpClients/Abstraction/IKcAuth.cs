using NETCore.Keycloak.Client.Exceptions;
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
        KcClientCredentials clientCredentials, KcUserLogin userLogin, string scope = null, string resource = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a user's password by attempting to retrieve an access token using the provided credentials
    /// and then revoking the token if the retrieval is successful.
    /// </summary>
    /// <param name="realm">The Keycloak realm to validate the user's password against.</param>
    /// <param name="clientCredentials">The client credentials used to authenticate the request.</param>
    /// <param name="userLogin">The user's login credentials to validate.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcOperationResponse{T}"/> containing a <c>true</c> response if the password is valid,
    /// or <c>false</c> if the validation fails, along with any associated error messages and monitoring metrics.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, client credentials, or user login information is invalid or missing.
    /// </exception>
    Task<KcOperationResponse<bool>> ValidatePasswordAsync(string realm, KcClientCredentials clientCredentials,
        KcUserLogin userLogin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="clientCredentials">Client credentials <see cref="KcClientCredentials"/></param>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcIdentityProviderToken>> RefreshAccessTokenAsync(string realm,
        KcClientCredentials clientCredentials, string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an access token for a Keycloak realm using the specified client credentials.
    /// </summary>
    /// <param name="realm">The Keycloak realm to which the token belongs.</param>
    /// <param name="clientCredentials">The client credentials used for authentication.</param>
    /// <param name="accessToken">The access token to revoke.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <c>true</c> response if the token was successfully revoked,
    /// or <c>false</c> if the revocation fails. The response includes any associated error messages
    /// and monitoring metrics.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm or client credentials are null or invalid.
    /// </exception>
    Task<KcResponse<bool>> RevokeAccessTokenAsync(string realm, KcClientCredentials clientCredentials,
        string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token for a Keycloak realm using the specified client credentials.
    /// </summary>
    /// <param name="realm">The Keycloak realm to which the token belongs.</param>
    /// <param name="clientCredentials">The client credentials used for authentication.</param>
    /// <param name="refreshToken">The refresh token to revoke.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <c>true</c> response if the token was successfully revoked,
    /// or <c>false</c> if the revocation fails. The response includes any associated error messages
    /// and monitoring metrics.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm or client credentials are null or invalid.
    /// </exception>
    Task<KcResponse<bool>> RevokeRefreshTokenAsync(string realm, KcClientCredentials clientCredentials,
        string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a Request Party Token (RPT) from Keycloak, which is used for resource-based access authorization.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the request is made.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="audience">The intended audience of the token.</param>
    /// <param name="permissions">
    /// A collection of permissions to request in the token, or <c>null</c> if no specific permissions are required.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcIdentityProviderToken"/> if successful,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or audience is null or invalid.
    /// </exception>
    Task<KcResponse<KcIdentityProviderToken>> GetRequestPartyTokenAsync(string realm, string accessToken,
        string audience, IEnumerable<string> permissions = null, CancellationToken cancellationToken = default);
}
