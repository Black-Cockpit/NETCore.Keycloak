using System.Net;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Contains tests for Keycloak's attack detection module, focusing on user-specific operations such as retrieving
/// user status and clearing login failure history.
/// </summary>
[TestClass]
[TestCategory("Combined")]
public class KcAttackDetectionTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Represents the test Keycloak user used in the current test execution.
    /// </summary>
    private static KcUser TestUser
    {
        get
        {
            try
            {
                // Retrieve and deserialize the user object from the environment variable.
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable($"{nameof(KcAttackDetectionTests)}_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcAttackDetectionTests)}_KCUSER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the required dependencies for the test class.
    /// Ensures that the attack detection and user management modules are properly initialized and accessible.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        // Ensure that the attack detection module is initialized
        Assert.IsNotNull(KeycloakRestClient.AttackDetection);

        // Ensure that the user management module is initialized
        Assert.IsNotNull(KeycloakRestClient.Users);
    }

    /// <summary>
    /// Creates a new test user in the Keycloak realm and assigns it to the <see cref="TestUser"/> property.
    /// </summary>
    [TestMethod]
    public async Task A_CreateTestUser() =>
        TestUser = await CreateAndGetRealmUserAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Verifies that the status of a user can be successfully retrieved from the Keycloak attack detection module.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldGetUserStatus()
    {
        // Ensure that the test user is not null
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve the user's status
        var getUserStatusResponse = await KeycloakRestClient.AttackDetection
            .GetUserStatusAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(getUserStatusResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(getUserStatusResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(getUserStatusResponse.Response);

        // Verify that the user's last failure is zero
        Assert.IsTrue(getUserStatusResponse.Response.LastFailure == 0);

        // Verify that the user is not disabled
        Assert.IsFalse(getUserStatusResponse.Response.Disabled);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserStatusResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Verifies that a user's login failure history can be successfully deleted in the Keycloak attack detection module.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldDeleteUsersLoginFailure()
    {
        // Ensure that the test user is not null
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the user's login failure history
        var deleteUsersLoginFailureResponse = await KeycloakRestClient.AttackDetection
            .DeleteUsersLoginFailureAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(deleteUsersLoginFailureResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(deleteUsersLoginFailureResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteUsersLoginFailureResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to delete a user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldDeleteUser()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the user deletion.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the specified user.
        var deleteUserResponse = await KeycloakRestClient.Users
            .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Validate the response from the user deletion operation.
        Assert.IsNotNull(deleteUserResponse);
        Assert.IsFalse(deleteUserResponse.IsError);

        // Validate the monitoring metrics for the successful user deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteUserResponse.MonitoringMetrics,
            HttpStatusCode.NoContent,
            HttpMethod.Delete);
    }
}
