namespace NETCore.Keycloak.Client.Authorization.Handlers;

/// <summary>
/// Defines a contract for handling Keycloak realm admin access tokens.
/// </summary>
/// <remarks>
/// This interface provides a method for retrieving an access token with administrative privileges for a specified Keycloak realm.
/// The admin token is typically used for performing privileged operations such as user management, role assignments, and resource configuration.
/// </remarks>
public interface IKcRealmAdminTokenHandler
{
    /// <summary>
    /// Attempts to fetch an admin access token for the specified Keycloak realm.
    /// </summary>
    /// <param name="realm">The name of the Keycloak realm for which the admin token is requested.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The result contains the admin access token as a string, or <c>null</c> if the token cannot be retrieved.
    /// </returns>
    /// <remarks>
    /// The returned token is used to authenticate administrative API requests for the specified realm. The <paramref name="realm"/> parameter must match the realm's configuration.
    /// This method may return <c>null</c> if the retrieval fails due to connectivity issues, expired credentials, or missing configurations.
    /// </remarks>
    public Task<string> TryGetAdminTokenAsync(string realm, CancellationToken cancellationToken = default);
}
