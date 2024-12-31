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
    /// Represents the context of the current test.
    /// Used for consistent naming and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = $"{nameof(KcAuthResourceOwnerPasswordTests)}";

    /// <summary>
    /// Sets up the test environment and initializes required components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Auth);

    /// <summary>
    /// Tests the successful retrieval of a Resource Owner Password token.
    /// Validates the presence and correctness of the token response properties.
    /// </summary>
    [TestMethod]
    public async Task ShouldGetResourceOwnerPasswordToken() =>
        await GetRealmAdminToken(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests validation of user login parameters in the Resource Owner Password flow.
    /// Ensures that exceptions are thrown for invalid or missing user login details.
    /// </summary>
    [TestMethod]
    public async Task ShouldValidateUserLogin()
    {
        // Assert that null login details throw an exception.
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
                    new KcClientCredentials
                    {
                        ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
                    }, null).ConfigureAwait(false))
            .ConfigureAwait(false);

        // Assert that an empty login object throws an exception.
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
                    new KcClientCredentials
                    {
                        ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
                    }, new KcUserLogin()).ConfigureAwait(false))
            .ConfigureAwait(false);

        // Assert that a login object with only a username throws an exception.
        _ = await Assert.ThrowsExceptionAsync<KcException>(
                async () => await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
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
                async () => await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.TestingRealm.Name,
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
