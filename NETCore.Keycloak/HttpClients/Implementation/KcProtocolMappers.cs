using Microsoft.Extensions.Logging;
using NETCore.Keycloak.HttpClients.Abstraction;
using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.ClientScope;
using NETCore.Keycloak.Models.KcEnum;
using NETCore.Keycloak.Requests;
using NETCore.Keycloak.Utils;

namespace NETCore.Keycloak.HttpClients.Implementation;

/// <inheritdoc cref="IKcProtocolMappers"/>
public class KcProtocolMappers : KcClientValidator, IKcProtocolMappers
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
    /// Protocol mappers client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcProtocolMappers(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcProtocolMappers.AddMappersAsync"/>
    public async Task<KcResponse<object>> AddMappersAsync(string realm, string accessToken,
        string clientScopeId,
        IEnumerable<KcProtocolMapper> protocolMappers,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientScopeId) )
        {
            throw new KcException($"{nameof(clientScopeId)} is required");
        }

        if ( protocolMappers == null )
        {
            throw new KcException($"{nameof(protocolMappers)} is required");
        }

        if ( !protocolMappers.Any() )
        {
            return new KcResponse<object>();
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/add-models",
                accessToken,
                KcRequestHandler.GetBody(protocolMappers));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add realm client scope protocol mappers", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.AddMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> AddMapperAsync(string realm, string accessToken,
        string clientScopeId, KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientScopeId) )
        {
            throw new KcException($"{nameof(clientScopeId)} is required");
        }

        if ( protocolMapper == null )
        {
            throw new KcException($"{nameof(protocolMapper)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models",
                accessToken,
                KcRequestHandler.GetBody(protocolMapper));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcProtocolMapper>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add realm client scope protocol mapper", e);
            }

            return new KcResponse<KcProtocolMapper>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListMappersAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersAsync(string realm,
        string accessToken,
        string clientScopeId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientScopeId) )
        {
            throw new KcException($"{nameof(clientScopeId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcProtocolMapper>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm client scope protocol mappers", e);
            }

            return new KcResponse<IEnumerable<KcProtocolMapper>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.GetMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> GetMapperAsync(string realm, string accessToken,
        string clientScopeId, string mapperId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientScopeId) )
        {
            throw new KcException($"{nameof(clientScopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(mapperId) )
        {
            throw new KcException($"{nameof(mapperId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models/{mapperId}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcProtocolMapper>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get realm client scope protocol mapper", e);
            }

            return new KcResponse<KcProtocolMapper>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.UpdateMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> UpdateMapperAsync(string realm,
        string accessToken,
        string clientScopeId, string mapperId, KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientScopeId) )
        {
            throw new KcException($"{nameof(clientScopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(mapperId) )
        {
            throw new KcException($"{nameof(mapperId)} is required");
        }

        if ( protocolMapper == null )
        {
            throw new KcException($"{nameof(protocolMapper)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models/{mapperId}",
                accessToken,
                KcRequestHandler.GetBody(protocolMapper));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcProtocolMapper>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update realm client scope protocol mapper", e);
            }

            return new KcResponse<KcProtocolMapper>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.DeleteMapperAsync"/>
    public async Task<KcResponse<object>> DeleteMapperAsync(string realm, string accessToken,
        string clientScopeId,
        string mapperId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientScopeId) )
        {
            throw new KcException($"{nameof(clientScopeId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(mapperId) )
        {
            throw new KcException($"{nameof(mapperId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models/{mapperId}",
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
                KcLoggerMessages.Error(_logger, "Unable to delete realm client scope protocol mapper", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListMappersByProtocolNameAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersByProtocolNameAsync(
        string realm,
        string accessToken, string clientScopeId, KcProtocol protocol,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientScopeId) )
        {
            throw new KcException($"{nameof(clientScopeId)} is required");
        }

        var protocolName = protocol == KcProtocol.Saml ? "saml" : "openid-connect";

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/protocol/{protocolName}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcProtocolMapper>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list realm client scope protocol mappers by protocol name",
                    e);
            }

            return new KcResponse<IEnumerable<KcProtocolMapper>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.AddClientMappersAsync"/>
    public async Task<KcResponse<object>> AddClientMappersAsync(string realm, string accessToken,
        string clientId,
        IEnumerable<KcProtocolMapper> protocolMappers,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( protocolMappers == null )
        {
            throw new KcException($"{nameof(protocolMappers)} is required");
        }

        if ( !protocolMappers.Any() )
        {
            return new KcResponse<object>();
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients/{clientId}/protocol-mappers/add-models",
                accessToken, KcRequestHandler.GetBody(protocolMappers));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add client level protocol mappers", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.AddClientMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> AddClientMapperAsync(string realm,
        string accessToken,
        string clientId, KcProtocolMapper kcProtocolMapper,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( kcProtocolMapper == null )
        {
            throw new KcException($"{nameof(kcProtocolMapper)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/clients/{clientId}/protocol-mappers/models",
                accessToken, KcRequestHandler.GetBody(kcProtocolMapper));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcProtocolMapper>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to add client level protocol mapper", e);
            }

            return new KcResponse<KcProtocolMapper>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListClientMappersAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListClientMappersAsync(
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
                $"{_baseUrl}/{realm}/clients/{clientId}/protocol-mappers/models", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcProtocolMapper>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list client level protocol mappers", e);
            }

            return new KcResponse<IEnumerable<KcProtocolMapper>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.GetClientMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> GetClientMapperAsync(string realm,
        string accessToken,
        string clientId, string mapperId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(mapperId) )
        {
            throw new KcException($"{nameof(mapperId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/clients/{clientId}/protocol-mappers/models/{mapperId}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcProtocolMapper>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get client level protocol mapper", e);
            }

            return new KcResponse<KcProtocolMapper>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.UpdateClientMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> UpdateClientMapperAsync(string realm,
        string accessToken,
        string clientId, string mapperId, KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(mapperId) )
        {
            throw new KcException($"{nameof(mapperId)} is required");
        }

        if ( protocolMapper == null )
        {
            throw new KcException($"{nameof(protocolMapper)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{clientId}/protocol-mappers/models/{mapperId}",
                accessToken,
                KcRequestHandler.GetBody(protocolMapper));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcProtocolMapper>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to update client level protocol mapper", e);
            }

            return new KcResponse<KcProtocolMapper>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.DeleteClientMapperAsync"/>
    public async Task<KcResponse<object>> DeleteClientMapperAsync(string realm, string accessToken,
        string clientId,
        string mapperId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(mapperId) )
        {
            throw new KcException($"{nameof(mapperId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/clients/{clientId}/protocol-mappers/models/{mapperId}",
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
                KcLoggerMessages.Error(_logger, "Unable to delete client level protocol mapper", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListClientMappersByProtocolNameAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>>
        ListClientMappersByProtocolNameAsync(string realm,
        string accessToken, string clientId, KcProtocol protocol,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(clientId) )
        {
            throw new KcException($"{nameof(clientId)} is required");
        }

        var protocolName = protocol == KcProtocol.Saml ? "saml" : "openid-connect";

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/clients/{clientId}/protocol-mappers/protocol/{protocolName}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcProtocolMapper>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to list client level protocol mappers by protocol name", e);
            }

            return new KcResponse<IEnumerable<KcProtocolMapper>>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
