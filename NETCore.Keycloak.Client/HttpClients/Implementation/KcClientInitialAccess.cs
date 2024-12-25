using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Requests;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcClientInitialAccess"/>
public class KcClientInitialAccess : KcClientValidator, IKcClientInitialAccess
{
    /// <summary>
    /// Logger <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Keycloak base URL
    /// </summary>
    private readonly string _baseUrl;

    /// <summary>
    /// Initial access client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcClientInitialAccess(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcClientInitialAccess.CreateInitialAccessTokenAsync"/>
    public async Task<KcResponse<KcClientInitialAccessModel>> CreateInitialAccessTokenAsync(
        string realm,
        string accessToken, KcCreateClientInitialAccess access,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( access == null )
        {
            throw new KcException($"{nameof(access)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients-initial-access", accessToken,
                KcRequestHandler.GetBody(access));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcClientInitialAccessModel>(response,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable create client initial access token", e);
            }

            return new KcResponse<KcClientInitialAccessModel>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientInitialAccess.GetInitialAccess"/>
    public async Task<KcResponse<IEnumerable<KcClientInitialAccessModel>>> GetInitialAccess(
        string realm,
        string accessToken, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients-initial-access", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcClientInitialAccessModel>>(
                response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list client initial access tokens", e);
            }

            return new KcResponse<IEnumerable<KcClientInitialAccessModel>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
