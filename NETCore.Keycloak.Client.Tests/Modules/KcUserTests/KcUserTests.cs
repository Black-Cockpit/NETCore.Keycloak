using System.Net;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcUserTests;

/// <summary>
/// Contains tests for validating the Keycloak user API functionalities under expected scenarios.
/// </summary>
[TestClass]
[TestCategory("Combined")]
public class KcUserTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Represents the password for the test Keycloak user.
    /// </summary>
    private static string TestUserPassword
    {
        // Retrieve the test user's password from the environment variable.
        get => Environment.GetEnvironmentVariable("KCUSER_PASSWORD");

        // Store the test user's password in the environment variable.
        set => Environment.SetEnvironmentVariable("KCUSER_PASSWORD", value);
    }

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
                    Environment.GetEnvironmentVariable($"{nameof(KcUserTests)}_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcUserTests)}_KCUSER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak group used for testing.
    /// </summary>
    private static KcGroup TestGroup
    {
        get
        {
            try
            {
                // Retrieve and deserialize the group object from the environment variable.
                return JsonConvert.DeserializeObject<KcGroup>(
                    Environment.GetEnvironmentVariable($"{nameof(KcUserTests)}_KC_GROUP") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcUserTests)}_KC_GROUP",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Represents the test Keycloak session used in the current test execution.
    /// </summary>
    private static KcSession TestSession
    {
        get
        {
            try
            {
                // Retrieve and deserialize the session object from the environment variable.
                return JsonConvert.DeserializeObject<KcSession>(
                    Environment.GetEnvironmentVariable($"{nameof(KcUserTests)}_KC_SESSION") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcUserTests)}_KC_SESSION",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the Keycloak REST client components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        Assert.IsNotNull(KeycloakRestClient.Groups);
        Assert.IsNotNull(KeycloakRestClient.Users);
        Assert.IsNotNull(KeycloakRestClient.Auth);
    }

    /// <summary>
    /// Tests the creation of a group in the Keycloak realm and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task A_CreateGroup() => TestGroup = await CreateAndGetGroupAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a test user in the Keycloak realm and assigns the generated user password.
    /// </summary>
    [TestMethod]
    public async Task B_CreateTestUser()
    {
        // Generate a password for the test user
        TestUserPassword = KcTestPasswordCreator.Create();

        // Create the test user and store the result
        TestUser = await CreateAndGetRealmUserAsync(TestContext, TestUserPassword).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests adding a user to a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldAddUserToGroup()
    {
        // Ensure that the test user and group exist
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to add the user to the group
        var addUserToGroupResponse = await KeycloakRestClient.Users.AddToGroupAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestUser.Id,
            TestGroup.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addUserToGroupResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addUserToGroupResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addUserToGroupResponse.Response);

        // Validate monitoring metrics for the add-to-group request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addUserToGroupResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Tests removing a user from a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldRemoveUserFromGroup()
    {
        // Ensure that the test user and group exist
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to remove the user from the group
        var removeUserFromGroupResponse = await KeycloakRestClient.Users
            .DeleteFromGroupAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id,
                TestGroup.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(removeUserFromGroupResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(removeUserFromGroupResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(removeUserFromGroupResponse.Response);

        // Validate monitoring metrics for the remove-from-group request
        KcCommonAssertion.AssertResponseMonitoringMetrics(removeUserFromGroupResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests performing a user login in the Keycloak realm using resource owner password credentials.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldPerformUserLogin()
    {
        // Act: Send a request to get an access token using the public client credentials and user login information
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Assert: Validate the token response
        KcCommonAssertion.AssertIdentityProviderTokenResponse(tokenResponse);

        // Validate monitoring metrics for the successful request
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Post);
    }

    /// <summary>
    /// Tests retrieving active user sessions for a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldGetUserSessions()
    {
        // Ensure that the test user exists
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get active user sessions
        var userSessionsResponse = await KeycloakRestClient.Users
            .SessionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(userSessionsResponse);
        Assert.IsFalse(userSessionsResponse.IsError);

        // Ensure that the response contains a list of active sessions
        Assert.IsNotNull(userSessionsResponse.Response);

        // Verify that there is at least one active session
        Assert.IsTrue(userSessionsResponse.Response.Any());

        // Assign the first active session to TestSession for further validation
        TestSession = userSessionsResponse.Response.First();

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(userSessionsResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting an active user session in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldDeleteUserSessions()
    {
        // Ensure that the test user and session exist
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestSession);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the user session
        var deleteUserSessionResponse = await KeycloakRestClient.Users
            .DeleteSessionAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestSession.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteUserSessionResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteUserSessionResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteUserSessionResponse.Response);

        // Validate monitoring metrics for the session deletion request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteUserSessionResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to delete a user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldDeleteUser()
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
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to delete a group in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZA_ShouldDeleteGroup()
    {
        // Ensure the test group is initialized before proceeding.
        Assert.IsNotNull(TestGroup);

        // Retrieve an access token for the realm admin to perform the group deletion.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the specified group.
        var deleteGroupResponse = await KeycloakRestClient.Groups
            .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id)
            .ConfigureAwait(false);

        // Validate the response from the group deletion operation.
        Assert.IsNotNull(deleteGroupResponse);
        Assert.IsFalse(deleteGroupResponse.IsError);

        // Validate the monitoring metrics for the successful group deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteGroupResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }
}
