using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Requests;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcClientRoleMappings"/>
public class KcClientRoleMappings : KcClientValidator, IKcClientRoleMappings
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
    /// Client role mapping client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcClientRoleMappings(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcClientRoleMappings.MapClientRolesToGroupAsync"/>
    public async Task<KcResponse<object>> MapClientRolesToGroupAsync(string realm,
        string accessToken, string groupId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

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
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}",
                accessToken,
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
                KcLoggerMessages.Error(_logger, $"Unable to assign client {clientId} roles to group", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupMappedClientRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetGroupMappedClientRolesAsync(string realm,
        string accessToken,
        string groupId, string clientId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}",
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
                KcLoggerMessages.Error(_logger, "Unable to get group client roles", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupMappedClientRolesAsync"/>
    public async Task<KcResponse<object>> DeleteGroupClientRoleMappingsAsync(string realm,
        string accessToken,
        string groupId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

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
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}",
                accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to delete client roles assigned to group", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupAvailableClientRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetGroupAvailableClientRolesAsync(
        string realm,
        string accessToken, string groupId, string clientId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}/available",
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
                KcLoggerMessages.Error(_logger,
                    "Unable to get available client-level roles that can be assigned to group", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupCompositeClientRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetGroupCompositeClientRolesAsync(string realm,
        string accessToken, string groupId, string clientId, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}/composite",
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
                KcLoggerMessages.Error(_logger, "Unable to get effective client-level roles assigned to group", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.MapClientRolesToUserAsync"/>
    public async Task<KcResponse<object>> MapClientRolesToUserAsync(string realm,
        string accessToken, string userId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

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
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to assign client roles to user", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetUserMappedClientRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetUserMappedClientRolesAsync(string realm,
        string accessToken,
        string userId, string clientId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get user client roles", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.DeleteUserClientRoleMappingsAsync"/>
    public async Task<KcResponse<object>> DeleteUserClientRoleMappingsAsync(string realm,
        string accessToken,
        string userId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

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
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to delete client roles assigned to user", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetUserAvailableClientRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetUserAvailableClientRolesAsync(
        string realm,
        string accessToken, string userId, string clientId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}/available",
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
                KcLoggerMessages.Error(_logger,
                    "Unable to get available client-level roles that can be assigned to user", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetUserCompositeClientRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetUserCompositeClientRolesAsync(
        string realm,
        string accessToken, string userId, string clientId, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}/composite",
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
                KcLoggerMessages.Error(_logger, "Unable to get effective client-level roles assigned to user", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
