using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Authentication.Claims;

/// <summary>
/// Transforms Keycloak roles from the <c>resource_access</c> or <c>realm_access</c> claims
/// into JWT role claims that are compatible with the ASP.NET Core authorization system.
/// Currently, only roles from the specified source (<see cref="KcRolesClaimSource"/>) are mapped.
/// </summary>
/// <example>
/// Example of a Keycloak <c>resource_access</c> claim:
/// <code>
/// "resource_access": {
///     "api": {
///         "roles": [ "role1", "role2" ]
///     },
///     "account": {
///         "roles": [
///             "view-profile"
///         ]
///     }
/// }
/// </code>
/// </example>
/// <remarks>
/// This class implements <see cref="IClaimsTransformation"/> and is invoked during
/// the authentication pipeline to transform claims for the authenticated user.
/// </remarks>
public class KcRolesClaimsTransformer : IClaimsTransformation
{
    private readonly string _roleClaimType;
    private readonly KcRolesClaimSource _roleSource;
    private readonly string _audience;

    /// <summary>
    /// Initializes a new instance of the <see cref="KcRolesClaimsTransformer"/> class.
    /// </summary>
    /// <param name="roleClaimType">The name of the claim type for roles (e.g., "roles").</param>
    /// <param name="roleSource">
    /// The source of roles within the Keycloak claims (<see cref="KcRolesClaimSource"/>).
    /// </param>
    /// <param name="audience">
    /// The audience to filter roles for when <see cref="KcRolesClaimSource.ResourceAccess"/> is used.
    /// </param>
    public KcRolesClaimsTransformer(
        string roleClaimType,
        KcRolesClaimSource roleSource,
        string audience)
    {
        _roleClaimType = roleClaimType;
        _roleSource = roleSource;
        _audience = audience;
    }

    /// <summary>
    /// Transforms the provided <see cref="ClaimsPrincipal"/> by mapping roles
    /// from the configured source into JWT role claims.
    /// </summary>
    /// <param name="principal">The authenticated user's <see cref="ClaimsPrincipal"/>.</param>
    /// <returns>
    /// A new <see cref="ClaimsPrincipal"/> with the transformed role claims added.
    /// </returns>
    /// <remarks>
    /// The transformation is idempotent. If roles are already added, this method will not duplicate them.
    /// This transformation is triggered on every <c>AuthenticateAsync</c> call.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="principal"/> is null.</exception>
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Validate that the principal is not null.
        if ( principal == null )
        {
            throw new ArgumentNullException(nameof(principal), $"{nameof(principal)} is required");
        }

        // Create a clone of the principal to apply transformations.
        var result = principal.Clone();

        // Ensure the identity is a ClaimsIdentity before proceeding with transformations.
        if ( result.Identity is not ClaimsIdentity identity )
        {
            return Task.FromResult(result);
        }

        // Determine the source of roles and process accordingly.
        switch ( _roleSource )
        {
            case KcRolesClaimSource.ResourceAccess:
                {
                    // Attempt to extract the "resource_access" claim value for the specified audience.
                    if ( principal.FindFirst("resource_access")?.Value is var resourceAccessValue &&
                         string.IsNullOrWhiteSpace(resourceAccessValue) )
                    {
                        // If the "resource_access" claim is missing or empty, return the original principal.
                        return Task.FromResult(result);
                    }

                    // Parse the "resource_access" claim as JSON to extract roles for the audience.
                    using var resourceAccess = JsonDocument.Parse(resourceAccessValue);

                    // Check if the claim contains roles for the specified audience.
                    if ( !resourceAccess.RootElement.TryGetProperty(_audience, out var rolesElement) )
                    {
                        return Task.FromResult(result);
                    }

                    // Enumerate the roles array and add each role as a claim.
                    foreach ( var role in rolesElement.GetProperty("roles").EnumerateArray() )
                    {
                        if ( role.GetString() is { } value && !string.IsNullOrWhiteSpace(value) )
                        {
                            identity.AddClaim(new Claim(_roleClaimType, value));
                        }
                    }

                    // Return the transformed principal.
                    return Task.FromResult(result);
                }

            case KcRolesClaimSource.Realm:
                {
                    // Attempt to extract the "realm_access" claim value to retrieve realm roles.
                    if ( principal.FindFirst("realm_access")?.Value is var realmAccessValue &&
                         string.IsNullOrWhiteSpace(realmAccessValue) )
                    {
                        // If the "realm_access" claim is missing or empty, return the original principal.
                        return Task.FromResult(result);
                    }

                    // Parse the "realm_access" claim as JSON to extract roles.
                    using var realmAccess = JsonDocument.Parse(realmAccessValue);

                    // Check if the claim contains a "roles" property.
                    if ( !realmAccess.RootElement.TryGetProperty("roles", out var rolesElement) )
                    {
                        return Task.FromResult(result);
                    }

                    // Enumerate the roles array and add each role as a claim.
                    foreach ( var role in rolesElement.EnumerateArray() )
                    {
                        if ( role.GetString() is { } value && !string.IsNullOrWhiteSpace(value) )
                        {
                            identity.AddClaim(new Claim(_roleClaimType, value));
                        }
                    }

                    // Return the transformed principal.
                    return Task.FromResult(result);
                }

            case KcRolesClaimSource.None:
            default:
                // If no role source is specified, or an unsupported source is configured,
                // no transformation is applied, and the original principal is returned.
                return Task.FromResult(result);
        }
    }
}
