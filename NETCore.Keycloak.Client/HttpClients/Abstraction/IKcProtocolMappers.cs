using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak protocol mappers REST client.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_protocol_mappers_resource"/>
/// </summary>
public interface IKcProtocolMappers
{
    /// <summary>
    /// Adds protocol mappers to a specific client scope in a Keycloak realm.
    ///
    /// POST /{realm}/client-scopes/{client-scope-id}/protocol-mappers/add-models
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientScopeId">The unique identifier of the client scope to which the mappers are being added.</param>
    /// <param name="protocolMappers">
    /// A collection of <see cref="KcProtocolMapper"/> objects representing the protocol mappers to be added.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating the result of the operation,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client scope ID, or protocol mappers collection is null or invalid.
    /// </exception>
    Task<KcResponse<object>> AddMappersAsync(string realm, string accessToken, string clientScopeId,
        IEnumerable<KcProtocolMapper> protocolMappers, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a single protocol mapper to a specific client scope in a Keycloak realm.
    ///
    /// POST /{realm}/client-scopes/{client-scope-id}/protocol-mappers/models
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientScopeId">The unique identifier of the client scope to which the mapper is being added.</param>
    /// <param name="protocolMapper">
    /// A <see cref="KcProtocolMapper"/> object representing the protocol mapper to be added.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the added protocol mapper if successful,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client scope ID, or protocol mapper is null or invalid.
    /// </exception>
    Task<KcResponse<KcProtocolMapper>> AddMapperAsync(string realm, string accessToken, string clientScopeId,
        KcProtocolMapper protocolMapper, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all protocol mappers associated with a specific client scope in a Keycloak realm.
    ///
    /// GET /{realm}/client-scopes/{client-scope-id}/protocol-mappers/models
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientScopeId">The unique identifier of the client scope whose protocol mappers are being listed.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcProtocolMapper"/> objects
    /// if successful, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client scope ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersAsync(string realm, string accessToken,
        string clientScopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific protocol mapper associated with a client scope in a Keycloak realm.
    ///
    /// GET /{realm}/client-scopes/{client-scope-id}/protocol-mappers/models/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientScopeId">The unique identifier of the client scope containing the protocol mapper.</param>
    /// <param name="mapperId">The unique identifier of the protocol mapper to retrieve.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcProtocolMapper"/> object if successful,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client scope ID, or mapper ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcProtocolMapper>> GetMapperAsync(string realm, string accessToken, string clientScopeId,
        string mapperId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing protocol mapper associated with a client scope in a Keycloak realm.
    ///
    /// PUT /{realm}/client-scopes/{client-scope-id}/protocol-mappers/models/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientScopeId">The unique identifier of the client scope containing the protocol mapper.</param>
    /// <param name="mapperId">The unique identifier of the protocol mapper to update.</param>
    /// <param name="protocolMapper">
    /// An instance of <see cref="KcProtocolMapper"/> representing the updated protocol mapper configuration.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated <see cref="KcProtocolMapper"/> object if successful,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client scope ID, mapper ID, or protocol mapper configuration is null or invalid.
    /// </exception>
    Task<KcResponse<KcProtocolMapper>> UpdateMapperAsync(string realm, string accessToken, string clientScopeId,
        string mapperId, KcProtocolMapper protocolMapper, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific protocol mapper associated with a client scope in a Keycloak realm.
    ///
    /// DELETE /{realm}/client-scopes/{client-scope-id}/protocol-mappers/models/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientScopeId">The unique identifier of the client scope containing the protocol mapper.</param>
    /// <param name="mapperId">The unique identifier of the protocol mapper to delete.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client scope ID, or mapper ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteMapperAsync(string realm, string accessToken, string clientScopeId, string mapperId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists protocol mappers associated with a client scope in a Keycloak realm by protocol name.
    ///
    /// GET /{realm}/client-scopes/{client-scope-id}/protocol-mappers/protocol/{protocol}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client scope resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientScopeId">The unique identifier of the client scope.</param>
    /// <param name="protocol">
    /// The protocol type (<see cref="KcProtocol.Saml"/> or <see cref="KcProtocol.OpenidConnect"/>)
    /// to filter the protocol mappers.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing a collection of <see cref="KcProtocolMapper"/> objects.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client scope ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersByProtocolNameAsync(string realm, string accessToken,
        string clientScopeId, KcProtocol protocol, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds protocol mappers to a client in a Keycloak realm.
    ///
    /// POST /{realm}/clients/{client-uuid}/protocol-mappers/add-models
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientId">The unique identifier of the client to which the mappers will be added.</param>
    /// <param name="protocolMappers">The collection of protocol mappers to add to the client.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or protocol mappers are null or invalid.
    /// </exception>
    Task<KcResponse<object>> AddClientMappersAsync(string realm, string accessToken, string clientId,
        IEnumerable<KcProtocolMapper> protocolMappers, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a protocol mapper to a client in a Keycloak realm.
    ///
    /// POST /{realm}/clients/{client-uuid}/protocol-mappers/models
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientId">The unique identifier of the client to which the mapper will be added.</param>
    /// <param name="kcProtocolMapper">The protocol mapper to add to the client.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing the added <see cref="KcProtocolMapper"/> or an error response.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or protocol mapper is null or invalid.
    /// </exception>
    Task<KcResponse<KcProtocolMapper>> AddClientMapperAsync(string realm, string accessToken, string clientId,
        KcProtocolMapper kcProtocolMapper, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all protocol mappers associated with a client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{client-uuid}/protocol-mappers/models
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientId">The unique identifier of the client for which protocol mappers are to be retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing a collection of <see cref="KcProtocolMapper"/> instances
    /// or an error response.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListClientMappersAsync(string realm, string accessToken,
        string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific protocol mapper associated with a client in a Keycloak realm.
    ///
    /// GET /{realm}/clients/{client-uuid}/protocol-mappers/models/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="mapperId">The unique identifier of the protocol mapper to retrieve.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing the requested <see cref="KcProtocolMapper"/>
    /// or an error response.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or mapper ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcProtocolMapper>> GetClientMapperAsync(string realm, string accessToken, string clientId,
        string mapperId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a specific protocol mapper associated with a client in a Keycloak realm.
    ///
    /// PUT /{realm}/clients/{client-uuid}/protocol-mappers/models/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="mapperId">The unique identifier of the protocol mapper to update.</param>
    /// <param name="protocolMapper">The updated protocol mapper details.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing the updated <see cref="KcProtocolMapper"/>
    /// or an error response.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, mapper ID, or protocol mapper is null or invalid.
    /// </exception>
    Task<KcResponse<KcProtocolMapper>> UpdateClientMapperAsync(string realm, string accessToken, string clientId,
        string mapperId, KcProtocolMapper protocolMapper, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific protocol mapper associated with a client in a Keycloak realm.
    ///
    /// DELETE /{realm}/clients/{client-uuid}/protocol-mappers/models/{id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="mapperId">The unique identifier of the protocol mapper to delete.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object indicating the result of the delete operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, client ID, or mapper ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteClientMapperAsync(string realm, string accessToken, string clientId, string mapperId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of protocol mappers associated with a client in a Keycloak realm, filtered by protocol name.
    ///
    /// GET /{realm}/clients/{client-uuid}/protocol-mappers/protocol/{protocol}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="protocol">The protocol types to filter the protocol mappers by (e.g., OpenID Connect or SAML).</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing a collection of <see cref="KcProtocolMapper"/> objects.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or client ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListClientMappersByProtocolNameAsync(string realm,
        string accessToken, string clientId, KcProtocol protocol, CancellationToken cancellationToken = default);
}
