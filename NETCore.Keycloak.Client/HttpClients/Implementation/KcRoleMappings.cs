using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcRoleMappings"/>
internal sealed class KcRoleMappings(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcRoleMappings
{
    /// <inheritdoc cref="IKcRoleMappings.GetGroupRoleMappingsAsync"/>
    public async Task<KcResponse<KcRoleMapping>> GetGroupRoleMappingsAsync(
        string realm,
        string accessToken,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL for retrieving the role mappings of the specified group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings";

        // Process the request to retrieve the role mappings.
        return await ProcessRequestAsync<KcRoleMapping>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get group role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.AddGroupRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> AddGroupRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string groupId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return early if no roles are provided.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for adding realm role mappings to the specified group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/realm";

        // Process the request to add the role mappings.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add group realm role mappings",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.ListGroupRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListGroupRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL for retrieving the group's realm-level role mappings.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/realm";

        // Process the request to retrieve the role mappings.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get group realm role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.DeleteGroupRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> DeleteGroupRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string groupId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // If no roles are specified, return an empty response.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for deleting the group's realm-level role mappings.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/realm";

        // Process the request to remove the specified roles.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete group realm role mappings",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.ListGroupAvailableRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListGroupAvailableRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL for retrieving available realm-level role mappings for the group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/realm/available";

        // Process the request to retrieve the available roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get group realm available role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.ListGroupEffectiveRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListGroupEffectiveRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string groupId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Initialize the filter if not provided.
        filter ??= new KcFilter();

        // Construct the URL for retrieving effective composite realm-level role mappings for the group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/realm/composite{filter.BuildQuery()}";

        // Process the request to retrieve the effective composite roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get group realm composite role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.GetUserRoleMappingsAsync"/>
    public async Task<KcResponse<KcRoleMapping>> GetUserRoleMappingsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL for retrieving role mappings for the user.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings";

        // Process the request to retrieve the user role mappings.
        return await ProcessRequestAsync<KcRoleMapping>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get user role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.AddUserRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> AddUserRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string userId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // If the role collection is empty, return a default response.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for adding realm role mappings to the user.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/realm";

        // Process the request to add the role mappings to the user.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add user realm role mappings",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.ListUserRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListUserRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL for retrieving the user's realm role mappings.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/realm";

        // Process the request to retrieve the user role mappings.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get user realm role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.DeleteUserRealmRoleMappingsAsync"/>
    public async Task<KcResponse<object>> DeleteUserRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string userId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // If the role collection is empty, return an empty response.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for deleting the user's realm role mappings.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/realm";

        // Process the request to remove the user role mappings.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete user realm role mappings",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.ListUserAvailableRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListUserAvailableRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL for retrieving the user's available realm role mappings.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/realm/available";

        // Process the request to retrieve the user available role mappings.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get user realm available role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoleMappings.ListUserEffectiveRealmRoleMappingsAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListUserEffectiveRealmRoleMappingsAsync(
        string realm,
        string accessToken,
        string userId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Initialize the filter if not provided.
        filter ??= new KcFilter();

        // Construct the URL for retrieving the user's effective realm role mappings with the applied filter.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/realm/composite{filter.BuildQuery()}";

        // Process the request to retrieve the effective user role mappings.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get user realm composite role mappings",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}
