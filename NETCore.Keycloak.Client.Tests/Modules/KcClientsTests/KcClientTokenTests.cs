using System.Net;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcClientsTests;

/// <summary>
/// Contains tests for validating the Keycloak client tokens API functionalities under expected scenarios.
/// </summary>
[TestClass]
[TestCategory("Combined")]
public class KcClientTokenTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// Used for consistent naming and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Holds the current state of the Keycloak client being tested.
    /// This property serializes/deserializes the client information to/from an environment variable.
    /// </summary>
    private static KcClient TestClient
    {
        get
        {
            try
            {
                // Deserialize the Keycloak client information from an environment variable.
                return JsonConvert.DeserializeObject<KcClient>(
                    Environment.GetEnvironmentVariable($"{nameof(KcClientTokenTests)}_KC_CLIENT") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientTokenTests)}_KC_CLIENT",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak user used for testing.
    /// </summary>
    private static KcUser TestUser
    {
        get
        {
            try
            {
                // Retrieve and deserialize the user object from the environment variable.
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable($"{nameof(KcClientTokenTests)}_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientTokenTests)}_KCUSER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the Keycloak REST client components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        Assert.IsNotNull(KeycloakRestClient.Users);
        Assert.IsNotNull(KeycloakRestClient.Clients);
    }

    /// <summary>
    /// Tests the creation of a Keycloak realm user and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task A_CreateUser() => TestUser = await CreateAndGetRealmUserAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a Keycloak client and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task B_CreateClient() => TestClient = await CreateAndGetClientAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the generation of an example access token for a Keycloak user and client.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldGenerateExampleAccessToken()
    {
        // Ensure that the test user and client exist
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to generate an example access token for the user and client
        var generateExampleAccessTokenResponse = await KeycloakRestClient.Clients
            .GenerateExampleAccessTokenAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestUser.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(generateExampleAccessTokenResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(generateExampleAccessTokenResponse.IsError);

        // Ensure that the response contains the generated access token
        Assert.IsNotNull(generateExampleAccessTokenResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(generateExampleAccessTokenResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests the generation of an example ID token for a Keycloak user and client.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldGenerateExampleIdToken()
    {
        // Ensure that the test user and client exist
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to generate an example ID token for the user and client
        var generateExampleIdTokenResponse = await KeycloakRestClient.Clients
            .GenerateExampleIdTokenAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestUser.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(generateExampleIdTokenResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(generateExampleIdTokenResponse.IsError);

        // Ensure that the response contains the generated ID token
        Assert.IsNotNull(generateExampleIdTokenResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(generateExampleIdTokenResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests the generation of example user information for a Keycloak user and client.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldGenerateExampleUserInfo()
    {
        // Ensure that the test user and client exist
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to generate example user information for the user and client
        var generateExampleExampleUserInfoResponse = await KeycloakRestClient.Clients
            .GenerateExampleUserInfoAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestUser.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(generateExampleExampleUserInfoResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(generateExampleExampleUserInfoResponse.IsError);

        // Ensure that the response contains the generated user information
        Assert.IsNotNull(generateExampleExampleUserInfoResponse.Response);

        // Ensure that the response contains at least one user information entry
        Assert.IsTrue(generateExampleExampleUserInfoResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(generateExampleExampleUserInfoResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
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
    /// Validates that a specific client can be deleted by its ID.
    /// </summary>
    [TestMethod]
    public async Task ZA_ShouldDeleteClient()
    {
        Assert.IsNotNull(TestClient);

        // Retrieve the realm admin token for authentication.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        Assert.IsNotNull(accessToken);

        // Delete the client by its unique ID.
        var clientResponse = await KeycloakRestClient.Clients
            .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        Assert.IsNotNull(clientResponse);
        Assert.IsFalse(clientResponse.IsError);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientResponse.MonitoringMetrics, HttpStatusCode.NoContent,
            HttpMethod.Delete);
    }
}
