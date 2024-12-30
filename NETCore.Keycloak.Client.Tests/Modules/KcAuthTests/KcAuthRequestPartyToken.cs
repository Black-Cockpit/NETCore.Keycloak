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
/// Test suite for validating the retrieval of Request Party Tokens (RPTs) in the Keycloak authentication client.
/// This class ensures that RPTs are handled correctly, including scenarios where access is forbidden.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcAuthRequestPartyToken : KcTestingModule
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
    /// Validates that a Request Party Token (RPT) request results in a forbidden error
    /// when the required permissions are not granted.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldGetRequestPartyTokenWithForbidden()
    {
        // Ensure the access token is available.
        Assert.IsNotNull(AccessToken);

        // Retrieve the first audience from the user's permissions.
        var audience = TestEnvironment.TestingRealm.User.Permissions.FirstOrDefault();

        // Validate the audience and its scopes.
        Assert.IsNotNull(audience);
        Assert.IsFalse(string.IsNullOrEmpty(audience.Name));
        Assert.IsNotNull(audience.Scopes);
        Assert.IsTrue(audience.Scopes.Any());

        // Act
        var rptResponse = await _client.Auth.GetRequestPartyTokenAsync(TestEnvironment.TestingRealm.Name,
            AccessToken.AccessToken, audience.Name, audience.Scopes).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(rptResponse);
        Assert.IsTrue(rptResponse.IsError);
        Assert.IsFalse(string.IsNullOrEmpty(rptResponse.ErrorMessage));

        // Validate monitoring metrics for the forbidden request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(rptResponse.MonitoringMetrics, HttpStatusCode.Forbidden,
            HttpMethod.Post, true);
    }
}
