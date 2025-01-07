using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Tests.Abstraction;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Test suite for validating common functionality of the <see cref="IKeycloakClient"/> implementation.
/// Inherits from <see cref="KcTestingModule"/> to use the shared test environment setup.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcCommonTests : KcTestingModule
{
    /// <summary>
    /// Mock instance of the <see cref="ILogger"/> for verifying logging behavior during tests.
    /// </summary>
    private Mock<ILogger> _mockLogger;

    /// <summary>
    /// Instance of the <see cref="IKeycloakClient"/> used for executing Keycloak-related operations.
    /// </summary>
    private IKeycloakClient _client;

    /// <summary>
    /// Sets up the test environment and initializes the required components before each test.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        // Load the configuration from the base testing module.
        LoadConfiguration();

        // Initialize the mock logger.
        _mockLogger = new Mock<ILogger>();
        _ = _mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Initialize the Keycloak client with the base URL and mock logger.
        _client = new KeycloakClient(TestEnvironment.BaseUrl, _mockLogger.Object);

        // Verify that the authentication module of the client is properly initialized.
        Assert.IsNotNull(_client.Auth);
    }

    /// <summary>
    /// Validates that an exception is thrown when a required string parameter is null or empty
    /// during the retrieval of a client credentials token.
    /// </summary>
    [TestMethod]
    public async Task ShouldValidateRequiredString() =>
        _ = await Assert.ThrowsExceptionAsync<KcException>(async () =>
            await _client.Auth.GetClientCredentialsTokenAsync(
                null,
                new KcClientCredentials
                {
                    ClientId = TestEnvironment.TestingRealm.PrivateClient.ClientId,
                    Secret = TestEnvironment.TestingRealm.PrivateClient.Secret
                }).ConfigureAwait(false)).ConfigureAwait(false);

    /// <summary>
    /// Tests that an exception is handled correctly when a request is made to an invalid Keycloak URL.
    /// Ensures that error monitoring metrics and exceptions are properly captured.
    /// </summary>
    [TestMethod]
    public async Task ShouldExecuteRequestWithException()
    {
        // Arrange
        _client = new KeycloakClient("https://nodns-for-keycloak.com", _mockLogger.Object);

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
        Assert.IsTrue(tokenResponse.IsError);
        Assert.IsNotNull(tokenResponse.Exception);

        // Validate the monitoring metrics for the failed request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, null, HttpMethod.Post, true);
    }

    /// <summary>
    /// Validates the behavior of the Keycloak client when a request results in a deserialization error.
    /// </summary>
    [TestMethod]
    public async Task ShouldExecuteRequestAndCaptureDeserializationError()
    {
        // Initialize the mock logger.
        var mockLogger = new Mock<ILogger>();
        _ = mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Arrange: Initialize the Keycloak client with an invalid base URL.
        var keycloakRestClient = new KeycloakClient(TestEnvironment.InvalidBaseUrl, mockLogger.Object);

        // Act: Attempt to list users using the invalid base URL.
        var userListResponse = await keycloakRestClient.Users.ListUserAsync(
            TestEnvironment.TestingRealm.Name, "RandomToken").ConfigureAwait(false);

        // Assert: Validate the response indicating an error and the presence of an exception.
        Assert.IsNotNull(userListResponse);
        Assert.IsNotNull(userListResponse.ErrorMessage);
        Assert.IsNotNull(userListResponse.Exception);
        Assert.IsTrue(userListResponse.IsError);

        // Validate monitoring metrics for the failed request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(userListResponse.MonitoringMetrics, null, HttpMethod.Get,
            true);
    }
}
