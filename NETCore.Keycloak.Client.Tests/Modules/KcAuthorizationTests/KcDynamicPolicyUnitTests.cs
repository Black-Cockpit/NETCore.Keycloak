using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NETCore.Keycloak.Client.Authorization.PolicyProviders;
using NETCore.Keycloak.Client.Authorization.Requirements;
using NETCore.Keycloak.Client.Authorization.Store;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Creates genuinely unregistered dynamic policies and validates the resulting requirements and cache.
/// </summary>
[TestClass]
[TestCategory("CoverageUnit")]
public sealed class KcDynamicPolicyUnitTests
{
    /// <summary>
    /// Builds and caches requirements for a resource/scope that is absent from AuthorizationOptions.
    /// </summary>
    [TestMethod]
    public async Task ShouldBuildAndReuseUnregisteredPolicies()
    {
        var store = new Mock<KcProtectedResourceStore>();
        using var services = new ServiceCollection().AddSingleton(store.Object).BuildServiceProvider();
        var provider = new KcProtectedResourcePolicyProvider(services, Options.Create(new AuthorizationOptions()));
        var first = await provider.GetPolicyAsync("orders#read").ConfigureAwait(false);
        var second = await provider.GetPolicyAsync("orders#read").ConfigureAwait(false);
        Assert.IsNotNull(first);
        Assert.AreSame(first, second);
        var requirement = first.Requirements.OfType<KcAuthorizationRequirement>().Single();
        Assert.AreSame(store.Object, requirement.ProtectedResourceStore);
        Assert.AreEqual("orders#read", requirement.ToString());
        var write = await provider.GetPolicyAsync("orders#write").ConfigureAwait(false);
        Assert.AreNotSame(first, write);
        Assert.AreEqual("orders#write", write.Requirements.OfType<KcAuthorizationRequirement>().Single().ToString());
    }

    /// <summary>
    /// Concurrent first requests for one policy return the same cached policy without duplicate-key failures.
    /// </summary>
    [TestMethod]
    public async Task ShouldShareOnePolicyAcrossConcurrentRequests()
    {
        var store = new Mock<KcProtectedResourceStore>();
        using var services = new ServiceCollection().AddSingleton(store.Object).BuildServiceProvider();
        var provider = new KcProtectedResourcePolicyProvider(services, Options.Create(new AuthorizationOptions()));
        var results = await Task.WhenAll(Enumerable.Range(0, 32)
            .Select(_ => Task.Run(() => provider.GetPolicyAsync("orders#read")))).ConfigureAwait(false);
        foreach ( var result in results )
        {
            Assert.AreSame(results[0], result);
        }
    }
}
