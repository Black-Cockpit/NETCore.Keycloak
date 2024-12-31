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
    /// Sets up the test environment and initializes required components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Auth);

    /// <summary>
    /// Tests the successful validation of a user's password using the Keycloak API.
    /// Ensures that the response is valid and indicates a successful validation.
    /// </summary>
    [TestMethod]
    public async Task ShouldValidatePassword()
    {
        // Act
        var validatePasswordResponse = await KeycloakRestClient.Auth.ValidatePasswordAsync(TestEnvironment.TestingRealm.Name,
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
        var validatePasswordResponse = await KeycloakRestClient.Auth.ValidatePasswordAsync(TestEnvironment.TestingRealm.Name,
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
