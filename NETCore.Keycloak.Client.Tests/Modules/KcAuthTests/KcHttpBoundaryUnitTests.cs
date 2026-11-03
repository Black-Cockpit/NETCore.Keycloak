using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Tests.Infrastructure;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Exercises malformed responses, optional OAuth parameters and no-op batch semantics without network I/O.
/// </summary>
[TestClass]
[TestCategory("CoverageUnit")]
public sealed class KcHttpBoundaryUnitTests
{
    /// <summary>
    /// Unreadable or malformed successful response bodies become structured failures for revocation/RPT operations.
    /// </summary>
    /// <param name="operation">The operation returning malformed JSON.</param>
    /// <param name="logging">Whether an optional logger is supplied.</param>
    [TestMethod]
    [DataRow("access", false)]
    [DataRow("access", true)]
    [DataRow("refresh", false)]
    [DataRow("refresh", true)]
    [DataRow("rpt", false)]
    [DataRow("rpt", true)]
    public async Task ShouldCaptureMalformedRevocationAndRptResponses(string operation, bool logging)
    {
        using var transport = new StubHttpTransport(_ =>
        {
            var response = StubHttpTransport.Json(operation == "rpt" ? "not-json" : "body");
            if ( operation != "rpt" )
            {
                response.Content.Headers.ContentType!.CharSet = "unknown-response-charset";
            }
            return Task.FromResult(response);
        });
        var client = new KeycloakClient("https://keycloak.test", logging ? NullLogger.Instance : null, transport);
        var credentials = new KcClientCredentials { ClientId = "client" };
        switch ( operation )
        {
            case "access":
                AssertMalformed(await client.Auth.RevokeAccessTokenAsync("test", credentials, "access")
                    .ConfigureAwait(false));
                break;
            case "refresh":
                AssertMalformed(await client.Auth.RevokeRefreshTokenAsync("test", credentials, "refresh")
                    .ConfigureAwait(false));
                break;
            default:
                AssertMalformed(await client.Auth.GetRequestPartyTokenAsync("test", "access", "api")
                    .ConfigureAwait(false));
                break;
        }
    }

