using Microsoft.Extensions.Logging;
using NETCore.Keycloak.HttpClients.Abstraction;
using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.Common;
using NETCore.Keycloak.Models.Roles;
using NETCore.Keycloak.Requests;
using NETCore.Keycloak.Utils;

namespace NETCore.Keycloak.HttpClients.Implementation;

/// <inheritdoc cref="IKcScopeMappings"/>
public class KcScopeMappings : KcClientValidator, IKcScopeMappings
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
    /// Scope mappings client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcScopeMappings(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcScopeMappings.AddClientRolesToScopeAsync"/>
    public async Task<KcResponse<object>> AddClientRolesToScopeAsync(string realm,
        string accessToken, string scopeId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
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
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}",
                accessToken, KcRequestHandler.GetBody(roles));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add client-level roles to the client’s scope", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToScopeAsync(
        string realm,
        string accessToken, string scopeId, string clientId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}",
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
                KcLoggerMessages.Error(_logger, "Unable to get the roles associated with a client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveClientRolesFromScopeAsync"/>
    public async Task<KcResponse<object>> RemoveClientRolesFromScopeAsync(string realm,
        string accessToken,
        string scopeId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to remove client-level roles from the client’s scope", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAvailableForScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForScopeAsync(
        string realm,
        string accessToken, string scopeId, string clientId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}/available",
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
                    "Unable to list the roles for the client that can be associated with the client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListCompositeClientRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>>
        ListCompositeClientRolesAssociatedToScopeAsync(string realm,
        string accessToken, string scopeId, string clientId, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}/composite{filter.BuildQuery()}",
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
                    "Unable to list the roles for the client that are associated with the client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.AddRolesToScopeAsync"/>
    public async Task<KcResponse<object>> AddRolesToScopeAsync(string realm, string accessToken,
        string scopeId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
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
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm", accessToken,
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
                KcLoggerMessages.Error(_logger,
                    "Unable to add a set of realm-level roles to the client’s scope", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToScopeAsync(string realm,
        string accessToken,
        string scopeId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm", accessToken);

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
                    "Unable to get realm-level roles associated with the client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveRolesFromScopeAsync"/>
    public async Task<KcResponse<object>> RemoveRolesFromScopeAsync(string realm,
        string accessToken, string scopeId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger,
                    "Unable to remove a set of realm-level roles from the client’s scope", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListRolesAvailableForScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForScopeAsync(string realm,
        string accessToken,
        string scopeId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm/available",
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
                    "Unable to list realm-level roles that are available to attach to this client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListCompositeRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeRolesAssociatedToScopeAsync(
        string realm,
        string accessToken, string scopeId, KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(scopeId) )
        {
            throw new KcException($"{nameof(scopeId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm/composite{filter.BuildQuery()}",
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
                    "Unable to list effective realm-level roles associated with the client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.AddClientRolesToClientScopeAsync"/>
    public async Task<KcResponse<object>> AddClientRolesToClientScopeAsync(string realm,
        string accessToken,
        string clientId, string clientName, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientName) )
        {
            throw new KcException($"{nameof(clientName)} is required");
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
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}",
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
                KcLoggerMessages.Error(_logger, "Unable to add client-level roles to the client’s scope", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAssociatedToClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToClientScopeAsync(
        string realm,
        string accessToken, string clientId, string clientName,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientName) )
        {
            throw new KcException($"{nameof(clientName)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}",
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
                KcLoggerMessages.Error(_logger, "Unable to get the roles associated with a client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveClientRolesFromClientScopeAsync"/>
    public async Task<KcResponse<object>> RemoveClientRolesFromClientScopeAsync(string realm,
        string accessToken,
        string clientId, string clientName, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientName) )
        {
            throw new KcException($"{nameof(clientName)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to remove client-level roles from the client’s scope", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAvailableForClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForClientScopeAsync(
        string realm,
        string accessToken, string clientId, string clientName,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientName) )
        {
            throw new KcException($"{nameof(clientName)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}/available",
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
                    "Unable to list the roles available for the client that can be associated with the client’s scope",
                    e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListCompositeClientRolesAssociatedToClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>>
        ListCompositeClientRolesAssociatedToClientScopeAsync(
        string realm, string accessToken, string clientId, string clientName,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(clientName) )
        {
            throw new KcException($"{nameof(clientName)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}/composite{filter.BuildQuery()}",
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
                    "Unable to list effective roles for the client that are associated with the client’s scope",
                    e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.AddRolesToScopeClientAsync"/>
    public async Task<KcResponse<object>> AddRolesToScopeClientAsync(string realm,
        string accessToken, string clientId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
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
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/realm", accessToken,
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
                KcLoggerMessages.Error(_logger, "Unable to add a set of realm-level roles to the client’s scope", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.ListRolesAssociatedToClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToClientScopeAsync(
        string realm,
        string accessToken, string clientId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/realm", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcRole>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm-level roles associated with the client’s scope",
                    e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveRolesFromClientScopeAsync"/>
    public async Task<KcResponse<object>> RemoveRolesFromClientScopeAsync(string realm,
        string accessToken,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/realm", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to remove a set of realm-level roles from the client’s scope",
                    e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForClientScopeAsync(
        string realm,
        string accessToken, string clientId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/realm/available",
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
                    "Unable to list realm-level roles that are available to attach to this client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcScopeMappings"/>
    public async Task<KcResponse<IEnumerable<KcRole>>>
        ListCompositeRolesAssociatedToClientScopeAsync(string realm,
        string accessToken, string clientId, KcFilter filter = null,
        CancellationToken cancellationToken = default)
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
                $"{_baseUrl}/{realm}/clients/{clientId}/scope-mappings/realm/composite{filter.BuildQuery()}",
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
                    "Unable to list effective realm-level roles associated with the client’s scope", e);
            }

            return new KcResponse<IEnumerable<KcRole>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
