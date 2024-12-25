using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Requests;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcGroups"/>
public class KcGroups : KcClientValidator, IKcGroups
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
    /// Groups client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcGroups(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcGroups.CreateAsync"/>
    public async Task<KcResponse<KcGroup>> CreateAsync(string realm, string accessToken,
        KcGroup kcGroup,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( kcGroup == null )
        {
            throw new KcException($"{nameof(kcGroup)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/groups", accessToken, KcRequestHandler.GetBody(kcGroup));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcGroup>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to create realm group", e);
            }

            return new KcResponse<KcGroup>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.ListAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> ListAsync(string realm, string accessToken,
        KcGroupFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        filter ??= new KcGroupFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcGroup>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm group", e);
            }

            return new KcResponse<IEnumerable<KcGroup>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.CountAsync"/>
    public async Task<KcResponse<object>> CountAsync(string realm, string accessToken,
        KcGroupFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        filter ??= new KcGroupFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/count{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to count realm group", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.GetAsync"/>
    public async Task<KcResponse<KcGroup>> GetAsync(string realm, string accessToken, string id,
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
                $"{_baseUrl}/{realm}/groups/{id}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcGroup>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm group", e);
            }

            return new KcResponse<KcGroup>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.UpdateAsync"/>
    public async Task<KcResponse<KcGroup>> UpdateAsync(string realm, string accessToken, string id,
        KcGroup kcGroup,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( kcGroup == null )
        {
            throw new KcException($"{nameof(kcGroup)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/groups/{id}", accessToken, KcRequestHandler.GetBody(kcGroup));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcGroup>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update realm group", e);
            }

            return new KcResponse<KcGroup>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.DeleteAsync"/>
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
                $"{_baseUrl}/{realm}/groups/{id}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete realm group", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.SetOrCreateChildAsync"/>
    public async Task<KcResponse<KcGroup>> SetOrCreateChildAsync(string realm, string accessToken,
        string id,
        KcGroup kcGroup, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( kcGroup == null )
        {
            throw new KcException($"{nameof(kcGroup)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/groups/{id}/children", accessToken,
                KcRequestHandler.GetBody(kcGroup));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcGroup>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to et sor create child for realm group", e);
            }

            return new KcResponse<KcGroup>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.GetAuthorizationManagementPermissionAsync"/>
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
                $"{_baseUrl}/{realm}/groups/{id}/management/permissions", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcPermissionManagement>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm group management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.SetAuthorizationManagementPermissionAsync"/>
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
                $"{_baseUrl}/{realm}/groups/{id}/management/permissions", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to set realm group management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcGroups.GetMembersAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> GetMembersAsync(string realm,
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
                $"{_baseUrl}/{realm}/groups/{id}/members", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcUser>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm group members", e);
            }

            return new KcResponse<IEnumerable<KcUser>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
