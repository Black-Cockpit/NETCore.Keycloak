using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.AttackDetection;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Attack the detection REST client.
/// <see href="https://www.keycloak.org/docs-api/24.0.5/rest-api/index.html#_attack_detection"/>
/// </summary>
public interface IKcAttackDetection
{
    /// <summary>
    /// Clear login failures.
    /// If user id is not specified, all real users login failure will be cleared, otherwise the specified
    /// user login failure will be cleared.
    ///
    /// DELETE {realm}/attack-detection/brute-force/users
    /// DELETE {realm}/attack-detection/brute-force/users/{userId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteUsersLoginFailureAsync(string realm, string accessToken, string userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the brute force detection status of a user.
    ///
    /// GET {realm}/attack-detection/brute-force/users/{userId}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="userId">Keycloak user id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcUserBruteForceStatus>> GetUserStatusAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);
}
