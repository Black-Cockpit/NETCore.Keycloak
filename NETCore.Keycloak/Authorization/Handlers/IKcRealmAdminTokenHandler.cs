namespace NETCore.Keycloak.Authorization.Handlers;

/// <summary>
/// Keycloak admin token
/// </summary>
public interface IKcRealmAdminTokenHandler
{
    /// <summary>
    /// Fetch realm admin access token
    /// </summary>
    /// <param name="realm">Realm name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string> TryGetAdminTokenAsync(string realm, CancellationToken cancellationToken = default);
}
