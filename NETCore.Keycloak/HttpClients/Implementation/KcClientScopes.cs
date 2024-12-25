using Microsoft.Extensions.Logging;
using NETCore.Keycloak.HttpClients.Abstraction;
using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.ClientScope;
using NETCore.Keycloak.Requests;
using NETCore.Keycloak.Utils;

namespace NETCore.Keycloak.HttpClients.Implementation;

/// <inheritdoc cref="IKcClientScopes"/>
public class KcClientScopes : KcClientValidator, IKcClientScopes
{
    /// <summary>
    /// Logger <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    private readonly string _baseUrl;

    /// <summary>
    /// Client scopes client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcClientScopes(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcClientScopes.CreateAsync"/>
    public async Task<KcResponse<KcClientScope>> CreateAsync(string realm, string accessToken,
        KcClientScope scope,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( scope == null )
        {
            throw new KcException($"{nameof(scope)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/client-scopes", accessToken, KcRequestHandler.GetBody(scope));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcClientScope>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to create client scope", e);
            }

            return new KcResponse<KcClientScope>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientScopes.ListAsync"/>
    public async Task<KcResponse<IEnumerable<KcClientScope>>> ListAsync(string realm,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcClientScope>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list client scopes", e);
            }

            return new KcResponse<IEnumerable<KcClientScope>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientScopes.GetAsync"/>
    public async Task<KcResponse<KcClientScope>> GetAsync(string realm, string accessToken,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcClientScope>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get client scopes", e);
            }

            return new KcResponse<KcClientScope>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientScopes.UpdateAsync"/>
    public async Task<KcResponse<KcClientScope>> UpdateAsync(string realm, string accessToken,
        string scopeId,
        KcClientScope scope,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        if ( scope == null )
        {
            throw new KcException($"{nameof(scope)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/client-scopes", accessToken, KcRequestHandler.GetBody(scope));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcClientScope>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update client scope", e);
            }

            return new KcResponse<KcClientScope>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientScopes.DeleteAsync"/>
    public async Task<KcResponse<object>> DeleteAsync(string realm, string accessToken,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete client scopes", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
