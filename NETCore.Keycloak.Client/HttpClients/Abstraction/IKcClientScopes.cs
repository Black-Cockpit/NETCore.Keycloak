using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.ClientScope;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak client scopes client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_client_scopes_resource"/>
/// </summary>
public interface IKcClientScopes
{
    /// <summary>
    /// Creates a new client scope in a specified Keycloak realm.
    ///
    /// POST /{realm}/client-scopes
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope will be created.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="scope">The <see cref="KcClientScope"/> object representing the client scope to be created.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the created <see cref="KcClientScope"/> object,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client scope object is null or invalid.
    /// </exception>
    Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcClientScope scope,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all client scopes in a specified Keycloak realm.
    ///
    /// GET /{realm}/client-scopes
    /// </summary>
    /// <param name="realm">The Keycloak realm from which the client scopes are retrieved.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcClientScope"/> objects,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm or access token is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcClientScope>>> ListAsync(string realm, string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific client scope by its ID from a specified Keycloak realm.
    ///
    /// GET /{realm}/client-scopes/{client-scope-id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="scopeId">The ID of the client scope to retrieve.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcClientScope"/> object representing the client scope,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client scope ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcClientScope>> GetAsync(string realm, string accessToken, string scopeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing client scope in a specified Keycloak realm.
    ///
    /// PUT /{realm}/client-scopes/{client-scope-id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="scopeId">The ID of the client scope to update.</param>
    /// <param name="scope">
    /// The <see cref="KcClientScope"/> object containing updated details for the client scope.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated <see cref="KcClientScope"/> object,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client scope ID, or client scope object is null or invalid.
    /// </exception>
    Task<KcResponse<KcClientScope>> UpdateAsync(string realm, string accessToken, string scopeId,
        KcClientScope scope, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified client scope from a Keycloak realm.
    ///
    /// DELETE /{realm}/client-scopes/{client-scope-id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="scopeId">The ID of the client scope to delete.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the delete operation,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client scope ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string scopeId,
        CancellationToken cancellationToken = default);
}
