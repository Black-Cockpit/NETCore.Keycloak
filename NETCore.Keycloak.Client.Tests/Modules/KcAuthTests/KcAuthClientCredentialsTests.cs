using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Tests.Abstraction;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Test suite for validating the client credentials flow in the Keycloak authentication client.
/// Inherits from <see cref="KcTestingModule"/> to leverage shared configuration and setup logic.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcAuthClientCredentialsTests : KcTestingModule
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
    /// Tests the successful retrieval of a client credentials token.
    /// Validates the presence and correctness of the token response properties.
    /// </summary>
    [TestMethod]
    public async Task ShouldGetClientCredentialsToken()
    {
        // Act
        var tokenResponse = await _client.Auth.GetClientCredentialsTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PrivateClient.ClientId,
                Secret = TestEnvironment.TestingRealm.PrivateClient.Secret
            }).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(tokenResponse);
        Assert.IsFalse(tokenResponse.IsError);
        Assert.IsNotNull(tokenResponse.Response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.Response.AccessToken));
        Assert.IsNotNull(tokenResponse.Response.Scope);
        Assert.IsNotNull(tokenResponse.Response.NotBeforePolicy);
        Assert.IsNotNull(tokenResponse.Response.ExpiresIn);
        Assert.IsNotNull(tokenResponse.Response.TokenType);
        Assert.IsNotNull(tokenResponse.Response.CreatedAt);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Post);
    }

    /// <summary>
    /// Tests that a <see cref="KcException"/> is thrown when invalid client credentials are provided.
    /// </summary>
    [TestMethod]
    public async Task ShouldValidateClientCredentials()
    {
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await _client.Auth
                    .GetClientCredentialsTokenAsync(TestEnvironment.TestingRealm.Name, null).ConfigureAwait(false))
            .ConfigureAwait(false);

        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await _client.Auth
                    .GetClientCredentialsTokenAsync(TestEnvironment.TestingRealm.Name, new KcClientCredentials())
                    .ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that an error response is received when invalid client credentials are used.
    /// Validates the error message and monitoring metrics for the failed request.
    /// </summary>
    [TestMethod]
    public async Task ShouldExecuteClientCredentialsWithError()
    {
        // Act
        var tokenResponse = await _client.Auth.GetClientCredentialsTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PrivateClient.ClientId,
                Secret = "fake_secret"
            }).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(tokenResponse);
        Assert.IsTrue(tokenResponse.IsError);
        Assert.IsTrue(!string.IsNullOrWhiteSpace(tokenResponse.ErrorMessage));

        // Validate monitoring metrics for the failed request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.Unauthorized,
            HttpMethod.Post, true);
    }
}
