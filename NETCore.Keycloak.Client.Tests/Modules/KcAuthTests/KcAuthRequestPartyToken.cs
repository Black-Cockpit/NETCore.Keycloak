using System.Net;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Test suite for validating the retrieval of Request Party Tokens (RPTs) in the Keycloak authentication client.
/// This class ensures that RPTs are handled correctly, including scenarios where access is forbidden.
/// </summary>
[TestClass]
[TestCategory("Combined")]
public class KcAuthRequestPartyToken : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// Used for consistent naming and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = $"{nameof(KcAuthRequestPartyToken)}";

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
    /// Gets or sets the Keycloak authorized user used for testing.
    /// </summary>
    private static KcUser TestAuthorizedUser
    {
        get
        {
            try
            {
                // Retrieve and deserialize the user object from the environment variable.
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable($"{nameof(KcAuthRequestPartyToken)}_AUTHORIZED_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcAuthRequestPartyToken)}_AUTHORIZED_KCUSER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak unauthorized user used for testing.
    /// </summary>
    private static KcUser TestUnAuthorizedUser
    {
        get
        {
            try
            {
                // Retrieve and deserialize the user object from the environment variable.
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable($"{nameof(KcAuthRequestPartyToken)}_UNAUTHORIZED_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcAuthRequestPartyToken)}_UNAUTHORIZED_KCUSER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment and initializes required components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        Assert.IsNotNull(KeycloakRestClient.Auth);
        Assert.IsNotNull(KeycloakRestClient.Users);
        Assert.IsNotNull(KeycloakRestClient.RoleMappings);
    }

    /// <summary>
    /// Verifies the creation of both authorized and unauthorized users by assigning attributes
    /// and invoking the user creation logic asynchronously.
    /// </summary>
    [TestMethod]
    public async Task A_CreateUsers()
    {
        // Generate a test password for the user creation process.
        TestUserPassword = KcTestPasswordCreator.Create();

        // Define user attributes for the authorized user.
        var attributes = new Dictionary<string, object>
        {
            {
                "account_owner", 1
            },
            {
                "business_account_owner", 1
            }
        };

        // Create an authorized user with specified attributes and store the result.
        TestAuthorizedUser = await CreateAndGetRealmUserAsync(TestContext, TestUserPassword, attributes)
            .ConfigureAwait(false);

        // Create an unauthorized user without additional attributes and store the result.
        TestUnAuthorizedUser = await CreateAndGetRealmUserAsync(TestContext, TestUserPassword)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that realm roles can be assigned to an authorized user.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldAssignRealmRolesToAuthorizedUser()
    {
        // Retrieve the realm administrator access token.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Validate that the access token is retrieved successfully.
        Assert.IsNotNull(accessToken);

        // Send a request to fetch the available realm roles for the unauthorized user.
        var getUserAvailableRolesResponse = await KeycloakRestClient.RoleMappings
            .ListUserAvailableRealmRoleMappingsAsync(
                TestEnvironment.TestingRealm.Name,
                accessToken.AccessToken,
                TestUnAuthorizedUser.Id)
            .ConfigureAwait(false);

        // Validate that the response is not null and does not indicate an error.
        Assert.IsNotNull(getUserAvailableRolesResponse);
        Assert.IsFalse(getUserAvailableRolesResponse.IsError);

        // Validate that the response contains a list of available realm roles.
        Assert.IsNotNull(getUserAvailableRolesResponse.Response);
        Assert.IsTrue(getUserAvailableRolesResponse.Response.Any());

        // Validate monitoring metrics for the role listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            getUserAvailableRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK,
            HttpMethod.Get);

        // Filter and identify the specific role to be assigned.
        var roles = getUserAvailableRolesResponse.Response
            .Where(role => role.Name == "kc_client_role_1")
            .ToList();

        // Send a request to map the identified realm role to the authorized user.
        var mapRolesToUserResponse = await KeycloakRestClient.RoleMappings.AddUserRealmRoleMappingsAsync(
                TestEnvironment.TestingRealm.Name,
                accessToken.AccessToken,
                TestAuthorizedUser.Id,
                roles)
            .ConfigureAwait(false);

        // Validate that the role mapping response is not null and does not indicate an error.
        Assert.IsNotNull(mapRolesToUserResponse);
        Assert.IsFalse(mapRolesToUserResponse.IsError);

        // Validate that the response content is null, as expected for this type of request.
        Assert.IsNull(mapRolesToUserResponse.Response);

        // Validate monitoring metrics for the role mapping request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            mapRolesToUserResponse.MonitoringMetrics,
            HttpStatusCode.NoContent,
            HttpMethod.Post);
    }

    /// <summary>
    /// Validates that a Request Party Token (RPT) request results in a forbidden error
    /// when the required permissions are not granted.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldGetRequestPartyTokenWithForbidden()
    {
        // Get access token
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            }, new KcUserLogin
            {
                Username = TestUnAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Assert access token
        KcCommonAssertion.AssertIdentityProviderTokenResponse(tokenResponse);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Post);

        // Retrieve the first audience from the user's permissions.
        var audience = TestEnvironment.TestingRealm.User.Permissions.FirstOrDefault();

        // Validate the audience and its scopes.
        Assert.IsNotNull(audience);
        Assert.IsFalse(string.IsNullOrEmpty(audience.Name));
        Assert.IsNotNull(audience.Scopes);
        Assert.IsTrue(audience.Scopes.Any());

        // Act
        var rptResponse = await KeycloakRestClient.Auth.GetRequestPartyTokenAsync(TestEnvironment.TestingRealm.Name,
            tokenResponse.Response.AccessToken, audience.Name, audience.Scopes).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(rptResponse);
        Assert.IsTrue(rptResponse.IsError);
        Assert.IsFalse(string.IsNullOrEmpty(rptResponse.ErrorMessage));

        // Validate monitoring metrics for the forbidden request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(rptResponse.MonitoringMetrics, HttpStatusCode.Forbidden,
            HttpMethod.Post, true);
    }

    /// <summary>
    /// Verifies that a Request Party Token (RPT) can be retrieved successfully for an authorized user.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldGetRequestPartyToken()
    {
        // Retrieve an access token using resource owner password credentials.
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Assert that the access token response is valid.
        KcCommonAssertion.AssertIdentityProviderTokenResponse(tokenResponse);

        // Validate monitoring metrics for the successful token request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            tokenResponse.MonitoringMetrics, HttpStatusCode.OK, HttpMethod.Post);

        // Retrieve the first available audience from the user's permissions.
        var audience = TestEnvironment.TestingRealm.User.Permissions.FirstOrDefault();

        // Validate the audience details and ensure that the necessary scopes are present.
        Assert.IsNotNull(audience);
        Assert.IsFalse(string.IsNullOrEmpty(audience.Name));
        Assert.IsNotNull(audience.Scopes);
        Assert.IsTrue(audience.Scopes.Any());

        // Request the Request Party Token (RPT) using the audience and scopes.
        var rptResponse = await KeycloakRestClient.Auth.GetRequestPartyTokenAsync(
                TestEnvironment.TestingRealm.Name,
                tokenResponse.Response.AccessToken,
                audience.Name,
                audience.Scopes)
            .ConfigureAwait(false);

        // Assert that the RPT response is valid.
        Assert.IsNotNull(rptResponse);

        // Validate monitoring metrics for the RPT request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            rptResponse.MonitoringMetrics,
            HttpStatusCode.OK,
            HttpMethod.Post);
    }

    /// <summary>
    /// Verifies that test users are successfully deleted from the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldDeleteTestUsers()
    {
        // Ensure that both test user instances are not null.
        Assert.IsNotNull(TestAuthorizedUser, "TestAuthorizedUser must not be null.");
        Assert.IsNotNull(TestUnAuthorizedUser, "TestUnAuthorizedUser must not be null.");

        // Create a list of users to be deleted.
        var usersList = new List<KcUser>
        {
            TestAuthorizedUser,
            TestUnAuthorizedUser
        };

        // Retrieve an access token for the realm admin to perform the user deletion.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken, "Access token for realm admin must not be null.");

        // Iterate over the user list and delete each user.
        foreach ( var user in usersList )
        {
            // Execute the user deletion operation using the Keycloak REST client.
            var deleteUserResponse = await KeycloakRestClient.Users
                .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, user.Id)
                .ConfigureAwait(false);

            // Validate the deletion response.
            Assert.IsNotNull(deleteUserResponse, $"Delete response for user {user.Id} must not be null.");
            Assert.IsFalse(deleteUserResponse.IsError,
                $"Delete request for user {user.Id} should not return an error.");

            // Validate the monitoring metrics for the deletion request.
            KcCommonAssertion.AssertResponseMonitoringMetrics(deleteUserResponse.MonitoringMetrics,
                HttpStatusCode.NoContent, HttpMethod.Delete);
        }
    }
}
