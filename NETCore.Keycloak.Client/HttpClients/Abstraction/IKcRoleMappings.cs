using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak role mapper REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_role_mapper_resource"/>
/// </summary>
public interface IKcRoleMappings
{
    /// <summary>
    /// Get group role mappings
    /// GET /{realm}/groups/{id}/role-mappings
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcRoleMapping>> GetGroupRoleMappingsAsync(string realm, string accessToken, string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Map realm roles to group
    /// POST /{realm}/groups/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Group id</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddGroupRealmRoleMappingsAsync(string realm, string accessToken, string groupId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// List group realm level role mappings
    /// GET /{realm}/groups/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListGroupRealmRoleMappingsAsync(string realm, string accessToken,
        string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete group role mappings
    /// DELETE /{realm}/groups/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Group id</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteGroupRealmRoleMappingsAsync(string realm, string accessToken, string groupId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// List available realm roles that can be assigned to group
    /// GET /{realm}/groups/{id}/role-mappings/realm/available
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListGroupAvailableRealmRoleMappingsAsync(string realm, string accessToken,
        string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// List effective realm roles that can be assigned to group.
    /// GET /{realm}/groups/{id}/role-mappings/realm/composite
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="groupId">Group id</param>
    /// <param name="filter">Keycloak filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListGroupEffectiveRealmRoleMappingsAsync(string realm, string accessToken,
        string groupId, KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user role mappings
    /// GET /{realm}/users/{id}/role-mappings
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">User id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcRoleMapping>> GetUserRoleMappingsAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Map realm roles to user
    /// POST /{realm}/users/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">User id</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddUserRealmRoleMappingsAsync(string realm, string accessToken, string userId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// List user realm level role mappings
    /// GET /{realm}/users/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">User id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListUserRealmRoleMappingsAsync(string realm, string accessToken,
        string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete user role mappings
    /// DELETE /{realm}/users/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">User id</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteUserRealmRoleMappingsAsync(string realm, string accessToken, string userId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// List available realm roles that can be assigned to user
    /// GET /{realm}/users/{id}/role-mappings/realm/available
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">User id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListUserAvailableRealmRoleMappingsAsync(string realm, string accessToken,
        string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// List effective realm roles that can be assigned to user.
    /// GET /{realm}/users/{id}/role-mappings/realm/composite
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">User id</param>
    /// <param name="filter">Keycloak filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListUserEffectiveRealmRoleMappingsAsync(string realm, string accessToken,
        string userId, KcFilter filter = null, CancellationToken cancellationToken = default);
}
