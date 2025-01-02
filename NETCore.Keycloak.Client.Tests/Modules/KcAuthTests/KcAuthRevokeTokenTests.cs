using System.Net;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Tests.Abstraction;

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
    /// Represents the context of the current test.
    /// Used for consistent naming and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = $"{nameof(KcAuthRevokeTokenTests)}";

    /// <summary>
    /// Sets up the test environment and initializes required components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Auth);

    /// <summary>
    /// Validates that a resource owner password token can be successfully retrieved.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldGetResourceOwnerPasswordToken() =>
        await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Validates that an access token can be successfully revoked.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldRevokeAccessToken()
    {
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        Assert.IsNotNull(accessToken);

        // Act
        var revokeAccessTokenResponse = await KeycloakRestClient.Auth.RevokeAccessTokenAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, accessToken.AccessToken).ConfigureAwait(false);

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
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        Assert.IsNotNull(accessToken);

        // Act
        var revokeAccessTokenResponse = await KeycloakRestClient.Auth.RevokeRefreshTokenAsync(TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, accessToken.RefreshToken).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(revokeAccessTokenResponse);
        Assert.IsFalse(revokeAccessTokenResponse.IsError);
        Assert.IsTrue(string.IsNullOrWhiteSpace(revokeAccessTokenResponse.ErrorMessage));

        // Validate monitoring metrics for the revocation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(revokeAccessTokenResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Post);
    }
}