    /// <summary>
    /// Sends optional scopes/resources intact and exercises the optional scope variants on evaluation endpoints.
    /// </summary>
    [TestMethod]
    public async Task ShouldForwardOptionalScopeAndResourceParameters()
    {
        var requests = new List<Uri>();
        string form = null;
        using var transport = new StubHttpTransport(async request =>
        {
            requests.Add(request.RequestUri!);
            if ( request.Method == HttpMethod.Post )
            {
                form = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return StubHttpTransport.Json(request.RequestUri!.AbsolutePath
                .EndsWith("/not-granted", StringComparison.Ordinal)
                ? "[]" : "{\"sub\":\"alice\",\"access_token\":\"access\"}");
        });
        var client = new KeycloakClient("https://keycloak.test", null, transport);
        var token = await client.Auth.GetResourceOwnerPasswordTokenAsync("test",
            new KcClientCredentials { ClientId = "client" }, new KcUserLogin { Username = "alice", Password = "test" },
            scope: "openid profile", resource: "https://api.test/orders").ConfigureAwait(false);
        Assert.IsFalse(token.IsError, token.ErrorMessage);
        StringAssert.Contains(form, "scope=openid+profile", StringComparison.Ordinal);
        StringAssert.Contains(form, "resource=https%3A%2F%2Fapi.test%2Forders", StringComparison.Ordinal);
        Assert.IsFalse((await client.Clients.GenerateExampleAccessTokenAsync("test", "admin", "client", "alice",
            "openid profile").ConfigureAwait(false)).IsError);
        Assert.IsFalse((await client.Clients.GenerateExampleIdTokenAsync("test", "admin", "client", "alice",
            "openid profile").ConfigureAwait(false)).IsError);
        Assert.IsFalse((await client.Clients.GenerateExampleUserInfoAsync("test", "admin", "client", "alice",
            "openid profile").ConfigureAwait(false)).IsError);
        Assert.IsFalse((await client.Clients.GetUnScopedProtocolMappersInContainerAsync("test", "admin", "client",
            "container", "openid profile").ConfigureAwait(false)).IsError);
        Assert.HasCount(5, requests);
        foreach ( var request in requests.Skip(1) )
        {
            StringAssert.Contains(Uri.UnescapeDataString(request.Query), "scope=openid profile",
                StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Forwards non-not-found failures from role existence checks instead of reporting a nonexistent role.
    /// </summary>
    [TestMethod]
    public async Task ShouldPropagateRoleLookupFailures()
    {
        using var transport = new StubHttpTransport(_ => Task.FromResult(
            StubHttpTransport.Json("{\"error\":\"service_unavailable\"}", HttpStatusCode.ServiceUnavailable)));
        var client = new KeycloakClient("https://keycloak.test", null, transport);
        var realm = await client.Roles.IsRolesExistsAsync("test", "admin", "reader").ConfigureAwait(false);
        var scoped = await client.Roles.IsClientRoleExistsAsync("test", "admin", "client", "reader")
            .ConfigureAwait(false);
        Assert.IsTrue(realm.IsError);
        Assert.IsTrue(scoped.IsError);
        StringAssert.Contains(realm.ErrorMessage, "service_unavailable", StringComparison.Ordinal);
        Assert.AreEqual(realm.ErrorMessage, scoped.ErrorMessage);
    }

    /// <summary>
    /// A truncated HTTP response retains its underlying exception in revocation results.
    /// </summary>
    /// <param name="refresh">Whether the operation revokes a refresh token.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task ShouldPreserveRevocationTransportFailure(bool refresh)
    {
        using var transport = new StubHttpTransport(_ => Task.FromResult(
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new TruncatedContent() }));
        var client = new KeycloakClient("https://keycloak.test", null, transport);
        var credentials = new KcClientCredentials { ClientId = "client" };
        var response = refresh
            ? await client.Auth.RevokeRefreshTokenAsync("test", credentials, "refresh").ConfigureAwait(false)
            : await client.Auth.RevokeAccessTokenAsync("test", credentials, "access").ConfigureAwait(false);
        Assert.IsTrue(response.IsError);
        Assert.IsNotNull(response.Exception);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.AreSame(response.Exception, response.MonitoringMetrics.RequestException);
    }

    /// <summary>
    /// Empty role batches must not trigger HTTP writes or destructive server changes.
    /// </summary>
    [TestMethod]
    public async Task ShouldAvoidRequestsForEmptyRoleBatches()
    {
        var calls = 0;
        using var transport = new StubHttpTransport(_ =>
        {
            calls++;
            return Task.FromResult(StubHttpTransport.Json("{}"));
        });
        var client = new KeycloakClient("https://keycloak.test", null, transport);
        var operations = new[]
        {
            client.Roles.AddCompositeAsync("test", "admin", "role", []),
            client.Roles.AddCompositeByIdAsync("test", "admin", "role", []),
            client.Roles.DeleteCompositeAsync("test", "admin", "role", []),
            client.Roles.DeleteCompositeByIdAsync("test", "admin", "role", []),
            client.Roles.AddClientRoleToCompositeAsync("test", "admin", "client", "role", []),
            client.ScopeMappings.AddClientRolesToScopeAsync("test", "admin", "scope", "client", []),
            client.ScopeMappings.RemoveClientRolesFromScopeAsync("test", "admin", "scope", "client", []),
            client.ScopeMappings.AddRolesToScopeAsync("test", "admin", "scope", []),
            client.ScopeMappings.RemoveRolesFromScopeAsync("test", "admin", "scope", []),
            client.ScopeMappings.AddClientRolesToClientScopeAsync("test", "admin", "client", "container", []),
            client.ScopeMappings.RemoveClientRolesFromClientScopeAsync("test", "admin", "client", "container", []),
            client.ScopeMappings.AddRolesToScopeClientAsync("test", "admin", "client", []),
            client.ScopeMappings.RemoveRolesFromClientScopeAsync("test", "admin", "client", []),
            client.ClientRoleMappings.MapClientRolesToGroupAsync("test", "admin", "group", "client", []),
            client.ClientRoleMappings.DeleteGroupClientRoleMappingsAsync("test", "admin", "group", "client", []),
            client.ClientRoleMappings.MapClientRolesToUserAsync("test", "admin", "alice", "client", []),
            client.ClientRoleMappings.DeleteUserClientRoleMappingsAsync("test", "admin", "alice", "client", [])
        };
        foreach ( var result in await Task.WhenAll(operations).ConfigureAwait(false) )
        {
            Assert.IsFalse(result.IsError);
            Assert.IsNull(result.Response);
        }
        Assert.AreEqual(0, calls);
        _ = Assert.ThrowsExactly<KcException>(() =>
            _ = client.ClientRoleMappings.DeleteGroupClientRoleMappingsAsync("test", "admin", "group", "client", null));
        _ = Assert.ThrowsExactly<KcException>(() => _ = new KeycloakClient(" "));
    }

    private static void AssertMalformed<T>(KcResponse<T> response)
    {
        Assert.IsTrue(response.IsError);
        Assert.IsNotNull(response.Exception);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
        Assert.AreEqual(HttpMethod.Post, response.MonitoringMetrics.HttpMethod);
        Assert.AreSame(response.Exception, response.MonitoringMetrics.RequestException);
    }

    private sealed class TruncatedContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
            Task.FromException(new IOException("The response body ended unexpectedly."));

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
