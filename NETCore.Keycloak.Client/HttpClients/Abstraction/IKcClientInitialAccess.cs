using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Clients;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Client initial access REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_client_initial_access_resource"/>
/// </summary>
public interface IKcClientInitialAccess
{
    /// <summary>
    /// Create client initial access
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="access">Client initial access <see cref="KcCreateClientInitialAccess"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcClientInitialAccessModel>> CreateInitialAccessTokenAsync(string realm,
        string accessToken, KcCreateClientInitialAccess access,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List client initial access
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcClientInitialAccessModel>>> GetInitialAccess(string realm,
        string accessToken,
        CancellationToken cancellationToken = default);
}
