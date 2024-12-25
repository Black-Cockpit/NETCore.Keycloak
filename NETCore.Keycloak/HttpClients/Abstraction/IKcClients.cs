using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.Clients;
using NETCore.Keycloak.Models.ClientScope;
using NETCore.Keycloak.Models.Common;
using NETCore.Keycloak.Models.Tokens;
using NETCore.Keycloak.Models.Users;

namespace NETCore.Keycloak.HttpClients.Abstraction;

/// <summary>
/// Keycloak clients REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_clients_resource"/>
/// </summary>
public interface IKcClients
{
    /// <summary>
    /// Create realm client
    /// POST /{realm}/clients
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="kcClient">Keycloak client <see cref="KcClient"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcClient kcClient,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List realm clients
    /// GET /{realm}/clients
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="filter">Keycloak client filter. <see cref="KcClientFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcClient>>> ListAsync(string realm, string accessToken,
        KcClientFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client by id (UUID)
    /// GET /{realm}/clients/{id}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcClient>> GetAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update client id
    /// PUT /{realm}/clients/{id}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="kcClient">Keycloak client <see cref="KcClient"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string id,
        KcClient kcClient, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client
    /// DELETE /{realm}/clients/{id}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate new client secret
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcCredentials>> GenerateNewSecretAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client secret
    /// POST /{realm}/clients/{id}/client-secret
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcCredentials>> GetSecretAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client rotated secret
    /// GET /{realm}/clients/{id}/client-secret
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcCredentials>> GetRotatedSecretAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidate client rotated secret
    /// GET /{realm}/clients/{id}/client-secret/rotated
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcCredentials>> InvalidateRotatedSecretAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// List client default scopes
    /// GET /{realm}/clients/{id}/default-client-scopes
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcClientScope>>> GetDefaultScopesAsync(string realm,
        string accessToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add scope to client default scopes
    /// PUT /{realm}/clients/{id}/default-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scopeId">Scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddDefaultScopesAsync(string realm, string accessToken, string id,
        string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client default scope by id
    /// DELETE /{realm}/clients/{id}/default-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scopeId">Scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteDefaultScopesAsync(string realm, string accessToken, string id,
        string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate example access token
    /// GET /{realm}/clients/{id}/evaluate-scopes/generate-example-access-token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scope">Scope name</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcAccessToken>> GenerateExampleAccessTokenAsync(string realm,
        string accessToken, string id, string scope = null, string userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate example id token
    /// GET /{realm}/clients/{id}/evaluate-scopes/generate-example-id-token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scope">Scope name</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcAccessToken>> GenerateExampleIdTokenAsync(string realm, string accessToken,
        string id, string scope = null, string userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate example user info
    /// GET /{realm}/clients/{id}/evaluate-scopes/generate-example-userinfo
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scope">Scope name</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> GenerateExampleUserInfoAsync(string realm, string accessToken,
        string id, string scope = null, string userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List client protocol mappers
    /// GET /{realm}/clients/{id}/evaluate-scopes/protocol-mappers
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scope">Scope name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetProtocolMappersAsync(string realm,
        string accessToken, string id, string scope = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get scoped protocol mappers in role container
    /// GET /{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/granted
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="roleContainerId">Keycloak role container id</param>
    /// <param name="scope">Client scope name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetScopedProtocolMappersInContainerAsync(
        string realm, string accessToken, string id, string roleContainerId, string scope = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get unscoped protocol mappers in role container
    /// GET /{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/not-granted
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="roleContainerId">Keycloak role container id</param>
    /// <param name="scope">Client scope name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetUnScopedProtocolMappersInContainerAsync(
        string realm, string accessToken, string id, string roleContainerId, string scope = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client authorization management permission
    /// GET /{realm}/clients/{id}/management/permissions
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationManagementPermissionAsync(string realm,
        string accessToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set client authorization management permission
    /// PUT /{realm}/clients/{id}/management/permissions
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="permissionManagement">Keycloak management permission <see cref="KcPermissionManagement"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationManagementPermissionAsync(string realm,
        string accessToken, string id, KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Register cluster node
    /// POST /{realm}/clients/{id}/nodes
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="formParams">Keycloak node form params</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> RegisterNodeAsync(string realm, string accessToken, string id,
        IDictionary<string, object> formParams, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete cluster node
    /// DELETE /{realm}/clients/{id}/nodes/{node}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="nodeName">Keycloak node name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteNodeAsync(string realm, string accessToken, string id,
        string nodeName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count client offline sessions
    /// GET /{realm}/clients/{id}/offline-session-count
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> CountOfflineSessionsAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List client offline sessions
    /// GET /{realm}/clients/{id}/offline-sessions
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="filter">Keycloak filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcSession>>> GetOfflineSessionsAsync(string realm,
        string accessToken, string id, KcFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List client optional scopes
    /// GET /{realm}/clients/{id}/optional-client-scopes
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcClientScope>>> GetOptionalScopesAsync(string realm,
        string accessToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add scope to client optional scopes
    /// PUT /{realm}/clients/{id}/optional-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddOptionalScopeAsync(string realm, string accessToken, string id,
        string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete scope to client optional scopes
    /// DELETE /{realm}/clients/{id}/optional-client-scopes/{clientScopeId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteOptionalScopeAsync(string realm, string accessToken, string id,
        string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Push token revocation
    /// POST /{realm}/clients/{id}/push-revocation
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcGlobalRequestResult>> PushRevocationAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create client registration access token
    /// POST /{realm}/clients/{id}/registration-access-token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcClient>> GetRegistrationAccessTokenAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client service account user
    /// GET /{realm}/clients/{id}/service-account-user
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcUser>> GetServiceAccountUserAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Count client sessions
    /// GET /{realm}/clients/{id}/session-count
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> CountSessionsAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Test client available nodes
    /// GET /{realm}/clients/{id}/test-nodes-available
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcGlobalRequestResult>> TestAvailableNodesAsync(string realm,
        string accessToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// List user sessions
    /// GET /{realm}/clients/{id}/user-sessions
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak Client id (UUID)</param>
    /// <param name="filter">Keycloak filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcSession>>> GetUsersSessionsAsync(string realm, string accessToken,
        string id, KcFilter filter = null, CancellationToken cancellationToken = default);
}
