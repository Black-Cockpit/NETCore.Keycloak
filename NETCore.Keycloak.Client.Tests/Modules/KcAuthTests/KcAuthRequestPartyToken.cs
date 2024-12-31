using System.Net;
using NETCore.Keycloak.Client.Tests.Abstraction;

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
    /// Represents the context of the current test.
    /// Used for consistent naming and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = $"{nameof(KcAuthRequestPartyToken)}";

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
        await GetRealmAdminToken(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Validates that a Request Party Token (RPT) request results in a forbidden error
    /// when the required permissions are not granted.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldGetRequestPartyTokenWithForbidden()
    {
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);

        // Ensure the access token is available.
        Assert.IsNotNull(accessToken);

        // Retrieve the first audience from the user's permissions.
        var audience = TestEnvironment.TestingRealm.User.Permissions.FirstOrDefault();

        // Validate the audience and its scopes.
        Assert.IsNotNull(audience);
        Assert.IsFalse(string.IsNullOrEmpty(audience.Name));
        Assert.IsNotNull(audience.Scopes);
        Assert.IsTrue(audience.Scopes.Any());

        // Act
        var rptResponse = await KeycloakRestClient.Auth.GetRequestPartyTokenAsync(TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken, audience.Name, audience.Scopes).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(rptResponse);
        Assert.IsTrue(rptResponse.IsError);
        Assert.IsFalse(string.IsNullOrEmpty(rptResponse.ErrorMessage));

        // Validate monitoring metrics for the forbidden request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(rptResponse.MonitoringMetrics, HttpStatusCode.Forbidden,
            HttpMethod.Post, true);
    }
}
