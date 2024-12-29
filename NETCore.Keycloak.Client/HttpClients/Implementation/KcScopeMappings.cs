using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcScopeMappings"/>
internal sealed class KcScopeMappings(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcScopeMappings
{
    /// <inheritdoc cref="IKcScopeMappings.AddClientRolesToScopeAsync"/>
    public async Task<KcResponse<object>> AddClientRolesToScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return an empty response if no roles are provided.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to add roles to the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}";

        // Process the request to add client-level roles to the client scope.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add client-level roles to the client’s scope",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm and access token inputs are valid.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to retrieve roles associated with the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}";

        // Process the request to retrieve the list of roles associated with the client scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get the roles associated with a client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveClientRolesFromScopeAsync"/>
    public async Task<KcResponse<object>> RemoveClientRolesFromScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to remove roles from the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}";

        // Process the request to remove roles from the client scope.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to remove client-level roles from the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAvailableForScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to list roles available for the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}/available";

        // Process the request to retrieve the available roles for the client scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list the roles for the client that can be associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListCompositeClientRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeClientRolesAssociatedToScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        string clientId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Initialize the filter if none is provided.
        filter ??= new KcFilter();

        // Construct the URL to list composite roles associated with the specified client scope.
        var url =
            $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/clients/{clientId}/composite{filter.BuildQuery()}";

        // Process the request to retrieve the associated composite roles for the client scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list the roles for the client that are associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.AddRolesToScopeAsync"/>
    public async Task<KcResponse<object>> AddRolesToScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return early if the role collection is empty.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to add realm-level roles to the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm";

        // Process the request to add the roles to the client scope.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add a set of realm-level roles to the client’s scope",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL to list the realm-level roles associated with the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm";

        // Process the request to retrieve the associated roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm-level roles associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveRolesFromScopeAsync"/>
    public async Task<KcResponse<object>> RemoveRolesFromScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL to remove roles from the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm";

        // Process the request to remove the specified roles from the client scope.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to remove a set of realm-level roles from the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListRolesAvailableForScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL to list roles available for the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm/available";

        // Process the request to retrieve the list of available roles for the client scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm-level roles that are available to attach to this client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListCompositeRolesAssociatedToScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeRolesAssociatedToScopeAsync(
        string realm,
        string accessToken,
        string scopeId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the scopeId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Initialize the filter if it is not provided.
        filter ??= new KcFilter();

        // Construct the URL to list effective roles associated with the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}/scope-mappings/realm/composite{filter.BuildQuery()}";

        // Process the request to retrieve the list of effective roles for the client scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list effective realm-level roles associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.AddClientRolesToClientScopeAsync"/>
    public async Task<KcResponse<object>> AddClientRolesToClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        string clientName,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the clientName parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientName), clientName);

        // Validate that the role parameter is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return early if the role collection is empty.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to add roles to the client’s scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}";

        // Process the request to add the roles to the client’s scope.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add client-level roles to the client’s scope",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAssociatedToClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAssociatedToClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        string clientName,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the clientName parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientName), clientName);

        // Construct the URL to retrieve the roles associated with the client’s scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}";

        // Process the request to retrieve the roles associated with the client’s scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get the roles associated with a client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveClientRolesFromClientScopeAsync"/>
    public async Task<KcResponse<object>> RemoveClientRolesFromClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        string clientName,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the clientName parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientName), clientName);

        // Validate that the role parameter is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return early if the role collection is empty.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to remove roles from the client’s scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}";

        // Process the request to remove the specified roles from the client’s scope.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to remove client-level roles from the client’s scope",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListClientRolesAvailableForClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListClientRolesAvailableForClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        string clientName,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the clientName parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientName), clientName);

        // Construct the URL to list the available roles for the client’s scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}/available";

        // Process the request to retrieve the list of available roles for the client’s scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list the roles available for the client that can be associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListCompositeClientRolesAssociatedToClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeClientRolesAssociatedToClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        string clientName,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the clientName parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientName), clientName);

        // Initialize the filter if it is not provided.
        filter ??= new KcFilter();

        // Construct the URL to list the composite roles for the client’s scope.
        var url =
            $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/clients/{clientName}/composite{filter.BuildQuery()}";

        // Process the request to retrieve the list of composite roles for the client’s scope.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list effective roles for the client that are associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.AddRolesToScopeClientAsync"/>
    public async Task<KcResponse<object>> AddRolesToScopeClientAsync(
        string realm,
        string accessToken,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return early if the role collection is empty.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to add the roles to the client’s scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/realm";

        // Process the request to add the roles to the client's scope.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add a set of realm-level roles to the client’s scope",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListRolesAssociatedToClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAssociatedToClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to retrieve the roles associated with the client’s scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/realm";

        // Process the request to retrieve the associated roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm-level roles associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.RemoveRolesFromClientScopeAsync"/>
    public async Task<KcResponse<object>> RemoveRolesFromClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        IEnumerable<KcRole> roles,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Validate that the role collection is not null.
        ValidateNotNull(nameof(roles), roles);

        // Return early if the role collection is empty.
        if ( !roles.Any() )
        {
            return new KcResponse<object>();
        }

        // Construct the URL to remove the roles from the client’s scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/realm";

        // Process the request to remove the specified roles.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to remove a set of realm-level roles from the client’s scope",
            roles,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListRolesAvailableForClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListRolesAvailableForClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Construct the URL to list available roles for the client's scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/realm/available";

        // Process the request to retrieve the list of available roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm-level roles that are available to attach to this client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcScopeMappings.ListCompositeRolesAssociatedToClientScopeAsync"/>
    public async Task<KcResponse<IEnumerable<KcRole>>> ListCompositeRolesAssociatedToClientScopeAsync(
        string realm,
        string accessToken,
        string clientId,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the clientId parameter is not null, empty, or whitespace.
        ValidateRequiredString(nameof(clientId), clientId);

        // Initialize the filter if it is null.
        filter ??= new KcFilter();

        // Construct the URL to list composite roles associated with the client's scope.
        var url = $"{BaseUrl}/{realm}/clients/{clientId}/scope-mappings/realm/composite{filter.BuildQuery()}";

        // Process the request to retrieve the list of composite roles.
        return await ProcessRequestAsync<IEnumerable<KcRole>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list effective realm-level roles associated with the client’s scope",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}
