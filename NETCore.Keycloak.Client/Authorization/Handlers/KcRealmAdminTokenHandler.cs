using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.Authorization.Handlers;

/// <summary>
/// Handles the retrieval and caching of Keycloak realm admin tokens.
/// </summary>
/// <remarks>
/// This class is responsible for fetching, refreshing, and caching Keycloak admin access tokens
/// to perform privileged operations within a Keycloak realm.
/// </remarks>
public sealed class KcRealmAdminTokenHandler : IKcRealmAdminTokenHandler
{
    /// <summary>
    /// Stores cached access and refresh tokens to avoid unnecessary API calls.
    /// </summary>
    private readonly ConcurrentDictionary<string, KcCachedToken> _tokensCache;

    /// <summary>
    /// Provides the Keycloak realm admin configurations.
    /// </summary>
    private readonly KcRealmAdminConfigurationStore _realmAdminConfigurationStore;

    /// <summary>
    /// Logger instance for logging operations.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KcRealmAdminTokenHandler"/> class.
    /// </summary>
    /// <param name="realmAdminConfigurationStore">The store containing realm admin configurations.</param>
    /// <param name="provider">The dependency injection service provider.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="realmAdminConfigurationStore"/> or its configurations are null.
    /// </exception>
    public KcRealmAdminTokenHandler(KcRealmAdminConfigurationStore realmAdminConfigurationStore,
        IServiceProvider provider)
    {
        // Validate that the realm configuration store is provided and not null.
        ArgumentNullException.ThrowIfNull(realmAdminConfigurationStore);

        // Ensure that the list of realm configurations is not null.
        ArgumentNullException.ThrowIfNull(realmAdminConfigurationStore.GetRealmsAdminConfiguration());

        // Validate each realm admin configuration to ensure all required properties are correctly set.
        foreach ( var configuration in realmAdminConfigurationStore.GetRealmsAdminConfiguration() )
        {
            configuration.Validate();
        }

        // Assign the validated configuration store to the internal field.
        _realmAdminConfigurationStore = realmAdminConfigurationStore;

        // Create a scoped service provider to resolve the logger service.
        using var scope = provider.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger>(); // Get the logger from the scoped provider.

        // Initialize the token cache to store access and refresh tokens for different realms.
        _tokensCache = new ConcurrentDictionary<string, KcCachedToken>();
    }

    /// <summary>
    /// Attempts to retrieve an admin token for the specified realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm name.</param>
    /// <param name="cancellationToken">A token used to propagate cancellation notifications.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the admin access token.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="realm"/> is null or empty.</exception>
    /// <exception cref="KcException">Thrown if the token retrieval process fails.</exception>
    public async Task<string> TryGetAdminTokenAsync(string realm, CancellationToken cancellationToken = default) =>
        string.IsNullOrWhiteSpace(realm)
            ? throw new ArgumentNullException(nameof(realm), $"{nameof(realm)} is required")
            : TryGetToken(AccessTokenCachingKey(realm)) is var cachedAccessToken &&
              !string.IsNullOrWhiteSpace(cachedAccessToken)
                ? cachedAccessToken
                : TryGetToken(RefreshTokenCachingKey(realm)) is var cachedRefreshToken &&
                  string.IsNullOrWhiteSpace(cachedRefreshToken) &&
                  await RefreshTokenAsync(realm, cachedAccessToken, cancellationToken)
                      .ConfigureAwait(false) is var token &&
                  !string.IsNullOrWhiteSpace(token)
                    ? token
                    : await GetAccessTokenAsync(realm, cancellationToken).ConfigureAwait(false) is var accessToken &&
                      !string.IsNullOrWhiteSpace(accessToken)
                        ? accessToken
                        : throw new KcException($"Failed to retrieve Keycloak realm {realm} admin token.");

    /// <summary>
    /// Retrieves an access token for the specified Keycloak realm using the configured client credentials and admin credentials.
    /// </summary>
    /// <param name="realm">The name of the Keycloak realm for which the access token is requested.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A string representing the access token if successful; otherwise, <c>null</c> if the token request fails.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="realm"/> parameter is null, empty, or consists only of whitespace.
    /// </exception>
    private async Task<string> GetAccessTokenAsync(string realm, CancellationToken cancellationToken = default)
    {
        // Ensure that the realm name is provided and not null or whitespace.
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new ArgumentNullException(nameof(realm), $"{nameof(realm)} is required");
        }

        // Retrieve the Keycloak configuration for the specified realm.
        var configuration = GetConfiguration(realm);

        // Request an access token using the configured Keycloak client and admin credentials.
        if ( await KeycloakClient(configuration.KeycloakBaseUrl).Auth
                .GetResourceOwnerPasswordTokenAsync(configuration.Realm,
                    new KcClientCredentials
                    {
                        ClientId = configuration.ClientId
                    },
                    new KcUserLogin
                    {
                        Username = configuration.RealmAdminCredentials.Username,
                        Password = configuration.RealmAdminCredentials.Password
                    }, cancellationToken: cancellationToken)
                .ConfigureAwait(false) is var adminTokenResponse && adminTokenResponse.IsError )
        {
            // Log an error if the access token request fails.
            KcLoggerMessages.Error(_logger,
                $"Unable to get {configuration.Realm} admin access token: {adminTokenResponse.ErrorMessage}",
                adminTokenResponse.Exception);
            return null;
        }

        // Cache the access token with a reduced expiry time to ensure early refresh.
        CacheToken(AccessTokenCachingKey(realm), adminTokenResponse.Response.AccessToken,
            adminTokenResponse.Response.ExpiresIn - 120);

