using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak role mapper REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_role_mapper_resource"/>
/// </summary>
public interface IKcRoleMappings
{
    /// <summary>
    /// Retrieves the role mappings associated with a specific group in a Keycloak realm.
    ///
    /// GET /{realm}/groups/{id}/role-mappings
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object containing the <see cref="KcRoleMapping"/> of the group.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcRoleMapping>> GetGroupRoleMappingsAsync(string realm, string accessToken, string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds realm-level role mappings to a specific group in a Keycloak realm.
    ///
    /// POST /{realm}/groups/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="roles">The collection of roles to be assigned to the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> object indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, group ID, or roles are null or invalid.
    /// </exception>
    Task<KcResponse<object>> AddGroupRealmRoleMappingsAsync(string realm, string accessToken, string groupId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of realm-level role mappings assigned to a specific group in a Keycloak realm.
    ///
    /// GET /{realm}/groups/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/>
    /// objects representing the group's realm-level role mappings.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID are null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListGroupRealmRoleMappingsAsync(string realm, string accessToken,
        string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes specified realm-level role mappings from a group in a Keycloak realm.
    ///
    /// DELETE /{realm}/groups/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="roles">The collection of roles to be removed from the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, group ID, or roles are null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteGroupRealmRoleMappingsAsync(string realm, string accessToken, string groupId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the available realm-level role mappings for a specific group in a Keycloak realm.
    ///
    /// GET /{realm}/groups/{id}/role-mappings/realm/available
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the collection of available roles that can be assigned to the group.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID are null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListGroupAvailableRealmRoleMappingsAsync(string realm, string accessToken,
        string groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the effective composite realm-level role mappings for a specific group in a Keycloak realm.
    ///
    /// GET /{realm}/groups/{id}/role-mappings/realm/composite
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="filter">
    /// An optional <see cref="KcFilter"/> to apply additional filtering to the retrieved roles.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the collection of effective composite roles assigned to the group.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID are null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListGroupEffectiveRealmRoleMappingsAsync(string realm, string accessToken,
        string groupId, KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the role mappings for a specific user in a Keycloak realm.
    ///
    /// GET /{realm}/users/{id}/role-mappings
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcRoleMapping"/> assigned to the user.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or user ID are null or invalid.
    /// </exception>
    Task<KcResponse<KcRoleMapping>> GetUserRoleMappingsAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds realm-level role mappings to a specific user in a Keycloak realm.
    ///
    /// POST /{realm}/users/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roles">A collection of <see cref="KcRole"/> to assign to the user.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, user ID, or role collection is null or invalid.
    /// </exception>
    Task<KcResponse<object>> AddUserRealmRoleMappingsAsync(string realm, string accessToken, string userId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of realm-level role mappings assigned to a specific user in a Keycloak realm.
    ///
    /// GET /{realm}/users/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a collection of <see cref="KcRole"/> objects assigned to the user.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or user ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListUserRealmRoleMappingsAsync(string realm, string accessToken,
        string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes realm-level role mappings assigned to a specific user in a Keycloak realm.
    ///
    /// DELETE /{realm}/users/{id}/role-mappings/realm
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roles">
    /// A collection of <see cref="KcRole"/> objects representing the roles to be removed from the user.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, user ID, or role collection is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteUserRealmRoleMappingsAsync(string realm, string accessToken, string userId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of available realm-level roles that can be assigned to a specific user in a Keycloak realm.
    ///
    /// GET /{realm}/users/{id}/role-mappings/realm/available
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/>
    /// objects representing the available roles.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or user ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListUserAvailableRealmRoleMappingsAsync(string realm, string accessToken,
        string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of effective (composite) realm-level roles assigned to a specific user in a Keycloak realm.
    ///
    /// GET /{realm}/users/{id}/role-mappings/realm/composite
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="filter">
    /// Optional filter to customize the results, such as querying for a brief representation.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/>
    /// objects representing the effective roles.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or user ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> ListUserEffectiveRealmRoleMappingsAsync(string realm, string accessToken,
        string userId, KcFilter filter = null, CancellationToken cancellationToken = default);
}
