using System.Net;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcClientsTests;

/// <summary>
/// Contains tests for Keycloak service account operations related to clients.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcClientServiceAccountTests : KcTestingModule
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
                    Environment.GetEnvironmentVariable($"{nameof(KcClientServiceAccountTests)}_KC_CLIENT") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientServiceAccountTests)}_KC_CLIENT",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the test environment and ensures that the Keycloak client module is available.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Clients);

    /// <summary>
    /// Tests the retrieval of a predefined private client from the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldGetPredefinedPrivateClient()
    {
        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // List clients matching the created client's name
        var clientsResponse = await KeycloakRestClient.Clients
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcClientFilter
            {
                Search = TestEnvironment.TestingRealm.PrivateClient.ClientId
            }).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(clientsResponse);
        Assert.IsFalse(clientsResponse.IsError);
        Assert.IsNotNull(clientsResponse.Response);

        // Ensure that the client matching the specified ClientId is present in the response
        Assert.IsTrue(clientsResponse.Response.Any(client =>
            client.ClientId == TestEnvironment.TestingRealm.PrivateClient.ClientId));

        // Validate monitoring metrics for the successful request
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientsResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Assign the retrieved client to the TestClient variable
        TestClient = clientsResponse.Response.FirstOrDefault(client =>
            client.ClientId == TestEnvironment.TestingRealm.PrivateClient.ClientId);
    }

    /// <summary>
    /// Tests the retrieval of the service account user associated with a predefined private client.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldGetPredefinedPrivateClientServiceAccount()
    {
        // Ensure that the test client exists
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get the service account user for the client
        var getClientServiceAccountResponse = await KeycloakRestClient.Clients
            .GetServiceAccountUserAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(getClientServiceAccountResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(getClientServiceAccountResponse.IsError);

        // Ensure that the response contains the service account user information
        Assert.IsNotNull(getClientServiceAccountResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getClientServiceAccountResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }
}
