using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcClientRoleMappings"/>
internal sealed class KcClientRoleMappings(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcClientRoleMappings
{
    /// <inheritdoc cref="IKcClientRoleMappings.MapClientRolesToGroupAsync"/>
    public Task<KcResponse<object>> MapClientRolesToGroupAsync(
        string realm,
        string accessToken,
        string groupId,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the groupId is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return an empty response if the role collection is empty.
        if ( !roles.Any() )
        {
            return Task.FromResult(new KcResponse<object>());
        }

        // Construct the URL for mapping client roles to the group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}";

        // Process the request to map the client roles to the group.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            $"Unable to assign client {clientId} roles to group {groupId}",
            roles,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupMappedClientRolesAsync"/>
    public Task<KcResponse<IEnumerable<KcRole>>> GetGroupMappedClientRolesAsync(
        string realm,
        string accessToken,
        string groupId,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the groupId is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL for retrieving the mapped client roles for the group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}";

        // Process the request to retrieve the mapped client roles for the group.
        return ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get group client roles",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupMappedClientRolesAsync"/>
    public Task<KcResponse<object>> DeleteGroupClientRoleMappingsAsync(
        string realm,
        string accessToken,
        string groupId,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the groupId is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Validate that the role collection is not null.
        if ( roles == null )
        {
            throw new KcException($"{nameof(roles)} is required");
        }

        // Return an empty response if the role collection is empty.
        if ( !roles.Any() )
        {
            return Task.FromResult(new KcResponse<object>());
        }

        // Construct the URL for deleting the mapped client roles from the group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}";

        // Process the request to remove the client roles from the group.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete client roles assigned to group",
            roles,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupAvailableClientRolesAsync"/>
    public Task<KcResponse<IEnumerable<KcRole>>> GetGroupAvailableClientRolesAsync(
        string realm,
        string accessToken,
        string groupId,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the groupId is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL for retrieving the available client roles for the group.
        var url = $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}/available";

        // Process the request to retrieve the available client roles for the group.
        return ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get available client-level roles that can be assigned to group",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetGroupCompositeClientRolesAsync"/>
    public Task<KcResponse<IEnumerable<KcRole>>> GetGroupCompositeClientRolesAsync(
        string realm,
        string accessToken,
        string groupId,
        string clientId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the groupId is not null or empty.
        ValidateRequiredString(nameof(groupId), groupId);

        // Construct the URL for retrieving the composite client roles for the group, applying filters if specified.
        var url = filter?.BriefRepresentation != null
            ? $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}/composite?briefRepresentation={filter.BriefRepresentation}"
            : $"{BaseUrl}/{realm}/groups/{groupId}/role-mappings/clients/{clientId}/composite";

        // Process the request to retrieve the composite client roles for the group.
        return ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get effective client-level roles assigned to group",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.MapClientRolesToUserAsync"/>
    public Task<KcResponse<object>> MapClientRolesToUserAsync(
        string realm,
        string accessToken,
        string userId,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the userId is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return an empty response if the role collection is empty.
        if ( !roles.Any() )
        {
            return Task.FromResult(new KcResponse<object>());
        }

        // Construct the URL for mapping client roles to the user.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}";

        // Process the request to map the client roles to the user.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to assign client roles to user",
            roles,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetUserMappedClientRolesAsync"/>
    public Task<KcResponse<IEnumerable<KcRole>>> GetUserMappedClientRolesAsync(
        string realm,
        string accessToken,
        string userId,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the userId is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL for retrieving the mapped client roles for the user.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}";

        // Process the request to retrieve the mapped client roles for the user.
        return ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get user client roles",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.DeleteUserClientRoleMappingsAsync"/>
    public Task<KcResponse<object>> DeleteUserClientRoleMappingsAsync(
        string realm,
        string accessToken,
        string userId,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the userId is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return an empty response if the role collection is empty.
        if ( !roles.Any() )
        {
            return Task.FromResult(new KcResponse<object>());
        }

        // Construct the URL for deleting the mapped client roles from the user.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}";

        // Process the request to remove the client roles from the user.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete client roles assigned to user",
            roles,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetUserAvailableClientRolesAsync"/>
    public Task<KcResponse<IEnumerable<KcRole>>> GetUserAvailableClientRolesAsync(
        string realm,
        string accessToken,
        string userId,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the userId is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL for retrieving the available client roles for the user.
        var url = $"{BaseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}/available";

        // Process the request to retrieve the available client roles for the user.
        return ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get available client-level roles that can be assigned to user",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientRoleMappings.GetUserCompositeClientRolesAsync"/>
    public Task<KcResponse<IEnumerable<KcRole>>> GetUserCompositeClientRolesAsync(
        string realm,
        string accessToken,
        string userId,
        string clientId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId is not null or empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the userId is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Construct the URL for retrieving the composite client roles for the user, applying filters if specified.
        var url = filter?.BriefRepresentation != null
            ? $"{BaseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}/composite?briefRepresentation={filter.BriefRepresentation}"
            : $"{BaseUrl}/{realm}/users/{userId}/role-mappings/clients/{clientId}/composite";

        // Process the request to retrieve the composite client roles for the user.
        return ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get effective client-level roles assigned to user",
            cancellationToken: cancellationToken
        );
    }
}
