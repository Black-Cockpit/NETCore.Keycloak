using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.Common;
using NETCore.Keycloak.Models.Roles;

namespace NETCore.Keycloak.HttpClients.Abstraction;

/// <summary>
/// Keycloak client role mapping client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_client_role_mappings_resource"/>
/// </summary>
public interface IKcClientRoleMappings
{
    /// <summary>
    /// Add client-level roles to the group role mapping
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Keycloak group id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> MapClientRolesToGroupAsync(string realm, string accessToken,
        string groupId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client-level role mappings for a group
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Keycloak group id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetGroupMappedClientRolesAsync(string realm,
        string accessToken, string groupId, string clientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client-level roles from group role mapping
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Keycloak group id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteGroupClientRoleMappingsAsync(string realm, string accessToken,
        string groupId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available client-level roles that can be mapped to a group
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Keycloak group id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetGroupAvailableClientRolesAsync(string realm,
        string accessToken, string groupId, string clientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get effective client-level role mappings for a group.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Keycloak group id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="filter">Keycloak roles filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetGroupCompositeClientRolesAsync(string realm,
        string accessToken, string groupId, string clientId, KcFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add client-level roles to the user role mapping
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id (UUID)</param>
    /// <param name="clientId">Keycloak group id (UUID)</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> MapClientRolesToUserAsync(string realm, string accessToken,
        string userId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client-level role mappings for a user
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id (UUID)</param>
    /// <param name="clientId">Keycloak group id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetUserMappedClientRolesAsync(string realm,
        string accessToken, string userId, string clientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client-level roles from user role mapping
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    Task<KcResponse<object>> DeleteUserClientRoleMappingsAsync(string realm, string accessToken,
        string userId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available client-level roles that can be mapped to a user
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    Task<KcResponse<IEnumerable<KcRole>>> GetUserAvailableClientRolesAsync(string realm,
        string accessToken, string userId, string clientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get effective client-level role mappings for a user.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="filter">Keycloak roles filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetUserCompositeClientRolesAsync(string realm,
        string accessToken, string userId, string clientId, KcFilter filter = null,
        CancellationToken cancellationToken = default);
}
