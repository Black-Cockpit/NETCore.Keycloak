using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcClients"/>
internal sealed class KcClients(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcClients
{
    /// <inheritdoc cref="IKcClients.CreateAsync"/>
    public Task<KcResponse<object>> CreateAsync(
        string realm,
        string accessToken,
        KcClient kcClient,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the kcClient object is not null.
        ValidateNotNull(nameof(kcClient), kcClient);

        // Construct the URL for creating the client in the specified realm.
        var url = $"{BaseUrl}/{realm}/clients";

        // Process the request to create the client.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add realm client",
            kcClient,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.ListAsync"/>
    public Task<KcResponse<IEnumerable<KcClient>>> ListAsync(
        string realm,
        string accessToken,
        KcClientFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Initialize the filter if it is null.
        filter ??= new KcClientFilter();

        // Construct the URL for listing the clients, including any filter criteria.
        var url = $"{BaseUrl}/{realm}/clients{filter.BuildQuery()}";

        // Process the request to retrieve the list of clients.
        return ProcessRequestAsync<IEnumerable<KcClient>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm client",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetAsync"/>
    public Task<KcResponse<KcClient>> GetAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the client by its ID.
        var url = $"{BaseUrl}/{realm}/clients/{id}";

        // Process the request to retrieve the client.
        return ProcessRequestAsync<KcClient>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.UpdateAsync"/>
    public Task<KcResponse<object>> UpdateAsync(
        string realm,
        string accessToken,
        string id,
        KcClient kcClient,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the kcClient object is not null.
        ValidateNotNull(nameof(kcClient), kcClient);

        // Construct the URL for updating the client in the specified realm.
        var url = $"{BaseUrl}/{realm}/clients/{id}";

        // Process the request to update the client.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update realm client",
            kcClient,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.DeleteAsync"/>
    public Task<KcResponse<object>> DeleteAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for deleting the client in the specified realm.
        var url = $"{BaseUrl}/{realm}/clients/{id}";

        // Process the request to delete the client.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm client",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GenerateNewSecretAsync"/>
    public Task<KcResponse<KcCredentials>> GenerateNewSecretAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for generating a new secret for the client.
        var url = $"{BaseUrl}/{realm}/clients/{id}/client-secret";

        // Process the request to generate the new secret.
        return ProcessRequestAsync<KcCredentials>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to generate realm client new secret",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetSecretAsync"/>
    public Task<KcResponse<KcCredentials>> GetSecretAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the client's secret.
        var url = $"{BaseUrl}/{realm}/clients/{id}/client-secret";

        // Process the request to retrieve the client secret.
        return ProcessRequestAsync<KcCredentials>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client secret",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetRotatedSecretAsync"/>
    public Task<KcResponse<KcCredentials>> GetRotatedSecretAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the client's rotated secret.
        var url = $"{BaseUrl}/{realm}/clients/{id}/client-secret/rotated";

        // Process the request to retrieve the rotated client secret.
        return ProcessRequestAsync<KcCredentials>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client rotated secret",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.InvalidateRotatedSecretAsync"/>
    public Task<KcResponse<KcCredentials>> InvalidateRotatedSecretAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for invalidating the client's rotated secret.
        var url = $"{BaseUrl}/{realm}/clients/{id}/client-secret/rotated";

        // Process the request to invalidate the rotated client secret.
        return ProcessRequestAsync<KcCredentials>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to invalidate realm client rotated secret",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetDefaultScopesAsync"/>
    public Task<KcResponse<IEnumerable<KcClientScope>>> GetDefaultScopesAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the default client scopes for the specified client.
        var url = $"{BaseUrl}/{realm}/clients/{id}/default-client-scopes";

        // Process the request to retrieve the default client scopes.
        return ProcessRequestAsync<IEnumerable<KcClientScope>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list realm client default scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.AddDefaultScopesAsync"/>
    public Task<KcResponse<object>> AddDefaultScopesAsync(
        string realm,
        string accessToken,
        string id,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the scope ID is not null or empty.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL for adding the scope to the client's default scopes.
        var url = $"{BaseUrl}/{realm}/clients/{id}/default-client-scopes/{scopeId}";

        // Process the request to add the default scope.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to add scope to realm client default scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.DeleteDefaultScopesAsync"/>
    public Task<KcResponse<object>> DeleteDefaultScopesAsync(
        string realm,
        string accessToken,
        string id,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the scope ID is not null or empty.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL for deleting the scope from the client's default scopes.
        var url = $"{BaseUrl}/{realm}/clients/{id}/default-client-scopes/{scopeId}";

        // Process the request to remove the default scope.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete scope from realm client default scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GenerateExampleAccessTokenAsync"/>
    public Task<KcResponse<KcAccessToken>> GenerateExampleAccessTokenAsync(
        string realm,
        string accessToken,
        string id,
        string userId,
        string scope = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Build the URL for generating the example access token.
        var appendedToUrl = false;
        var urlBuilder =
            new StringBuilder($"{BaseUrl}/{realm}/clients/{id}/evaluate-scopes/generate-example-access-token");

        // Append the optional scope to the URL if provided.
        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        // Append the user ID to the URL if provided.
        _ = urlBuilder.Append(appendedToUrl ? $"&userId={userId}" : $"?userId={userId}");

        // Process the request to generate the example access token.
        return ProcessRequestAsync<KcAccessToken>(
            urlBuilder.ToString(),
            HttpMethod.Get,
            accessToken,
            "Unable to generate realm client example access token",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GenerateExampleIdTokenAsync"/>
    public Task<KcResponse<KcAccessToken>> GenerateExampleIdTokenAsync(
        string realm,
        string accessToken,
        string id,
        string userId,
        string scope = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Build the URL for generating the example ID token.
        var appendedToUrl = false;
        var urlBuilder = new StringBuilder($"{BaseUrl}/{realm}/clients/{id}/evaluate-scopes/generate-example-id-token");

        // Append the optional scope to the URL if provided.
        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        // Append the user ID to the URL if provided.
        _ = urlBuilder.Append(appendedToUrl ? $"&userId={userId}" : $"?userId={userId}");

        // Process the request to generate the example ID token.
        return ProcessRequestAsync<KcAccessToken>(
            urlBuilder.ToString(),
            HttpMethod.Get,
            accessToken,
            "Unable to generate realm client example ID token",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GenerateExampleUserInfoAsync"/>
    public Task<KcResponse<IDictionary<string, string>>> GenerateExampleUserInfoAsync(
        string realm,
        string accessToken,
        string id,
        string userId,
        string scope = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the user ID is not null or empty.
        ValidateRequiredString(nameof(userId), userId);

        // Build the URL for generating the example user info.
        var appendedToUrl = false;
        var urlBuilder = new StringBuilder($"{BaseUrl}/{realm}/clients/{id}/evaluate-scopes/generate-example-userinfo");

        // Append the optional scope to the URL if provided.
        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        // Append the user ID to the URL if provided.
        _ = urlBuilder.Append(appendedToUrl ? $"&userId={userId}" : $"?userId={userId}");

        // Process the request to generate the example user info.
        return ProcessRequestAsync<IDictionary<string, string>>(
            urlBuilder.ToString(),
            HttpMethod.Get,
            accessToken,
            "Unable to generate realm client example user info",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetProtocolMappersAsync"/>
    public Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetProtocolMappersAsync(
        string realm,
        string accessToken,
        string id,
        string scope = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the protocol mappers, optionally including a scope filter.
        var url = string.IsNullOrWhiteSpace(scope)
            ? $"{BaseUrl}/{realm}/clients/{id}/evaluate-scopes/protocol-mappers"
            : $"{BaseUrl}/{realm}/clients/{id}/evaluate-scopes/protocol-mappers?scope={scope}";

        // Process the request to retrieve the protocol mappers.
        return ProcessRequestAsync<IEnumerable<KcProtocolMapper>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to retrieve protocol mappers for the client",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetScopedProtocolMappersInContainerAsync"/>
    public Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetScopedProtocolMappersInContainerAsync(
        string realm,
        string accessToken,
        string id,
        string roleContainerId,
        string scope = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Build the URL for retrieving the scoped protocol mappers.
        var appendedToUrl = false;
        var urlBuilder = new StringBuilder(
            $"{BaseUrl}/{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/granted");

        // Append the optional scope to the URL if provided.
        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        // Append the role container ID to the URL if provided.
        if ( !string.IsNullOrWhiteSpace(roleContainerId) )
        {
            _ = urlBuilder.Append(appendedToUrl
                ? $"&roleContainerId={roleContainerId}"
                : $"?roleContainerId={roleContainerId}");
        }

        // Process the request to retrieve the scoped protocol mappers.
        return ProcessRequestAsync<IEnumerable<KcProtocolMapper>>(
            urlBuilder.ToString(),
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client scoped protocol mappers in role container",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetUnScopedProtocolMappersInContainerAsync"/>
    public Task<KcResponse<IEnumerable<KcProtocolMapper>>> GetUnScopedProtocolMappersInContainerAsync(
        string realm,
        string accessToken,
        string id,
        string roleContainerId,
        string scope = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Build the URL for retrieving the unscoped protocol mappers.
        var appendedToUrl = false;
        var urlBuilder = new StringBuilder(
            $"{BaseUrl}/{realm}/clients/{id}/evaluate-scopes/scope-mappings/{roleContainerId}/not-granted");

        // Append the optional scope to the URL if provided.
        if ( !string.IsNullOrWhiteSpace(scope) )
        {
            _ = urlBuilder.Append(CultureInfo.CurrentCulture, $"?scope={scope}");
            appendedToUrl = true;
        }

        // Append the role container ID to the URL if provided.
        if ( !string.IsNullOrWhiteSpace(roleContainerId) )
        {
            _ = urlBuilder.Append(appendedToUrl
                ? $"&roleContainerId={roleContainerId}"
                : $"?roleContainerId={roleContainerId}");
        }

        // Process the request to retrieve the unscoped protocol mappers.
        return ProcessRequestAsync<IEnumerable<KcProtocolMapper>>(
            urlBuilder.ToString(),
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client unscoped protocol mappers in role container",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetAuthorizationManagementPermissionAsync"/>
    public Task<KcResponse<KcPermissionManagement>> GetAuthorizationManagementPermissionAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the client's authorization management permissions.
        var url = $"{BaseUrl}/{realm}/clients/{id}/management/permissions";

        // Process the request to retrieve the authorization management permissions.
        return ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to retrieve client authorization management permissions",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.SetAuthorizationManagementPermissionAsync"/>
    public Task<KcResponse<KcPermissionManagement>> SetAuthorizationManagementPermissionAsync(
        string realm,
        string accessToken,
        string id,
        KcPermissionManagement permissionManagement,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the permission management object is not null.
        ValidateNotNull(nameof(permissionManagement), permissionManagement);

        // Construct the URL for setting the client's authorization management permissions.
        var url = $"{BaseUrl}/{realm}/clients/{id}/management/permissions";

        // Process the request to set the authorization management permissions.
        return ProcessRequestAsync<KcPermissionManagement>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to set client authorization management permissions",
            permissionManagement,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.RegisterNodeAsync"/>
    public Task<KcResponse<object>> RegisterNodeAsync(
        string realm,
        string accessToken,
        string id,
        IDictionary<string, object> formParams,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the form parameters are not null.
        ValidateNotNull(nameof(formParams), formParams);

        // Construct the URL for registering the new cluster node.
        var url = $"{BaseUrl}/{realm}/clients/{id}/nodes";

        // Process the request to register the new cluster node.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to register realm client new cluster node",
            formParams,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.DeleteNodeAsync"/>
    public Task<KcResponse<object>> DeleteNodeAsync(
        string realm,
        string accessToken,
        string id,
        string nodeName,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the node name is not null or empty.
        ValidateNotNull(nameof(nodeName), nodeName);

        // Construct the URL for deleting the cluster node.
        var url = $"{BaseUrl}/{realm}/clients/{id}/nodes/{nodeName}";

        // Process the request to delete the cluster node.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete realm client cluster node",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.CountOfflineSessionsAsync"/>
    public Task<KcResponse<KcCount>> CountOfflineSessionsAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the offline session count.
        var url = $"{BaseUrl}/{realm}/clients/{id}/offline-session-count";

        // Process the request to retrieve the offline session count.
        return ProcessRequestAsync<KcCount>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client offline sessions count",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetOfflineSessionsAsync"/>
    public Task<KcResponse<IEnumerable<KcSession>>> GetOfflineSessionsAsync(
        string realm,
        string accessToken,
        string id,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Ensure a filter is provided; if not, use an empty filter.
        filter ??= new KcFilter();

        // Construct the URL for retrieving the offline sessions with optional query parameters from the filter.
        var url = $"{BaseUrl}/{realm}/clients/{id}/offline-sessions{filter.BuildQuery()}";

        // Process the request to retrieve the offline sessions.
        return ProcessRequestAsync<IEnumerable<KcSession>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client offline sessions",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetOptionalScopesAsync"/>
    public Task<KcResponse<IEnumerable<KcClientScope>>> GetOptionalScopesAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the optional client scopes.
        var url = $"{BaseUrl}/{realm}/clients/{id}/optional-client-scopes";

        // Process the request to retrieve the optional client scopes.
        return ProcessRequestAsync<IEnumerable<KcClientScope>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client optional scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.AddOptionalScopeAsync"/>
    public Task<KcResponse<object>> AddOptionalScopeAsync(
        string realm,
        string accessToken,
        string id,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the scope ID is not null or empty.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL for adding the optional client scope.
        var url = $"{BaseUrl}/{realm}/clients/{id}/optional-client-scopes/{scopeId}";

        // Process the request to add the optional client scope.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to add scope to realm client optional scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.DeleteOptionalScopeAsync"/>
    public Task<KcResponse<object>> DeleteOptionalScopeAsync(
        string realm,
        string accessToken,
        string id,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Validate that the scope ID is not null or empty.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL for deleting the optional client scope.
        var url = $"{BaseUrl}/{realm}/clients/{id}/optional-client-scopes/{scopeId}";

        // Process the request to remove the optional client scope.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete scope from realm client optional scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.PushRevocationAsync"/>
    public Task<KcResponse<KcGlobalRequestResult>> PushRevocationAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for pushing the revocation.
        var url = $"{BaseUrl}/{realm}/clients/{id}/push-revocation";

        // Process the request to push the revocation.
        return ProcessRequestAsync<KcGlobalRequestResult>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to push realm client revocation",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetRegistrationAccessTokenAsync"/>
    public Task<KcResponse<KcClient>> GetRegistrationAccessTokenAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the registration access token.
        var url = $"{BaseUrl}/{realm}/clients/{id}/registration-access-token";

        // Process the request to generate the registration access token.
        return ProcessRequestAsync<KcClient>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create realm client registration access token",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetServiceAccountUserAsync"/>
    public Task<KcResponse<KcUser>> GetServiceAccountUserAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the service account user.
        var url = $"{BaseUrl}/{realm}/clients/{id}/service-account-user";

        // Process the request to retrieve the service account user.
        return ProcessRequestAsync<KcUser>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client service account user",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.CountSessionsAsync"/>
    public Task<KcResponse<KcCount>> CountSessionsAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for retrieving the session count.
        var url = $"{BaseUrl}/{realm}/clients/{id}/session-count";

        // Process the request to retrieve the session count.
        return ProcessRequestAsync<KcCount>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client sessions count",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.TestAvailableNodesAsync"/>
    public Task<KcResponse<KcGlobalRequestResult>> TestAvailableNodesAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for testing the availability of nodes.
        var url = $"{BaseUrl}/{realm}/clients/{id}/test-nodes-available";

        // Process the request to test the availability of nodes.
        return ProcessRequestAsync<KcGlobalRequestResult>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to test realm client available nodes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClients.GetUsersSessionsAsync"/>
    public Task<KcResponse<IEnumerable<KcSession>>> GetUsersSessionsAsync(
        string realm,
        string accessToken,
        string id,
        KcFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Initialize the filter if it is not provided.
        filter ??= new KcFilter();

        // Construct the URL for retrieving user sessions with optional query parameters from the filter.
        var url = $"{BaseUrl}/{realm}/clients/{id}/user-sessions{filter.BuildQuery()}";

        // Process the request to retrieve user sessions.
        return ProcessRequestAsync<IEnumerable<KcSession>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get realm client users sessions",
            cancellationToken: cancellationToken
        );
    }
}
