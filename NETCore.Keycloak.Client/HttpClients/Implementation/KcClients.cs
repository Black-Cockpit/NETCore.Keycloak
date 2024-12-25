using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Requests;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcClients"/>
public class KcClients : KcClientValidator, IKcClients
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
    /// Client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcClients(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcClients.CreateAsync"/>
    public async Task<KcResponse<object>> CreateAsync(string realm, string accessToken,
        KcClient kcClient,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( kcClient == null )
        {
            throw new KcException($"{nameof(kcClient)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients", accessToken, KcRequestHandler.GetBody(kcClient));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add realm client", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.ListAsync"/>
    public async Task<KcResponse<IEnumerable<KcClient>>> ListAsync(string realm, string accessToken,
        KcClientFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        filter ??= new KcClientFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcClient>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm client", e);
            }

            return new KcResponse<IEnumerable<KcClient>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetAsync"/>
    public async Task<KcResponse<KcClient>> GetAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}", accessToken);

            using var client = new HttpClient();

            var response =
                await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcClient>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client", e);
            }

            return new KcResponse<KcClient>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.UpdateAsync"/>
    public async Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string id,
        KcClient kcClient,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( kcClient == null )
        {
            throw new KcException($"{nameof(kcClient)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{id}", accessToken,
                KcRequestHandler.GetBody(kcClient));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update realm client", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.DeleteAsync"/>
    public async Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{id}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete realm client", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GenerateNewSecretAsync"/>
    public async Task<KcResponse<KcCredentials>> GenerateNewSecretAsync(string realm,
        string accessToken, string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients/{id}/client-secret", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcCredentials>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable generate realm client new secret", e);
            }

            return new KcResponse<KcCredentials>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetSecretAsync"/>
    public async Task<KcResponse<KcCredentials>> GetSecretAsync(string realm, string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/client-secret", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcCredentials>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable get realm client secret", e);
            }

            return new KcResponse<KcCredentials>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetRotatedSecretAsync"/>
    public async Task<KcResponse<KcCredentials>> GetRotatedSecretAsync(string realm,
        string accessToken, string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/client-secret/rotated", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcCredentials>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable get realm client rotated secret", e);
            }

            return new KcResponse<KcCredentials>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.InvalidateRotatedSecretAsync"/>
    public async Task<KcResponse<KcCredentials>> InvalidateRotatedSecretAsync(string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{id}/client-secret/rotated", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcCredentials>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable invalidate realm client rotated secret", e);
            }

            return new KcResponse<KcCredentials>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetDefaultScopesAsync"/>
    public async Task<KcResponse<IEnumerable<KcClientScope>>> GetDefaultScopesAsync(string realm,
        string accessToken,
        string id, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/default-client-scopes", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcClientScope>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable list realm client default scopes", e);
            }

            return new KcResponse<IEnumerable<KcClientScope>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.AddDefaultScopesAsync"/>
    public async Task<KcResponse<object>> AddDefaultScopesAsync(string realm, string accessToken,
        string id,
        string scopeId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{id}/default-client-scopes/{scopeId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add scope to realm client default scopes", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.DeleteDefaultScopesAsync"/>
    public async Task<KcResponse<object>> DeleteDefaultScopesAsync(string realm, string accessToken,
        string id,
        string scopeId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{id}/default-client-scopes/{scopeId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete scope from realm client default scopes", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GenerateExampleAccessTokenAsync"/>
    public async Task<KcResponse<KcAccessToken>> GenerateExampleAccessTokenAsync(string realm,
        string accessToken,
        string id, string scope = null, string userId = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        var appendedToUrl = false;

        var urlBuilder =
            new StringBuilder(
                $"{_baseUrl}/{realm}/clients/{id}/evaluate-scopes/generate-example-access-token");

        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        if ( !string.IsNullOrWhiteSpace(userId) )
        {
            _ = urlBuilder.Append(appendedToUrl ? $"&userId={userId}" : $"?userId={userId}");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                urlBuilder.ToString(), accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcAccessToken>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to generate realm client example access token", e);
            }

            return new KcResponse<KcAccessToken>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GenerateExampleIdTokenAsync"/>
    public async Task<KcResponse<KcAccessToken>> GenerateExampleIdTokenAsync(string realm,
        string accessToken, string id, string scope = null, string userId = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        var appendedToUrl = false;

        var urlBuilder =
            new StringBuilder(
                $"{_baseUrl}/{realm}/clients/{id}/evaluate-scopes/generate-example-id-token");

        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        if ( !string.IsNullOrWhiteSpace(userId) )
        {
            _ = urlBuilder.Append(appendedToUrl ? $"&userId={userId}" : $"?userId={userId}");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                urlBuilder.ToString(), accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcAccessToken>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to generate realm client example id token", e);
            }

            return new KcResponse<KcAccessToken>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GenerateExampleUserInfoAsync"/>
    public async Task<KcResponse<object>> GenerateExampleUserInfoAsync(string realm,
        string accessToken, string id,
        string scope = null, string userId = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        var appendedToUrl = false;

        var urlBuilder =
            new StringBuilder(
                $"{_baseUrl}/{realm}/clients/{id}/evaluate-scopes/generate-example-userinfo");

        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        if ( !string.IsNullOrWhiteSpace(userId) )
        {
            _ = urlBuilder.Append(appendedToUrl ? $"&userId={userId}" : $"?userId={userId}");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                urlBuilder.ToString(), accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to generate realm client example user info", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetProtocolMappersAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetProtocolMappersAsync(
        string realm,
        string accessToken, string id, string scope = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        var url = string.IsNullOrWhiteSpace(scope)
            ? $"{_baseUrl}/{realm}/clients/{id}/evaluate-scopes/protocol-mappers"
            : $"{_baseUrl}/{realm}/clients/{id}/evaluate-scopes/protocol-mappers?scope={scope}";

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                url, accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcProtocolMapper>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable get to realm client protocol mappers", e);
            }

            return new KcResponse<IEnumerable<KcProtocolMapper>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetScopedProtocolMappersInContainerAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>>
        GetScopedProtocolMappersInContainerAsync(string realm,
        string accessToken, string id, string roleContainerId, string scope = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        var appendedToUrl = false;

        var urlBuilder =
            new StringBuilder(
                $"{_baseUrl}/{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/granted");

        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        if ( !string.IsNullOrWhiteSpace(roleContainerId) )
        {
            _ = urlBuilder.Append(appendedToUrl
                ? $"&roleContainerId={roleContainerId}"
                : $"?roleContainerId={roleContainerId}");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                urlBuilder.ToString(), accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcProtocolMapper>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client scoped protocol mappers in role container",
                    e);
            }

            return new KcResponse<IEnumerable<KcProtocolMapper>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetUnScopedProtocolMappersInContainerAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>>
        GetUnScopedProtocolMappersInContainerAsync(
        string realm, string accessToken, string id, string roleContainerId,
        string scope = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        var appendedToUrl = false;

        var urlBuilder =
            new StringBuilder(
                $"{_baseUrl}/{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/not-granted");

        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        if ( !string.IsNullOrWhiteSpace(roleContainerId) )
        {
            _ = urlBuilder.Append(appendedToUrl
                ? $"&roleContainerId={roleContainerId}"
                : $"?roleContainerId={roleContainerId}");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                urlBuilder.ToString(), accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcProtocolMapper>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger,
                    "Unable to get realm client unscoped protocol mappers in role container", e);
            }

            return new KcResponse<IEnumerable<KcProtocolMapper>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetAuthorizationManagementPermissionAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> GetAuthorizationManagementPermissionAsync(
        string realm,
        string accessToken, string id, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/management/permissions", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcPermissionManagement>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable get realm client authorization management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.SetAuthorizationManagementPermissionAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> SetAuthorizationManagementPermissionAsync(
        string realm,
        string accessToken, string id, KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( permissionManagement == null )
        {
            throw new KcException($"{nameof(permissionManagement)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{id}/management/permissions", accessToken,
                KcRequestHandler.GetBody(permissionManagement));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcPermissionManagement>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to set realm client authorization management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.RegisterNodeAsync"/>
    public async Task<KcResponse<object>> RegisterNodeAsync(string realm, string accessToken,
        string id,
        IDictionary<string, object> formParams, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( formParams == null )
        {
            throw new KcException($"{nameof(formParams)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients/{id}/nodes", accessToken,
                KcRequestHandler.GetBody(formParams));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to register realm client new cluster node", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.DeleteNodeAsync"/>
    public async Task<KcResponse<object>> DeleteNodeAsync(string realm, string accessToken,
        string id, string nodeName,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( string.IsNullOrWhiteSpace(nodeName) )
        {
            throw new KcException($"{nameof(nodeName)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{id}/nodes/{nodeName}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete realm client cluster node", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.CountOfflineSessionsAsync"/>
    public async Task<KcResponse<object>> CountOfflineSessionsAsync(string realm,
        string accessToken, string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/offline-session-count", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client offline sessions count", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetOfflineSessionsAsync"/>
    public async Task<KcResponse<IEnumerable<KcSession>>> GetOfflineSessionsAsync(string realm,
        string accessToken,
        string id, KcFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/offline-sessions{filter.BuildQuery()}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcSession>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client offline sessions", e);
            }

            return new KcResponse<IEnumerable<KcSession>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetOptionalScopesAsync"/>
    public async Task<KcResponse<IEnumerable<KcClientScope>>> GetOptionalScopesAsync(string realm,
        string accessToken,
        string id, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/optional-client-scopes", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcClientScope>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client optional scopes", e);
            }

            return new KcResponse<IEnumerable<KcClientScope>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.AddOptionalScopeAsync"/>
    public async Task<KcResponse<object>> AddOptionalScopeAsync(string realm, string accessToken,
        string id,
        string scopeId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{id}/optional-client-scopes/{scopeId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add scope to realm client optional scopes", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.DeleteOptionalScopeAsync"/>
    public async Task<KcResponse<object>> DeleteOptionalScopeAsync(string realm, string accessToken,
        string id,
        string scopeId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{id}/optional-client-scopes/{scopeId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete scope from realm client optional scopes", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.PushRevocationAsync"/>
    public async Task<KcResponse<KcGlobalRequestResult>> PushRevocationAsync(string realm,
        string accessToken,
        string id, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients/{id}/push-revocation", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcGlobalRequestResult>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to push realm client revocation", e);
            }

            return new KcResponse<KcGlobalRequestResult>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetRegistrationAccessTokenAsync"/>
    public async Task<KcResponse<KcClient>> GetRegistrationAccessTokenAsync(string realm,
        string accessToken, string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients/{id}/registration-access-token", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcClient>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to create realm client registration access token", e);
            }

            return new KcResponse<KcClient>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetServiceAccountUserAsync"/>
    public async Task<KcResponse<KcUser>> GetServiceAccountUserAsync(string realm,
        string accessToken, string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/service-account-user", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcUser>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client service account user", e);
            }

            return new KcResponse<KcUser>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.CountSessionsAsync"/>
    public async Task<KcResponse<object>> CountSessionsAsync(string realm, string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/session-count", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client sessions count", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.TestAvailableNodesAsync"/>
    public async Task<KcResponse<KcGlobalRequestResult>> TestAvailableNodesAsync(string realm,
        string accessToken,
        string id, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/test-nodes-available", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcGlobalRequestResult>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to test realm client available nodes", e);
            }

            return new KcResponse<KcGlobalRequestResult>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClients.GetUsersSessionsAsync"/>
    public async Task<KcResponse<IEnumerable<KcSession>>> GetUsersSessionsAsync(string realm,
        string accessToken,
        string id, KcFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{id}/user-sessions{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcSession>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client users sessions", e);
            }

            return new KcResponse<IEnumerable<KcSession>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
