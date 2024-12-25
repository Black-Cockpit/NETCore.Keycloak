using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.AttackDetection;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Attack detection REST client.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_attack_detection_resource"/>
/// </summary>
public interface IKcAttackDetection
{
    /// <summary>
    /// Clear login failures.
    /// If user id is not specified all real users login failure will be cleared, otherwise the specified
    /// user login failure will be cleared.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteUsersLoginFailure(string realm, string accessToken,
        string userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get brute force detection status of a user.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcUserBruteForceStatus>> GetUserStatus(string realm, string accessToken,
        string userId,
        CancellationToken cancellationToken = default);
}
