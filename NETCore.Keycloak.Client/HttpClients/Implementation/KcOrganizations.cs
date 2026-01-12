using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Organizations;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <summary>
///     Organization service API for managing organizations in Keycloak.
/// </summary>
internal sealed class KcOrganizations(string baseUrl, ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcOrganizations
{
    // Primary constructor on the class declaration is used; no explicit ctor body required.

    public Task<KcResponse<object>> CreateAsync(
        string realm,
        string accessToken,
        KcOrganization organization,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateNotNull(nameof(organization), organization);

        var url = $"{BaseUrl}/{realm}/organizations";
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to create organization",
            organization,
            "application/json",
            cancellationToken);
    }

    public Task<KcResponse<object>> UpdateAsync(
        string realm,
        string accessToken,
        string organizationId,
        KcOrganization organization,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        ValidateNotNull(nameof(organization), organization);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}";
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Put,
            accessToken,
            "Unable to update organization",
            organization,
            "application/json",
            cancellationToken);
    }

    public Task<KcResponse<object>> DeleteAsync(
        string realm,
        string accessToken,
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}";
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to delete organization",
            null,
            "application/json",
            cancellationToken);
    }

    public Task<KcResponse<KcOrganization>> GetAsync(
        string realm,
        string accessToken,
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}";
        return ProcessRequestAsync<KcOrganization>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get organization",
            null,
            "application/json",
            cancellationToken);
    }

    public Task<KcResponse<IEnumerable<KcOrganization>>> ListAsync(
        string realm,
        string accessToken,
        KcOrganizationFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        filter ??= new KcOrganizationFilter();

        var url = $"{BaseUrl}/{realm}/organizations{filter.BuildQuery()}";
        return ProcessRequestAsync<IEnumerable<KcOrganization>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to list organizations",
            null,
            "application/json",
            cancellationToken);
    }

    public Task<KcResponse<long>> CountAsync(
        string realm,
        string accessToken,
        KcOrganizationFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        filter ??= new KcOrganizationFilter();

        var url = $"{BaseUrl}/{realm}/organizations/count{filter.BuildQuery()}";
        return ProcessRequestAsync<long>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to count organizations",
            null,
            "application/json",
            cancellationToken);
    }
}
