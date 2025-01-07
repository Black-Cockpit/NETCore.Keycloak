using System.Net;
using Bogus;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcClientsTests;

/// <summary>
/// Contains tests for Keycloak private client operations.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcPrivateClientTests : KcTestingModule
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
                    Environment.GetEnvironmentVariable($"{nameof(KcPrivateClientTests)}_KC_CLIENT") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcPrivateClientTests)}_KC_CLIENT",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Validates that a Keycloak private client can be successfully created.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreatePrivateClient()
    {
        // Retrieve the realm admin token for authentication.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        Assert.IsNotNull(accessToken);

        // Use Faker to generate realistic test data for the Keycloak client.
        var faker = new Faker();

        // Define the Keycloak client object with properties and attributes.
        var kClient = KcClientMocks.GeneratePrivateClient(faker);

        Assert.IsNotNull(kClient);

        // Create the Keycloak client via the REST API.
        var clientResponse = await KeycloakRestClient.Clients
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, kClient).ConfigureAwait(false);

        Assert.IsNotNull(clientResponse);
        Assert.IsFalse(clientResponse.IsError);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);

        TestClient = kClient;
    }

    /// <summary>
    /// Validates that a list of clients can be retrieved and the created client is present in the list.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldListClients()
    {
        Assert.IsNotNull(TestClient);

        // Retrieve the realm admin token for authentication.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        Assert.IsNotNull(accessToken);

        // List clients matching the created client's name.
        var clientsResponse = await KeycloakRestClient.Clients
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcClientFilter
            {
                Search = TestClient.Name
            }).ConfigureAwait(false);

        Assert.IsNotNull(clientsResponse);
        Assert.IsFalse(clientsResponse.IsError);
        Assert.IsNotNull(clientsResponse.Response);
        Assert.IsTrue(clientsResponse.Response.Any(client => client.ClientId == TestClient.ClientId));

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientsResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        TestClient = clientsResponse.Response.FirstOrDefault(client => client.ClientId == TestClient.ClientId);
    }

    /// <summary>
    /// Validates that a specific client can be deleted by its ID.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldDeleteClient()
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
