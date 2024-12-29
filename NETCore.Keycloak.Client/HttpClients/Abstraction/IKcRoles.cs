using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak roles client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_roles_resource"/>
/// <seealso href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_roles_by_id_resource"/>
/// </summary>
public interface IKcRoles
{
    /// <summary>
    /// Creates a new role in the specified realm.
    ///
    /// POST /{realm}/roles
    /// </summary>
    /// <param name="realm">The realm in which the role will be created.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="role">The role details to be created.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A <see cref="KcResponse{T}"/> indicating the result of the operation.</returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="role"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of roles in the specified realm, optionally filtered by the provided criteria.
    ///
    /// GET /{realm}/roles
    /// </summary>
    /// <param name="realm">The realm from which roles will be listed.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="filter">Optional filter criteria for the roles.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects representing the roles in the realm.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/> or <paramref name="accessToken"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListAsync(string realm, string accessToken, KcFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific role in the specified realm by its name.
    ///
    /// GET /{realm}/roles/{role-name}
    /// </summary>
    /// <param name="realm">The realm from which the role will be retrieved.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role to retrieve.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcRole"/> object representing the role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcRole>> GetAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role with the specified name exists in the specified realm.
    /// </summary>
    /// <param name="realm">The realm in which to check for the role.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role to check for existence.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a boolean value indicating whether the role exists.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<bool>> IsRolesExistsAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a realm role by its unique identifier.
    ///
    /// GET /{realm}/roles-by-id/{role-id}
    /// </summary>
    /// <param name="realm">The realm in which to retrieve the role.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The unique identifier of the role to retrieve.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the role information if successful.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="id"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcRole>> GetByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing realm role with the provided details.
    ///
    /// PUT /{realm}/roles/{role-name}
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role to update.</param>
    /// <param name="role">The updated details of the role.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="name"/>, or <paramref name="role"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string name, KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing realm role by its ID with the provided details.
    ///
    /// PUT /{realm}/roles-by-id/{role-id}
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The unique identifier of the role to update.</param>
    /// <param name="role">The updated details of the role.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated role details or error information.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="id"/>, or <paramref name="role"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcRole>> UpdateByIdAsync(string realm, string accessToken, string id, KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a realm role by its name.
    ///
    /// DELETE /{realm}/roles/{role-name}
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role to delete.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a realm role by its unique identifier.
    ///
    /// DELETE /{realm}/roles-by-id/{role-id}
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The unique identifier of the role to delete.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="id"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds composite roles to a specified realm role.
    ///
    /// POST /{realm}/roles/{role-name}/composites
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role to which the composites will be added.</param>
    /// <param name="roles">The collection of roles to add as composites.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="name"/>, or <paramref name="roles"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> AddCompositeAsync(string realm, string accessToken, string name, IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds composite roles to a specified realm role by its ID.
    ///
    /// POST /{realm}/roles-by-id/{role-id}/composites
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The ID of the role to which the composites will be added.</param>
    /// <param name="roles">The collection of roles to add as composites.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="id"/>, or <paramref name="roles"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> AddCompositeByIdAsync(string realm, string accessToken, string id,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of composite roles associated with a specified realm role.
    ///
    /// GET /{realm}/roles/{role-name}/composites
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role whose composites are being listed.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the list of composite roles or an error if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of composite roles associated with a specified realm role by its ID.
    ///
    /// GET /{realm}/roles-by-id/{role-id}/composites
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The ID of the role whose composites are being listed.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the list of composite roles or an error if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="id"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListCompositeByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all composite roles associated with a specified realm role by its name.
    ///
    /// DELETE /{realm}/roles/{role-name}/composites
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role whose composites are being deleted.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteCompositeAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all composite roles associated with a specified realm role by its ID.
    ///
    /// DELETE /{realm}/roles-by-id/{role-id}/composites
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The ID of the role whose composites are being deleted.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="id"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteCompositeByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the client-level composite roles associated with a specified realm role.
    ///
    /// GET /{realm}/roles/{role-name}/composites/clients/{client-uuid}
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role for which composites are being retrieved.</param>
    /// <param name="clientId">The client ID for which the composite roles are scoped.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects representing the client-level composites.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="name"/>, or <paramref name="clientId"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesAsync(string realm, string accessToken, string name,
        string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the client-level composite roles associated with a specified realm role by its ID.
    ///
    /// GET /{realm}/roles-by-id/{role-id}/composites/clients/{clientUuid}
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The ID of the role for which composites are being retrieved.</param>
    /// <param name="clientId">The client ID for which the composite roles are scoped.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects representing the client-level composites.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="id"/>, or <paramref name="clientId"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesByIdAsync(string realm, string accessToken, string id,
        string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the realm-level composite roles associated with a specified realm role by its name.
    ///
    /// GET /{realm}/roles/{role-name}/composites/realm
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role for which composites are being retrieved.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects representing the realm-level composites.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesAsync(string realm, string accessToken, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the realm-level composite roles associated with a specified realm role by its ID.
    ///
    /// GET /{realm}/roles-by-id/{role-id}/composites/realm
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The ID of the role for which composites are being retrieved.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects representing the realm-level composites.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="id"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesByIdAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the groups associated with a specified realm role.
    ///
    /// GET /{realm}/roles/{role-name}/groups
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role for which groups are being retrieved.</param>
    /// <param name="filter">Optional filter criteria to narrow the search results.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcGroup"/> objects associated with the specified role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsAsync(string realm, string accessToken, string name,
        KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the authorization permissions for a specified realm role.
    ///
    /// GET /{realm}/roles/{role-name}/management/permissions
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role for which permissions are being retrieved.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcPermissionManagement"/> details
    /// of the specified role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsAsync(string realm,
        string accessToken,
        string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the authorization permissions for a specified realm role by its ID.
    ///
    /// GET /{realm}/roles-by-id/{role-id}/management/permissions
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The ID of the role for which permissions are being retrieved.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcPermissionManagement"/> details
    /// of the specified role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="id"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsByIdAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the authorization permissions for a specified realm role by its name.
    ///
    /// PUT /{realm}/roles/{role-name}/management/permissions
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role for which permissions are being set.</param>
    /// <param name="permissionManagement">The permissions to set for the specified role.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated <see cref="KcPermissionManagement"/> details of the role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="name"/>, or <paramref name="permissionManagement"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsAsync(string realm, string accessToken,
        string name, KcPermissionManagement permissionManagement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the authorization permissions for a specified realm role by its unique identifier.
    ///
    /// PUT /{realm}/roles-by-id/{role-id}/management/permissions
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="id">The unique identifier of the role for which permissions are being set.</param>
    /// <param name="permissionManagement">The permissions to set for the specified role.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated <see cref="KcPermissionManagement"/> details of the role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="id"/>, or <paramref name="permissionManagement"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsByIdAsync(string realm, string accessToken,
        string id, KcPermissionManagement permissionManagement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of users associated with a specific realm role.
    ///
    /// GET /{realm}/roles/{role-name}/users
    /// </summary>
    /// <param name="realm">The realm in which the role resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="name">The name of the role whose users are to be retrieved.</param>
    /// <param name="filter">An optional filter to apply when retrieving users.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcUser"/> objects associated with the specified role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcUser>>> GetUserInRoleAsync(string realm, string accessToken, string name,
        KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new client role in the specified realm and client.
    ///
    /// POST /{realm}/clients/{client-uuid}/roles
    /// </summary>
    /// <param name="realm">The realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="clientId">The ID of the client where the role is to be created.</param>
    /// <param name="role">The <see cref="KcRole"/> object representing the role to be created.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the response of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="clientId"/>, or <paramref name="role"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> CreateClientRoleAsync(string realm, string accessToken, string clientId, KcRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of roles for a specified client within a realm.
    ///
    /// GET /{realm}/clients/{client-uuid}/roles
    /// </summary>
    /// <param name="realm">The realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="clientId">The ID of the client whose roles are to be retrieved.</param>
    /// <param name="filter">
    /// An optional <see cref="KcFilter"/> object to filter the results. If not provided, a default filter is used.
    /// </param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a list of <see cref="KcRole"/> objects for the specified client.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="clientId"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListClientRoleAsync(string realm, string accessToken, string clientId,
        KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of a specific role for a given client within a realm.
    ///
    /// GET /{realm}/clients/{client-uuid}/roles/{role-name}
    /// </summary>
    /// <param name="realm">The realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="clientId">The ID of the client whose role is to be retrieved.</param>
    /// <param name="name">The name of the role to retrieve.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcRole"/> object with details of the specified role.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="clientId"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<KcRole>> GetClientRolesAsync(string realm, string accessToken, string clientId, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether a specific role exists for a given client within a realm.
    /// </summary>
    /// <param name="realm">The realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="clientId">The ID of the client whose role existence is to be checked.</param>
    /// <param name="name">The name of the role to check for existence.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating whether the role exists.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="clientId"/>, or <paramref name="name"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<bool>> IsClientRoleExistsAsync(string realm, string accessToken, string clientId, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing client role within a specified realm and client.
    ///
    /// PUT /{realm}/clients/{client-uuid}/roles/{role-name}
    /// </summary>
    /// <param name="realm">The realm where the client resides.</param>
    /// <param name="accessToken">The access token used for authorization.</param>
    /// <param name="clientId">The ID of the client to which the role belongs.</param>
    /// <param name="name">The name of the role to be updated.</param>
    /// <param name="role">The updated role details.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the update operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="clientId"/>,
    /// <paramref name="name"/>, or <paramref name="role"/> parameters are invalid.
    /// </exception>
    Task<KcResponse<object>> UpdateClientRoleAsync(string realm, string accessToken, string clientId, string name,
        KcRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a client role with the specified name in the given realm and client.
    ///
    /// DELETE /{realm}/clients/{client-uuid}/roles/{role-name}
    /// </summary>
    /// <param name="realm">The realm in which the client role resides.</param>
    /// <param name="accessToken">The access token to authenticate the request.</param>
    /// <param name="clientId">The ID of the client to which the role belongs.</param>
    /// <param name="name">The name of the client role to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="KcResponse{T}"/> containing the result of the deletion operation.</returns>
    /// <exception cref="KcException">
    /// Thrown when any of the following conditions occur:
    /// <list type="bullet">
    /// <item><description>The realm or access token is invalid.</description></item>
    /// <item><description>The clientId parameter is null, empty, or whitespace.</description></item>
    /// <item><description>The name parameter is null, empty, or whitespace.</description></item>
    /// </list>
    /// </exception>
    Task<KcResponse<object>> DeleteClientRoleAsync(string realm, string accessToken, string clientId, string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a client role to a composite role.
    ///
    /// POST /{realm}/clients/{client-uuid}/roles/{role-name}/composites
    /// </summary>
    /// <param name="realm">The realm name where the roles exist.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="name">The name of the role to which composites are being added.</param>
    /// <param name="roles">The collection of roles to be added as composites.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing the result of the operation or an error if the operation fails.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null or invalid.</exception>
    Task<KcResponse<object>> AddClientRoleToCompositeAsync(string realm, string accessToken, string clientId,
        string name, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the composite roles associated with a specific client role.
    ///
    /// GET /{realm}/clients/{client-uuid}/roles/{role-name}/composites
    /// </summary>
    /// <param name="realm">The realm name where the roles exist.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="name">The name of the client role whose composites are being retrieved.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing a collection of composite roles
    /// or an error if the operation fails.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null or invalid.</exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetClientCompositeRolesAsync(string realm, string accessToken,
        string clientId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes composite roles associated with a specific client role.
    ///
    /// DELETE /{realm}/clients/{client-uuid}/roles/{role-name}/composites
    /// </summary>
    /// <param name="realm">The realm name where the roles exist.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="name">The name of the client role from which composites are being removed.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null or invalid.</exception>
    Task<KcResponse<object>> RemoveClientRoleFromCompositeAsync(string realm, string accessToken, string clientId,
        string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the groups associated with a specific client role.
    ///
    /// GET /{realm}/clients/{client-uuid}/roles/{role-name}/groups
    /// </summary>
    /// <param name="realm">The realm name where the client role exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="name">The name of the client role.</param>
    /// <param name="filter">An optional filter to refine the group search.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a list of <see cref="KcGroup"/> objects associated with the client role.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null or invalid.</exception>
    Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsInClientRoleAsync(string realm, string accessToken, string clientId,
        string name, KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the management permissions associated with a specific client role.
    ///
    /// GET /{realm}/clients/{client-uuid}/roles/{role-name}/management/permissions
    /// </summary>
    /// <param name="realm">The realm name where the client role exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="name">The name of the client role.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcPermissionManagement"/> object
    /// associated with the specified client role.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null or invalid.</exception>
    Task<KcResponse<KcPermissionManagement>> GetClientRoleAuthorizationPermissionsAsync(string realm,
        string accessToken, string clientId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the management permissions associated with a specific client role.
    ///
    /// PUT /{realm}/clients/{client-uuid}/roles/{role-name}/management/permissions
    /// </summary>
    /// <param name="realm">The realm name where the client role exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="name">The name of the client role.</param>
    /// <param name="permissionManagement">
    /// The <see cref="KcPermissionManagement"/> object representing the updated permissions.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated <see cref="KcPermissionManagement"/> object.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null or invalid.</exception>
    Task<KcResponse<KcPermissionManagement>> SetClientRoleAuthorizationPermissionsAsync(string realm,
        string accessToken, string clientId, string name, KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of users associated with a specific client role in a given realm.
    ///
    /// GET /{realm}/clients/{client-uuid}/roles/{role-name}/users
    /// </summary>
    /// <param name="realm">The realm name where the client role exists.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="clientId">The unique identifier of the client containing the role.</param>
    /// <param name="name">The name of the client role.</param>
    /// <param name="filter">
    /// An optional <see cref="KcFilter"/> object to apply query filters for the list of users.
    /// If no filter is provided, a default filter is used.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcUser"/> objects
    /// representing the users associated with the specified client role.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null or invalid.</exception>
    Task<KcResponse<IEnumerable<KcUser>>> GetUsersInClientRoleAsync(string realm, string accessToken, string clientId,
        string name, KcFilter filter = null, CancellationToken cancellationToken = default);
}
