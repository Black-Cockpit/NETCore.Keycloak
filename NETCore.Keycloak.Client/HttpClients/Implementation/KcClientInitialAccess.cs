using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Clients;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcClientInitialAccess"/>
internal sealed class KcClientInitialAccess(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcClientInitialAccess
{
    /// <inheritdoc cref="IKcClientInitialAccess.CreateInitialAccessTokenAsync"/>
    public Task<KcResponse<KcClientInitialAccessModel>> CreateInitialAccessTokenAsync(
        string realm,
        string accessToken,
        KcCreateClientInitialAccess access,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the access configuration is provided.
        ValidateNotNull(nameof(access), access);

        // Construct the URL for creating the initial access token.
        var url = $"{BaseUrl}/{realm}/clients-initial-access";

        // Process the request to create the initial access token.
        return ProcessRequestAsync<KcClientInitialAccessModel>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create client initial access token",
            access,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientInitialAccess.GetInitialAccessAsync"/>
    public Task<KcResponse<IEnumerable<KcClientInitialAccessModel>>> GetInitialAccessAsync(
        string realm,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Construct the URL for retrieving the initial access tokens.
        var url = $"{BaseUrl}/{realm}/clients-initial-access";

        // Process the request to retrieve the initial access tokens.
        return ProcessRequestAsync<IEnumerable<KcClientInitialAccessModel>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list client initial access tokens",
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc cref="IKcClientInitialAccess.DeleteInitialAccessTokenAsync"/>
    public Task<KcResponse<object>> DeleteInitialAccessTokenAsync(
        string realm,
        string accessToken,
        string id,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the initial access token ID is not null or empty.
        ValidateRequiredString(nameof(id), id);

        // Construct the URL for deleting the initial access token in the specified realm.
        var url = $"{BaseUrl}/{realm}/clients-initial-access/{id}";

        // Process the request to delete the initial access token.
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete client initial access token",
            cancellationToken: cancellationToken
        );
    }
}
