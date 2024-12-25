using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.Common;
using NETCore.Keycloak.Models.Groups;
using NETCore.Keycloak.Models.Users;

namespace NETCore.Keycloak.HttpClients.Abstraction;

/// <summary>
/// Keycloak users client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_users_resource"/>
/// </summary>
public interface IKcUsers
{
    /// <summary>
    /// Create user
    /// POST /{realm}/users
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="user">Keycloak user <see cref="KcUser"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Return true if user exists
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="email">Email address</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<bool>> IsUserExistsByEmailAsync(string realm, string accessToken, string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List users
    /// GET /{realm}/users
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="filter">Users filter <see cref="KcUserFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcUser>>> ListUserAsync(string realm, string accessToken,
        KcUserFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count users
    /// GET /{realm}/users/count
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="filter">Users filter <see cref="KcUserFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<int>> CountAsync(string realm, string accessToken,
        KcUserFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user
    /// GET /{realm}/users/{id}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcUser>> GetAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user
    /// PUT /{realm}/users/{id}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="user">Keycloak user</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string userId,
        KcUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete user
    /// DELETE /{realm}/users/{id}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user credentials
    /// GET /{realm}/users/{id}/credentials
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcCredentials>>> GetCredentialsAsync(string realm,
        string accessToken, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete user credentials
    /// DELETE /{realm}/users/{id}/credentials/{credentialId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="credentialsId">Keycloak credentials id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteCredentialsAsync(string realm, string accessToken, string userId,
        string credentialsId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user credentials label
    /// PUT /{realm}/users/{id}/credentials/{credentialId}/userLabel
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="credentialsId">Keycloak credentials id</param>
    /// <param name="label">New credentials label</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> UpdateCredentialsLabelAsync(string realm, string accessToken,
        string userId, string credentialsId, string label,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List user assigned groups
    /// GET /{realm}/users/{id}/groups
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="filter">Keycloak groups filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcGroup>>> UserGroupsAsync(string realm, string accessToken,
        string userId, KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count user assigned groups
    /// GET /{realm}/users/{id}/groups/count
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="filter">Keycloak groups filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<int>> CountGroupsAsync(string realm, string accessToken, string userId,
        KcFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add user to group
    /// PUT /{realm}/users/{id}/groups/{groupId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="groupId">Keycloak group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddToGroupAsync(string realm, string accessToken, string userId,
        string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete user from group
    /// DELETE /{realm}/users/{id}/groups/{groupId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="groupId">Keycloak group id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteFromGroupAsync(string realm, string accessToken, string userId,
        string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset user password
    /// PUT /{realm}/users/{id}/reset-password
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="credentials">New credentials <see cref="KcCredentials"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcCredentials>> ResetPasswordAsync(string realm, string accessToken,
        string userId, KcCredentials credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// List user sessions
    /// GET /{realm}/users/{id}/sessions
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcSession>>> SessionsAsync(string realm, string accessToken,
        string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete session
    /// DELETE /{realm}/sessions/{session}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="sessionId">Keycloak session id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteSessionAsync(string realm, string accessToken, string sessionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout user from all sessions
    /// POST /{realm}/users/{id}/logout
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin or user access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> LogoutFromAllSessionsAsync(string realm, string accessToken,
        string userId, CancellationToken cancellationToken = default);
}
