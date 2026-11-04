using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NETCore.Keycloak.Client.Authentication;
using NETCore.Keycloak.Client.Authentication.Claims;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthentication;

/// <summary>
/// Resolves the actual named JWT options and checks token validation and missing-role boundaries.
/// </summary>
[TestClass]
[TestCategory("CoverageUnit")]
public sealed class KcAuthenticationBoundaryUnitTests
{
    /// <summary>
    /// Executes the registered options delegate and preserves the caller's configuration overrides.
    /// </summary>
    /// <param name="ssl">Whether metadata must use HTTPS.</param>
    /// <param name="customize">Whether a caller overrides SaveToken.</param>
    [TestMethod]
    [DataRow(true, false)]
    [DataRow(false, true)]
    public void ShouldApplyNamedJwtOptions(bool ssl, bool customize)
    {
        var services = new ServiceCollection().AddLogging();
        var invoked = false;
        _ = services.AddKeycloakAuthentication("keycloak-unit", config =>
        {
            config.Url = "https://keycloak.test";
            config.Issuer = "https://keycloak.test/";
            config.Realm = "test";
            config.Resource = "api";
            config.RequireSsl = ssl;
            config.TokenClockSkew = TimeSpan.Zero;
            config.RoleClaimType = "roles";
            config.NameClaimType = "username";
            config.ValidAudiences = ["api"];
        }, customize ? options => { invoked = true; options.SaveToken = true; }
        : null);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get("keycloak-unit");
        Assert.AreEqual("https://keycloak.test/realms/test", options.Authority);
        Assert.AreEqual("api", options.Audience);
        Assert.AreEqual(ssl, options.RequireHttpsMetadata);
        Assert.AreEqual(customize, options.SaveToken);
        Assert.AreEqual(customize, invoked);
        var validation = options.TokenValidationParameters;
        Assert.IsTrue(validation.ValidateIssuer);
        Assert.IsTrue(validation.ValidateAudience);
        Assert.IsTrue(validation.ValidateLifetime);
        Assert.IsTrue(validation.RequireExpirationTime);
        Assert.AreEqual(TimeSpan.Zero, validation.ClockSkew);
        Assert.AreEqual("username", validation.NameClaimType);
        Assert.AreEqual("roles", validation.RoleClaimType);
        Assert.AreEqual("https://keycloak.test/realms/test", validation.ValidIssuer);

        // Exercise the configured issuer/audience/lifetime checks using a local signing key, without discovery I/O.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-only-signing-key-at-least-32-bytes-long"));
        validation.IssuerSigningKey = key;
        var handler = new JwtSecurityTokenHandler();
        string Sign(string issuer, string audience, bool expired = false) => handler.WriteToken(new JwtSecurityToken(
            issuer, audience, [new Claim("sub", "alice")], DateTime.UtcNow.AddMinutes(-10),
            expired ? DateTime.UtcNow.AddMinutes(-1) : DateTime.UtcNow.AddMinutes(5),
            new SigningCredentials(key, SecurityAlgorithms.HmacSha256)));
        _ = handler.ValidateToken(Sign(validation.ValidIssuer, "api"), validation, out _);
        _ = Assert.ThrowsExactly<SecurityTokenInvalidIssuerException>(() =>
            handler.ValidateToken(Sign("https://other.test/realms/test", "api"), validation, out _));
        _ = Assert.ThrowsExactly<SecurityTokenInvalidAudienceException>(() =>
            handler.ValidateToken(Sign(validation.ValidIssuer, "other-api"), validation, out _));
        _ = Assert.ThrowsExactly<SecurityTokenExpiredException>(() =>
            handler.ValidateToken(Sign(validation.ValidIssuer, "api", expired: true), validation, out _));
    }

    /// <summary>
    /// Missing or unrelated claims must not create roles or change the original identity.
    /// </summary>
    /// <param name="source">The configured Keycloak role source.</param>
    /// <param name="claim">The input claim type, if present.</param>
    /// <param name="json">The input claim value.</param>
    [TestMethod]
    [DataRow(KcRolesClaimSource.ResourceAccess, null, null)]
    [DataRow(KcRolesClaimSource.ResourceAccess, "resource_access", "{\"other-api\":{\"roles\":[\"admin\"]}}")]
    [DataRow(KcRolesClaimSource.Realm, null, null)]
    [DataRow(KcRolesClaimSource.Realm, "realm_access", "{}")]
    public async Task ShouldNotGrantRolesFromMissingOrUnrelatedClaims(KcRolesClaimSource source,
        string claim, string json)
    {
        var identity = new ClaimsIdentity("test");
        if ( claim != null )
        {
            identity.AddClaim(new Claim(claim, json));
        }
        var principal = new ClaimsPrincipal(identity);
        var transformed = await new KcRolesClaimsTransformer("roles", source, "api").TransformAsync(principal)
            .ConfigureAwait(false);
        Assert.IsFalse(transformed.HasClaim(value => value.Type == "roles"));
        Assert.IsFalse(principal.HasClaim(value => value.Type == "roles"));
    }

    /// <summary>
    /// A principal without an identity remains without roles.
    /// </summary>
    [TestMethod]
    public async Task ShouldLeavePrincipalWithoutIdentityUnchanged()
    {
        var transformed = await new KcRolesClaimsTransformer("roles", KcRolesClaimSource.Realm, "api")
            .TransformAsync(new ClaimsPrincipal()).ConfigureAwait(false);
        Assert.IsNull(transformed.Identity);
    }
}
