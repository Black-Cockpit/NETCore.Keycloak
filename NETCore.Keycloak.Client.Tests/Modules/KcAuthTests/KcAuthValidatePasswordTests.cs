using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Tests.Abstraction;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Test suite for validating password authentication in the Keycloak client.
/// This class tests the ability to verify user passwords using the Keycloak API.
/// Inherits from <see cref="KcTestingModule"/> to leverage shared configuration and setup logic.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcAuthValidatePasswordTests : KcTestingModule
{
    /// <summary>
    /// Mock instance of the <see cref="ILogger"/> for testing logging behavior during Keycloak operations.
    /// </summary>
    private Mock<ILogger> _mockLogger;

    /// <summary>
    /// Instance of the <see cref="IKeycloakClient"/> used to perform Keycloak authentication operations.
    /// </summary>
    private IKeycloakClient _client;

    /// <summary>
    /// Sets up the test environment and initializes required components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        // Load the test environment configuration from the base module.
        LoadConfiguration();

        // Initialize the mock logger.
        _mockLogger = new Mock<ILogger>();
        _ = _mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Initialize the Keycloak client using the configured base URL and mock logger.
        _client = new KeycloakClient(TestEnvironment.BaseUrl, _mockLogger.Object);

        // Assert that the authentication module is initialized correctly.
        Assert.IsNotNull(_client.Auth);
    }

    /// <summary>
    /// Tests the successful validation of a user's password using the Keycloak API.
    /// Ensures that the response is valid and indicates a successful validation.
    /// </summary>
    [TestMethod]
    public async Task ShouldValidatePassword()
    {
        // Act
        var validatePasswordResponse = await _client.Auth.ValidatePasswordAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, new KcUserLogin
            {
                Username = TestEnvironment.TestingRealm.User.Username,
                Password = TestEnvironment.TestingRealm.User.Password
            }).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(validatePasswordResponse);
        Assert.IsFalse(validatePasswordResponse.IsError);
        Assert.IsNotNull(validatePasswordResponse.Response);
        Assert.IsTrue(validatePasswordResponse.Response);

        // Validate monitoring metrics
        Assert.IsNotNull(validatePasswordResponse.MonitoringMetrics);
        Assert.IsTrue(validatePasswordResponse.MonitoringMetrics.Count != 0);
    }

    /// <summary>
    /// Tests the unsuccessful validation of a user's password using the Keycloak API.
    /// Ensures that the response is valid but indicates a failed validation due to incorrect credentials.
    /// </summary>
    [TestMethod]
    public async Task ShouldNotValidatePassword()
    {
        // Act
        var validatePasswordResponse = await _client.Auth.ValidatePasswordAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, new KcUserLogin
            {
                Username = TestEnvironment.TestingRealm.User.Username,
                Password = "IncorrectPassword"
            }).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(validatePasswordResponse);
        Assert.IsFalse(validatePasswordResponse.IsError);
        Assert.IsNotNull(validatePasswordResponse.Response);
        Assert.IsFalse(validatePasswordResponse.Response);

        // Validate monitoring metrics
        Assert.IsNotNull(validatePasswordResponse.MonitoringMetrics);
        Assert.IsTrue(validatePasswordResponse.MonitoringMetrics.Count != 0);
    }
}
