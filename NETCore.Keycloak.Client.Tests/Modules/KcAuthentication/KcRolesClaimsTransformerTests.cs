using System.Security.Claims;
using NETCore.Keycloak.Client.Authentication.Claims;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthentication;

/// <summary>
/// Tests for the <see cref="KcRolesClaimsTransformer"/> class,
/// verifying its behavior when transforming Keycloak roles into claims.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcRolesClaimsTransformerTests
{
    /// <summary>
    /// Specifies the type of the role claim to be added to the transformed principal.
    /// </summary>
    private const string RoleClaimType = "roles";

    /// <summary>
    /// Specifies the audience to be used when extracting roles from the "resource_access" claim.
    /// </summary>
    private const string Audience = "test-audience";

    /// <summary>
    /// Verifies that the <see cref="KcRolesClaimsTransformer.TransformAsync"/> method
    /// throws an <see cref="ArgumentNullException"/> when the provided principal is null.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task ShouldThrowExceptionWhenPrincipalIsNull()
    {
        // Arrange
        // Create a transformer instance with predefined role claim type, role source, and audience.
        var transformer = new KcRolesClaimsTransformer(RoleClaimType, KcRolesClaimSource.ResourceAccess, Audience);

        // Act
        // Attempt to transform a null ClaimsPrincipal, which should throw an exception.
        // ReSharper disable once AssignNullToNotNullAttribute
        _ = await transformer.TransformAsync(null).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the <see cref="KcRolesClaimsTransformer.TransformAsync"/> method
    /// correctly extracts roles from the <c>resource_access</c> claim for the specified audience
    /// and adds them as claims to the transformed <see cref="ClaimsPrincipal"/>.
    /// </summary>
    [TestMethod]
    public async Task ShouldAddRolesFromResourceAccess()
    {
        // Arrange
        // Create an instance of the roles claims transformer configured for the ResourceAccess role source.
        var transformer = new KcRolesClaimsTransformer(RoleClaimType, KcRolesClaimSource.ResourceAccess, Audience);

        // Create a ClaimsPrincipal with a resource_access claim containing roles for the specified audience.
        var principal = CreatePrincipalWithClaim("resource_access", """
                                                                        {
                                                                            "test-audience": {
                                                                                "roles": ["role1", "role2"]
                                                                            }
                                                                        }
                                                                    """);

        // Act
        // Transform the principal by extracting roles from the resource_access claim.
        var transformedPrincipal = await transformer.TransformAsync(principal).ConfigureAwait(false);

        // Assert
        // Extract roles from the transformed ClaimsPrincipal and validate their correctness.
        var roles = transformedPrincipal.Claims
            .Where(c => c.Type == RoleClaimType)
            .Select(c => c.Value)
            .ToList();

        CollectionAssert.AreEquivalent(new[]
        {
            "role1", "role2"
        }, roles, "Roles should be extracted from resource_access.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcRolesClaimsTransformer.TransformAsync"/> method
    /// correctly extracts roles from the <c>realm_access</c> claim and adds them as claims
    /// to the transformed <see cref="ClaimsPrincipal"/>.
    /// </summary>
    [TestMethod]
    public async Task ShouldAddRolesFromRealmAccess()
    {
        // Arrange
        // Create an instance of the roles claims transformer configured for the Realm role source.
        var transformer = new KcRolesClaimsTransformer(RoleClaimType, KcRolesClaimSource.Realm, Audience);

        // Create a ClaimsPrincipal with a realm_access claim containing roles.
        var principal = CreatePrincipalWithClaim("realm_access", """
                                                                     {
                                                                         "roles": ["realm-role1", "realm-role2"]
                                                                     }
                                                                 """);

        // Act
        // Transform the principal by extracting roles from the realm_access claim.
        var transformedPrincipal = await transformer.TransformAsync(principal).ConfigureAwait(false);

        // Assert
        // Extract roles from the transformed ClaimsPrincipal and validate their correctness.
        var roles = transformedPrincipal.Claims
            .Where(c => c.Type == RoleClaimType)
            .Select(c => c.Value)
            .ToList();

        CollectionAssert.AreEquivalent(new[]
        {
            "realm-role1", "realm-role2"
        }, roles, "Roles should be extracted from realm_access.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcRolesClaimsTransformer.TransformAsync"/> method
    /// does not add any roles when the <c>resource_access</c> claim exists but is empty.
    /// </summary>
    [TestMethod]
    public async Task ShouldNotAddRolesWhenClaimsAreEmpty()
    {
        // Arrange
        // Create an instance of the roles claims transformer configured for the ResourceAccess role source.
        var transformer = new KcRolesClaimsTransformer(RoleClaimType, KcRolesClaimSource.ResourceAccess, Audience);

        // Create a ClaimsPrincipal with an empty resource_access claim.
        var principal = CreatePrincipalWithClaim("resource_access", "{}");

        // Act
        // Transform the principal by processing the resource_access claim.
        var transformedPrincipal = await transformer.TransformAsync(principal).ConfigureAwait(false);

        // Assert
        // Extract roles from the transformed ClaimsPrincipal and verify no roles were added.
        var roles = transformedPrincipal.Claims
            .Where(c => c.Type == RoleClaimType)
            .Select(c => c.Value)
            .ToList();

        Assert.AreEqual(0, roles.Count, "No roles should be added when resource_access claim is empty.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcRolesClaimsTransformer.TransformAsync"/> method
    /// does not transform or add any roles when the role source is set to <see cref="KcRolesClaimSource.None"/>.
    /// </summary>
    [TestMethod]
    public async Task ShouldNotTransformRolesWhenSourceIsNone()
    {
        // Arrange
        // Create an instance of the roles claims transformer with the role source set to None.
        var transformer = new KcRolesClaimsTransformer(RoleClaimType, KcRolesClaimSource.None, Audience);

        // Create a ClaimsPrincipal with a resource_access claim containing roles.
        var principal = CreatePrincipalWithClaim("resource_access", """
                                                                        {
                                                                            "test-audience": {
                                                                                "roles": ["role1", "role2"]
                                                                            }
                                                                        }
                                                                    """);

        // Act
        // Transform the principal with the transformer, expecting no roles to be added.
        var transformedPrincipal = await transformer.TransformAsync(principal).ConfigureAwait(false);

        // Assert
        // Extract roles from the transformed ClaimsPrincipal and verify no roles were added.
        var roles = transformedPrincipal.Claims
            .Where(c => c.Type == RoleClaimType)
            .Select(c => c.Value)
            .ToList();

        Assert.AreEqual(0, roles.Count, "No roles should be added when source is None.");
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with a specified claim type and value.
    /// </summary>
    /// <param name="claimType">The type of the claim to add.</param>
    /// <param name="claimValue">The value of the claim to add.</param>
    /// <returns>
    /// A <see cref="ClaimsPrincipal"/> containing the specified claim.
    /// </returns>
    private static ClaimsPrincipal CreatePrincipalWithClaim(string claimType, string claimValue)
    {
        // Create a claims identity and add the specified claim.
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(claimType, claimValue));

        // Return a ClaimsPrincipal containing the claims identity.
        return new ClaimsPrincipal(identity);
    }
}
