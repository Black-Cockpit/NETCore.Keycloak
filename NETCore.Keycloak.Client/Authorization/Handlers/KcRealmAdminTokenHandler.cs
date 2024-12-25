using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.Authorization.Handlers;

/// <inheritdoc cref="IKcRealmAdminTokenHandler"/>
public class KcRealmAdminTokenHandler : IKcRealmAdminTokenHandler
{
    /// <summary>
    /// Tokens cache
    /// </summary>
    private readonly ConcurrentDictionary<string, KcCachedToken> _tokensCache;

    /// <summary>
    /// Realm admin configuration. <see cref="KcRealmAdminConfiguration"/>
    /// </summary>
    private readonly KcRealmAdminConfigurationStore _realmAdminConfigurationStore;

    /// <summary>
    /// Logger <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Keycloak realm admin configuration. <see cref="KcRealmAdminConfiguration"/>
    /// </summary>
    /// <param name="realm"></param>
    /// <returns></returns>
    private KcRealmAdminConfiguration GetConfiguration(string realm) => _realmAdminConfigurationStore
        .GetRealmsAdminConfiguration().FirstOrDefault(configuration => configuration.Realm == realm);

    /// <summary>
    /// Keycloak realm admin token handler
    /// </summary>
    /// <param name="realmAdminConfigurationStore"></param>
    /// <param name="provider"></param>
    public KcRealmAdminTokenHandler(KcRealmAdminConfigurationStore realmAdminConfigurationStore,
        IServiceProvider provider)
    {
        // Ensure realms admin configuration store is not null
        ArgumentNullException.ThrowIfNull(realmAdminConfigurationStore);

        // Validate all realms configuration
        ArgumentNullException.ThrowIfNull(realmAdminConfigurationStore.GetRealmsAdminConfiguration());

        foreach ( var configuration in realmAdminConfigurationStore.GetRealmsAdminConfiguration() )
        {
            configuration.Validate();
        }

        _realmAdminConfigurationStore = realmAdminConfigurationStore;

        using var scope = provider.CreateScope();

        _logger = scope.ServiceProvider.GetRequiredService<ILogger>();

        _tokensCache = new ConcurrentDictionary<string, KcCachedToken>();
    }

    /// <inheritdoc cref="IKcRealmAdminTokenHandler.TryGetAdminTokenAsync"/>
    public async Task<string> TryGetAdminTokenAsync(string realm, CancellationToken cancellationToken = default)
    {
        // Ensure realm is not null.
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new ArgumentNullException(nameof(realm), $"{nameof(realm)} is required");
        }

        // Try to get access token from cache
        if ( TryGetToken(AccessTokenCachingKey(realm)) is var cachedAccessToken &&
             !string.IsNullOrWhiteSpace(cachedAccessToken) )
        {
            return cachedAccessToken;
        }

