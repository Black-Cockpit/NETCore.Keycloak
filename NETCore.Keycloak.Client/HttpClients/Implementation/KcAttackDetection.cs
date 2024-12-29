using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.AttackDetection;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcAttackDetection"/>
/// <summary>
/// Attack detection client constructor
/// </summary>
/// <param name="baseUrl">Keycloak server base url.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
/// <param name="logger">Logger <see cref="ILogger"/></param>
internal sealed class KcAttackDetection(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcAttackDetection
{
    /// <inheritdoc cref="IKcAttackDetection.DeleteUsersLoginFailureAsync"/>
    public async Task<KcResponse<object>> DeleteUsersLoginFailureAsync(
        string realm,
        string accessToken,
        string userId = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Construct the URL based on whether a specific user ID is provided.
        var url = string.IsNullOrWhiteSpace(userId)
            ? $"{BaseUrl}/{realm}/attack-detection/brute-force/users" // Clear all users' login failures.
            : $"{BaseUrl}/{realm}/attack-detection/brute-force/users/{userId}"; // Clear a specific user's login failures.

        // Process the request to delete login failures and return the result.
        return await ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to clear users login failure",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IKcAttackDetection.GetUserStatusAsync"/>
    public async Task<KcResponse<KcUserBruteForceStatus>> GetUserStatusAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Construct the URL to retrieve the brute force status of a specific user.
        var url = $"{BaseUrl}/{realm}/attack-detection/brute-force/users/{userId}";

        // Process the request to retrieve the brute force status and return the result.
        return await ProcessRequestAsync<KcUserBruteForceStatus>(
            url,
            HttpMethod.Get,
            accessToken,
            $"Unable to get user {userId} login failure status",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }
}