        // Cache the refresh token with a different expiry time to support token refresh operations.
        CacheToken(RefreshTokenCachingKey(realm), adminTokenResponse.Response.RefreshToken,
            adminTokenResponse.Response.ExpiresIn - 5 * 60);

        // Return the access token.
        return adminTokenResponse.Response.AccessToken;
    }

    /// <summary>
    /// Refreshes the access token for the specified Keycloak realm using the provided refresh token.
    /// </summary>
    /// <param name="realm">The name of the Keycloak realm for which the access token is refreshed.</param>
    /// <param name="refreshToken">The refresh token used to obtain a new access token.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A string representing the refreshed access token if successful; otherwise, <c>null</c> if the refresh request fails or the refresh token is invalid.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="realm"/> parameter is null, empty, or consists only of whitespace.
    /// </exception>
    private async Task<string> RefreshTokenAsync(string realm, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // Ensure that the realm name is provided and not null or whitespace.
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new ArgumentNullException(nameof(realm), $"{nameof(realm)} is required");
        }

        // Return null if the refresh token is not provided or is empty.
        if ( string.IsNullOrWhiteSpace(refreshToken) )
        {
            return null;
        }

        // Retrieve the Keycloak configuration for the specified realm.
        var configuration = GetConfiguration(realm);

        // Request a new access token using the provided refresh token.
        if ( await KeycloakClient(configuration.KeycloakBaseUrl).Auth.RefreshAccessTokenAsync(
                    configuration.Realm,
                    new KcClientCredentials
                    {
                        ClientId = configuration.ClientId
                    },
                    refreshToken, cancellationToken: cancellationToken)
                .ConfigureAwait(false) is var adminTokenResponse && adminTokenResponse.IsError )
        {
            // Log an error if the token refresh request fails.
            KcLoggerMessages.Error(_logger,
                $"Unable to refresh {configuration.Realm} admin access token: {adminTokenResponse.ErrorMessage}",
                adminTokenResponse.Exception);
            return null;
        }

        // Cache the refreshed access token with a reduced expiry time for early refresh.
        CacheToken(AccessTokenCachingKey(realm), adminTokenResponse.Response.AccessToken,
            adminTokenResponse.Response.ExpiresIn - 120);

        // Cache the refreshed refresh token with a margin to support future refresh operations.
        CacheToken(RefreshTokenCachingKey(realm), adminTokenResponse.Response.RefreshToken,
            adminTokenResponse.Response.ExpiresIn - 5 * 60);

        // Return the refreshed access token.
        return adminTokenResponse.Response.AccessToken;
    }

    /// <summary>
    /// Attempts to retrieve a cached token.
    /// </summary>
    /// <param name="cachingKey">The caching key associated with the token.</param>
    /// <returns>The cached token value, or <c>null</c> if the token is expired or not present.</returns>
    private string TryGetToken(string cachingKey) =>
        _tokensCache.TryGetValue(cachingKey, out var token) && !token.IsExpired &&
        !string.IsNullOrWhiteSpace(token.Value)
            ? token.Value
            : null;

    /// <summary>
    /// Caches a token with the specified key and expiry time in the token cache.
    /// </summary>
    /// <param name="cachingKey">The key used to store the token in the cache.</param>
    /// <param name="token">The token value to be cached.</param>
    /// <param name="expiryInSeconds">The expiry time for the cached token in seconds. Defaults to 60 seconds if not specified or less than or equal to zero.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="cachingKey"/> or <paramref name="token"/> is null, empty, or consists only of whitespace.
    /// </exception>
    private void CacheToken(string cachingKey, string token, long expiryInSeconds = 60)
    {
        // Ensure the caching key is provided and not null or empty.
        if ( string.IsNullOrWhiteSpace(cachingKey) )
        {
            throw new ArgumentNullException(nameof(cachingKey), $"{nameof(cachingKey)} is required");
        }

        // Ensure the token value is provided and not null or empty.
        if ( string.IsNullOrWhiteSpace(token) )
        {
            throw new ArgumentNullException(nameof(token), $"{nameof(token)} is required");
        }

        // If the expiry time is zero or negative, set it to the default of 60 seconds.
        if ( expiryInSeconds <= 0 )
        {
            expiryInSeconds = 60;
        }

        // Cache the token with the specified key and expiry time.
        _tokensCache[cachingKey] = new KcCachedToken
        {
            Value = token,
            Expiry = expiryInSeconds
        };
    }

    /// <summary>
    /// Retrieves the Keycloak client instance for the specified base URL.
    /// </summary>
    private KeycloakClient KeycloakClient(string keycloakBaseUrl) => new(keycloakBaseUrl, _logger);

    /// <summary>
    /// Builds the caching key for the access token.
    /// </summary>
    private static string AccessTokenCachingKey(string realm) =>
        $"{nameof(KcRealmAdminTokenHandler)}_{realm}_access_token";

    /// <summary>
    /// Builds the caching key for the refresh token.
    /// </summary>
    private static string RefreshTokenCachingKey(string realm) =>
        $"{nameof(KcRealmAdminTokenHandler)}_{realm}_refresh_token";

    /// <summary>
    /// Retrieves the Keycloak realm admin configuration for the specified realm.
    /// </summary>
    private KcRealmAdminConfiguration GetConfiguration(string realm) =>
        _realmAdminConfigurationStore.GetRealmsAdminConfiguration()
            .FirstOrDefault(configuration => configuration.Realm == realm);
}
