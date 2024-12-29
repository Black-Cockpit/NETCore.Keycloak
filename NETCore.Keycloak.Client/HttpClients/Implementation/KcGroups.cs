using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcGroups"/>
internal sealed class KcGroups(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcGroups
{
    /// <inheritdoc cref="IKcGroups.CreateAsync"/>
    public async Task<KcResponse<KcGroup>> CreateAsync(
        string realm,
        string accessToken,
        KcGroup kcGroup,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group object is not null.
        ValidateNotNull(nameof(kcGroup), kcGroup);

        // Construct the URL for creating a new group in the specified realm.
        var url = $"{BaseUrl}/{realm}/groups";

        // Process the request to create the group.
        return await ProcessRequestAsync<KcGroup>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create realm group",
            kcGroup,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.ListAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> ListAsync(
        string realm,
        string accessToken,
        KcGroupFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Initialize the filter if not provided.
        filter ??= new KcGroupFilter();

        // Construct the URL for retrieving groups, including query parameters if specified.
        var url = $"{BaseUrl}/{realm}/groups{filter.BuildQuery()}";

        // Process the request to retrieve the list of groups.
        return await ProcessRequestAsync<IEnumerable<KcGroup>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm group",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.CountAsync"/>
    public async Task<KcResponse<object>> CountAsync(
        string realm,
        string accessToken,
        KcGroupFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Initialize the filter if not provided.
        filter ??= new KcGroupFilter();

        // Construct the URL for counting groups, including query parameters if specified.
        var url = $"{BaseUrl}/{realm}/groups/count{filter.BuildQuery()}";

        // Process the request to retrieve the count of groups.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to count realm group",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.GetAsync"/>
    public async Task<KcResponse<KcGroup>> GetAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the group details.
        var url = $"{BaseUrl}/{realm}/groups/{id}";

        // Process the request to retrieve the group details.
        return await ProcessRequestAsync<KcGroup>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm group",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.UpdateAsync"/>
    public async Task<KcResponse<KcGroup>> UpdateAsync(
        string realm,
        string accessToken,
        string id,
        KcGroup kcGroup,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the updated group data is not null.
        ValidateNotNull(nameof(kcGroup), kcGroup);

        // Construct the URL for updating the group details.
        var url = $"{BaseUrl}/{realm}/groups/{id}";

        // Process the request to update the group details.
        return await ProcessRequestAsync<KcGroup>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update realm group",
            kcGroup,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.DeleteAsync"/>
    public async Task<KcResponse<object>> DeleteAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for deleting the group.
        var url = $"{BaseUrl}/{realm}/groups/{id}";

        // Process the request to remove the group.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm group",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.SetOrCreateChildAsync"/>
    public async Task<KcResponse<KcGroup>> SetOrCreateChildAsync(
        string realm,
        string accessToken,
        string id,
        KcGroup kcGroup,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the parent group ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the child group details are provided.
        ValidateNotNull(nameof(kcGroup), kcGroup);

        // Construct the URL for setting or creating a child group.
        var url = $"{BaseUrl}/{realm}/groups/{id}/children";

        // Process the request to set or create the child group.
        return await ProcessRequestAsync<KcGroup>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to set or create child for realm group",
            kcGroup,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.GetAuthorizationManagementPermissionAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> GetAuthorizationManagementPermissionAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the group's management permissions.
        var url = $"{BaseUrl}/{realm}/groups/{id}/management/permissions";

        // Process the request to retrieve the management permissions.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm group management permission",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.SetAuthorizationManagementPermissionAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> SetAuthorizationManagementPermissionAsync(
        string realm,
        string accessToken,
        string id,
        KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the permissionManagement object is not null.
        ValidateNotNull(nameof(permissionManagement), permissionManagement);

        // Construct the URL for setting the group's management permissions.
        var url = $"{BaseUrl}/{realm}/groups/{id}/management/permissions";

        // Process the request to set the management permissions.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to set realm group management permission",
            permissionManagement,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcGroups.GetMembersAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> GetMembersAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the group ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the group's members.
        var url = $"{BaseUrl}/{realm}/groups/{id}/members";

        // Process the request to retrieve the group's members.
        return await ProcessRequestAsync<IEnumerable<KcUser>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm group members",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}
