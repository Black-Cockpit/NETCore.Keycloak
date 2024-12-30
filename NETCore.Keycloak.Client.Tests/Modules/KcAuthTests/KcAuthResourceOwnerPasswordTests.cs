using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Tests.Abstraction;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Test suite for validating the Resource Owner Password flow in the Keycloak authentication client.
/// Inherits from <see cref="KcTestingModule"/> to leverage shared configuration and setup logic.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcAuthResourceOwnerPasswordTests : KcTestingModule
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
    /// Tests the successful retrieval of a Resource Owner Password token.
    /// Validates the presence and correctness of the token response properties.
    /// </summary>
    [TestMethod]
    public async Task ShouldGetResourceOwnerPasswordToken()
    {
        // Act
        var tokenResponse = await _client.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, new KcUserLogin
            {
                Username = TestEnvironment.TestingRealm.User.Username,
                Password = TestEnvironment.TestingRealm.User.Password
            }).ConfigureAwait(false);

        // Assert
        KcCommonAssertion.AssertIdentityProviderTokenResponse(tokenResponse);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Post);
    }

    /// <summary>
    /// Tests validation of user login parameters in the Resource Owner Password flow.
    /// Ensures that exceptions are thrown for invalid or missing user login details.
    /// </summary>
    [TestMethod]
    public async Task ShouldValidateUserLogin()
    {
        // Assert that null login details throw an exception.
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await _client.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
                    new KcClientCredentials
                    {
                        ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
                    }, null).ConfigureAwait(false))
            .ConfigureAwait(false);

        // Assert that an empty login object throws an exception.
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await _client.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
                    new KcClientCredentials
                    {
                        ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
                    }, new KcUserLogin()).ConfigureAwait(false))
            .ConfigureAwait(false);

        // Assert that a login object with only a username throws an exception.
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await _client.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
                    new KcClientCredentials
                    {
                        ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
                    }, new KcUserLogin
                    {
                        Username = TestEnvironment.TestingRealm.User.Username
                    }).ConfigureAwait(false))
            .ConfigureAwait(false);

        // Assert that a login object with only a password throws an exception.
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await _client.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
                    new KcClientCredentials
                    {
                        ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
                    }, new KcUserLogin
                    {
                        Password = TestEnvironment.TestingRealm.User.Password
                    }).ConfigureAwait(false))
            .ConfigureAwait(false);
    }
}
