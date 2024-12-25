using Microsoft.Extensions.Logging;
using NETCore.Keycloak.HttpClients.Abstraction;
using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.Common;
using NETCore.Keycloak.Models.Roles;
using NETCore.Keycloak.Requests;
using NETCore.Keycloak.Utils;

namespace NETCore.Keycloak.HttpClients.Implementation;

/// <inheritdoc cref="IKcRoleMappings"/>
public class KcRoleMappings : KcClientValidator, IKcRoleMappings
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
    /// Role mapper client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcRoleMappings(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcRoleMappings.GetGroupRoleMappingsAsync"/>
    public async Task<KcResponse<KcRoleMapping>> GetGroupRoleMappingsAsync(string realm,
        string accessToken,
        string groupId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcRoleMapping>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get group role mappings", e);
            }

            return new KcResponse<KcRoleMapping>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.AddGroupRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> AddGroupRealmRoleMappingsAsync(string realm,
        string accessToken,
        string groupId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        if ( roles == null )
        {
            throw new KcException($"{nameof(roles)} is required");
        }

        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/realm", accessToken,
                KcRequestHandler.GetBody(roles));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add group realm role mappings", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.ListGroupRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListGroupRealmRoleMappingsAsync(string realm,
        string accessToken,
        string groupId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/realm", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get group realm role mappings", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.DeleteGroupRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> DeleteGroupRealmRoleMappingsAsync(string realm,
        string accessToken,
        string groupId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        if ( roles == null )
        {
            throw new KcException($"{nameof(roles)} is required");
        }

        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/realm", accessToken,
                KcRequestHandler.GetBody(roles));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete group realm role mappings", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.ListGroupAvailableRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListGroupAvailableRealmRoleMappingsAsync(
        string realm,
        string accessToken, string groupId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/realm/available", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get group realm available role mappings", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.ListGroupEffectiveRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListGroupEffectiveRealmRoleMappingsAsync(
        string realm,
        string accessToken, string groupId, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/realm/composite{filter.BuildQuery()}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get group realm composite role mappings", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.GetUserRoleMappingsAsync"/>
    public async Task<KcResponse<KcRoleMapping>> GetUserRoleMappingsAsync(string realm,
        string accessToken,
        string userId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcRoleMapping>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get user role mappings", e);
            }

            return new KcResponse<KcRoleMapping>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.AddUserRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> AddUserRealmRoleMappingsAsync(string realm,
        string accessToken, string userId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( roles == null )
        {
            throw new KcException($"{nameof(roles)} is required");
        }

        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/realm", accessToken,
                KcRequestHandler.GetBody(roles));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add user realm role mappings", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.ListUserRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListUserRealmRoleMappingsAsync(string realm,
        string accessToken,
        string userId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/realm", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get user realm role mappings", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.DeleteUserRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> DeleteUserRealmRoleMappingsAsync(string realm,
        string accessToken,
        string userId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( roles == null )
        {
            throw new KcException($"{nameof(roles)} is required");
        }

        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/realm", accessToken,
                KcRequestHandler.GetBody(roles));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete user realm role mappings", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.ListUserAvailableRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListUserAvailableRealmRoleMappingsAsync(
        string realm,
        string accessToken, string userId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/realm/available", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get user realm available role mappings", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoleMappings.ListUserEffectiveRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListUserEffectiveRealmRoleMappingsAsync(
        string realm, string accessToken, string userId, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/realm/composite{filter.BuildQuery()}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get user realm composite role mappings", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
