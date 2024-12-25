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
    /// Add protocol mappers to realm client scope
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientScopeId">Keycloak realm client scope id</param>
    /// <param name="protocolMappers">List of keycloak protocol mapper. <see cref="KcProtocolMapper"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddMappersAsync(string realm, string accessToken, string clientScopeId,
        IEnumerable<KcProtocolMapper> protocolMappers, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add protocol mapper to realm client scope
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientScopeId">Keycloak realm client scope id</param>
    /// <param name="protocolMapper">Keycloak protocol mapper. <see cref="KcProtocolMapper"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcProtocolMapper>> AddMapperAsync(string realm, string accessToken,
        string clientScopeId, KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List realm client scope protocol mappers.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientScopeId">Keycloak realm client scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersAsync(string realm,
        string accessToken, string clientScopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm client scope protocol mapper.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientScopeId">Keycloak realm client scope id</param>
    /// <param name="mapperId">Keycloak protocol mapper id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcProtocolMapper>> GetMapperAsync(string realm, string accessToken,
        string clientScopeId, string mapperId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update realm client scope protocol mapper.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientScopeId">Keycloak realm client scope id</param>
    /// <param name="mapperId">Keycloak protocol mapper id</param>
    /// <param name="protocolMapper">Keycloak protocol mapper. <see cref="KcProtocolMapper"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcProtocolMapper>> UpdateMapperAsync(string realm, string accessToken,
        string clientScopeId, string mapperId, KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete realm client scope protocol mapper.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientScopeId">Keycloak realm client scope id</param>
    /// <param name="mapperId">Keycloak protocol mapper id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteMapperAsync(string realm, string accessToken,
        string clientScopeId, string mapperId, CancellationToken cancellationToken = default);

    /// <summary>
    /// List realm client scope protocol mappers by protocol name.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientScopeId">Keycloak realm client scope id</param>
    /// <param name="protocol">Keycloak protocol <see cref="KcProtocol"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListMappersByProtocolNameAsync(string realm,
        string accessToken, string clientScopeId, KcProtocol protocol,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add client level protocol mappers
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="protocolMappers">List of keycloak protocol mapper. <see cref="KcProtocolMapper"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddClientMappersAsync(string realm, string accessToken,
        string clientId, IEnumerable<KcProtocolMapper> protocolMappers,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add client level protocol mapper.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="kcProtocolMapper">Keycloak protocol mapper <see cref="KcProtocolMapper"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcProtocolMapper>> AddClientMapperAsync(string realm, string accessToken,
        string clientId, KcProtocolMapper kcProtocolMapper,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List client level protocol mappers.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListClientMappersAsync(string realm,
        string accessToken, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get client level protocol mapper.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="mapperId">Keycloak mapper id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcProtocolMapper>> GetClientMapperAsync(string realm, string accessToken,
        string clientId, string mapperId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update client level protocol mapper.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="mapperId">Keycloak mapper id</param>
    /// <param name="protocolMapper">Keycloak protocol mapper <see cref="KcProtocolMapper"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<KcProtocolMapper>> UpdateClientMapperAsync(string realm, string accessToken,
        string clientId, string mapperId, KcProtocolMapper protocolMapper,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete client level protocol mapper
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="mapperId">Keycloak mapper id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> DeleteClientMapperAsync(string realm, string accessToken,
        string clientId, string mapperId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Client client level protocol mappers by protocol name.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="protocol">Keycloak protocol <see cref="KcProtocol"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcProtocolMapper>>> ListClientMappersByProtocolNameAsync(
        string realm, string accessToken, string clientId, KcProtocol protocol,
        CancellationToken cancellationToken = default);
}
