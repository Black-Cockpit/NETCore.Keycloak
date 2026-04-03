using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Organizations;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak organizations REST client.
/// <see href="https://www.keycloak.org/docs-api/latest/rest-api/index.html#_organizations"/>
/// </summary>
public interface IKcOrganizations
{
    /// <summary>
    /// Creates a new organization in a specified Keycloak realm.
    ///
    /// POST /{realm}/organizations
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization will be created.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organization">The organization representation to create.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<object>> CreateAsync(
        string realm,
        string accessToken,
        KcOrganization organization,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing organization in a specified Keycloak realm.
    ///
    /// PUT /{realm}/organizations/{organizationId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization to update.</param>
    /// <param name="organization">The updated organization representation.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<object>> UpdateAsync(
        string realm,
        string accessToken,
        string organizationId,
        KcOrganization organization,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an organization from a specified Keycloak realm.
    ///
    /// DELETE /{realm}/organizations/{organizationId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization to delete.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<object>> DeleteAsync(
        string realm,
        string accessToken,
        string organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific organization by its ID from a specified Keycloak realm.
    ///
    /// GET /{realm}/organizations/{organizationId}
    /// </summary>
    /// <param name="realm">The Keycloak realm to query.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization to retrieve.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcOrganization"/> details.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<KcOrganization>> GetAsync(
        string realm,
        string accessToken,
        string organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of organizations from a specified Keycloak realm, optionally filtered by criteria.
    ///
    /// GET /{realm}/organizations
    /// </summary>
    /// <param name="realm">The Keycloak realm from which organizations will be listed.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="filter">Optional filter criteria.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcOrganization"/> objects.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<IEnumerable<KcOrganization>>> ListAsync(
        string realm,
        string accessToken,
        KcOrganizationFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of organizations in a specified Keycloak realm, optionally filtered.
    ///
    /// GET /{realm}/organizations/count
    /// </summary>
    /// <param name="realm">The Keycloak realm to query.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="filter">Optional filter criteria.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> with the count of organizations.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<long>> CountAsync(
        string realm,
        string accessToken,
        KcOrganizationFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a user as a member of an organization in a specified Keycloak realm.
    ///
    /// POST /{realm}/organizations/{organizationId}/members
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="userId">The ID of the user to add as a member.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<object>> AddMemberAsync(
        string realm,
        string accessToken,
        string organizationId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of members of an organization in a specified Keycloak realm.
    ///
    /// GET /{realm}/organizations/{organizationId}/members
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="filter">Optional filter criteria for pagination and search.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcUser"/> objects.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<IEnumerable<KcUser>>> GetMembersAsync(
        string realm,
        string accessToken,
        string organizationId,
        KcOrganizationMemberFilter filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of members in an organization in a specified Keycloak realm.
    ///
    /// GET /{realm}/organizations/{organizationId}/members/count
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> with the count of members.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<long>> GetMembersCountAsync(
        string realm,
        string accessToken,
        string organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific member of an organization by their ID in a specified Keycloak realm.
    ///
    /// GET /{realm}/organizations/{organizationId}/members/{memberId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="memberId">The ID of the member to retrieve.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing the <see cref="KcUser"/> details.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<KcUser>> GetMemberAsync(
        string realm,
        string accessToken,
        string organizationId,
        string memberId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a member from an organization in a specified Keycloak realm.
    ///
    /// DELETE /{realm}/organizations/{organizationId}/members/{memberId}
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="memberId">The ID of the member to remove.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<object>> RemoveMemberAsync(
        string realm,
        string accessToken,
        string organizationId,
        string memberId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the organizations associated with a specific member within an organization context.
    ///
    /// GET /{realm}/organizations/{organizationId}/members/{memberId}/organizations
    /// </summary>
    /// <param name="realm">The Keycloak realm to query.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization context.</param>
    /// <param name="memberId">The ID of the member whose organizations are being retrieved.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcOrganization"/> objects.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<IEnumerable<KcOrganization>>> GetMemberOrganizationsAsync(
        string realm,
        string accessToken,
        string organizationId,
        string memberId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the organizations associated with a user by their ID (top-level endpoint).
    ///
    /// GET /{realm}/organizations/members/{userId}/organizations
    /// </summary>
    /// <param name="realm">The Keycloak realm to query.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="userId">The ID of the user whose organizations are being retrieved.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> containing an enumerable of <see cref="KcOrganization"/> objects.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<IEnumerable<KcOrganization>>> GetUserOrganizationsAsync(
        string realm,
        string accessToken,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invites an existing user to an organization using the specified user ID.
    ///
    /// POST /{realm}/organizations/{organizationId}/members/invite-existing-user
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="userId">The ID of the existing user to invite.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<object>> InviteExistingUserAsync(
        string realm,
        string accessToken,
        string organizationId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invites an existing user or sends a registration link to a new user based on the provided e-mail address.
    /// If the user with the given e-mail address exists, it sends an invitation link;
    /// otherwise, it sends a registration link.
    ///
    /// POST /{realm}/organizations/{organizationId}/members/invite-user
    /// </summary>
    /// <param name="realm">The Keycloak realm where the organization exists.</param>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="email">The e-mail address of the user to invite.</param>
    /// <param name="firstName">Optional first name of the user.</param>
    /// <param name="lastName">Optional last name of the user.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A <see cref="KcResponse{T}"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="KcException">Thrown if any required parameter is null, empty, or invalid.</exception>
    Task<KcResponse<object>> InviteUserAsync(
        string realm,
        string accessToken,
        string organizationId,
        string email,
        string firstName = null,
        string lastName = null,
        CancellationToken cancellationToken = default);
}
