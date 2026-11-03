using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NETCore.Keycloak.Client.Authorization.Handlers;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Tests.Infrastructure;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Exercises expiry and refresh through the public handler with deterministic time and HTTP responses.
/// </summary>
[TestClass]
[TestCategory("CoverageUnit")]
public sealed class KcTokenRenewalUnitTests
{
    /// <summary>
    /// Refreshes an expired access token, forwards the refresh token and caches the rotated pair.
    /// </summary>
    [TestMethod]
    public async Task ShouldRefreshExpiredAccessTokenAndCacheRotatedTokens()
    {
        var clock = new TestClock();
        var requests = new List<string>();
        using var transport = new StubHttpTransport(async request =>
        {
            var body = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            requests.Add(body);
            return StubHttpTransport.Json(requests.Count == 1 ? Token("access-1", "refresh-1") :
                requests.Count == 2 ? Token("access-2", "refresh-2") : Token("access-3", "refresh-3"));
        });
        using var provider = CreateProvider(clock, transport);
        var handler = CreateHandler(provider);
        Assert.AreEqual("access-1", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        Assert.AreEqual("access-1", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        Assert.HasCount(1, requests);
        clock.Advance(TimeSpan.FromSeconds(181));
        Assert.AreEqual("access-2", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        StringAssert.Contains(requests[1], "grant_type=refresh_token", StringComparison.Ordinal);
        StringAssert.Contains(requests[1], "refresh_token=refresh-1", StringComparison.Ordinal);
        Assert.AreEqual("access-2", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        Assert.HasCount(2, requests);
        clock.Advance(TimeSpan.FromSeconds(181));
        Assert.AreEqual("access-3", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        StringAssert.Contains(requests[2], "refresh_token=refresh-2", StringComparison.Ordinal);
    }

    /// <summary>
    /// Falls back to credentials after refresh rejection and rejects failed fallback authentication.
    /// </summary>
    /// <param name="fallbackSucceeds">Whether password authentication succeeds after refresh rejection.</param>
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ShouldFallbackAfterRejectedRefresh(bool fallbackSucceeds)
    {
        var clock = new TestClock();
        var requests = new List<string>();
        using var transport = new StubHttpTransport(async request =>
        {
            requests.Add(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
            return requests.Count switch
            {
                1 => StubHttpTransport.Json(Token("access-1", "refresh-1")),
                2 => StubHttpTransport.Json("{\"error\":\"invalid_grant\"}", HttpStatusCode.BadRequest),
                _ => fallbackSucceeds ? StubHttpTransport.Json(Token("fallback", "new-refresh")) :
                    StubHttpTransport.Json("{\"error\":\"invalid_grant\"}", HttpStatusCode.Unauthorized)
            };
        });
        using var provider = CreateProvider(clock, transport);
        var handler = CreateHandler(provider);
        _ = await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false);
        clock.Advance(TimeSpan.FromSeconds(181));
        if ( fallbackSucceeds )
        {
            Assert.AreEqual("fallback", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        }
        else
        {
            _ = await Assert.ThrowsExactlyAsync<KcException>(() => handler.TryGetAdminTokenAsync("test"))
                .ConfigureAwait(false);
        }
        Assert.HasCount(3, requests);
        StringAssert.Contains(requests[1], "refresh_token=refresh-1", StringComparison.Ordinal);
        StringAssert.Contains(requests[2], "grant_type=password", StringComparison.Ordinal);
    }

    /// <summary>
    /// Uses the refresh lifetime independently of the access lifetime and never reuses an expired refresh token.
    /// </summary>
    /// <param name="refreshLifetime">The refresh token lifetime returned by Keycloak.</param>
    /// <param name="expectedGrant">The grant expected after the access token expires.</param>
    [TestMethod]
    [DataRow(1800, "refresh_token")]
    [DataRow(120, "password")]
    [DataRow(0, "password")]
    public async Task ShouldRespectRefreshTokenLifetime(int refreshLifetime, string expectedGrant)
    {
        var clock = new TestClock();
        var requests = new List<string>();
        using var transport = new StubHttpTransport(async request =>
        {
            requests.Add(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
            return StubHttpTransport.Json(Token("access", "refresh", refreshLifetime: refreshLifetime));
        });
        using var provider = CreateProvider(clock, transport);
        var handler = CreateHandler(provider);
        _ = await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false);
        clock.Advance(TimeSpan.FromSeconds(181));
        _ = await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false);
        Assert.HasCount(2, requests);
        StringAssert.Contains(requests[1], $"grant_type={expectedGrant}", StringComparison.Ordinal);
    }

    /// <summary>
    /// Caches a short-lived token briefly while refreshing before its real expiry.
    /// </summary>
    [TestMethod]
    public async Task ShouldBoundSafetyMarginForShortLivedAccessTokens()
    {
        var clock = new TestClock();
        var requests = 0;
        using var transport = new StubHttpTransport(_ =>
        {
            requests++;
            return Task.FromResult(StubHttpTransport.Json(Token($"access-{requests}", "refresh", accessLifetime: 60)));
        });
        using var provider = CreateProvider(clock, transport);
        var handler = CreateHandler(provider);
        Assert.AreEqual("access-1", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        clock.Advance(TimeSpan.FromSeconds(29));
        Assert.AreEqual("access-1", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        clock.Advance(TimeSpan.FromSeconds(2));
        Assert.AreEqual("access-2", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
    }

    private static string Token(string access, string refresh, int accessLifetime = 300, int refreshLifetime = 1800) =>
        $$"""
          {"access_token":"{{access}}","refresh_token":"{{refresh}}",
           "expires_in":{{accessLifetime}},"refresh_expires_in":{{refreshLifetime}}}
          """;

    /// <summary>
    /// Coalesces simultaneous expiry requests so a rotating refresh token is submitted only once.
    /// </summary>
    [TestMethod]
    public async Task ShouldShareOneRefreshAcrossConcurrentExpiryRequests()
    {
        var clock = new TestClock();
        var releaseRefresh = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var refreshes = 0;
        using var transport = new StubHttpTransport(async request =>
        {
            var body = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            if ( body.Contains("grant_type=refresh_token", StringComparison.Ordinal) )
            {
                _ = Interlocked.Increment(ref refreshes);
                await releaseRefresh.Task.ConfigureAwait(false);
                return StubHttpTransport.Json(Token("shared-access", "rotated-refresh"));
            }
            return StubHttpTransport.Json(Token("initial-access", "initial-refresh"));
        });
        using var provider = CreateProvider(clock, transport);
        var handler = CreateHandler(provider);
        _ = await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false);
        clock.Advance(TimeSpan.FromSeconds(181));
        var requests = Enumerable.Range(0, 16).Select(_ => handler.TryGetAdminTokenAsync("test")).ToArray();
        var submittedBeforeResponse = Volatile.Read(ref refreshes);
        releaseRefresh.SetResult();
        var results = await Task.WhenAll(requests).ConfigureAwait(false);
        Assert.AreEqual(1, submittedBeforeResponse, "One refresh token must not be submitted concurrently.");
        Assert.AreEqual(1, refreshes);
        foreach ( var result in results )
        {
            Assert.AreEqual("shared-access", result);
        }
    }

    /// <summary>
    /// A canceled waiter cannot release another request's renewal lock or invalidate the shared result.
    /// </summary>
    [TestMethod]
    public async Task ShouldCancelRenewalWaitWithoutReleasingAnotherRequestLock()
    {
        var clock = new TestClock();
        var releaseRefresh = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var refreshes = 0;
        using var transport = new StubHttpTransport(async request =>
        {
            var body = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            if ( body.Contains("grant_type=refresh_token", StringComparison.Ordinal) )
            {
                _ = Interlocked.Increment(ref refreshes);
                await releaseRefresh.Task.ConfigureAwait(false);
                return StubHttpTransport.Json(Token("shared-access", "rotated-refresh"));
            }
            return StubHttpTransport.Json(Token("initial-access", "initial-refresh"));
        });
        using var provider = CreateProvider(clock, transport);
        var handler = CreateHandler(provider);
        _ = await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false);
        clock.Advance(TimeSpan.FromSeconds(181));
        var renewal = handler.TryGetAdminTokenAsync("test");
        try
        {
            _ = await Assert.ThrowsExactlyAsync<TaskCanceledException>(() =>
                handler.TryGetAdminTokenAsync("test", new CancellationToken(canceled: true))).ConfigureAwait(false);
            Assert.AreEqual(1, refreshes);
        }
        finally
        {
            releaseRefresh.SetResult();
        }
        Assert.AreEqual("shared-access", await renewal.ConfigureAwait(false));
        Assert.AreEqual("shared-access", await handler.TryGetAdminTokenAsync("test").ConfigureAwait(false));
        Assert.AreEqual(1, refreshes);
    }

    private static ServiceProvider CreateProvider(TimeProvider clock, IHttpClientFactory factory) =>
        new ServiceCollection().AddLogging().AddSingleton(clock).AddSingleton(factory).BuildServiceProvider();

    private static KcRealmAdminTokenHandler CreateHandler(IServiceProvider provider)
    {
        var store = new Mock<KcRealmAdminConfigurationStore>();
        _ = store.Setup(value => value.GetRealmsAdminConfiguration()).Returns([
            new KcRealmAdminConfiguration
            {
                Realm = "test", ClientId = "client", KeycloakBaseUrl = "https://keycloak.test",
                RealmAdminCredentials = new KcUserLogin { Username = "admin", Password = "test-password" }
            }
        ]);
        return new KcRealmAdminTokenHandler(store.Object, provider);
    }

    private sealed class TestClock : TimeProvider
    {
        private DateTimeOffset _now = DateTimeOffset.UtcNow;
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan elapsed) => _now += elapsed;
    }
}
