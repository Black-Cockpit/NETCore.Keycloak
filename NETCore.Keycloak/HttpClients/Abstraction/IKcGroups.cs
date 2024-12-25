using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.Common;
using NETCore.Keycloak.Models.Groups;
using NETCore.Keycloak.Models.Users;

namespace NETCore.Keycloak.HttpClients.Abstraction;

/// <summary>
/// Keycloak groups REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_groups_resource"/>
/// </summary>
public interface IKcGroups
{
    /// <summary>
    /// Create realm group
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="kcGroup">Keycloak group. <see cref="KcGroup"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcGroup>> CreateAsync(string realm, string accessToken, KcGroup kcGroup,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List realm groups.
    /// <remarks>Only name and ids are returned.</remarks>
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="filter">Keycloak filter. <see cref="KcGroupFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcGroup>>> ListAsync(string realm, string accessToken,
        KcGroupFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count realm groups
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="filter">Keycloak filter. <see cref="KcGroupFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> CountAsync(string realm, string accessToken,
        KcGroupFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm group
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcGroup>> GetAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update realm group
    /// <remarks>Subgroups are ignored</remarks>
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak group id</param>
    /// <param name="kcGroup">Keycloak group. <see cref="KcGroup"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcGroup>> UpdateAsync(string realm, string accessToken, string id,
        KcGroup kcGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete realm group
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set or create child.
    /// <remarks>
    /// If the group does not exists, it will be create and then set the sub-groups,
    /// otherwise the existing group will be set to the sub-groups.
    /// </remarks>
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak group id</param>
    /// <param name="kcGroup">Keycloak group. <see cref="KcGroup"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcGroup>> SetOrCreateChildAsync(string realm, string accessToken, string id,
        KcGroup kcGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm group authorization management permission.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationManagementPermissionAsync(string realm,
        string accessToken, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set realm group authorization management permission.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak group id</param>
    /// <param name="permissionManagement">Keycloak authorization management permission. <see cref="KcPermissionManagement"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationManagementPermissionAsync(string realm,
        string accessToken, string id, KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm group members
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcUser>>> GetMembersAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);
}
