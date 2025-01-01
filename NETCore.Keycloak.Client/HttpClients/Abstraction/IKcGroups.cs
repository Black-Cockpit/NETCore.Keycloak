using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak groups REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_groups_resource"/>
/// </summary>
public interface IKcGroups
{
    /// <summary>
    /// Creates a new group in a specified Keycloak realm.
    ///
    /// POST /{realm}/groups
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group will be created.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="kcGroup">The <see cref="KcGroup"/> object containing the details of the group to create.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the created <see cref="KcGroup"/> object,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group object is null or invalid.
    /// </exception>
    Task<KcResponse<KcGroup>> CreateAsync(string realm, string accessToken, KcGroup kcGroup,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of groups from a specified Keycloak realm, optionally filtered by query parameters.
    ///
    /// GET /{realm}/groups
    /// </summary>
    /// <param name="realm">The Keycloak realm from which to retrieve the groups.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="filter">
    /// An optional <see cref="KcGroupFilter"/> object containing query parameters for filtering the results.
    /// If not provided, all groups will be retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcGroup"/> objects
    /// representing the groups in the realm, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm or access token is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcGroup>>> ListAsync(string realm, string accessToken, KcGroupFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of groups in a specified Keycloak realm, optionally filtered by query parameters.
    ///
    /// GET /{realm}/groups/count
    /// </summary>
    /// <param name="realm">The Keycloak realm for which to retrieve the group count.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="filter">
    /// An optional <see cref="KcGroupFilter"/> object containing query parameters to filter the group count.
    /// If not provided, all groups will be included in the count.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the count of groups as an object,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm or access token is null or invalid.
    /// </exception>
    Task<KcResponse<KcCount>> CountAsync(string realm, string accessToken, KcGroupFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves details of a specific group in a specified Keycloak realm.
    ///
    /// GET /{realm}/groups/{group-id}
    /// </summary>
    /// <param name="realm">The Keycloak realm to query.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The unique identifier of the group to retrieve.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcGroup"/> object representing the group details,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcGroup>> GetAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the details of a specific group in a specified Keycloak realm.
    ///
    /// PUT /{realm}/groups/{group-id}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The unique identifier of the group to update.</param>
    /// <param name="kcGroup">
    /// A <see cref="KcGroup"/> object containing the updated details of the group.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a <see cref="KcGroup"/> object representing the updated group details,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, group ID, or updated group data is null or invalid.
    /// </exception>
    Task<KcResponse<KcGroup>> UpdateAsync(string realm, string accessToken, string id, KcGroup kcGroup,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific group from a specified Keycloak realm.
    ///
    /// DELETE /{realm}/groups/{group-id}
    /// </summary>
    /// <param name="realm">The Keycloak realm from which the group will be deleted.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The unique identifier of the group to be deleted.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an object indicating the result of the delete operation,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID is null or invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets or creates a child group for a specified group in a Keycloak realm.
    ///
    /// POST /{realm}/groups/{group-id}/children
    /// </summary>
    /// <remarks>
    /// If the group does not exist, it will be created and then set the subgroups,
    /// otherwise the existing group will be set to the subgroups.
    /// </remarks>
    /// <param name="realm">The Keycloak realm in which the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The unique identifier of the parent group.</param>
    /// <param name="kcGroup">
    /// The child group to be created or set. Contains the group details.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the created or updated <see cref="KcGroup"/> details,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, parent group ID, or child group is null or invalid.
    /// </exception>
    Task<KcResponse<KcGroup>> SetOrCreateChildAsync(string realm, string accessToken, string id, KcGroup kcGroup,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the management permissions for a specific group in a Keycloak realm.
    ///
    /// GET /{realm}/groups/{group-id}/management/permissions
    /// </summary>
    /// <param name="realm">The Keycloak realm in which the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The unique identifier of the group whose management permissions are being retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcPermissionManagement"/> details
    /// for the group, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID is null or invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> GetAuthorizationManagementPermissionAsync(string realm, string accessToken,
        string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the management permissions for a specific group in a Keycloak realm.
    ///
    /// PUT /{realm}/groups/{group-id}/management/permissions
    /// </summary>
    /// <param name="realm">The Keycloak realm in which the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The unique identifier of the group whose management permissions are being set.</param>
    /// <param name="permissionManagement">
    /// The <see cref="KcPermissionManagement"/> object containing the management permissions to be set.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated <see cref="KcPermissionManagement"/> details
    /// for the group, or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, group ID, or permissionManagement object is null or invalid.
    /// </exception>
    Task<KcResponse<KcPermissionManagement>> SetAuthorizationManagementPermissionAsync(string realm, string accessToken,
        string id, KcPermissionManagement permissionManagement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the members of a specific group in a Keycloak realm.
    ///
    /// GET /{realm}/groups/{group-id}/members
    /// </summary>
    /// <param name="realm">The Keycloak realm in which the group resides.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="id">The unique identifier of the group whose members are being retrieved.</param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a list of <see cref="KcUser"/>
    /// objects representing the group's members,
    /// or an error response if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the realm, access token, or group ID is null or invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcUser>>> GetMembersAsync(string realm, string accessToken, string id,
        CancellationToken cancellationToken = default);
}
