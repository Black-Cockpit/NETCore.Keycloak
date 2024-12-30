using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Test suite for validating token revocation functionality in the Keycloak authentication client.
/// This includes testing both access token and refresh token revocation flows.
/// Inherits from <see cref="KcTestingModule"/> for shared configuration and setup logic.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcAuthRevokeTokenTests : KcTestingModule
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
    /// Gets or sets the current access token used during tests.
    /// The token is stored as an environment variable and serialized/deserialized as needed.
    /// </summary>
    private static KcIdentityProviderToken AccessToken
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<KcIdentityProviderToken>(
                    Environment.GetEnvironmentVariable($"{nameof(KcAuthRefreshAccessTokenTests)}_KC_TOKEN") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcAuthRefreshAccessTokenTests)}_KC_TOKEN",
            JsonConvert.SerializeObject(value));
    }

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
    /// Validates that a resource owner password token can be successfully retrieved.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldGetResourceOwnerPasswordToken()
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

        // Store the retrieved access token for later use.
        AccessToken = tokenResponse.Response;
    }

    /// <summary>
    /// Validates that an access token can be successfully revoked.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldRevokeAccessToken()
    {
        Assert.IsNotNull(AccessToken);

        // Act
        var revokeAccessTokenResponse = await _client.Auth.RevokeAccessTokenAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, AccessToken.AccessToken).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(revokeAccessTokenResponse);
        Assert.IsFalse(revokeAccessTokenResponse.IsError);
        Assert.IsTrue(string.IsNullOrWhiteSpace(revokeAccessTokenResponse.ErrorMessage));

        // Validate monitoring metrics for the revocation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(revokeAccessTokenResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Post);
    }

    /// <summary>
    /// Validates that a refresh token can be successfully revoked.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldRevokeRefreshToken()
    {
        Assert.IsNotNull(AccessToken);

        // Act
        var revokeAccessTokenResponse = await _client.Auth.RevokeRefreshTokenAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, AccessToken.RefreshToken).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(revokeAccessTokenResponse);
        Assert.IsFalse(revokeAccessTokenResponse.IsError);
        Assert.IsTrue(string.IsNullOrWhiteSpace(revokeAccessTokenResponse.ErrorMessage));

        // Validate monitoring metrics for the revocation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(revokeAccessTokenResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Post);
    }
}
