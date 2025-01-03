using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcProtocolMappers"/>
internal sealed class KcProtocolMappers(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcProtocolMappers
{
    /// <inheritdoc cref="IKcProtocolMappers.AddMappersAsync"/>
    public async Task<KcResponse<object>> AddMappersAsync(
        string realm,
        string accessToken,
        string clientScopeId,
        IEnumerable<KcProtocolMapper> protocolMappers,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(clientScopeId), clientScopeId);

        // Validate that the protocol mappers collection is not null.
        ValidateNotNull(nameof(protocolMappers), protocolMappers);

        // Return an empty response if the protocol mappers collection is empty.
        if ( !protocolMappers.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for adding protocol mappers to the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/add-models";

        // Process the request to add the protocol mappers.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add realm client scope protocol mappers",
            protocolMappers,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.AddMapperAsync"/>
    public async Task<KcResponse<object>> AddMapperAsync(
        string realm,
        string accessToken,
        string clientScopeId,
        KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(clientScopeId), clientScopeId);

        // Validate that the protocol mapper is not null.
        ValidateNotNull(nameof(protocolMapper), protocolMapper);

        // Construct the URL for adding the protocol mapper to the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models";

        // Process the request to add the protocol mapper.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add realm client scope protocol mapper",
            protocolMapper,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListMappersAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersAsync(
        string realm,
        string accessToken,
        string clientScopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(clientScopeId), clientScopeId);

        // Construct the URL for listing protocol mappers of the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models";

        // Process the request to retrieve the protocol mappers.
        return await ProcessRequestAsync<IEnumerable<KcProtocolMapper>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm client scope protocol mappers",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.GetMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> GetMapperAsync(
        string realm,
        string accessToken,
        string clientScopeId,
        string mapperId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(clientScopeId), clientScopeId);

        // Validate that the mapper ID is not null or empty.
        ValidateRequiredString(nameof(mapperId), mapperId);

        // Construct the URL for retrieving the specified protocol mapper.
        var url = $"{BaseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models/{mapperId}";

        // Process the request to retrieve the protocol mapper.
        return await ProcessRequestAsync<KcProtocolMapper>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client scope protocol mapper",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.UpdateMapperAsync"/>
    public async Task<KcResponse<object>> UpdateMapperAsync(
        string realm,
        string accessToken,
        string clientScopeId,
        string mapperId,
        KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(clientScopeId), clientScopeId);

        // Validate that the mapper ID is not null or empty.
        ValidateRequiredString(nameof(mapperId), mapperId);

        // Validate that the protocol mapper configuration is not null.
        ValidateNotNull(nameof(protocolMapper), protocolMapper);

        // Construct the URL for updating the specified protocol mapper.
        var url = $"{BaseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models/{mapperId}";

        // Process the request to update the protocol mapper.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update realm client scope protocol mapper",
            protocolMapper,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.DeleteMapperAsync"/>
    public async Task<KcResponse<object>> DeleteMapperAsync(
        string realm,
        string accessToken,
        string clientScopeId,
        string mapperId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(clientScopeId), clientScopeId);

        // Validate that the mapper ID is not null or empty.
        ValidateRequiredString(nameof(mapperId), mapperId);

        // Construct the URL for deleting the specified protocol mapper.
        var url = $"{BaseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/models/{mapperId}";

        // Process the request to remove the protocol mapper.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm client scope protocol mapper",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListMappersByProtocolNameAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersByProtocolNameAsync(
        string realm,
        string accessToken,
        string clientScopeId,
        KcProtocol protocol,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(clientScopeId), clientScopeId);

        // Determine the protocol name based on the protocol type.
        var protocolName = protocol == KcProtocol.Saml ? "saml" : "openid-connect";

        // Construct the URL for listing protocol mappers by protocol name.
        var url = $"{BaseUrl}/{realm}/client-scopes/{clientScopeId}/protocol-mappers/protocol/{protocolName}";

        // Process the request to retrieve the protocol mappers.
        return await ProcessRequestAsync<IEnumerable<KcProtocolMapper>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm client scope protocol mappers by protocol name",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.AddClientMappersAsync"/>
    public async Task<KcResponse<object>> AddClientMappersAsync(
        string realm,
        string accessToken,
        string clientId,
        IEnumerable<KcProtocolMapper> protocolMappers,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the protocol mappers are not null.
        ValidateNotNull(nameof(protocolMappers), protocolMappers);

        // Return early if the protocol mappers collection is empty.
        if ( !protocolMappers.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for adding protocol mappers to the client.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/protocol-mappers/add-models";

        // Process the request to add the client protocol mappers.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add client level protocol mappers",
            protocolMappers,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.AddClientMapperAsync"/>
    public async Task<KcResponse<object>> AddClientMapperAsync(
        string realm,
        string accessToken,
        string clientId,
        KcProtocolMapper kcProtocolMapper,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the protocol mapper is not null.
        ValidateNotNull(nameof(kcProtocolMapper), kcProtocolMapper);

        // Construct the URL for adding the protocol mapper to the client.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/protocol-mappers/models";

        // Process the request to add the client protocol mapper.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add client level protocol mapper",
            kcProtocolMapper,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListClientMappersAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListClientMappersAsync(
        string realm,
        string accessToken,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL for retrieving the client protocol mappers.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/protocol-mappers/models";

        // Process the request to retrieve the client protocol mappers.
        return await ProcessRequestAsync<IEnumerable<KcProtocolMapper>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list client level protocol mappers",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.GetClientMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> GetClientMapperAsync(
        string realm,
        string accessToken,
        string clientId,
        string mapperId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the mapper ID is not null or empty.
        ValidateRequiredString(nameof(mapperId), mapperId);

        // Construct the URL for retrieving the specific protocol mapper.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/protocol-mappers/models/{mapperId}";

        // Process the request to retrieve the client protocol mapper.
        return await ProcessRequestAsync<KcProtocolMapper>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get client level protocol mapper",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.UpdateClientMapperAsync"/>
    public async Task<KcResponse<KcProtocolMapper>> UpdateClientMapperAsync(
        string realm,
        string accessToken,
        string clientId,
        string mapperId,
        KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the mapper ID is not null or empty.
        ValidateRequiredString(nameof(mapperId), mapperId);

        // Validate that the protocol mapper details are not null.
        ValidateNotNull(nameof(protocolMapper), protocolMapper);

        // Construct the URL for updating the specific protocol mapper.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/protocol-mappers/models/{mapperId}";

        // Process the request to update the client protocol mapper.
        return await ProcessRequestAsync<KcProtocolMapper>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update client level protocol mapper",
            protocolMapper,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.DeleteClientMapperAsync"/>
    public async Task<KcResponse<object>> DeleteClientMapperAsync(
        string realm,
        string accessToken,
        string clientId,
        string mapperId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the mapper ID is not null or empty.
        ValidateRequiredString(nameof(mapperId), mapperId);

        // Construct the URL for deleting the specific protocol mapper.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/protocol-mappers/models/{mapperId}";

        // Process the request to remove the client protocol mapper.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete client level protocol mapper",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcProtocolMappers.ListClientMappersByProtocolNameAsync"/>
    public async Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListClientMappersByProtocolNameAsync(
        string realm,
        string accessToken,
        string clientId,
        KcProtocol protocol,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Determine the protocol name based on the input protocol type.
        var protocolName = protocol == KcProtocol.Saml ? "saml" : "openid-connect";

        // Construct the URL for retrieving the protocol mappers filtered by protocol name.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/protocol-mappers/protocol/{protocolName}";

        // Process the request to retrieve the client protocol mappers.
        return await ProcessRequestAsync<IEnumerable<KcProtocolMapper>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list client level protocol mappers by protocol name",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}
