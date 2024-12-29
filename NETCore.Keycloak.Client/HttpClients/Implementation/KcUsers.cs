using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcUsers"/>
internal sealed class KcUsers(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcUsers
{
    /// <inheritdoc cref="IKcUsers.CreateAsync"/>
    public async Task<KcResponse<object>> CreateAsync(
        string realm,
        string accessToken,
        KcUser user,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the user object is not null.
        ValidateNotNull(nameof(user), user);

        // Construct the URL to create a new user in the specified realm.
        var url = $"{BaseUrl}/{realm}/users";

        // Process the request to create the user.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create user",
            user,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.IsUserExistsByEmailAsync"/>
    public async Task<KcResponse<bool>> IsUserExistsByEmailAsync(
        string realm,
        string accessToken,
        string email,
        CancellationToken cancellationToken = default)
    {
        // Retrieve the list of users matching the specified email in the realm.
        var usersList = await ListUserAsync(realm, accessToken, new KcUserFilter
        {
            Email = email,
            Exact = true
        }, cancellationToken).ConfigureAwait(false);

        // Return the response based on the results of the user lookup.
        return usersList.IsError
            ? new KcResponse<bool>
            {
                IsError = usersList.IsError,
                Exception = usersList.Exception,
                ErrorMessage = usersList.ErrorMessage
            }
            : new KcResponse<bool>
            {
                Response = usersList.Response.Any()
            };
    }

    /// <inheritdoc cref="IKcUsers.ListUserAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> ListUserAsync(
        string realm,
        string accessToken,
        KcUserFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure a default user filter is used if none is provided.
        filter ??= new KcUserFilter();

        // Construct the URL for retrieving users with the optional filter.
        var url = $"{BaseUrl}/{realm}/users{filter.BuildQuery()}";

        // Process the request to retrieve the list of users.
        return await ProcessRequestAsync<IEnumerable<KcUser>>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to list realm {realm} users",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.CountAsync"/>
    public async Task<KcResponse<object>> CountAsync(
        string realm,
        string accessToken,
        KcUserFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure a default user filter is used if none is provided.
        filter ??= new KcUserFilter();

        // Construct the URL for retrieving the user count with the optional filter.
        var url = $"{BaseUrl}/{realm}/users/count{filter.BuildQuery()}";

        // Process the request to retrieve the count of users.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to count realm {realm} users",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.GetAsync"/>
    public async Task<KcResponse<KcUser>> GetAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL to retrieve the user details by ID.
        var url = $"{BaseUrl}/{realm}/users/{userId}";

        // Process the request to retrieve the user details.
        return await ProcessRequestAsync<KcUser>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to get user {userId}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.UpdateAsync"/>
    public async Task<KcResponse<object>> UpdateAsync(
        string realm,
        string accessToken,
        string userId,
        KcUser user,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Ensure the user object is provided and not null.
        ValidateNotNull(nameof(user), user);

        // Construct the URL to update the user details by ID.
        var url = $"{BaseUrl}/{realm}/users/{userId}";

        // Process the request to update the user details.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            $"Unable to update user {userId}",
            user,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.DeleteAsync"/>
    public async Task<KcResponse<object>> DeleteAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL to delete the user by ID.
        var url = $"{BaseUrl}/{realm}/users/{userId}";

        // Process the request to delete the user.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            $"Unable to delete user {userId}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.GetCredentialsAsync"/>
    public async Task<KcResponse<IEnumerable<KcCredentials>>> GetCredentialsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL to retrieve the user's credentials.
        var url = $"{BaseUrl}/{realm}/users/{userId}/credentials";

        // Process the request to retrieve the user's credentials.
        return await ProcessRequestAsync<IEnumerable<KcCredentials>>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to get user {userId} credentials",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.DeleteCredentialsAsync"/>
    public async Task<KcResponse<object>> DeleteCredentialsAsync(
        string realm,
        string accessToken,
        string userId,
        string credentialsId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Ensure the credentialsId parameter is provided and valid.
        ValidateRequiredString(nameof(credentialsId), credentialsId);

        // Construct the URL to delete the specific credential.
        var url = $"{BaseUrl}/{realm}/users/{userId}/credentials/{credentialsId}";

        // Process the request to delete the credential.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            $"Unable to delete user {userId} credentials",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.UpdateCredentialsLabelAsync"/>
    public async Task<KcResponse<object>> UpdateCredentialsLabelAsync(
        string realm,
        string accessToken,
        string userId,
        string credentialsId,
        string label,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Ensure the credentialsId parameter is provided and valid.
        ValidateRequiredString(nameof(credentialsId), credentialsId);

        // Ensure the label parameter is provided and valid.
        ValidateRequiredString(nameof(label), label);

        // Construct the URL to update the credential label.
        var url = $"{BaseUrl}/{realm}/users/{userId}/credentials/{credentialsId}/userLabel";

        // Create the content for the label update.
        using var content = new StringContent(label);

        // Process the request to update the credential label.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            $"Unable to update user {userId} credentials label",
            contentType: "text/plain",
            content: content,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.UserGroupsAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> UserGroupsAsync(
        string realm,
        string accessToken,
        string userId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Initialize the filter if none is provided.
        filter ??= new KcFilter();

        // Construct the URL to retrieve the user's groups.
        var url = $"{BaseUrl}/{realm}/users/{userId}/groups{filter.BuildQuery()}";

        // Process the request to retrieve the user's groups.
        return await ProcessRequestAsync<IEnumerable<KcGroup>>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to get user {userId} groups",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.CountGroupsAsync"/>
    public async Task<KcResponse<object>> CountGroupsAsync(
        string realm,
        string accessToken,
        string userId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Initialize the filter if none is provided.
        filter ??= new KcFilter();

        // Construct the URL to retrieve the count of user's groups.
        var url = $"{BaseUrl}/{realm}/users/{userId}/groups/count{filter.BuildQuery()}";

        // Process the request to count the user's groups.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to count user {userId} groups",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.AddToGroupAsync"/>
    public async Task<KcResponse<object>> AddToGroupAsync(
        string realm,
        string accessToken,
        string userId,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Ensure the groupId parameter is provided and valid.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL to add the user to the specified group.
        var url = $"{BaseUrl}/{realm}/users/{userId}/groups/{groupId}";

        // Process the request to add the user to the group.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            $"Unable to add user {userId} to group {groupId}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.DeleteFromGroupAsync"/>
    public async Task<KcResponse<object>> DeleteFromGroupAsync(
        string realm,
        string accessToken,
        string userId,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Ensure the groupId parameter is provided and valid.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL to remove the user from the specified group.
        var url = $"{BaseUrl}/{realm}/users/{userId}/groups/{groupId}";

        // Process the request to remove the user from the group.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            $"Unable to delete user {userId} from group {groupId}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.ResetPasswordAsync"/>
    public async Task<KcResponse<KcCredentials>> ResetPasswordAsync(
        string realm,
        string accessToken,
        string userId,
        KcCredentials credentials,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Ensure the credential parameter is provided and not null.
        ValidateNotNull(nameof(credentials), credentials);

        // Construct the URL to reset the user's password.
        var url = $"{BaseUrl}/{realm}/users/{userId}/reset-password";

        // Process the request to reset the user's password.
        return await ProcessRequestAsync<KcCredentials>(
            url,
            HttpMethod.Put,
            accessToken,
            $"Unable to update reset password for user {userId}",
            credentials,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.SessionsAsync"/>
    public async Task<KcResponse<IEnumerable<KcSession>>> SessionsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL to retrieve the user's sessions.
        var url = $"{BaseUrl}/{realm}/users/{userId}/sessions";

        // Process the request to retrieve the user's active sessions.
        return await ProcessRequestAsync<IEnumerable<KcSession>>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to get user {userId} sessions",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.DeleteSessionAsync"/>
    public async Task<KcResponse<object>> DeleteSessionAsync(
        string realm,
        string accessToken,
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the sessionId parameter is provided and valid.
        ValidateRequiredString(nameof(sessionId), sessionId);

        // Construct the URL to delete the specified session.
        var url = $"{BaseUrl}/{realm}/sessions/{sessionId}";

        // Process the request to delete the session.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            $"Unable delete session {sessionId}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcUsers.LogoutFromAllSessionsAsync"/>
    public async Task<KcResponse<object>> LogoutFromAllSessionsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the userId parameter is provided and valid.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL to logout the specified user from all sessions.
        var url = $"{BaseUrl}/{realm}/users/{userId}/logout";

        // Process the request to perform the logout operation.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to logout from all sessions",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}
