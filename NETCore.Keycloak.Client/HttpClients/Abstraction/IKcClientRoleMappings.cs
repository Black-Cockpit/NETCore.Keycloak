using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak client role mapping client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_client_role_mappings_resource"/>
/// </summary>
public interface IKcClientRoleMappings
{
    /// <summary>
    /// Maps client-level roles to a group in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The ID of the group to which the roles will be assigned.</param>
    /// <param name="clientId">The ID of the client whose roles are being mapped.</param>
    /// <param name="roles">The collection of roles to assign to the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, groupId, clientId, or roles collection is null or invalid.
    /// </exception>
    Task<KcResponse<object>> MapClientRolesToGroupAsync(string realm, string accessToken, string groupId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the client-level roles that are mapped to a group in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The ID of the group for which the roles are being retrieved.</param>
    /// <param name="clientId">The ID of the client whose roles are mapped to the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/> objects
    /// representing the roles mapped to the group, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, groupId, or clientId is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetGroupMappedClientRolesAsync(string realm, string accessToken,
        string groupId, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified client-level roles mapped to a group in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The ID of the group from which the roles will be removed.</param>
    /// <param name="clientId">The ID of the client whose roles are being unmapped.</param>
    /// <param name="roles">The collection of roles to remove from the group.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, groupId, clientId, or roles collection is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteGroupClientRoleMappingsAsync(string realm, string accessToken, string groupId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the client-level roles available to be assigned to a group in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The ID of the group for which the available roles are being retrieved.</param>
    /// <param name="clientId">The ID of the client whose available roles are being retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/> objects
    /// representing the roles available for assignment to the group,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, groupId, or clientId is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetGroupAvailableClientRolesAsync(string realm, string accessToken,
        string groupId, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the composite (effective) client-level roles assigned to a group in a Keycloak realm,
    /// optionally filtered by specific criteria.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="groupId">The ID of the group for which the composite roles are being retrieved.</param>
    /// <param name="clientId">The ID of the client whose composite roles are being retrieved.</param>
    /// <param name="filter">
    /// Optional filter criteria for the roles, such as whether a brief representation is required.
    /// If <c>null</c>, all composite roles are retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/> objects
    /// representing the composite roles assigned to the group, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, groupId, or clientId is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetGroupCompositeClientRolesAsync(string realm, string accessToken,
        string groupId, string clientId, KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Maps client-level roles to a user in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The ID of the user to whom the roles will be assigned.</param>
    /// <param name="clientId">The ID of the client whose roles are being mapped.</param>
    /// <param name="roles">The collection of roles to assign to the user.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, userId, clientId, or roles collection is null or invalid.
    /// </exception>
    Task<KcResponse<object>> MapClientRolesToUserAsync(string realm, string accessToken, string userId, string clientId,
        IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the client-level roles that are mapped to a user in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The ID of the user whose mapped roles are being retrieved.</param>
    /// <param name="clientId">The ID of the client whose roles are mapped to the user.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/> objects
    /// representing the roles mapped to the user, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, userId, or clientId is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetUserMappedClientRolesAsync(string realm, string accessToken, string userId,
        string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes client-level roles mapped to a user in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The ID of the user from whom the roles will be removed.</param>
    /// <param name="clientId">The ID of the client whose roles are being unmapped.</param>
    /// <param name="roles">The collection of roles to remove from the user.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating success or failure,
    /// along with any associated error messages or exceptions.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, userId, clientId, or roles collection is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteUserClientRoleMappingsAsync(string realm, string accessToken, string userId,
        string clientId, IEnumerable<KcRole> roles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the client-specific roles available to be assigned to a user in a Keycloak realm.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The ID of the user for whom the available roles are being retrieved.</param>
    /// <param name="clientId">The ID of the client whose available roles are being retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/> objects
    /// representing the roles available for assignment to the user,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, userId, or clientId is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetUserAvailableClientRolesAsync(string realm, string accessToken,
        string userId, string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the composite (effective) client-specific roles assigned to a user in a Keycloak realm,
    /// optionally filtered by specific criteria.
    /// </summary>
    /// <param name="realm">The Keycloak realm where the user resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The ID of the user whose composite roles are being retrieved.</param>
    /// <param name="clientId">The ID of the client whose composite roles are being retrieved.</param>
    /// <param name="filter">
    /// Optional filter criteria for the roles, such as whether a brief representation is required.
    /// If <c>null</c>, all composite roles are retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcRole"/> objects
    /// representing the composite roles assigned to the user, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, userId, or clientId is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcRole>>> GetUserCompositeClientRolesAsync(string realm, string accessToken,
        string userId, string clientId, KcFilter filter = null, CancellationToken cancellationToken = default);
}
