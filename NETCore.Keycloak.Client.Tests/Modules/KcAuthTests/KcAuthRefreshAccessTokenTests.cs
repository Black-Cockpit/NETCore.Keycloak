using System.Net;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Test suite for validating the access token refresh functionality in the Keycloak authentication client.
/// This class ensures that resource owner password tokens can be obtained and refreshed as expected.
/// Inherits from <see cref="KcTestingModule"/> for shared configuration and setup logic.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcAuthRefreshAccessTokenTests : KcTestingModule
{
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
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Auth);

    /// <summary>
    /// Validates that a resource owner password token can be successfully retrieved.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldGetResourceOwnerPasswordToken()
    {
        // Act
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
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
    /// Validates that an access token can be successfully refreshed using a valid refresh token.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldRefreshAccessToken()
    {
        Assert.IsNotNull(AccessToken);

        // Act
        var tokenResponse = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, AccessToken.RefreshToken).ConfigureAwait(false);

        // Assert
        KcCommonAssertion.AssertIdentityProviderTokenResponse(tokenResponse);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Post);

        // Update the stored access token with the refreshed token.
        AccessToken = tokenResponse.Response;
    }

    /// <summary>
    /// Validates that an error response is received when attempting to refresh an access token with an invalid token.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldRefreshAccessTokenWithError()
    {
        Assert.IsNotNull(AccessToken);

        // Act
        var tokenResponse = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, AccessToken.AccessToken).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(tokenResponse);
        Assert.IsTrue(tokenResponse.IsError);
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.ErrorMessage));

        // Validate monitoring metrics for the failed request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.BadRequest,
            HttpMethod.Post, true);
    }
}
