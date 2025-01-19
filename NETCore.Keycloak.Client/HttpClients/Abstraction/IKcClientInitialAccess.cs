using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Clients;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Client initial access REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_client_initial_access_resource"/>
/// </summary>
public interface IKcClientInitialAccess
{
    /// <summary>
    /// Creates an initial access token for a Keycloak realm, allowing for client registrations.
    ///
    /// POST {realm}/clients-initial-access
    /// </summary>
    /// <param name="realm">The Keycloak realm in which the token is to be created.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="access">The configuration for creating the initial access token.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcClientInitialAccessModel"/> if successful,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or access configuration is null or invalid.
    /// </exception>
    Task<KcResponse<KcClientInitialAccessModel>> CreateInitialAccessTokenAsync(string realm, string accessToken,
        KcCreateClientInitialAccess access, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of client initial access tokens for a Keycloak realm.
    ///
    /// GET {realm}/clients-initial-access
    /// </summary>
    /// <param name="realm">The Keycloak realm for which the tokens are to be retrieved.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcClientInitialAccessModel"/> objects
    /// if successful, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm or access token is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcClientInitialAccessModel>>> GetInitialAccessAsync(string realm,
        string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an initial access token associated with a client in a Keycloak realm.
    ///
    /// DELETE {realm}/clients-initial-access/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the initial access token to be deleted.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the result of the deletion,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or initial access token ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteInitialAccessTokenAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);
}
