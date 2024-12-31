using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak clients REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_clients_resource"/>
/// </summary>
public interface IKcClients
{
    /// <summary>
    /// Creates a new client in a specified Keycloak realm.
    ///
    /// POST /{realm}/clients
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client will be created.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="kcClient">The client configuration to be created in the realm.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or kcClient is null or invalid.
    /// </exception>
    Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcClient kcClient,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IKcClients.ListAsync"/>
    /// <summary>
    /// Retrieves a list of clients from a specified Keycloak realm, optionally filtered by specified criteria.
    ///
    /// GET /{realm}/clients
    /// </summary>
    /// <param name="realm">The Keycloak realm from which the clients will be listed.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="filter">
    /// Optional filter criteria to narrow down the list of clients. If <c>null</c>, no filters are applied.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcClient"/> objects
    /// representing the clients in the realm, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm or access token is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcClient>>> ListAsync(string realm, string accessToken, KcClientFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific client by its ID from a specified Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcClient"/> object representing the client,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcClient>> GetAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing client in a specified Keycloak realm.
    ///
    /// PUT /{realm}/clients/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client to be updated.</param>
    /// <param name="kcClient">The updated client configuration to be applied.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or kcClient is null or invalid.
    /// </exception>
    Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string id, KcClient kcClient,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a client from a specified Keycloak realm by its ID.
    ///
    /// DELETE /{realm}/clients/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm from which the client will be deleted.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client to be deleted.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new secret for a client in a specified Keycloak realm.
    ///
    /// POST /{realm}/clients/{client-uuid}/client-secret
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the new secret will be generated.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcCredentials"/> object
    /// representing the new client secret, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcCredentials>> GenerateNewSecretAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current secret of a client in a specified Keycloak realm.
    ///
    /// POST /{realm}/clients/{id}/client-secret
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose secret will be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcCredentials"/> object representing the client secret,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcCredentials>> GetSecretAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the rotated (previous) secret of a client in a specified Keycloak realm.
    ///
    /// GET /{realm}/clients/{client-uuid}/client-secret/rotated
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose rotated secret will be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcCredentials"/> object
    /// representing the rotated client secret, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcCredentials>> GetRotatedSecretAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the rotated (previous) secret of a client in a specified Keycloak realm.
    ///
    /// DELETE /{realm}/clients/{client-uuid}/client-secret/rotated
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose rotated secret will be invalidated.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcCredentials"/> object representing the response,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcCredentials>> InvalidateRotatedSecretAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the default client scopes assigned to a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/default-client-scopes
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose default scopes are to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcClientScope"/> objects
    /// representing the default client scopes, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcClientScope>>> GetDefaultScopesAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a specific client scope to the default client scopes of a client in a Keycloak realm.
    ///
    /// PUT /{realm}/clients/{id}/default-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client to which the default scope will be added.</param>
    /// <param name="scopeId">The ID of the scope to be added as a default scope.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or scope ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> AddDefaultScopesAsync(string realm, string accessToken, string id, string scopeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific client scope from the default client scopes of a client in a Keycloak realm.
    ///
    /// DELETE /{realm}/clients/{id}/default-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client from which the default scope will be removed.</param>
    /// <param name="scopeId">The ID of the scope to be removed from the default scopes.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or scope ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteDefaultScopesAsync(string realm, string accessToken, string id, string scopeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an example access token for a client in a specified Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/evaluate-scopes/generate-example-access-token
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the example access token will be generated.</param>
    /// <param name="userId">
    /// User ID to associate with the example access token.
    /// </param>
    /// <param name="scope">
    /// Optional scope to include in the example access token. If <c>null</c>, no specific scope is included.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcAccessToken"/> object representing the generated example access token,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcAccessToken>> GenerateExampleAccessTokenAsync(string realm, string accessToken, string id,
        string userId, string scope = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an example ID token for a client in a specified Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/evaluate-scopes/generate-example-id-token
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the example ID token will be generated.</param>
    /// <param name="scope">
    /// Optional scope to include in the example ID token. If <c>null</c>, no specific scope is included.
    /// </param>
    /// <param name="userId">
    /// Optional user ID to associate with the example ID token. If <c>null</c>, no user is associated.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcAccessToken"/> object
    /// representing the generated example ID token, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcAccessToken>> GenerateExampleIdTokenAsync(string realm, string accessToken, string id,
        string scope = null, string userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates example user info for a client in a specified Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/evaluate-scopes/generate-example-userinfo
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the example user info will be generated.</param>
    /// <param name="scope">
    /// Optional scope to include in the example user info. If <c>null</c>, no specific scope is included.
    /// </param>
    /// <param name="userId">
    /// Optional user ID to associate with the example user info. If <c>null</c>, no user is associated.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the generated example user info,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> GenerateExampleUserInfoAsync(string realm, string accessToken, string id,
        string scope = null, string userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the protocol mappers associated with a client in a specified Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/evaluate-scopes/protocol-mappers
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose protocol mappers are to be retrieved.</param>
    /// <param name="scope">
    /// Optional scope to filter the protocol mappers. If <c>null</c>, all protocol mappers are retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcProtocolMapper"/> objects
    /// representing the protocol mappers, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetProtocolMappersAsync(string realm, string accessToken, string id,
        string scope = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the scoped protocol mappers for a client in a specified Keycloak realm within a role container.
    ///
    /// GET /{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/granted
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose scoped protocol mappers are to be retrieved.</param>
    /// <param name="roleContainerId">
    /// The ID of the role container to which the scoped protocol mappers are assigned.
    /// </param>
    /// <param name="scope">
    /// Optional scope to filter the protocol mappers.
    /// If <c>null</c>, all protocol mappers in the container are retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcProtocolMapper"/> objects
    /// representing the scoped protocol mappers, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetScopedProtocolMappersInContainerAsync(string realm,
        string accessToken, string id, string roleContainerId, string scope = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the unscoped protocol mappers for a client in a specified Keycloak realm within a role container.
    ///
    /// GET /{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/not-granted
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose unscoped protocol mappers are to be retrieved.</param>
    /// <param name="roleContainerId">
    /// The ID of the role container to which the unscoped protocol mappers are assigned.
    /// </param>
    /// <param name="scope">
    /// Optional scope to filter the protocol mappers. If <c>null</c>, all unscoped protocol mappers in the container are retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcProtocolMapper"/> objects
    /// representing the unscoped protocol mappers, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetUnScopedProtocolMappersInContainerAsync(string realm,
        string accessToken, string id, string roleContainerId, string scope = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the authorization management permissions for a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/management/permissions
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose management permissions are to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcPermissionManagement"/> object
    /// representing the client's authorization management permissions,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationManagementPermissionAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the authorization management permissions for a specific client in a Keycloak realm.
    ///
    /// PUT /{realm}/clients/{id}/management/permissions
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the management permissions are to be set.</param>
    /// <param name="permissionManagement">
    /// The <see cref="KcPermissionManagement"/> object containing the updated management permissions for the client.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcPermissionManagement"/> object
    /// representing the updated authorization management permissions,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or <paramref name="permissionManagement"/> is null or invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationManagementPermissionAsync(string realm, string accessToken,
        string id, KcPermissionManagement permissionManagement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new cluster node for a specific client in a Keycloak realm.
    ///
    /// POST /{realm}/clients/{id}/nodes
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the cluster node is being registered.</param>
    /// <param name="formParams">
    /// A dictionary containing the form parameters required for the node registration.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the result of the node registration,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or <paramref name="formParams"/> is null or invalid.
    /// </exception>
    Task<KcResponse<object>> RegisterNodeAsync(string realm, string accessToken, string id,
        IDictionary<string, object> formParams, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a cluster node for a specific client in a Keycloak realm.
    ///
    /// DELETE /{realm}/clients/{id}/nodes/{node}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose cluster node is to be deleted.</param>
    /// <param name="nodeName">The name of the cluster node to delete.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the result of the node deletion,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or <paramref name="nodeName"/> is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteNodeAsync(string realm, string accessToken, string id, string nodeName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of offline sessions for a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/offline-session-count
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose offline session count is to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the offline session count,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcCount>> CountOfflineSessionsAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the offline sessions for a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/offline-sessions
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose offline sessions are to be retrieved.</param>
    /// <param name="filter">
    /// An optional <see cref="KcFilter"/> object to filter the offline sessions.
    /// If <c>null</c>, all offline sessions are retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcSession"/> objects
    /// representing the offline sessions, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcSession>>> GetOfflineSessionsAsync(string realm, string accessToken, string id,
        KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the optional client scopes for a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/optional-client-scopes
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose optional client scopes are to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcClientScope"/> objects
    /// representing the optional client scopes, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcClientScope>>> GetOptionalScopesAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an optional client scope to a specific client in a Keycloak realm.
    ///
    /// PUT /{realm}/clients/{id}/optional-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client to which the optional scope is to be added.</param>
    /// <param name="scopeId">The ID of the scope to be added as an optional scope.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the result of the operation,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or <paramref name="scopeId"/> is null or invalid.
    /// </exception>
    Task<KcResponse<object>> AddOptionalScopeAsync(string realm, string accessToken, string id, string scopeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an optional client scope from a specific client in a Keycloak realm.
    ///
    /// DELETE /{realm}/clients/{id}/optional-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client from which the optional scope is to be deleted.</param>
    /// <param name="scopeId">The ID of the scope to be removed from the optional scopes.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the result of the operation,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or <paramref name="scopeId"/> is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteOptionalScopeAsync(string realm, string accessToken, string id, string scopeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Pushes revocation for a specific client in a Keycloak realm, invalidating all active sessions
    /// for the client and notifying its registered nodes of the revocation event.
    ///
    /// POST /{realm}/clients/{id}/push-revocation
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the revocation is to be pushed.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcGlobalRequestResult"/> object
    /// representing the result of the revocation push, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcGlobalRequestResult>> PushRevocationAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a new registration access token for a specific client in a Keycloak realm.
    /// This token can be used to manage the client's configuration via the Keycloak Client Registration API.
    ///
    /// POST /{realm}/clients/{id}/registration-access-token
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client for which the registration access token is to be generated.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcClient"/> object
    /// representing the client with the new registration access token,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcClient>> GetRegistrationAccessTokenAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the service account user associated with a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/service-account-user
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose service account user is to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcUser"/> object
    /// representing the service account user, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcUser>> GetServiceAccountUserAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of active sessions for a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/session-count
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose session count is to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object representing the session count,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcCount>> CountSessionsAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests the availability of nodes registered with a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/test-nodes-available
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose nodes' availability is to be tested.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcGlobalRequestResult"/> object
    /// representing the result of the node availability test, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcGlobalRequestResult>> TestAvailableNodesAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the user sessions associated with a specific client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{id}/user-sessions
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The ID of the client whose user sessions are to be retrieved.</param>
    /// <param name="filter">Optional filter criteria to narrow down the user sessions result.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcSession"/> objects
    /// representing the user sessions, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcSession>>> GetUsersSessionsAsync(string realm, string accessToken, string id,
        KcFilter filter = null, CancellationToken cancellationToken = default);
}
