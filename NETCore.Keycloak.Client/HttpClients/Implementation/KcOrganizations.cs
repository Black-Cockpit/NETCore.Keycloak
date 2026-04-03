using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Organizations;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcOrganizations"/>
internal sealed class KcOrganizations(string baseUrl,
    ILogger logger,
    IHttpClientFactory httpClientFactory = null) : KcHttpClientBase(logger, baseUrl, httpClientFactory), IKcOrganizations
{
    // Primary constructor on the class declaration is used; no explicit ctor body required.

    /// <inheritdoc cref="IKcOrganizations.CreateAsync"/>
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

    /// <inheritdoc cref="IKcOrganizations.UpdateAsync"/>
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

    /// <inheritdoc cref="IKcOrganizations.DeleteAsync"/>
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

    /// <inheritdoc cref="IKcOrganizations.GetAsync"/>
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

    /// <inheritdoc cref="IKcOrganizations.ListAsync"/>
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

    /// <inheritdoc cref="IKcOrganizations.CountAsync"/>
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

    /// <inheritdoc cref="IKcOrganizations.AddMemberAsync"/>
    public Task<KcResponse<object>> AddMemberAsync(
        string realm,
        string accessToken,
        string organizationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        ValidateRequiredString(nameof(userId), userId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members";
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Post,
            accessToken,
            "Unable to add member to organization",
            userId,
            "application/json",
            cancellationToken);
    }

    /// <inheritdoc cref="IKcOrganizations.GetMembersAsync"/>
    public Task<KcResponse<IEnumerable<KcUser>>> GetMembersAsync(
        string realm,
        string accessToken,
        string organizationId,
        KcOrganizationMemberFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        filter ??= new KcOrganizationMemberFilter();

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members{filter.BuildQuery()}";
        return ProcessRequestAsync<IEnumerable<KcUser>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get organization members",
            null,
            "application/json",
            cancellationToken);
    }

    /// <inheritdoc cref="IKcOrganizations.GetMembersCountAsync"/>
    public Task<KcResponse<long>> GetMembersCountAsync(
        string realm,
        string accessToken,
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members/count";
        return ProcessRequestAsync<long>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to count organization members",
            null,
            "application/json",
            cancellationToken);
    }

    /// <inheritdoc cref="IKcOrganizations.GetMemberAsync"/>
    public Task<KcResponse<KcUser>> GetMemberAsync(
        string realm,
        string accessToken,
        string organizationId,
        string memberId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        ValidateRequiredString(nameof(memberId), memberId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members/{memberId}";
        return ProcessRequestAsync<KcUser>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get organization member",
            null,
            "application/json",
            cancellationToken);
    }

    /// <inheritdoc cref="IKcOrganizations.RemoveMemberAsync"/>
    public Task<KcResponse<object>> RemoveMemberAsync(
        string realm,
        string accessToken,
        string organizationId,
        string memberId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        ValidateRequiredString(nameof(memberId), memberId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members/{memberId}";
        return ProcessRequestAsync<object>(
            url,
            HttpMethod.Delete,
            accessToken,
            "Unable to remove member from organization",
            null,
            "application/json",
            cancellationToken);
    }

    /// <inheritdoc cref="IKcOrganizations.GetMemberOrganizationsAsync"/>
    public Task<KcResponse<IEnumerable<KcOrganization>>> GetMemberOrganizationsAsync(
        string realm,
        string accessToken,
        string organizationId,
        string memberId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        ValidateRequiredString(nameof(memberId), memberId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members/{memberId}/organizations";
        return ProcessRequestAsync<IEnumerable<KcOrganization>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get member organizations",
            null,
            "application/json",
            cancellationToken);
    }

    /// <inheritdoc cref="IKcOrganizations.GetUserOrganizationsAsync"/>
    public Task<KcResponse<IEnumerable<KcOrganization>>> GetUserOrganizationsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(userId), userId);

        var url = $"{BaseUrl}/{realm}/organizations/members/{userId}/organizations";
        return ProcessRequestAsync<IEnumerable<KcOrganization>>(
            url,
            HttpMethod.Get,
            accessToken,
            "Unable to get user organizations",
            null,
            "application/json",
            cancellationToken);
    }

    /// <inheritdoc cref="IKcOrganizations.InviteExistingUserAsync"/>
    public async Task<KcResponse<object>> InviteExistingUserAsync(
        string realm,
        string accessToken,
        string organizationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        ValidateRequiredString(nameof(userId), userId);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members/invite-existing-user";

        try
        {
            using var response = await ExecuteRequest(async () =>
            {
                var client = CreateHttpClient();

                using var form = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { "id", userId }
                    });

                _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                return await client.PostAsync(new Uri(url), form, cancellationToken)
                    .ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = url,
                HttpMethod = HttpMethod.Post
            }).ConfigureAwait(false);

            return await HandleAsync<object>(response, cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to invite existing user to organization", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(url),
                    RequestException = e
                }
            };
        }
    }

    /// <inheritdoc cref="IKcOrganizations.InviteUserAsync"/>
    public async Task<KcResponse<object>> InviteUserAsync(
        string realm,
        string accessToken,
        string organizationId,
        string email,
        string firstName = null,
        string lastName = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);
        ValidateRequiredString(nameof(organizationId), organizationId);
        ValidateRequiredString(nameof(email), email);

        var url = $"{BaseUrl}/{realm}/organizations/{organizationId}/members/invite-user";

        try
        {
            using var response = await ExecuteRequest(async () =>
            {
                var client = CreateHttpClient();

                var formData = new Dictionary<string, string>
                {
                    { "email", email }
                };

                if ( !string.IsNullOrWhiteSpace(firstName) )
                {
                    formData.Add("firstName", firstName);
                }

                if ( !string.IsNullOrWhiteSpace(lastName) )
                {
                    formData.Add("lastName", lastName);
                }

                using var form = new FormUrlEncodedContent(formData);

                _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                return await client.PostAsync(new Uri(url), form, cancellationToken)
                    .ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = url,
                HttpMethod = HttpMethod.Post
            }).ConfigureAwait(false);

            return await HandleAsync<object>(response, cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to invite user to organization", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(url),
                    RequestException = e
                }
            };
        }
    }
}
