using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.ClientScope;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcClientScopes"/>
internal sealed class KcClientScopes(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcClientScopes
{
    /// <inheritdoc cref="IKcClientScopes.CreateAsync"/>
    public Task<KcResponse<object>> CreateAsync(
        string realm,
        string accessToken,
        KcClientScope scope,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope object is not null.
        ValidateNotNull(nameof(scope), scope);

        // Construct the URL for creating the client scope in the specified realm.
        var url = $"{BaseUrl}/{realm}/client-scopes";

        // Process the request to create the client scope.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create client scope",
            scope,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientScopes.ListAsync"/>
    public Task<KcResponse<IEnumerable<KcClientScope>>> ListAsync(
        string realm,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Construct the URL for retrieving the client scopes from the specified realm.
        var url = $"{BaseUrl}/{realm}/client-scopes";

        // Process the request to retrieve the list of client scopes.
        return ProcessRequestAsync<IEnumerable<KcClientScope>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list client scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientScopes.GetAsync"/>
    public Task<KcResponse<KcClientScope>> GetAsync(
        string realm,
        string accessToken,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL for retrieving the specific client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}";

        // Process the request to retrieve the client scope.
        return ProcessRequestAsync<KcClientScope>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get client scopes",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientScopes.UpdateAsync"/>
    public Task<KcResponse<KcClientScope>> UpdateAsync(
        string realm,
        string accessToken,
        string scopeId,
        KcClientScope scope,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Validate that the client scope object is not null.
        ValidateNotNull(nameof(scope), scope);

        // Construct the URL for updating the client scope in the specified realm.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}";

        // Process the request to update the client scope.
        return ProcessRequestAsync<KcClientScope>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update client scope",
            scope,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientScopes.DeleteAsync"/>
    public Task<KcResponse<object>> DeleteAsync(
        string realm,
        string accessToken,
        string scopeId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the client scope ID is not null or empty.
        ValidateRequiredString(nameof(scopeId), scopeId);

        // Construct the URL for deleting the specified client scope.
        var url = $"{BaseUrl}/{realm}/client-scopes/{scopeId}";

        // Process the request to remove the client scope.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete client scopes",
            cancellationToken: cancellationToken
        );
    }
}
