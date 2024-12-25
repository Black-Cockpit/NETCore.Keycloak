using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.ClientScope;

namespace NETCore.Keycloak.HttpClients.Abstraction;

/// <summary>
/// Keycloak client scopes client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_client_scopes_resource"/>
/// </summary>
public interface IKcClientScopes
{
    /// <summary>
    /// Create client scope
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scope">Keycloak client scope <see cref="KcClientScope"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcClientScope>> CreateAsync(string realm, string accessToken,
        KcClientScope scope, CancellationToken cancellationToken = default);

    /// <summary>
    /// List client scopes
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcClientScope>>> ListAsync(string realm, string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client scope
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcClientScope>> GetAsync(string realm, string accessToken, string scopeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update client scope
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="scope">Keycloak client scope <see cref="KcClientScope"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcClientScope>> UpdateAsync(string realm, string accessToken, string scopeId,
        KcClientScope scope, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client scope
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string scopeId,
        CancellationToken cancellationToken = default);
}