        // Try to get refresh token from cache then refresh access token
        return TryGetToken(RefreshTokenCachingKey(realm)) is var cachedRefreshToken &&
               string.IsNullOrWhiteSpace(cachedRefreshToken) &&
               await RefreshTokenAsync(realm, cachedAccessToken, cancellationToken)
                   .ConfigureAwait(false) is var token &&
               !string.IsNullOrWhiteSpace(token)
            ? token
            // Get a new access token and cache it
            : await GetAccessTokenAsync(realm, cancellationToken).ConfigureAwait(false) is var accessToken &&
              !string.IsNullOrWhiteSpace(accessToken)
                ? accessToken
                : throw new KcException(
                    $"Failed to retrieved keycloak realm {realm} admin token");
    }

    /// <summary>
    /// Get access token
    /// </summary>
    /// <param name="realm">Realm name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<string> GetAccessTokenAsync(string realm, CancellationToken cancellationToken = default)
    {
        // Ensure realm is not null.
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new ArgumentNullException(nameof(realm), $"{nameof(realm)} is required");
        }

        // Get realm admin configuration
        var configuration = GetConfiguration(realm);

        // Get admin access token
        if ( await KeycloakClient(configuration.KeycloakBaseUrl).Auth
                .GetResourceOwnerPasswordTokenAsync(
                    configuration.Realm,
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
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger,
                    $"Unable to get {configuration.Realm} admin access token : {adminTokenResponse.ErrorMessage}",
                    adminTokenResponse.Exception);
            }
            return null;
        }

        // Cache access token
        CacheToken(AccessTokenCachingKey(realm), adminTokenResponse.Response.AccessToken,
            adminTokenResponse.Response.ExpiresIn - 120);

        // Cache refresh token
        CacheToken(RefreshTokenCachingKey(realm), adminTokenResponse.Response.RefreshToken,
            adminTokenResponse.Response.ExpiresIn - 5 * 60);

        return adminTokenResponse.Response.AccessToken;
    }

    /// <summary>
    /// Get refresh token
    /// </summary>
    /// <param name="realm">Realm name</param>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<string> RefreshTokenAsync(string realm, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // Ensure realm is not null.
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new ArgumentNullException(nameof(realm), $"{nameof(realm)} is required");
        }

        // Check if refresh token is null
        if ( string.IsNullOrWhiteSpace(refreshToken) )
        {
            return null;
        }

        // Get realm admin configuration
        var configuration = GetConfiguration(realm);

        // Get admin refresh token
        if ( await KeycloakClient(configuration.KeycloakBaseUrl).Auth.RefreshAccessTokenAsync(
                        configuration.Realm,
                        new KcClientCredentials
                        {
                            ClientId = configuration.ClientId
                        },
                        refreshToken, cancellationToken: cancellationToken)
                    .ConfigureAwait(false)
                is var adminTokenResponse && adminTokenResponse.IsError )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger,
                    $"Unable to refresh {configuration.Realm} admin access token : {adminTokenResponse.ErrorMessage}",
                    adminTokenResponse.Exception);
            }

            return null;
        }

        // Cache access token
        CacheToken(AccessTokenCachingKey(realm), adminTokenResponse.Response.AccessToken,
            adminTokenResponse.Response.ExpiresIn - 120);

        // Cache refresh token
        CacheToken(RefreshTokenCachingKey(realm), adminTokenResponse.Response.RefreshToken,
            adminTokenResponse.Response.ExpiresIn - 5 * 60);

        return adminTokenResponse.Response.AccessToken;
    }

    /// <summary>
    /// Try to get token from <see cref="_tokensCache"/>
    /// </summary>
    /// <param name="cachingKey">Caching key</param>
    /// <returns></returns>
    private string TryGetToken(string cachingKey) => _tokensCache.TryGetValue(cachingKey, out var token) &&
                                                     !token.IsExpired && !string.IsNullOrWhiteSpace(token.Value)
        ? token.Value
        : null;

    /// <summary>
    /// Add token to cache
    /// </summary>
    /// <param name="cachingKey">Token caching key</param>
    /// <param name="token">Token value</param>
    /// <param name="expiryInSeconds">Expiry time in seconds</param>
    private void CacheToken(string cachingKey, string token, long expiryInSeconds = 60)
    {
        // Ensure caching key is not null
        if ( string.IsNullOrWhiteSpace(cachingKey) )
        {
            throw new ArgumentNullException(nameof(cachingKey), $"{nameof(cachingKey)} is required");
        }

        // Ensure token key is not null
        if ( string.IsNullOrWhiteSpace(token) )
        {
            throw new ArgumentNullException(nameof(token), $"{nameof(token)} is required");
        }

        // Set expiry time if it is less or equal to zero
        if ( expiryInSeconds <= 0 )
        {
            expiryInSeconds = 60;
        }

        // Update cache if the key exists
        if ( _tokensCache.TryGetValue(cachingKey, out _) )
        {
            _tokensCache[cachingKey] = new KcCachedToken
            {
                Value = token,
                Expiry = expiryInSeconds
            };
        }
        else
        {
            // Add new entry if key does not exists
            _ = _tokensCache.TryAdd(cachingKey, new KcCachedToken
            {
                Value = token,
                Expiry = expiryInSeconds
            });
        }
    }

    /// <summary>
    /// Get keycloak client <see cref="IKeycloakClient"/>
    /// </summary>
    private KeycloakClient KeycloakClient(string keycloakBaseUrl) => new(keycloakBaseUrl, _logger);

    /// <summary>
    /// Access token caching key
    /// </summary>
    private static string AccessTokenCachingKey(string realm) =>
        $"{nameof(KcRealmAdminTokenHandler)}_{realm}_access_token";

    /// <summary>
    /// Refresh token caching key
    /// </summary>
    private static string RefreshTokenCachingKey(string realm) =>
        $"{nameof(KcRealmAdminTokenHandler)}_{realm}_refresh_token";
}
