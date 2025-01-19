using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NETCore.Keycloak.Client.Authorization.PolicyProviders;
using NETCore.Keycloak.Client.Authorization.Store;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Tests for the <see cref="KcProtectedResourcePolicyProvider"/> class.
/// These tests ensure the correct behavior of the policy provider,
/// including handling of null or invalid policies, caching policies,
/// and creating new policies based on resource and scope.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcProtectedResourcePolicyProviderTests
{
    /// <summary>
    /// Mock service provider for resolving dependencies such as logging services.
    /// </summary>
    private Mock<IServiceProvider> _mockProvider;

    /// <summary>
    /// Mock implementation of <see cref="IOptions{TOptions}"/> for <see cref="AuthorizationOptions"/>.
    /// Used to simulate authorization options during unit tests.
    /// </summary>
    private Mock<IOptions<AuthorizationOptions>> _mockOptions;

    /// <summary>
    /// Mock implementation of the <see cref="KcProtectedResourceStore"/>.
    /// Used to simulate the behavior of the resource store during unit tests.
    /// </summary>
    private Mock<KcProtectedResourceStore> _mockResourceStore;

    /// <summary>
    /// Instance of <see cref="KcProtectedResourcePolicyProvider"/> being tested.
    /// This is initialized with mocked dependencies to ensure isolated unit testing.
    /// </summary>
    private KcProtectedResourcePolicyProvider _policyProvider;

    /// <summary>
    /// Initializes the test environment by setting up mocks and configuring dependencies.
    /// This method is executed before each test to ensure consistent and isolated test conditions.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        // Create mock instances for the service provider, options, and resource store.
        _mockProvider = new Mock<IServiceProvider>();
        _mockOptions = new Mock<IOptions<AuthorizationOptions>>();
        _mockResourceStore = new Mock<KcProtectedResourceStore>();

        // Initialize a mock service scope and service scope factory to simulate dependency resolution.
        var mockScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        // Configure the mock service provider to resolve the IServiceScopeFactory.
        _ = _mockProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockScopeFactory.Object);

        // Configure the mock IServiceScopeFactory to return a mock service scope.
        _ = mockScopeFactory.Setup(factory => factory.CreateScope())
            .Returns(mockScope.Object);

        // Configure the mock service scope to resolve services from the mock service provider.
        _ = mockScope.Setup(x => x.ServiceProvider).Returns(_mockProvider.Object);

        // Configure the mock service provider to resolve the mocked KcProtectedResourceStore.
        _ = _mockProvider
            .Setup(provider => provider.GetService(typeof(KcProtectedResourceStore)))
            .Returns(_mockResourceStore.Object);

        // Initialize the KcProtectedResourcePolicyProvider with the mocked dependencies.
        _policyProvider = new KcProtectedResourcePolicyProvider(_mockProvider.Object, _mockOptions.Object);
    }

    /// <summary>
    /// Verifies that <see cref="KcProtectedResourcePolicyProvider.GetPolicyAsync"/>
    /// throws an <see cref="ArgumentNullException"/> when the policy name is null or empty.
    /// </summary>
    [TestMethod]
    public async Task ShouldThrowsArgumentNullExceptionForNullOrEmptyPolicyName()
    {
        // Arrange
        string policyName = null;

        // Act & Assert
        _ = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            // ReSharper disable once AssignNullToNotNullAttribute
            await _policyProvider.GetPolicyAsync(policyName).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that <see cref="KcProtectedResourcePolicyProvider.GetPolicyAsync"/> correctly retrieves
    /// a registered policy when the policy name exists in the <see cref="AuthorizationOptions"/>.
    /// </summary>
    [TestMethod]
    public async Task ShouldReturnsRegisteredPolicy()
    {
        // Define a valid test policy name.
        const string policyName = "resource#scope";

        // Create a test policy that requires authenticated users.
        var registeredPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        // Create authorization options to configure test policies.
        var options = new AuthorizationOptions();

        // Add the test policy to authorization options with the specified policy name.
        options.AddPolicy(policyName, registeredPolicy);

        // Configure mock options to return the test authorization options.
        _ = _mockOptions.Setup(o => o.Value).Returns(options);

        // Reinitialize the policy provider with the mock options and service provider.
        _policyProvider = new KcProtectedResourcePolicyProvider(_mockProvider.Object, _mockOptions.Object);

        // Retrieve the policy using the policy provider's GetPolicyAsync method.
        var policy = await _policyProvider.GetPolicyAsync(policyName).ConfigureAwait(false);

        // Verify that the retrieved policy is not null.
        Assert.IsNotNull(policy, "The policy should not be null.");

        // Verify that the retrieved policy matches the registered policy.
        Assert.AreEqual(registeredPolicy, policy, "The returned policy should match the registered policy.");
    }

    /// <summary>
    /// Tests that <see cref="KcProtectedResourcePolicyProvider.GetPolicyAsync"/> returns null
    /// when the provided policy name does not follow the expected format "resource#scope".
    /// </summary>
    [TestMethod]
    public async Task ShouldReturnsNullForInvalidPolicyFormat()
    {
        // Define an invalid policy name that does not follow the expected "resource#scope" format.
        const string invalidPolicyName = "invalidPolicy";

        // Create a test policy that requires authenticated users.
        var registeredPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        // Create authorization options to configure test policies.
        var options = new AuthorizationOptions();

        // Add the test policy to the authorization options with the specified invalid policy name.
        options.AddPolicy(invalidPolicyName, registeredPolicy);

        // Configure mock options to return the test authorization options when accessed.
        _ = _mockOptions.Setup(o => o.Value).Returns(options);

        // Reinitialize the policy provider with the updated mock options and service provider.
        _policyProvider = new KcProtectedResourcePolicyProvider(_mockProvider.Object, _mockOptions.Object);

        // Attempt to retrieve the policy using the policy provider's GetPolicyAsync method with the invalid policy name.
        var policy = await _policyProvider.GetPolicyAsync(invalidPolicyName).ConfigureAwait(false);

        // Assert that the retrieved policy is null, as the policy name is invalid.
        Assert.IsNull(policy, "The policy should be null for an invalid policy format.");
    }

    /// <summary>
    /// Tests that <see cref="KcProtectedResourcePolicyProvider.GetPolicyAsync"/> correctly creates
    /// and caches a new policy when provided with a valid policy name in the format "resource#scope".
    /// </summary>
    [TestMethod]
    public async Task ShouldCreatesAndCachesPolicyForResourceAndScope()
    {
        // Define a valid test policy name in the "resource#scope" format.
        const string policyName = "resource#scope";

        // Create a test policy that requires authenticated users.
        var registeredPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        // Create a new instance of AuthorizationOptions to configure policies for testing.
        var options = new AuthorizationOptions();

        // Add the test policy to the AuthorizationOptions with the specified policy name.
        options.AddPolicy(policyName, registeredPolicy);

        // Configure the mock IOptions to return the configured AuthorizationOptions.
        _ = _mockOptions.Setup(o => o.Value).Returns(options);

        // Reinitialize the policy provider with the mock options and service provider.
        _policyProvider = new KcProtectedResourcePolicyProvider(_mockProvider.Object, _mockOptions.Object);

        // Retrieve the policy using the policy provider's GetPolicyAsync method.
        var policy = await _policyProvider.GetPolicyAsync(policyName).ConfigureAwait(false);

        // Verify that the retrieved policy is not null, ensuring it was successfully created.
        Assert.IsNotNull(policy, "The policy should not be null.");

        // Verify that the retrieved policy contains at least one requirement.
        Assert.IsTrue(policy.Requirements.Count > 0, "The policy should contain at least one requirement.");
    }

    /// <summary>
    /// Tests that <see cref="KcProtectedResourcePolicyProvider.GetPolicyAsync"/> correctly returns
    /// the cached policy when the same policy name is requested multiple times.
    /// </summary>
    [TestMethod]
    public async Task ShouldReturnsCachedPolicyForPreviouslyCreatedPolicy()
    {
        // Define a valid test policy name in the "resource#scope" format to simulate caching behavior.
        const string policyName = "cachedResource#cachedScope";

        // Create a test policy that requires authenticated users.
        var registeredPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        // Create a new instance of AuthorizationOptions to configure policies for testing.
        var options = new AuthorizationOptions();

        // Add the test policy to the AuthorizationOptions with the specified policy name.
        options.AddPolicy(policyName, registeredPolicy);

        // Configure the mock IOptions to return the configured AuthorizationOptions.
        _ = _mockOptions.Setup(o => o.Value).Returns(options);

        // Reinitialize the policy provider with the mock options and service provider.
        _policyProvider = new KcProtectedResourcePolicyProvider(_mockProvider.Object, _mockOptions.Object);

        // Retrieve the policy for the first time using the policy provider's GetPolicyAsync method.
        var firstPolicy = await _policyProvider.GetPolicyAsync(policyName).ConfigureAwait(false);

        // Retrieve the policy for the second time using the same policy name.
        var secondPolicy = await _policyProvider.GetPolicyAsync(policyName).ConfigureAwait(false);

        // Assert that the first and second retrieved policies are the same, confirming caching.
        Assert.AreEqual(firstPolicy, secondPolicy, "The policy should be cached and identical for repeated requests.");
    }
}
