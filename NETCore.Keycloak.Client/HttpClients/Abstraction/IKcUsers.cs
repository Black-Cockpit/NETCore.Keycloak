using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak users client REST client
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_users_resource"/>
/// </summary>
public interface IKcUsers
{
    /// <summary>
    /// Creates a new user in the specified realm.
    ///
    /// POST /{realm}/users
    /// </summary>
    /// <param name="realm">The realm name where the user will be created.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="user">The <see cref="KcUser"/> object containing the details of the user to be created.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the user creation operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="user"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified email address exists in the given realm.
    /// </summary>
    /// <param name="realm">The realm name where the user lookup will be performed.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="email">The email address of the user to check for existence.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a boolean value indicating whether a user with the given email exists.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="email"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<bool>> IsUserExistsByEmailAsync(string realm, string accessToken, string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of users from the specified realm, optionally filtered by the provided user filter.
    ///
    /// GET /{realm}/users
    /// </summary>
    /// <param name="realm">The realm name where the user lookup will be performed.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="filter">An optional <see cref="KcUserFilter"/> to apply specific criteria for user retrieval.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable collection of users matching the specified criteria.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/> or <paramref name="accessToken"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcUser>>> ListUserAsync(string realm, string accessToken, KcUserFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of users in the specified realm, optionally filtered by the provided user filter.
    ///
    /// GET /{realm}/users/count
    /// </summary>
    /// <param name="realm">The realm name where the users count will be calculated.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="filter">An optional <see cref="KcUserFilter"/> to apply specific criteria for filtering users.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the count of users matching the specified criteria.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/> or <paramref name="accessToken"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<object>> CountAsync(string realm, string accessToken, KcUserFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of a specific user within the specified realm by user ID.
    ///
    /// GET /{realm}/users/{id}
    /// </summary>
    /// <param name="realm">The realm name where the user resides.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the details of the specified user.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="userId"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<KcUser>> GetAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the details of a specific user within the specified realm by user ID.
    ///
    /// PUT /{realm}/users/{id}
    /// </summary>
    /// <param name="realm">The realm name where the user resides.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user to update.</param>
    /// <param name="user">The user object containing the updated details.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the update operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="userId"/>, or <paramref name="user"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<object>> UpdateAsync(string realm, string accessToken, string userId, KcUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific user from the specified realm by user ID.
    ///
    /// DELETE /{realm}/users/{id}
    /// </summary>
    /// <param name="realm">The realm name where the user resides.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the delete operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="userId"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the credentials of a specific user from the specified realm.
    ///
    /// GET /{realm}/users/{id}/credentials
    /// </summary>
    /// <param name="realm">The realm name where the user resides.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user whose credentials are to be retrieved.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a list of <see cref="KcCredentials"/> for the user.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="userId"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcCredentials>>> GetCredentialsAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific credential associated with a user in the specified realm.
    ///
    /// DELETE /{realm}/users/{id}/credentials/{credentialId}
    /// </summary>
    /// <param name="realm">The realm name where the user resides.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user whose credential is to be deleted.</param>
    /// <param name="credentialsId">The unique identifier of the credential to be deleted.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="userId"/>,
    /// or <paramref name="credentialsId"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteCredentialsAsync(string realm, string accessToken, string userId,
        string credentialsId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the label of a specific credential for a user in the specified realm.
    ///
    /// PUT /{realm}/users/{id}/credentials/{credentialId}/userLabel
    /// </summary>
    /// <param name="realm">The realm name where the user resides.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user whose credential label is to be updated.</param>
    /// <param name="credentialsId">The unique identifier of the credential whose label is to be updated.</param>
    /// <param name="label">The new label to assign to the credential.</param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="userId"/>,
    /// <paramref name="credentialsId"/>, or <paramref name="label"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<object>> UpdateCredentialsLabelAsync(string realm, string accessToken, string userId,
        string credentialsId, string label, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the list of groups associated with a specific user in the specified realm.
    ///
    /// GET /{realm}/users/{id}/groups
    /// </summary>
    /// <param name="realm">The realm name where the user resides.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user whose groups are to be retrieved.</param>
    /// <param name="filter">
    /// Optional. A <see cref="KcFilter"/> object to filter the groups retrieved for the user.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a list of <see cref="KcGroup"/> objects associated with the user.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if the <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="userId"/> parameter is invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcGroup>>> UserGroupsAsync(string realm, string accessToken, string userId,
        KcFilter filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the groups associated with a specific user in the specified realm.
    ///
    /// GET /{realm}/users/{id}/groups/count
    /// </summary>
    /// <param name="realm">The realm name where the user is located.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user whose group count is being retrieved.</param>
    /// <param name="filter">
    /// Optional. A <see cref="KcFilter"/> instance to filter the groups included in the count.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the count of groups associated with the specified user.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="userId"/> is invalid.
    /// </exception>
    Task<KcResponse<KcCount>> CountGroupsAsync(string realm, string accessToken, string userId, KcFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a user to a specified group in the given realm.
    ///
    /// PUT /{realm}/users/{id}/groups/{groupId}
    /// </summary>
    /// <param name="realm">The realm name where the group is located.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user to be added to the group.</param>
    /// <param name="groupId">The unique identifier of the group to which the user will be added.</param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="userId"/>, or <paramref name="groupId"/> is invalid.
    /// </exception>
    Task<KcResponse<object>> AddToGroupAsync(string realm, string accessToken, string userId, string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a user from a specified group in the given realm.
    ///
    /// DELETE /{realm}/users/{id}/groups/{groupId}
    /// </summary>
    /// <param name="realm">The realm name where the group is located.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user to be removed from the group.</param>
    /// <param name="groupId">The unique identifier of the group from which the user will be removed.</param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="userId"/>, or <paramref name="groupId"/> is invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteFromGroupAsync(string realm, string accessToken, string userId, string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the password for a specified user in the given realm.
    ///
    /// PUT /{realm}/users/{id}/reset-password
    /// </summary>
    /// <param name="realm">The realm name where the user is located.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user whose password will be reset.</param>
    /// <param name="credentials">The new credentials for resetting the password.</param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the updated credentials or an error if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if <paramref name="realm"/>, <paramref name="accessToken"/>, <paramref name="userId"/>, or <paramref name="credentials"/> is invalid.
    /// </exception>
    Task<KcResponse<object>> ResetPasswordAsync(string realm, string accessToken, string userId,
        KcCredentials credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active sessions for a specified user within the given realm.
    ///
    /// GET /{realm}/users/{id}/sessions
    /// </summary>
    /// <param name="realm">The realm name where the user is located.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user whose sessions will be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing a list of the user's active sessions or an error if the operation fails.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="userId"/> is invalid.
    /// </exception>
    Task<KcResponse<IEnumerable<KcSession>>> SessionsAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific session by its ID within the specified realm.
    ///
    /// DELETE /{realm}/sessions/{session}
    /// </summary>
    /// <param name="realm">The realm name where the session is located.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="sessionId">The unique identifier of the session to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the session deletion operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="sessionId"/> is invalid.
    /// </exception>
    Task<KcResponse<object>> DeleteSessionAsync(string realm, string accessToken, string sessionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the specified user from all active sessions within the specified realm.
    ///
    /// POST /{realm}/users/{id}/logout
    /// </summary>
    /// <param name="realm">The realm name where the user is located.</param>
    /// <param name="accessToken">The access token for authentication and authorization.</param>
    /// <param name="userId">The unique identifier of the user to be logged out.</param>
    /// <param name="cancellationToken">A token to monitor for request cancellation.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the success or failure of the logout operation.
    /// </returns>
    /// <exception cref="KcException">
    /// Thrown if <paramref name="realm"/>, <paramref name="accessToken"/>, or <paramref name="userId"/> is invalid.
    /// </exception>
    Task<KcResponse<object>> LogoutFromAllSessionsAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default);
}
