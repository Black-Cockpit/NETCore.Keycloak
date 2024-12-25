using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Requests;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcRoles"/>
public class KcRoles : KcClientValidator, IKcRoles
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
    /// Roles client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcRoles(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcRoles.CreateAsync"/>
    public async Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcRole role,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( role == null )
        {
            throw new KcException($"{nameof(role)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/roles", accessToken, KcRequestHandler.GetBody(role));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to create realm role", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.ListAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListAsync(string realm, string accessToken,
        KcFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm roles", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetAsync"/>
    public async Task<KcResponse<KcRole>> GetAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles/{name}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcRole>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm role", e);
            }

            return new KcResponse<KcRole>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.IsRolesExistsAsync"/>
    public async Task<KcResponse<bool>> IsRolesExistsAsync(string realm, string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        var roleResponse = await GetAsync(realm, accessToken, name, cancellationToken)
            .ConfigureAwait(false);

        return roleResponse.IsError switch
        {
            true when roleResponse.ErrorMessage.Contains("Could not find role",
                StringComparison.OrdinalIgnoreCase) => new
                KcResponse<bool>
                {
                    Response = false
                },
            false when !string.IsNullOrWhiteSpace(roleResponse.Response?.Id) => new KcResponse<bool>
            {
                Response = true
            },
            _ => new KcResponse<bool>
            {
                IsError = roleResponse.IsError,
                Exception = roleResponse.Exception,
                ErrorMessage = roleResponse.ErrorMessage
            }
        };
    }

    /// <inheritdoc cref="IKcRoles.GetByIdAsync"/>
    public async Task<KcResponse<KcRole>> GetByIdAsync(string realm, string accessToken, string id,
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcRole>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm role by id", e);
            }

            return new KcResponse<KcRole>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.UpdateAsync"/>
    public async Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string name,
        KcRole role,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        if ( role == null )
        {
            throw new KcException($"{nameof(role)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/roles/{name}", accessToken, KcRequestHandler.GetBody(role));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update realm role", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.UpdateByIdAsync"/>
    public async Task<KcResponse<KcRole>> UpdateByIdAsync(string realm, string accessToken,
        string id, KcRole role,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( role == null )
        {
            throw new KcException($"{nameof(role)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/roles-by-id/{id}", accessToken,
                KcRequestHandler.GetBody(role));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcRole>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update realm role by id", e);
            }

            return new KcResponse<KcRole>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.DeleteAsync"/>
    public async Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/roles/{name}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete realm role", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.DeleteByIdAsync"/>
    public async Task<KcResponse<object>> DeleteByIdAsync(string realm, string accessToken,
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete realm role by id", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.AddCompositeAsync"/>
    public async Task<KcResponse<object>> AddCompositeAsync(string realm, string accessToken,
        string name,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
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
                $"{_baseUrl}/{realm}/roles/{name}/composites", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to add realm role composites", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.AddCompositeByIdAsync"/>
    public async Task<KcResponse<object>> AddCompositeByIdAsync(string realm, string accessToken,
        string id,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}/composites", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to add realm role composites by role id", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.ListCompositeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeAsync(string realm,
        string accessToken, string name,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles/{name}/composites", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm role composites", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.ListCompositeByIdAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeByIdAsync(string realm,
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}/composites", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm role composites by role id", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.DeleteCompositeAsync"/>
    public async Task<KcResponse<object>> DeleteCompositeAsync(string realm, string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/roles/{name}/composites", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete realm composite role", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.DeleteCompositeByIdAsync"/>
    public async Task<KcResponse<object>> DeleteCompositeByIdAsync(string realm, string accessToken,
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}/composites", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete realm composite role by role id", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetClientLevelCompositesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesAsync(string realm,
        string accessToken,
        string name, string clientId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles/{name}/composites/clients/{clientId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get role client level composites", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetClientLevelCompositesByIdAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesByIdAsync(
        string realm,
        string accessToken, string id, string clientId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(id) )
        {
            throw new KcException($"{nameof(id)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles-by-id/{id}/composites/clients/{clientId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get role client level composites by role id", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetRealmLevelCompositesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesAsync(string realm,
        string accessToken,
        string name, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles/{name}/composites/realm", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get role realm level composites", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetRealmLevelCompositesByIdAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesByIdAsync(
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}/composites/realm", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get role realm level composites by role id", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetGroupsAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsAsync(string realm,
        string accessToken, string name,
        KcFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles/{name}/groups{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcGroup>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm role groups", e);
            }

            return new KcResponse<IEnumerable<KcGroup>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsAsync(
        string realm,
        string accessToken, string name, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles/{name}/management/permissions", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcPermissionManagement>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm role management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetAuthorizationPermissionsByIdAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsByIdAsync(
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}/management/permissions", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcPermissionManagement>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm role management permission by role id", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.SetAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsAsync(
        string realm,
        string accessToken, string name, KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        if ( permissionManagement == null )
        {
            throw new KcException($"{nameof(permissionManagement)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/roles/{name}/management/permissions", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to set realm role management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.SetAuthorizationPermissionsByIdAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsByIdAsync(
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
                $"{_baseUrl}/{realm}/roles-by-id/{id}/management/permissions", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to set realm role management permission by role id", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.CreateClientRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> GetUserInRoleAsync(string realm,
        string accessToken, string name,
        KcFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/roles/{name}/users{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcUser>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm role users", e);
            }

            return new KcResponse<IEnumerable<KcUser>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.CreateClientRoleAsync"/>
    public async Task<KcResponse<object>> CreateClientRoleAsync(string realm, string accessToken,
        string clientId,
        KcRole role, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( role == null )
        {
            throw new KcException($"{nameof(role)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles", accessToken,
                KcRequestHandler.GetBody(role));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to create client role", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.ListClientRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRoleAsync(string realm,
        string accessToken,
        string clientId, KcFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list client roles", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetClientRolesAsync"/>
    public async Task<KcResponse<KcRole>> GetClientRolesAsync(string realm, string accessToken,
        string clientId,
        string name, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcRole>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get client role", e);
            }

            return new KcResponse<KcRole>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.IsClientRoleExistsAsync"/>
    public async Task<KcResponse<bool>> IsClientRoleExistsAsync(string realm, string accessToken,
        string clientId,
        string name, CancellationToken cancellationToken = default)
    {
        var roleResponse =
            await GetClientRolesAsync(realm, accessToken, clientId, name, cancellationToken)
                .ConfigureAwait(false);

        return roleResponse.IsError switch
        {
            true when roleResponse.ErrorMessage.Contains("Could not find role", StringComparison.OrdinalIgnoreCase) =>
                new KcResponse<bool>
                {
                    Response = false
                },
            false when !string.IsNullOrWhiteSpace(roleResponse.Response?.Id) => new KcResponse<bool>
            {
                Response = true
            },
            _ => new KcResponse<bool>
            {
                IsError = roleResponse.IsError,
                Exception = roleResponse.Exception,
                ErrorMessage = roleResponse.ErrorMessage
            }
        };
    }

    /// <inheritdoc cref="IKcRoles.UpdateClientRoleAsync"/>
    public async Task<KcResponse<object>> UpdateClientRoleAsync(string realm, string accessToken,
        string clientId,
        string name, KcRole role, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        if ( role == null )
        {
            throw new KcException($"{nameof(role)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}", accessToken,
                KcRequestHandler.GetBody(role));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update client role", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.DeleteClientRoleAsync"/>
    public async Task<KcResponse<object>> DeleteClientRoleAsync(string realm, string accessToken,
        string clientId,
        string name, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to delete client role", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.AddClientRoleToCompositeAsync"/>
    public async Task<KcResponse<object>> AddClientRoleToCompositeAsync(string realm,
        string accessToken,
        string clientId, string name, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
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
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}/composites", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to add composite to the client roles", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetClientCompositeRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetClientCompositeRolesAsync(string realm,
        string accessToken,
        string clientId, string name, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}/composites", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get client composite roles", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.RemoveClientRoleFromCompositeAsync"/>
    public async Task<KcResponse<object>> RemoveClientRoleFromCompositeAsync(string realm,
        string accessToken,
        string clientId, string name, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}/composites", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable remove composite from the client roles", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetGroupsInClientRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsInClientRoleAsync(string realm,
        string accessToken,
        string clientId, string name, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}/groups{filter.BuildQuery()}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcGroup>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list client role groups", e);
            }

            return new KcResponse<IEnumerable<KcGroup>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetClientRoleAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>>
        GetClientRoleAuthorizationPermissionsAsync(string realm,
        string accessToken, string clientId, string name,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}/management/permissions",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcPermissionManagement>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get client role management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.SetClientRoleAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>>
        SetClientRoleAuthorizationPermissionsAsync(string realm,
        string accessToken, string clientId, string name,
        KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        if ( permissionManagement == null )
        {
            throw new KcException($"{nameof(permissionManagement)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}/management/permissions",
                accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to set client role management permission", e);
            }

            return new KcResponse<KcPermissionManagement>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcRoles.GetUsersInClientRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> GetUsersInClientRoleAsync(string realm,
        string accessToken,
        string clientId, string name, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(name) )
        {
            throw new KcException($"{nameof(name)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/roles/{name}/users{filter.BuildQuery()}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcUser>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get client role users", e);
            }

            return new KcResponse<IEnumerable<KcUser>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
