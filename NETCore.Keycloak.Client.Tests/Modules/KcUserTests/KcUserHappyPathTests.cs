using System.Net;
using Bogus;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcUserTests;

/// <summary>
/// Contains tests for validating the Keycloak User API functionalities under expected scenarios (Happy Path).
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcUserHappyPathTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak user used for testing in the happy path scenarios.
    /// </summary>
    private static KcUser TestUser
    {
        get
        {
            try
            {
                // Retrieve and deserialize the user object from the environment variable.
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable($"{nameof(KcUserHappyPathTests)}_KCUSER_HAPPY_PATH") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcUserHappyPathTests)}_KCUSER_HAPPY_PATH",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak credentials used for testing in happy path scenarios.
    /// </summary>
    private static KcCredentials TestCredentials
    {
        get
        {
            try
            {
                // Retrieve and deserialize the credential object from the environment variable.
                return JsonConvert.DeserializeObject<KcCredentials>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcUserHappyPathTests)}_KCUSER_CREDENTIALS_HAPPY_PATH") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }

        set => Environment.SetEnvironmentVariable($"{nameof(KcUserHappyPathTests)}_KCUSER_CREDENTIALS_HAPPY_PATH",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak client module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Users);

    /// <summary>
    /// Validates the functionality to create a new user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreateUser()
    {
        // Retrieve an access token for the realm admin to perform the user creation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Generate a mock user using the Faker instance.
        var faker = new Faker();
        var user = KcUserMocks.GenerateUser(faker);
        Assert.IsNotNull(user);

        // Execute the operation to create the user in the specified realm.
        var createUserResponse = await KeycloakRestClient.Users
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, user)
            .ConfigureAwait(false);

        // Validate the response from the user creation operation.
        Assert.IsNotNull(createUserResponse);
        Assert.IsFalse(createUserResponse.IsError);

        // Validate the monitoring metrics for the successful user creation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(createUserResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);

        // Update the test user to reference the newly created user.
        TestUser = user;
    }

    /// <summary>
    /// Validates the functionality to list users in the Keycloak system based on a specified filter.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldListUsers()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the user listing.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to list users matching the specified filter criteria.
        var listUsersResponse = await KeycloakRestClient.Users.ListUserAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            new KcUserFilter
            {
                Email = TestUser.Email,
                Exact = true
            }).ConfigureAwait(false);

        // Validate the response from the user listing operation.
        Assert.IsNotNull(listUsersResponse);
        Assert.IsFalse(listUsersResponse.IsError);
        Assert.IsNotNull(listUsersResponse.Response);

        // Ensure that the response contains exactly one user matching the filter criteria.
        Assert.IsTrue(listUsersResponse.Response.Count() == 1);

        // Validate the monitoring metrics for the successful user listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listUsersResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Update the test user to reference the first user in the response.
        TestUser = listUsersResponse.Response.First();
    }

    /// <summary>
    /// Validates the functionality to retrieve a user's details in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldGetUsers()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the user retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve the user details by their ID.
        var getUserResponse = await KeycloakRestClient.Users
            .GetAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Validate the response from the user retrieval operation.
        Assert.IsNotNull(getUserResponse);
        Assert.IsFalse(getUserResponse.IsError);
        Assert.IsNotNull(getUserResponse.Response);

        // Ensure that the response is of the expected user type.
        Assert.IsInstanceOfType<KcUser>(getUserResponse.Response);

        // Validate the monitoring metrics for the successful user retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Update the test user with the retrieved user details.
        TestUser = getUserResponse.Response;
    }

    /// <summary>
    /// Validates the functionality to update a user's details in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldUpdateUser()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the user update.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Modify the user data to be updated.
        TestUser.Enabled = false;

        // Execute the operation to update the user details.
        var updateUserResponse = await KeycloakRestClient.Users
            .UpdateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id, TestUser)
            .ConfigureAwait(false);

        // Validate the response from the user update operation.
        Assert.IsNotNull(updateUserResponse);
        Assert.IsFalse(updateUserResponse.IsError);

        // Validate the monitoring metrics for the successful user update request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateUserResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Validates the functionality to check the existence of a user by their email in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldCheckUserExistence()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the user existence check.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to check if a user exists by their email address.
        var checkIfUserExistsResponse = await KeycloakRestClient.Users
            .IsUserExistsByEmailAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Email)
            .ConfigureAwait(false);

        // Validate the response from the user existence check operation.
        Assert.IsNotNull(checkIfUserExistsResponse);
        Assert.IsFalse(checkIfUserExistsResponse.IsError);
        Assert.IsNotNull(checkIfUserExistsResponse.Response);

        // Ensure the response indicates that the user exists.
        Assert.IsTrue(checkIfUserExistsResponse.Response);

        // Validate the absence of monitoring metrics for this operation.
        Assert.IsNull(checkIfUserExistsResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Validates the functionality to count users in the Keycloak system based on a specified filter.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldCountUsers()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the user count retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to count users matching the specified filter criteria.
        var usersCountResponse = await KeycloakRestClient.Users.CountAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            new KcUserFilter
            {
                Email = TestUser.Email,
                Exact = true
            }).ConfigureAwait(false);

        // Validate the response from the user count retrieval operation.
        Assert.IsNotNull(usersCountResponse);
        Assert.IsFalse(usersCountResponse.IsError);
        Assert.IsNotNull(usersCountResponse.Response);

        // Validate that the response contains a valid integer representing the count.
        Assert.IsNotNull(usersCountResponse.GetResponseAsInt());

        // Ensure the count matches the expected value (1).
        Assert.IsTrue(usersCountResponse.GetResponseAsInt() == 1);

        // Validate the monitoring metrics for the successful user count retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(usersCountResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve credentials associated with a user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldGetCredentials()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the credential retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve credentials associated with the user.
        var getCredentialsResponse = await KeycloakRestClient.Users.GetCredentialsAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id).ConfigureAwait(false);

        // Validate the response from the credential retrieval operation.
        Assert.IsNotNull(getCredentialsResponse);
        Assert.IsFalse(getCredentialsResponse.IsError);
        Assert.IsNotNull(getCredentialsResponse.Response);

        // Ensure that the response contains at least one credential.
        Assert.IsTrue(getCredentialsResponse.Response.Any());

        // Validate the monitoring metrics for the successful credential retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getCredentialsResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Update the test credentials with the first retrieved credential.
        TestCredentials = getCredentialsResponse.Response.First();
    }

    /// <summary>
    /// Validates the functionality to update the label of a user's credential in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldUpdateCredentialsLabel()
    {
        // Ensure the test user and test credentials are initialized before proceeding.
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestCredentials);

        // Retrieve an access token for the realm admin to perform the label update.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to update the label of the user's credential.
        var updateCredentialLabelResponse = await KeycloakRestClient.Users
            .UpdateCredentialsLabelAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id,
                TestCredentials.Id, "NewLabel").ConfigureAwait(false);

        // Validate the response from the credential label update operation.
        Assert.IsNotNull(updateCredentialLabelResponse);
        Assert.IsFalse(updateCredentialLabelResponse.IsError);

        // Validate the monitoring metrics for the successful label update request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateCredentialLabelResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Validates the functionality to delete a user's credential in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldDeleteCredentials()
    {
        // Ensure the test user and test credentials are initialized before proceeding.
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestCredentials);

        // Retrieve an access token for the realm admin to perform the credential deletion.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the user's credential.
        var deleteCredentialResponse = await KeycloakRestClient.Users.DeleteCredentialsAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id, TestCredentials.Id)
            .ConfigureAwait(false);

        // Validate the response from the credential deletion operation.
        Assert.IsNotNull(deleteCredentialResponse);
        Assert.IsFalse(deleteCredentialResponse.IsError);

        // Validate the monitoring metrics for the successful credential deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteCredentialResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to retrieve groups associated with a user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldGetUserGroups()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the group retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve groups associated with the user.
        var getUserGroupsResponse = await KeycloakRestClient.Users
            .UserGroupsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Validate the response from the group retrieval operation.
        Assert.IsNotNull(getUserGroupsResponse);
        Assert.IsFalse(getUserGroupsResponse.IsError);
        Assert.IsNotNull(getUserGroupsResponse.Response);

        // Ensure that the user is not a member of any groups.
        Assert.IsFalse(getUserGroupsResponse.Response.Any());

        // Validate the monitoring metrics for the successful group retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserGroupsResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to count the groups associated with a user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task K_ShouldCountUserGroups()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the group count retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to count the groups associated with the user.
        var userGroupsCountResponse = await KeycloakRestClient.Users
            .CountGroupsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Validate the response from the group count retrieval operation.
        Assert.IsNotNull(userGroupsCountResponse);
        Assert.IsFalse(userGroupsCountResponse.IsError);
        Assert.IsNotNull(userGroupsCountResponse.Response);

        // Validate that the group count response is not null and matches the expected value (0).
        Assert.IsNotNull(userGroupsCountResponse.Response.Count);
        Assert.IsTrue(userGroupsCountResponse.Response.Count == 0);

        // Validate the monitoring metrics for the successful group count retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(userGroupsCountResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to reset a user's password in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldResetPassword()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the password reset.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to reset the user's password with new credentials.
        var resetUserPasswordResponse = await KeycloakRestClient.Users
            .ResetPasswordAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id,
                new KcCredentials
                {
                    Temporary = false,
                    UserLabel = "Password",
                    Type = "password",
                    Value = KcTestPasswordCreator.Create()
                }).ConfigureAwait(false);

        // Validate the response from the password reset operation.
        Assert.IsNotNull(resetUserPasswordResponse);
        Assert.IsFalse(resetUserPasswordResponse.IsError);

        // Validate the monitoring metrics for the successful password reset request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(resetUserPasswordResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Validates the functionality to retrieve active sessions for a user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldGetUserSessions()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the session retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve active sessions for the user.
        var userSessionsResponse = await KeycloakRestClient.Users
            .SessionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Validate the response from the session retrieval operation.
        Assert.IsNotNull(userSessionsResponse);
        Assert.IsFalse(userSessionsResponse.IsError);
        Assert.IsNotNull(userSessionsResponse.Response);

        // Ensure that there are no active sessions associated with the user.
        Assert.IsFalse(userSessionsResponse.Response.Any());

        // Validate the monitoring metrics for the successful session retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(userSessionsResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to log out a user from all active sessions in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task O_ShouldLogoutFromAllSessions()
    {
        // Ensure the test user is initialized before proceeding.
        Assert.IsNotNull(TestUser);

        // Retrieve an access token for the realm admin to perform the logout operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to log out the user from all active sessions.
        var logoutFromAllSessionsResponse = await KeycloakRestClient.Users
            .LogoutFromAllSessionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Validate the response from the logout operation.
        Assert.IsNotNull(logoutFromAllSessionsResponse);
        Assert.IsFalse(logoutFromAllSessionsResponse.IsError);

        // Validate the monitoring metrics for the successful logout request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(logoutFromAllSessionsResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
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
}
