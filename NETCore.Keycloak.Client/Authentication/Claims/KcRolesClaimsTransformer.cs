using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Authentication.Claims;

/// <summary>
/// Transforms keycloak roles in the resource_access claim to jwt role claims.
/// Note, realm roles are not mapped atm.
/// </summary>
/// <example>
/// Example of keycloak resource_access claim
/// "resource_access": {
///     "api": {
///         "roles": [ "role1", "role2" ]
///     },
///     "account": {
///         "roles": [
///             "view-profile"
///         ]
///     }
/// },
/// </example>
/// <seealso cref="IClaimsTransformation" />
public class KcRolesClaimsTransformer : IClaimsTransformation
{
    private readonly string _roleClaimType;
    private readonly KcRolesClaimSource _roleSource;
    private readonly string _audience;

    /// <summary>
    /// Initializes a new instance of the <see cref="KcRolesClaimsTransformer"/> class.
    /// </summary>
    /// <param name="roleClaimType">Type of the role claim.</param>
    /// <param name="roleSource">Role claim source <see cref="KcRolesClaimSource"/></param>
    /// <param name="audience">The audience.</param>
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
    /// Provides a central transformation point to change the specified principal.
    /// Note: this will be run on each AuthenticateAsync call, so its safer to
    /// return a new ClaimsPrincipal if your transformation is not idempotent.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal" /> to transform.</param>
    /// <returns>
    /// The transformed principal.
    /// </returns>
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if ( principal == null )
        {
            throw new ArgumentNullException(nameof(principal), $"{nameof(principal)} is required");
        }

        var result = principal.Clone();
        if ( result.Identity is not ClaimsIdentity identity )
        {
            return Task.FromResult(result);
        }

        switch ( _roleSource )
        {
            case KcRolesClaimSource.ResourceAccess:
                {
                    // Extract resource access
                    if ( principal.FindFirst("resource_access")?.Value is var resourceAccessValue &&
                         string.IsNullOrWhiteSpace(resourceAccessValue) )
                    {
                        return Task.FromResult(result);
                    }

                    using var resourceAccess = JsonDocument.Parse(resourceAccessValue);

                    var containsAudienceRoles = resourceAccess
                        .RootElement
                        .TryGetProperty(_audience, out var rolesElement);

                    if ( !containsAudienceRoles )
                    {
                        return Task.FromResult(result);
                    }

                    var clientRoles = rolesElement.GetProperty("roles");

                    foreach ( var role in clientRoles.EnumerateArray() )
                    {
                        if ( role.GetString() is var value && !string.IsNullOrWhiteSpace(value) )
                        {
                            identity.AddClaim(new Claim(_roleClaimType, value));
                        }
                    }

                    return Task.FromResult(result);
                }
            case KcRolesClaimSource.Realm:
                {
                    // Extract realm access
                    if ( principal.FindFirst("realm_access")?.Value is var realmAccessValue &&
                         string.IsNullOrWhiteSpace(realmAccessValue) )
                    {
                        return Task.FromResult(result);
                    }

                    using var realmAccess = JsonDocument.Parse(realmAccessValue);

                    var containsRoles = realmAccess
                        .RootElement
                        .TryGetProperty("roles", out var rolesElement);

                    if ( !containsRoles )
                    {
                        return Task.FromResult(result);
                    }

                    foreach ( var role in rolesElement.EnumerateArray() )
                    {
                        if ( role.GetString() is var value && !string.IsNullOrWhiteSpace(value) )
                        {
                            identity.AddClaim(new Claim(_roleClaimType, value));
                        }
                    }

                    return Task.FromResult(result);
                }
            case KcRolesClaimSource.None:
            default:
                return Task.FromResult(result);
        }
    }
}
