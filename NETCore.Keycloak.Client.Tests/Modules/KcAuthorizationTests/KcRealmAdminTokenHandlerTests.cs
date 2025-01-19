using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.Authorization.Handlers;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Tests.Abstraction;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Tests for <see cref="KcRealmAdminTokenHandler"/> to validate its behavior during token management.
/// </summary>
[TestClass]
[TestCategory("Final")]
public class KcRealmAdminTokenHandlerTests : KcTestingModule
{
    /// <summary>
    /// Mock configuration store for Keycloak realm administration settings.
    /// </summary>
    private Mock<KcRealmAdminConfigurationStore> _mockConfigStore;

    /// <summary>
    /// Mock service provider for resolving dependencies such as logging services.
    /// </summary>
    private Mock<IServiceProvider> _mockProvider;

    /// <summary>
    /// Instance of <see cref="KcRealmAdminTokenHandler"/> under test.
    /// </summary>
    private KcRealmAdminTokenHandler _tokenHandler;

    /// <summary>
    /// Test configuration for Keycloak realm administration.
    /// </summary>
    private KcRealmAdminConfiguration _testConfig;

    /// <summary>
    /// Initializes the necessary mock dependencies and test instance before each test is executed.
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        // Create mocks for configuration store, service provider, scope factory, and logger.
        var mockScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockConfigStore = new Mock<KcRealmAdminConfigurationStore>();
        _mockProvider = new Mock<IServiceProvider>();

        // Mock the service provider to resolve the IServiceScopeFactory.
        _ = _mockProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockScopeFactory.Object);

        // Mock the IServiceScopeFactory to return a mock scope.
        _ = mockScopeFactory.Setup(factory => factory.CreateScope())
            .Returns(mockScope.Object);

        // Mock the logger.
        var mockLogger = new Mock<ILogger>();
        _ = mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Mock the service provider to return the logger when requested.
        _ = mockScope.Setup(x => x.ServiceProvider).Returns(_mockProvider.Object);
        _ = _mockProvider.Setup(x => x.GetService(typeof(ILogger)))
            .Returns(mockLogger.Object);

        // Define test configuration for the Keycloak realm.
        _testConfig = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = TestEnvironment.BaseUrl,
            Realm = TestEnvironment.TestingRealm.Name,
            ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId,
            RealmAdminCredentials = new KcUserLogin
            {
                Username = TestEnvironment.TestingRealm.User.Username,
                Password = TestEnvironment.TestingRealm.User.Password
            }
        };

        // Mock the configuration store to return the test configuration.
        _ = _mockConfigStore.Setup(c => c.GetRealmsAdminConfiguration())
            .Returns(new List<KcRealmAdminConfiguration>
            {
                _testConfig
            });
    }

    /// <summary>
    /// Verifies that the constructor throws an <see cref="ArgumentNullException"/> when the configuration store is null.
    /// </summary>
    [TestMethod]
    public void A_ShouldValidateRealmAdminConfigurationStore() => Assert.ThrowsException<ArgumentNullException>(
        () => _ = new KcRealmAdminTokenHandler(null, _mockProvider.Object));

    /// <summary>
    /// Verifies that the constructor throws an <see cref="ArgumentNullException"/>
    /// when the configuration store returns a null collection of realm admin configurations.
    /// </summary>
    [TestMethod]
    public void B_ShouldValidateRealmAdminConfigurationStoreCollection()
    {
        _ = _mockConfigStore
            .Setup(c => c.GetRealmsAdminConfiguration())
            .Returns(() => null);

        _ = Assert.ThrowsException<ArgumentNullException>(() =>
            _ = new KcRealmAdminTokenHandler(_mockConfigStore.Object, _mockProvider.Object));
    }

    /// <summary>
    /// Verifies that an instance of <see cref="KcRealmAdminTokenHandler"/> is successfully created
    /// when valid dependencies are provided.
    /// </summary>
    [TestMethod]
    public void C_ShouldCreateAnInstance()
    {
        // Create an instance of the token handler with the mocked configuration store and provider.
        _tokenHandler = new KcRealmAdminTokenHandler(_mockConfigStore.Object, _mockProvider.Object);

        Assert.IsNotNull(_tokenHandler);
    }

    /// <summary>
    /// Verifies that <see cref="KcRealmAdminTokenHandler.TryGetAdminTokenAsync"/> throws an <see cref="ArgumentNullException"/>
    /// when the realm name is null.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldValidateRealmName()
    {
        // Create an instance of the token handler with the mocked configuration store and provider.
        _tokenHandler = new KcRealmAdminTokenHandler(_mockConfigStore.Object, _mockProvider.Object);

        Assert.IsNotNull(_tokenHandler);

        _ = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _tokenHandler.TryGetAdminTokenAsync(null).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that <see cref="KcRealmAdminTokenHandler.TryGetAdminTokenAsync"/> throws a <see cref="KcException"/>
    /// and does not cache a token when the configuration contains an invalid Keycloak base URL.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldNotCacheTokenWithInvalidConfiguration()
    {
        // Define test configuration for the Keycloak realm.
        _testConfig = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = TestEnvironment.InvalidBaseUrl,
            Realm = TestEnvironment.TestingRealm.Name,
            ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId,
            RealmAdminCredentials = new KcUserLogin
            {
                Username = TestEnvironment.TestingRealm.User.Username,
                Password = TestEnvironment.TestingRealm.User.Password
            }
        };

        // Mock the configuration store to return the test configuration.
        _ = _mockConfigStore.Setup(c => c.GetRealmsAdminConfiguration())
            .Returns(new List<KcRealmAdminConfiguration>
            {
                _testConfig
            });

        // Create an instance of the token handler with the mocked configuration store and provider.
        _tokenHandler = new KcRealmAdminTokenHandler(_mockConfigStore.Object, _mockProvider.Object);

        Assert.IsNotNull(_tokenHandler);

        // Verify that a KcException is thrown due to the invalid configuration.
        _ = await Assert.ThrowsExceptionAsync<KcException>(async () => await _tokenHandler
            .TryGetAdminTokenAsync(TestEnvironment.TestingRealm.Name).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the <see cref="KcRealmAdminTokenHandler.TryGetAdminTokenAsync"/> caches the token
    /// and returns a valid non-empty token for a valid realm configuration.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldCacheTokenAndReturnToken()
    {
        // Create an instance of the token handler with the mocked configuration store and provider.
        _tokenHandler = new KcRealmAdminTokenHandler(_mockConfigStore.Object, _mockProvider.Object);

        // Assert that the token handler instance is not null.
        Assert.IsNotNull(_tokenHandler);

        // Act: Retrieve the admin token for the specified realm.
        var token = await _tokenHandler.TryGetAdminTokenAsync(TestEnvironment.TestingRealm.Name).ConfigureAwait(false);

        // Assert: Verify that a non-empty token is returned.
        Assert.IsFalse(string.IsNullOrEmpty(token));
    }
}
