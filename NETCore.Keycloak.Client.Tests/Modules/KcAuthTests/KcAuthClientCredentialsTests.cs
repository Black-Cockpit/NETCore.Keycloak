using System.Net;
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
    /// Sets up the test environment and initializes required components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Auth);

    /// <summary>
    /// Tests the successful retrieval of a client credentials token.
    /// Validates the presence and correctness of the token response properties.
    /// </summary>
    [TestMethod]
    public async Task ShouldGetClientCredentialsToken()
    {
        // Act
        var tokenResponse = await KeycloakRestClient.Auth.GetClientCredentialsTokenAsync(
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
                async () => await KeycloakRestClient.Auth
                    .GetClientCredentialsTokenAsync(TestEnvironment.TestingRealm.Name, null).ConfigureAwait(false))
            .ConfigureAwait(false);

        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await KeycloakRestClient.Auth
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
        var tokenResponse = await KeycloakRestClient.Auth.GetClientCredentialsTokenAsync(
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
