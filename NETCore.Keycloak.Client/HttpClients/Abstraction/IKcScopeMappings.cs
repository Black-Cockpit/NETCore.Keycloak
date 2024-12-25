using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak scope mappings REST client.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_scope_mappings_resource"/>
/// </summary>
public interface IKcScopeMappings
{
    /// <summary>
    /// Add client-level roles to the client’s scope
    /// POST /{realm}/client-scopes/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddClientRolesToScopeAsync(string realm, string accessToken,
        string scopeId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the roles associated with a client’s scope
    /// GET /{realm}/client-scopes/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToScopeAsync(string realm,
        string accessToken, string scopeId, string clientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove client-level roles from the client’s scope.
    /// DELETE /{realm}/client-scopes/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="roles">List of keycloak roles <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> RemoveClientRolesFromScopeAsync(string realm, string accessToken,
        string scopeId, string clientId, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List roles for the client that can be associated with the client’s scope.
    /// GET /{realm}/client-scopes/{id}/scope-mappings/clients/{client}/available
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForScopeAsync(string realm,
        string accessToken, string scopeId, string clientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List the roles for the client that are associated with the client’s scope.
    /// GET /{realm}/client-scopes/{id}/scope-mappings/clients/{client}/composite
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="filter">Keycloak filter <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeClientRolesAssociatedToScopeAsync(
        string realm, string accessToken, string scopeId, string clientId,
        KcFilter filter = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a set of realm-level roles to the client’s scope
    /// POST /{realm}/client-scopes/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="roles">List of keycloak roles. <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddRolesToScopeAsync(string realm, string accessToken, string scopeId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm-level roles associated with the client’s scope
    /// GET /{realm}/client-scopes/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToScopeAsync(string realm,
        string accessToken, string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a set of realm-level roles from the client’s scope.
    /// DELETE /{realm}/client-scopes/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="roles">List of keycloak roles. <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> RemoveRolesFromScopeAsync(string realm, string accessToken,
        string scopeId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm-level roles that are available to attach to this client’s scope.
    /// GET /{realm}/client-scopes/{id}/scope-mappings/realm/available
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForScopeAsync(string realm,
        string accessToken, string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get effective realm-level roles associated with the client’s scope
    /// what this does is recurse any composite roles associated with the client’s scope and adds the roles to this lists.
    /// GET /{realm}/client-scopes/{id}/scope-mappings/realm/composite
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="scopeId">Keycloak scope id</param>
    /// <param name="filter">Keycloak filter. <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeRolesAssociatedToScopeAsync(string realm,
        string accessToken, string scopeId, KcFilter filter = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add client-level roles to the client’s scope.
    /// POST /{realm}/clients/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="clientName">Keycloak client name</param>
    /// <param name="roles">List of keycloak roles. <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddClientRolesToClientScopeAsync(string realm, string accessToken,
        string clientId, string clientName, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the roles associated with a client’s scope.
    /// GET /{realm}/clients/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="clientName">Keycloak client name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToClientScopeAsync(string realm,
        string accessToken, string clientId, string clientName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove client-level roles from the client’s scope.
    /// DELETE /{realm}/clients/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="clientName">Keycloak client name</param>
    /// <param name="roles">List of keycloak roles. <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> RemoveClientRolesFromClientScopeAsync(string realm, string accessToken,
        string clientId, string clientName, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List the roles available for the client that can be associated with the client’s scope.
    /// GET /{realm}/clients/{id}/scope-mappings/clients/{client}/available
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="clientName">Keycloak client name</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForClientScopeAsync(string realm,
        string accessToken, string clientId, string clientName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List effective roles for the client that are associated with the client’s scope.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="clientName">Keycloak client name</param>
    /// <param name="filter">Keycloak filter. <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeClientRolesAssociatedToClientScopeAsync(
        string realm, string accessToken, string clientId, string clientName,
        KcFilter filter = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a set of realm-level roles to the client’s scope.
    /// POST /{realm}/clients/{id}/scope-mappings/realm.
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="roles">List of keycloak roles. <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> AddRolesToScopeClientAsync(string realm, string accessToken,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm-level roles associated with the client’s scope.
    /// GET /{realm}/clients/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToClientScopeAsync(string realm,
        string accessToken, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a set of realm-level roles from the client’s scope.
    /// DELETE /{realm}/clients/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="roles">List of keycloak roles. <see cref="KcRole"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<object>> RemoveRolesFromClientScopeAsync(string realm, string accessToken,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get realm-level roles that are available to attach to this client’s scope.
    /// GET /{realm}/clients/{id}/scope-mappings/realm/available
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForClientScopeAsync(string realm,
        string accessToken, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get effective realm-level roles associated with the client’s scope.
    /// what this does is recurse any composite roles associated with the client’s scope and adds the roles to this lists.
    /// GET /{realm}/clients/{id}/scope-mappings/realm/composite
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <param name="clientId">Keycloak client id (UUID)</param>
    /// <param name="filter">Keycloak filter. <see cref="KcFilter"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeRolesAssociatedToClientScopeAsync(
        string realm, string accessToken, string clientId, KcFilter filter = default,
        CancellationToken cancellationToken = default);
}
