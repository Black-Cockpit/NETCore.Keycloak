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
    /// Adds client-level roles to a specified client scope in the given realm.
    ///
    /// POST /{realm}/client-scopes/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope to which roles will be added.</param>
    /// <param name="clientId">The unique identifier of the client containing the roles.</param>
    /// <param name="roles">
    /// A collection of <see cref="KcRole"/> objects representing the roles to be added to the client scope.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> AddClientRolesToScopeAsync(string realm, string accessToken, string scopeId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of client roles associated with a specific client scope in the given realm.
    ///
    /// GET /{realm}/client-scopes/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope whose associated roles are to be retrieved.</param>
    /// <param name="clientId">The unique identifier of the client containing the roles.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the collection of <see cref="KcRole"/> objects associated with the client scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToScopeAsync(string realm, string accessToken,
        string scopeId, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes client roles from a specified client scope in the given realm.
    ///
    /// DELETE /{realm}/client-scopes/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope from which roles are to be removed.</param>
    /// <param name="clientId">The unique identifier of the client containing the roles to be removed.</param>
    /// <param name="roles">The collection of <see cref="KcRole"/> objects to be removed from the client scope.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> RemoveClientRolesFromScopeAsync(string realm, string accessToken, string scopeId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the client roles available for association with a specified client scope in the given realm.
    ///
    /// GET /{realm}/client-scopes/{id}/scope-mappings/clients/{client}/available
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope to check for available roles.</param>
    /// <param name="clientId">The unique identifier of the client whose roles are being queried.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects
    /// that are available for association with the client scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForScopeAsync(string realm, string accessToken,
        string scopeId, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the composite client roles associated with a specified client scope in the given realm.
    ///
    /// GET /{realm}/client-scopes/{id}/scope-mappings/clients/{client}/composite
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope to check for associated roles.</param>
    /// <param name="clientId">The unique identifier of the client whose associated roles are being queried.</param>
    /// <param name="filter">An optional filter for customizing the role query.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects
    /// that are associated with the client scope as composite roles.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeClientRolesAssociatedToScopeAsync(string realm,
        string accessToken, string scopeId, string clientId, KcFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a set of realm-level roles to a specified client scope in the given realm.
    ///
    /// POST /{realm}/client-scopes/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope to which roles will be added.</param>
    /// <param name="roles">The collection of <see cref="KcRole"/> objects to be added to the client scope.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> AddRolesToScopeAsync(string realm, string accessToken, string scopeId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of realm-level roles associated with a specified client scope in the given realm.
    ///
    /// GET /{realm}/client-scopes/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope whose associated roles will be retrieved.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects associated with the client scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToScopeAsync(string realm, string accessToken,
        string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a set of realm-level roles from a specified client scope in the given realm.
    ///
    /// DELETE /{realm}/client-scopes/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope from which roles will be removed.</param>
    /// <param name="roles">A collection of <see cref="KcRole"/> objects to be removed from the client scope.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> RemoveRolesFromScopeAsync(string realm, string accessToken, string scopeId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the realm-level roles that can be attached to a specified client scope in the given realm.
    ///
    /// GET /{realm}/client-scopes/{id}/scope-mappings/realm/available
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope to check for available roles.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects that are available for the client scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForScopeAsync(string realm, string accessToken,
        string scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the effective realm-level roles that are associated with a specified client scope in the given realm.
    ///
    /// GET /{realm}/client-scopes/{id}/scope-mappings/realm/composite
    /// </summary>
    /// <param name="realm">The realm name where the client scope exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="scopeId">The unique identifier of the client scope to retrieve associated roles for.</param>
    /// <param name="filter">Optional filter to apply to the query when listing roles.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects that are effectively associated with the client scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeRolesAssociatedToScopeAsync(string realm, string accessToken,
        string scopeId, KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a set of client-level roles to the scope of a specified client in the given realm.
    ///
    /// POST /{realm}/clients/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client to which roles are being added.</param>
    /// <param name="clientName">The name of the client to which roles are being added.</param>
    /// <param name="roles">The collection of <see cref="KcRole"/> objects representing the roles to add.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> AddClientRolesToClientScopeAsync(string realm, string accessToken, string clientId,
        string clientName, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the client-level roles associated with the scope of a specified client in the given realm.
    ///
    /// GET /{realm}/clients/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client for which roles are being retrieved.</param>
    /// <param name="clientName">The name of the client for which roles are being retrieved.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the collection of <see cref="KcRole"/> objects associated with the client's scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToClientScopeAsync(string realm, string accessToken,
        string clientId, string clientName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a set of client-level roles from the scope of a specified client in the given realm.*
    ///
    /// DELETE /{realm}/clients/{id}/scope-mappings/clients/{client}
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client from which roles are being removed.</param>
    /// <param name="clientName">The name of the client from which roles are being removed.</param>
    /// <param name="roles">The collection of roles to be removed from the client's scope.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> RemoveClientRolesFromClientScopeAsync(string realm, string accessToken, string clientId,
        string clientName, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of client-level roles available for association with a specific client's scope in the given realm.
    ///
    /// GET /{realm}/clients/{id}/scope-mappings/clients/{client}/available
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client whose available roles are being listed.</param>
    /// <param name="clientName">The name of the client whose available roles are being listed.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the list of available client-level roles.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForClientScopeAsync(string realm, string accessToken,
        string clientId, string clientName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of composite client-level roles that are effectively associated with a specific client's scope in the given realm.
    ///
    /// GET /{realm}/clients/{id}/scope-mappings/clients/{client}/composite
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client whose composite roles are being listed.</param>
    /// <param name="clientName">The name of the client whose composite roles are being listed.</param>
    /// <param name="filter">Optional filter to apply to the request for filtering the results.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the list of composite client-level roles.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeClientRolesAssociatedToClientScopeAsync(string realm,
        string accessToken, string clientId, string clientName, KcFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a set of realm-level roles to the scope of a specific client in the given realm.
    ///
    /// POST /{realm}/clients/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client to which the roles will be added.</param>
    /// <param name="roles">The collection of roles to be added to the client's scope.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> AddRolesToScopeClientAsync(string realm, string accessToken, string clientId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of realm-level roles associated with the scope of a specific client in the given realm.
    ///
    /// GET /{realm}/clients/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client whose associated roles will be retrieved.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the collection of roles associated with the client's scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToClientScopeAsync(string realm, string accessToken,
        string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a set of realm-level roles from the scope of a specific client in the given realm.
    ///
    /// DELETE /{realm}/clients/{id}/scope-mappings/realm
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client whose roles will be removed from the scope.</param>
    /// <param name="roles">The collection of roles to be removed from the client's scope.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<object>> RemoveRolesFromClientScopeAsync(string realm, string accessToken, string clientId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of realm-level roles that can be associated with
    /// the scope of a specific client in the given realm.
    ///
    /// GET /{realm}/clients/{id}/scope-mappings/realm/available
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client for which available roles are being listed.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of roles that are available for association with the client's scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForClientScopeAsync(string realm, string accessToken,
        string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of composite (effective) realm-level roles that are
    /// associated with the scope of a specific client in the given realm.
    ///
    /// GET /{realm}/clients/{id}/scope-mappings/realm/composite
    /// </summary>
    /// <param name="realm">The realm name where the client exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client for which composite roles are being listed.</param>
    /// <param name="filter">An optional filter to narrow down the roles list.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of composite roles associated with the client's scope.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if any required parameter is null, empty, or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeRolesAssociatedToClientScopeAsync(string realm,
        string accessToken, string clientId, KcFilter filter = null, CancellationToken cancellationToken = default);
}
