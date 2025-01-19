using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NETCore.Keycloak.Client.Authorization;
using NETCore.Keycloak.Client.Authorization.Handlers;
using NETCore.Keycloak.Client.Authorization.PolicyProviders;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Tests for the <see cref="KcAuthorizationExtension"/> class, which provides
/// extension methods for configuring Keycloak-based authorization services.
/// </summary>
[TestClass]
[TestCategory("Final")]
public class KcAuthorizationExtensionTests : KcTestingModule
{
    /// <summary>
    /// Represents the service collection used to register and configure
    /// dependencies for the tests in this class.
    /// </summary>
    private IServiceCollection _services;

    /// <summary>
    /// Initializes the test environment by creating a new instance of
    /// <see cref="IServiceCollection"/>. This ensures a fresh and isolated
    /// service collection is available for each test.
    /// </summary>
    [TestInitialize]
    public void Setup() => _services = new ServiceCollection();

    /// <summary>
    /// Tests the <see cref="KcAuthorizationExtension.AddKeycloakAuthorization"/> method to ensure
    /// proper registration of Keycloak-related authorization services.
    /// </summary>
    [TestMethod]
    public void ShouldRegisterAuthorizationHandler()
    {
        // Create a mock for the Keycloak configuration store.
        var mockConfigStore = new Mock<KcRealmAdminConfigurationStore>();

        // Define a test Keycloak configuration for the realm.
        var testConfig = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = TestEnvironment.BaseUrl, // Set the base URL of the Keycloak server.
            Realm = TestEnvironment.TestingRealm.Name, // Specify the realm name.
            ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId, // Use the client ID for the realm.
            RealmAdminCredentials = new KcUserLogin
            {
                Username = TestEnvironment.TestingRealm.User.Username, // Admin username.
                Password = TestEnvironment.TestingRealm.User.Password // Admin password.
            }
        };

        // Configure the mock to return the test configuration.
        _ = mockConfigStore.Setup(c => c.GetRealmsAdminConfiguration())
            .Returns(new List<KcRealmAdminConfiguration>
            {
                testConfig
            });

        // Create a mock for ILogger to simulate logging functionality.
        var mockLogger = new Mock<ILogger>();
        _ = mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Add Keycloak services, the token handler, and the logger to the service collection.
        _ = _services.AddKeycloakAuthorization()
            .AddTransient<IKcRealmAdminTokenHandler, KcRealmAdminTokenHandler>(provider =>
                new KcRealmAdminTokenHandler(mockConfigStore.Object, provider)) // Register the token handler.
            .AddTransient(_ => mockLogger.Object); // Register the mock logger.

        // Build the service provider to resolve the registered services.
        var serviceProvider = _services.BuildServiceProvider();

        // Retrieve the registered authorization handler.
        var authorizationHandler = serviceProvider.GetService<IAuthorizationHandler>();

        // Retrieve the HTTP context accessor.
        var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

        // Verify that the authorization handler is registered and of the expected type.
        Assert.IsNotNull(authorizationHandler, "IAuthorizationHandler should be registered.");
        Assert.IsInstanceOfType(authorizationHandler, typeof(KcBearerAuthorizationHandler));

        // Verify that the HTTP context accessor is registered.
        Assert.IsNotNull(httpContextAccessor, "IHttpContextAccessor should be registered.");
    }

    /// <summary>
    /// Tests that <see cref="KcAuthorizationExtension.AddKeycloakProtectedResourcesPolicies{T, TV}"/>
    /// correctly registers all required services for managing Keycloak protected resources policies.
    /// </summary>
    [TestMethod]
    public void ShouldRegisterKeycloakProtectedResourcesPoliciesServices()
    {
        // Arrange
        // Create a mock for the KcProtectedResourceStore to simulate resource storage.
        var mockProtectedResourceStore = new Mock<KcProtectedResourceStore>();

        // Create a mock for the KcRealmAdminConfigurationStore to simulate configuration storage.
        var mockRealmAdminConfigurationStore = new Mock<KcRealmAdminConfigurationStore>();

        // Create a mock for IOptions<AuthorizationOptions> to simulate authorization configuration.
        var mockAuthorizationOptions = new Mock<IOptions<AuthorizationOptions>>();

        // Create a mock for ILogger to simulate logging functionality.
        var mockLogger = new Mock<ILogger>();
        _ = mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Add mocked services to the service collection.
        _ = _services.AddSingleton(mockProtectedResourceStore.Object);
        _ = _services.AddSingleton(mockRealmAdminConfigurationStore.Object);
        _ = _services.AddSingleton(mockAuthorizationOptions.Object);

        // Register the mock logger in the service collection.
        _ = _services.AddTransient(_ => mockLogger.Object);

        // Add Keycloak protected resources policies using the extension method.
        _ = _services.AddKeycloakProtectedResourcesPolicies<
            MockKcProtectedResourceStore, MockKcRealmAdminConfigurationStore>();

        // Build the service provider from the service collection.
        var serviceProvider = _services.BuildServiceProvider();

        // Act
        // Retrieve services from the service provider.
        var protectedResourceStore = serviceProvider.GetService<KcProtectedResourceStore>();
        var realmAdminConfigurationStore = serviceProvider.GetService<KcRealmAdminConfigurationStore>();
        var tokenHandler = serviceProvider.GetService<IKcRealmAdminTokenHandler>();
        var policyProvider = serviceProvider.GetService<IAuthorizationPolicyProvider>();

        // Assert
        // Ensure the KcProtectedResourceStore is registered and resolves correctly.
        Assert.IsNotNull(protectedResourceStore, "KcProtectedResourceStore should be registered.");

        // Ensure the KcRealmAdminConfigurationStore is registered and resolves correctly.
        Assert.IsNotNull(realmAdminConfigurationStore, "KcRealmAdminConfigurationStore should be registered.");

        // Ensure the IKcRealmAdminTokenHandler is registered and resolves correctly.
        Assert.IsNotNull(tokenHandler, "IKcRealmAdminTokenHandler should be registered.");

        // Ensure the IAuthorizationPolicyProvider is registered and resolves correctly.
        Assert.IsNotNull(policyProvider, "IAuthorizationPolicyProvider should be registered.");

        // Verify that the IAuthorizationPolicyProvider is of the expected type.
        Assert.IsInstanceOfType(policyProvider, typeof(KcProtectedResourcePolicyProvider));
    }
}
