using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcRoles"/>
internal sealed class KcRoles(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcRoles
{
    /// <inheritdoc cref="IKcRoles.CreateAsync"/>
    public async Task<KcResponse<object>> CreateAsync(
        string realm,
        string accessToken,
        KcRole role,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the role object is not null.
        ValidateNotNull(nameof(role), role);

        // Construct the URL for creating the role.
        var url = $"{BaseUrl}/{realm}/roles";

        // Process the request to create the realm role.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create realm role",
            role,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.ListAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListAsync(
        string realm,
        string accessToken,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Initialize the filter if not provided.
        filter ??= new KcFilter();

        // Construct the URL with the optional query parameters from the filter.
        var url = $"{BaseUrl}/{realm}/roles{filter.BuildQuery()}";

        // Process the request to retrieve the realm roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm roles",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetAsync"/>
    public async Task<KcResponse<KcRole>> GetAsync(
        string realm,
        string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the role name is provided.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL to retrieve the role by its name.
        var url = $"{BaseUrl}/{realm}/roles/{name}";

        // Process the request to retrieve the realm role by name.
        return await ProcessRequestAsync<KcRole>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm role",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.IsRolesExistsAsync"/>
    public async Task<KcResponse<bool>> IsRolesExistsAsync(
        string realm,
        string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Retrieve the role using its name.
        var roleResponse = await GetAsync(realm, accessToken, name, cancellationToken).ConfigureAwait(false);

        // Determine the existence of the role based on the response.
        return roleResponse.IsError switch
        {
            // If an error indicates the role was not found, return false.
            true when roleResponse.ErrorMessage.Contains("Could not find role", StringComparison.OrdinalIgnoreCase) =>
                new KcResponse<bool>
                {
                    Response = false
                },
            // If the response contains a valid role ID, return true.
            false when !string.IsNullOrWhiteSpace(roleResponse.Response?.Id) => new KcResponse<bool>
            {
                Response = true
            },
            // Otherwise, propagate the error details.
            _ => new KcResponse<bool>
            {
                IsError = roleResponse.IsError,
                Exception = roleResponse.Exception,
                ErrorMessage = roleResponse.ErrorMessage
            }
        };
    }

    /// <inheritdoc cref="IKcRoles.GetByIdAsync"/>
    public async Task<KcResponse<KcRole>> GetByIdAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the role by its ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}";

        // Process the request to retrieve the realm role by ID.
        return await ProcessRequestAsync<KcRole>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm role by id",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.UpdateAsync"/>
    public async Task<KcResponse<object>> UpdateAsync(
        string realm,
        string accessToken,
        string name,
        KcRole role,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Validate that the role object is not null.
        ValidateNotNull(nameof(role), role);

        // Construct the URL for updating the realm role.
        var url = $"{BaseUrl}/{realm}/roles/{name}";

        // Process the request to update the realm role by name.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update realm role",
            role,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.UpdateByIdAsync"/>
    public async Task<KcResponse<KcRole>> UpdateByIdAsync(
        string realm,
        string accessToken,
        string id,
        KcRole role,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the role object is not null.
        ValidateNotNull(nameof(role), role);

        // Construct the URL for updating the realm role by ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}";

        // Process the request to update the realm role by ID.
        return await ProcessRequestAsync<KcRole>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update realm role by id",
            role,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.DeleteAsync"/>
    public async Task<KcResponse<object>> DeleteAsync(
        string realm,
        string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL for deleting the realm role by name.
        var url = $"{BaseUrl}/{realm}/roles/{name}";

        // Process the request to delete the realm role by name.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm role by name",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.DeleteByIdAsync"/>
    public async Task<KcResponse<object>> DeleteByIdAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for deleting the realm role by ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}";

        // Process the request to delete the realm role by ID.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm role by id",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.AddCompositeAsync"/>
    public async Task<KcResponse<object>> AddCompositeAsync(
        string realm,
        string accessToken,
        string name,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // If the role collection is empty, return a response indicating no operation was performed.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for adding composite roles to the specified realm role.
        var url = $"{BaseUrl}/{realm}/roles/{name}/composites";

        // Process the request to add composite roles.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add realm role composites",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.AddCompositeByIdAsync"/>
    public async Task<KcResponse<object>> AddCompositeByIdAsync(
        string realm,
        string accessToken,
        string id,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // If the role collection is empty, return a response indicating no operation was performed.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for adding composite roles to the specified realm role by ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}/composites";

        // Process the request to add composite roles.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add realm role composites by role id",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.ListCompositeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeAsync(
        string realm,
        string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL to retrieve the composite roles for the specified realm role.
        var url = $"{BaseUrl}/{realm}/roles/{name}/composites";

        // Process the request to retrieve the list of the realm composite roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm role composites",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.ListCompositeByIdAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeByIdAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL to retrieve the composite roles for the specified role ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}/composites";

        // Process the request to retrieve the list of composite roles by role ID.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm role composites by role id",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.DeleteCompositeAsync"/>
    public async Task<KcResponse<object>> DeleteCompositeAsync(
        string realm,
        string accessToken,
        string name,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // If the role collection is empty, return a response indicating no operation was performed.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to delete the composite roles for the specified role name.
        var url = $"{BaseUrl}/{realm}/roles/{name}/composites";

        // Process the request to delete the composite roles associated with the specified role name.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm composite role",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.DeleteCompositeByIdAsync"/>
    public async Task<KcResponse<object>> DeleteCompositeByIdAsync(
        string realm,
        string accessToken,
        string id,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // If the role collection is empty, return a response indicating no operation was performed.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to delete the composite roles for the specified role ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}/composites";

        // Process the request to delete the composite roles associated with the specified role ID.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm composite role by role id",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetClientLevelCompositesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesAsync(
        string realm,
        string accessToken,
        string name,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Validate that the client ID is provided and not empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to retrieve the client-level composites for the specified role.
        var url = $"{BaseUrl}/{realm}/roles/{name}/composites/clients/{clientId}";

        // Process the request to retrieve the client-level composite roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get role client level composites",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetClientLevelCompositesByIdAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetClientLevelCompositesByIdAsync(
        string realm,
        string accessToken,
        string id,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the client ID is provided and not empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to retrieve the client-level composites for the specified role by ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}/composites/clients/{clientId}";

        // Process the request to retrieve the client-level composite roles by role ID.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get role client level composites by role id",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetRealmLevelCompositesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesAsync(
        string realm,
        string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL to retrieve the realm-level composites for the specified role.
        var url = $"{BaseUrl}/{realm}/roles/{name}/composites/realm";

        // Process the request to retrieve the realm-level composite roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get role realm level composites",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetRealmLevelCompositesByIdAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetRealmLevelCompositesByIdAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL to retrieve the realm-level composites for the specified role by its ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}/composites/realm";

        // Process the request to retrieve the realm-level composite roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get role realm level composites by role id",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetGroupsAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsAsync(
        string realm,
        string accessToken,
        string name,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Initialize the filter if it is null.
        filter ??= new KcFilter();

        // Construct the URL to retrieve the groups associated with the specified role.
        var url = $"{BaseUrl}/{realm}/roles/{name}/groups{filter.BuildQuery()}";

        // Process the request to retrieve the groups associated with the role.
        return await ProcessRequestAsync<IEnumerable<KcGroup>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm role groups",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsAsync(
        string realm,
        string accessToken,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL to retrieve the authorization permissions for the specified role.
        var url = $"{BaseUrl}/{realm}/roles/{name}/management/permissions";

        // Process the request to retrieve the authorization permissions for the role.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm role management permission",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetAuthorizationPermissionsByIdAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> GetAuthorizationPermissionsByIdAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL to retrieve the authorization permissions for the specified role by ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}/management/permissions";

        // Process the request to retrieve the authorization permissions for the role by ID.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm role management permission by role id",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.SetAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsAsync(
        string realm,
        string accessToken,
        string name,
        KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Validate that the permission management object is not null.
        ValidateNotNull(nameof(permissionManagement), permissionManagement);

        // Construct the URL to set the authorization permissions for the specified role by name.
        var url = $"{BaseUrl}/{realm}/roles/{name}/management/permissions";

        // Process the request to set the authorization permissions for the role by name.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to set realm role management permission",
            permissionManagement,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.SetAuthorizationPermissionsByIdAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> SetAuthorizationPermissionsByIdAsync(
        string realm,
        string accessToken,
        string id,
        KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role ID is provided and not empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the permission management object is not null.
        ValidateNotNull(nameof(permissionManagement), permissionManagement);

        // Construct the URL to set the authorization permissions for the specified role by ID.
        var url = $"{BaseUrl}/{realm}/roles-by-id/{id}/management/permissions";

        // Process the request to set the authorization permissions for the role by ID.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to set realm role management permission by role id",
            permissionManagement,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetUserInRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> GetUserInRoleAsync(
        string realm,
        string accessToken,
        string name,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Initialize the filter if not provided.
        filter ??= new KcFilter();

        // Construct the URL to retrieve users associated with the specified role.
        var url = $"{BaseUrl}/{realm}/roles/{name}/users{filter.BuildQuery()}";

        // Process the request to retrieve the users associated with the specified role.
        return await ProcessRequestAsync<IEnumerable<KcUser>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm role users",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.CreateClientRoleAsync"/>
    public async Task<KcResponse<object>> CreateClientRoleAsync(
        string realm,
        string accessToken,
        string clientId,
        KcRole role,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the client ID is provided and not empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Ensure the role object is provided and not null.
        ValidateNotNull(nameof(role), role);

        // Construct the URL to create a new role for the specified client.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles";

        // Process the request to create the client role.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create client role",
            role,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.ListClientRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRoleAsync(
        string realm,
        string accessToken,
        string clientId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Ensure the client ID is provided and not empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Apply the provided filter or use the default filter.
        filter ??= new KcFilter();

        // Construct the URL to list roles for the specified client, including any query filters.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles{filter.BuildQuery()}";

        // Process the request to retrieve the list of client roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list client roles",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetClientRolesAsync"/>
    public async Task<KcResponse<KcRole>> GetClientRolesAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is provided and not empty.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the role name is provided and not empty.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL to get the specified role for the client.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}";

        // Process the request to retrieve the client role details.
        return await ProcessRequestAsync<KcRole>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get client role",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.IsClientRoleExistsAsync"/>
    public async Task<KcResponse<bool>> IsClientRoleExistsAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Retrieve the client role details using the provided parameters.
        var roleResponse = await GetClientRolesAsync(realm, accessToken, clientId, name, cancellationToken)
            .ConfigureAwait(false);

        // Determine the existence of the role based on the response.
        return roleResponse.IsError switch
        {
            // Role does not exist if the error message indicates it could not be found.
            true when roleResponse.ErrorMessage.Contains("Could not find role", StringComparison.OrdinalIgnoreCase) =>
                new KcResponse<bool>
                {
                    Response = false
                },
            // Role exists if the response contains a valid role ID.
            false when !string.IsNullOrWhiteSpace(roleResponse.Response?.Id) =>
                new KcResponse<bool>
                {
                    Response = true
                },
            // Return the error details if an unexpected error occurred.
            _ => new KcResponse<bool>
            {
                IsError = roleResponse.IsError,
                Exception = roleResponse.Exception,
                ErrorMessage = roleResponse.ErrorMessage
            }
        };
    }

    /// <inheritdoc cref="IKcRoles.UpdateClientRoleAsync"/>
    public async Task<KcResponse<object>> UpdateClientRoleAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        KcRole role,
        CancellationToken cancellationToken = default)
    {
        // Validate that the provided realm and access token are valid and accessible.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Validate that the role object is not null.
        ValidateNotNull(nameof(role), role);

        // Construct the URL for updating the specified client role.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}";

        // Process the request to update the client role and return the response.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update client role",
            role,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.DeleteClientRoleAsync"/>
    public async Task<KcResponse<object>> DeleteClientRoleAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate that the provided realm and access token are valid and accessible.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL for deleting the specified client role.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}";

        // Process the request to delete the client role and return the response.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete client role",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.AddClientRoleToCompositeAsync"/>
    public async Task<KcResponse<object>> AddClientRoleToCompositeAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Validate that the role collection parameter is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return early if the role collection is empty.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL for adding roles to the composite role.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}/composites";

        // Process the request to add the roles to the composite.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add composite to the client roles",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetClientCompositeRolesAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> GetClientCompositeRolesAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL for retrieving the composite roles.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}/composites";

        // Process the request to retrieve the composite roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get client composite roles",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.RemoveClientRoleFromCompositeAsync"/>
    public async Task<KcResponse<object>> RemoveClientRoleFromCompositeAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Construct the URL for removing composite roles.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}/composites";

        // Process the request to remove the composite roles.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to remove composite from the client roles",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetGroupsInClientRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> GetGroupsInClientRoleAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Initialize the filter if it is null.
        filter ??= new KcFilter();

        // Construct the URL to retrieve the groups associated with the client role.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}/groups{filter.BuildQuery()}";

        // Process the request to retrieve the list of groups.
        return await ProcessRequestAsync<IEnumerable<KcGroup>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list client role groups",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetClientRoleAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> GetClientRoleAuthorizationPermissionsAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to retrieve the management permissions for the client role.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}/management/permissions";

        // Process the request to retrieve the management permissions.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get client role management permission",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.SetClientRoleAuthorizationPermissionsAsync"/>
    public async Task<KcResponse<KcPermissionManagement>> SetClientRoleAuthorizationPermissionsAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the permission management object is not null.
        ValidateNotNull(nameof(permissionManagement), permissionManagement);

        // Construct the URL to update the management permissions for the client role.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}/management/permissions";

        // Process the request to update the management permissions.
        return await ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to set client role management permission",
            permissionManagement,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcRoles.GetUsersInClientRoleAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> GetUsersInClientRoleAsync(
        string realm,
        string accessToken,
        string clientId,
        string name,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the name parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(name), name);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Initialize the filter if not provided.
        filter ??= new KcFilter();

        // Construct the URL to retrieve the users in the specified client role.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/roles/{name}/users{filter.BuildQuery()}";

        // Process the request to retrieve the list of users.
        return await ProcessRequestAsync<IEnumerable<KcUser>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get client role users",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}
