using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak roles client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_roles_resource"/>
/// <seealso href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_roles_by_id_resource"/>
/// </summary>
public interface IKcRoles
{
    /// <summary>
    /// Get realm role by name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="role">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List realm roles
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="filter">Keycloak roles filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListAsync(string realm, string accessToken,
        KcFilter filter = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm role by name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcRole>> GetAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Return true if realm role exists
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<bool>> IsRolesExistsAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm role by id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak role id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcRole>> GetByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update realm role by name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak realm role name</param>
    /// <param name="role">Keycloak updated role <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string name, KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update realm role by id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="role">Keycloak updated role <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcRole>> UpdateByIdAsync(string realm, string accessToken, string id,
        KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete realm role by name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete realm role by id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Associate roles to realm role by realm role name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddCompositeAsync(string realm, string accessToken, string name,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Associate roles to realm role by realm role id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddCompositeByIdAsync(string realm, string accessToken, string id,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// List associated roles by realm role  name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeAsync(string realm, string accessToken,
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List associated roles by realm role id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeByIdAsync(string realm, string accessToken,
        string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete associated role by realm role name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteCompositeAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete associated role by realm role id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteCompositeByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client level associated roles by realm role name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesAsync(string realm,
        string accessToken, string name,
        string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client level associated roles by realm role id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesByIdAsync(string realm,
        string accessToken,
        string id, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get ream level associated roles by realm role name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesAsync(string realm,
        string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get ream level associated roles by realm role name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesByIdAsync(string realm,
        string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get groups in realm role
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="filter">Keycloak role filter <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsAsync(string realm, string accessToken,
        string name,
        KcFilter filter = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm role authorization permission by realm role name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsAsync(string realm,
        string accessToken,
        string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm role authorization permission by realm role id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsByIdAsync(string realm,
        string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set realm role authorization permission by realm role name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="permissionManagement">Keycloak role management permission <see cref="KcPermissionManagement"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsAsync(string realm,
        string accessToken, string name, KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set realm role authorization permission by realm role id
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="id">Keycloak realm role id (UUID)</param>
    /// <param name="permissionManagement">Keycloak role management permission <see cref="KcPermissionManagement"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsByIdAsync(string realm,
        string accessToken, string id, KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users in realm role
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="name">Keycloak role name</param>
    /// <param name="filter">Keycloak role filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcUser>>> GetUserInRoleAsync(string realm, string accessToken,
        string name, KcFilter filter = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create client role
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="role">Keycloak role <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> CreateClientRoleAsync(string realm, string accessToken,
        string clientId, KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List client roles
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="filter">Keycloak role filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRoleAsync(string realm, string accessToken,
        string clientId,
        KcFilter filter = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client role by name
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcRole>> GetClientRolesAsync(string realm, string accessToken, string clientId,
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Return true if client role exists
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<bool>> IsClientRoleExistsAsync(string realm, string accessToken,
        string clientId, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update client role
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="role">Keycloak role <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> UpdateClientRoleAsync(string realm, string accessToken,
        string clientId, string name,
        KcRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client role
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteClientRoleAsync(string realm, string accessToken,
        string clientId, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add client roles to composite
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="roles">Roles to be added to composite</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddClientRoleToCompositeAsync(string realm, string accessToken,
        string clientId,
        string name, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client role composites
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> GetClientCompositeRolesAsync(string realm,
        string accessToken,
        string clientId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove client role from composite
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> RemoveClientRoleFromCompositeAsync(string realm, string accessToken,
        string clientId,
        string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get groups in client role
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="filter">Keycloak role filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsInClientRoleAsync(string realm,
        string accessToken, string clientId,
        string name, KcFilter filter = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client role authorization permission
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> GetClientRoleAuthorizationPermissionsAsync(
        string realm,
        string accessToken, string clientId, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set client role authorization permission
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="permissionManagement">Keycloak role management permission <see cref="KcPermissionManagement"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcPermissionManagement>> SetClientRoleAuthorizationPermissionsAsync(
        string realm,
        string accessToken, string clientId, string name,
        KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users in client role
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="name">Role name</param>
    /// <param name="filter">keycloak roles filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcUser>>> GetUsersInClientRoleAsync(string realm,
        string accessToken, string clientId,
        string name, KcFilter filter = default, CancellationToken cancellationToken = default);
}
