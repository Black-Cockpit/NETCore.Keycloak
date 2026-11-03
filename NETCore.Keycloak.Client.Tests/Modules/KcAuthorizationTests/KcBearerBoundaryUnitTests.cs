using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NETCore.Keycloak.Client.Authorization.Handlers;
using NETCore.Keycloak.Client.Authorization.Requirements;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.Infrastructure;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Rejects malformed bearer inputs and fails closed when session validation cannot complete.
/// </summary>
[TestClass]
[TestCategory("CoverageUnit")]
public sealed class KcBearerBoundaryUnitTests
{
    /// <summary>
    /// Invalid input must never grant access or trigger administrative HTTP calls.
    /// </summary>
    /// <param name="scenario">The malformed header or issuer scenario.</param>
    /// <param name="failed">Whether the handler explicitly fails the authorization context.</param>
    [TestMethod]
    [DataRow("no-header", false)]
    [DataRow("basic", false)]
    [DataRow("no-token", false)]
    [DataRow("empty-token", true)]
    [DataRow("malformed-token", true)]
    [DataRow("no-issuer", true)]
    [DataRow("malformed-issuer", true)]
    [DataRow("unregistered-realm", true)]
    public async Task ShouldNeverAuthorizeMalformedInputs(string scenario, bool failed)
    {
        var jwt = new JwtSecurityTokenHandler();
        var header = scenario switch
        {
            "no-header" => string.Empty,
            "basic" => "Basic credentials",
            "no-token" => "Bearer",
            "empty-token" => "Bearer ",
            "malformed-token" => "Bearer invalid-token",
            "no-issuer" => "Bearer " + jwt.WriteToken(new JwtSecurityToken()),
            "malformed-issuer" => "Bearer " + jwt.WriteToken(new JwtSecurityToken(issuer: "not-a-url")),
            _ => "Bearer " + jwt.WriteToken(new JwtSecurityToken(issuer: "https://keycloak.test/realms/test"))
        };
        var http = new DefaultHttpContext();
        http.Request.Headers.Authorization = header;
        var admin = new Mock<IKcRealmAdminTokenHandler>(MockBehavior.Strict);
        using var provider = new ServiceCollection().AddLogging()
            .AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = http })
            .AddSingleton(admin.Object).BuildServiceProvider();
        var store = new Mock<KcProtectedResourceStore>();
        _ = store.Setup(value => value.GetRealmProtectedResources()).Returns([]);
        var requirement = new KcAuthorizationRequirement(store.Object, "orders", "read");
        var context = new AuthorizationHandlerContext([requirement],
            new ClaimsPrincipal(new ClaimsIdentity("test")), null);
        await new KcTestableBearerAuthorizationHandler(provider).TestHandleRequirementAsync(context, requirement)
            .ConfigureAwait(false);
        Assert.IsFalse(context.HasSucceeded);
        Assert.AreEqual(failed, context.HasFailed);
        admin.VerifyNoOtherCalls();
    }

    /// <summary>
    /// A failed session query denies access, preserves the transport failure and never requests an RPT.
    /// </summary>
    [TestMethod]
    public async Task ShouldPreserveSessionQueryFailureAndDenyAccess()
    {
        var failure = new HttpRequestException("session service unavailable");
        var requests = new List<string>();
        using var transport = new StubHttpTransport(request =>
        {
            var path = request.RequestUri!.AbsolutePath;
            requests.Add(path);
            if ( path.EndsWith("/sessions", StringComparison.Ordinal) )
            {
                throw failure;
            }
            return Task.FromResult(StubHttpTransport.Json("{\"id\":\"alice\",\"username\":\"alice\"}"));
        });
        var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: "https://keycloak.test/realms/test",
            claims: [new Claim("sub", "alice"), new Claim("sid", "session")]));
        var http = new DefaultHttpContext();
        http.Request.Headers.Authorization = "Bearer " + jwt;
        var admin = new Mock<IKcRealmAdminTokenHandler>();
        _ = admin.Setup(value => value.TryGetAdminTokenAsync("test", It.IsAny<CancellationToken>()))
            .ReturnsAsync("admin-token");
        using var provider = new ServiceCollection().AddLogging().AddSingleton<IHttpClientFactory>(transport)
            .AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = http })
            .AddSingleton(admin.Object).BuildServiceProvider();
        var store = new Mock<KcProtectedResourceStore>();
        _ = store.Setup(value => value.GetRealmProtectedResources()).Returns([
            new KcRealmProtectedResources { Realm = "test", ProtectedResourceName = "api" }
        ]);
        var requirement = new KcAuthorizationRequirement(store.Object, "orders", "read");
        var principal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, "alice"), new Claim("sid", "session")
        ], "test"));
        var context = new AuthorizationHandlerContext([requirement], principal, null);
        var exception = await Assert.ThrowsExactlyAsync<KcSessionClosedException>(() =>
            new KcTestableBearerAuthorizationHandler(provider).TestHandleRequirementAsync(context, requirement))
            .ConfigureAwait(false);
        Assert.AreSame(failure, exception.InnerException);
        Assert.IsFalse(context.HasSucceeded);
        Assert.HasCount(2, requests);
    }
}
