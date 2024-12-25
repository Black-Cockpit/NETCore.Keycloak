using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.AttackDetection;
using NETCore.Keycloak.Client.Requests;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcAttackDetection"/>
public class KcAttackDetection : KcClientValidator, IKcAttackDetection
{
    /// <summary>
    /// Logger <see cref="ILogger{KcAttackDetection}"/>
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Keycloak base URL
    /// </summary>
    private readonly string _baseUrl;

    /// <summary>
    /// Attack detection client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcAttackDetection(string baseUrl, ILogger logger)
    {
        if ( string.IsNullOrWhiteSpace(baseUrl) )
        {
            throw new KcException($"{nameof(baseUrl)} is required");
        }

        // Remove last "/" from base url
        _baseUrl = baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? baseUrl.Remove(baseUrl.Length - 1, 1)
            : baseUrl;

        _logger = logger;
    }

    /// <inheritdoc cref="IKcAttackDetection.DeleteUsersLoginFailure"/>
    public async Task<KcResponse<object>> DeleteUsersLoginFailure(string realm, string accessToken,
        string userId = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        try
        {
            var url = string.IsNullOrWhiteSpace(userId)
                ? $"{_baseUrl}/{realm}/attack-detection/brute-force/users"
                : $"{_baseUrl}/{realm}/attack-detection/brute-force/users/{userId}";

            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete, url, accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to clear users login failure", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcAttackDetection.GetUserStatus"/>
    public async Task<KcResponse<KcUserBruteForceStatus>> GetUserStatus(string realm,
        string accessToken, string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/attack-detection/brute-force/users/{userId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcUserBruteForceStatus>(response,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable get user {userId} login failure status", e);
            }

            return new KcResponse<KcUserBruteForceStatus>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
