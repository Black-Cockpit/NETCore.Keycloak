using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Organizations;

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
}
