using System.Globalization;
using System.Net;
using Bogus;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcProtocolMapperTests;

/// <summary>
/// Contains tests for validating the Keycloak protocol mapper API functionalities under expected scenarios.
/// </summary>
[TestClass]
[TestCategory("Combined")]
public class KcProtocolMapperTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak protocol mapper used for testing in happy path scenarios.
    /// </summary>
    private static KcProtocolMapper TestProtocolMapper
    {
        get
        {
            try
            {
                // Retrieve and deserialize the protocol mapper object from the environment variable.
                return JsonConvert.DeserializeObject<KcProtocolMapper>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcProtocolMapperTests)}_KC_PROTOCOL_MAPPER") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcProtocolMapperTests)}_KC_PROTOCOL_MAPPER",
            JsonConvert.SerializeObject(value));
    }

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
                    Environment.GetEnvironmentVariable($"{nameof(KcProtocolMapperTests)}_KC_CLIENT") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcProtocolMapperTests)}_KC_CLIENT",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak client scope used for testing in happy path scenarios.
    /// </summary>
    private static KcClientScope TestClientScope
    {
        get
        {
            try
            {
                // Retrieve and deserialize the client scope object from the environment variable.
                return JsonConvert.DeserializeObject<KcClientScope>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcProtocolMapperTests)}_KC_CLIENT_SCOPE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcProtocolMapperTests)}_KC_CLIENT_SCOPE",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak protocol mapper module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.ProtocolMappers);

    /// <summary>
    /// Tests the creation of a Keycloak client scope and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task A_CreateClientScope() =>
        TestClientScope = await CreateAndGetClientScopeAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the addition of protocol mappers to an existing Keycloak client scope.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldAddProtocolMappersToClientScope()
    {
        // Ensure that the test client scope exists
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Generate protocol mappers for testing
        var protocolMappers = KcProtocolMapperMocks.Generate(2);

        // Send the request to add protocol mappers to the client scope
        var addProtocolMappersToClientScopeResponse = await KeycloakRestClient.ProtocolMappers.AddMappersAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id, protocolMappers)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addProtocolMappersToClientScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addProtocolMappersToClientScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addProtocolMappersToClientScopeResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addProtocolMappersToClientScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests that attempting to add an empty list of protocol mappers to a Keycloak
    /// </summary>
    [TestMethod]
    public async Task BA_ShouldNotAddEmptyProtocolMappersToClientScope()
    {
        // Ensure that the test client scope exists
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to add an empty list of protocol mappers to the client scope
        var addProtocolMappersToClientScopeResponse = await KeycloakRestClient.ProtocolMappers.AddMappersAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id,
            new List<KcProtocolMapper>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addProtocolMappersToClientScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addProtocolMappersToClientScopeResponse.IsError);

        // Ensure that the response content is null as expected
        Assert.IsNull(addProtocolMappersToClientScopeResponse.Response);

        // Ensure that monitoring metrics are not returned for the error response
        Assert.IsNull(addProtocolMappersToClientScopeResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests the addition of a single protocol mapper to an existing Keycloak client scope.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldAddProtocolMapperToClientScope()
    {
        // Ensure that the test client scope exists
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Generate a single protocol mapper for testing
        var protocolMapper = KcProtocolMapperMocks.Generate();

        // Assign the protocol mapper to the test variable for further validation
        TestProtocolMapper = protocolMapper;

        // Send the request to add the protocol mapper to the client scope
        var addProtocolMapperToClientScopeResponse = await KeycloakRestClient.ProtocolMappers.AddMapperAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id, protocolMapper)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addProtocolMapperToClientScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addProtocolMapperToClientScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addProtocolMapperToClientScopeResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addProtocolMapperToClientScopeResponse.MonitoringMetrics,
            HttpStatusCode.Created, HttpMethod.Post);
    }

    /// <summary>
    /// Tests listing all protocol mappers associated with an existing Keycloak client scope.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldListProtocolMappersOfClientScope()
    {
        // Ensure that the test client scope and protocol mapper exist
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list all protocol mappers associated with the client scope
        var listClientScopeProtocolMappersResponse = await KeycloakRestClient.ProtocolMappers
            .ListMappersAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(listClientScopeProtocolMappersResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(listClientScopeProtocolMappersResponse.IsError);

        // Ensure that the response contains a list of protocol mappers
        Assert.IsNotNull(listClientScopeProtocolMappersResponse.Response);

        // Verify that the expected number of protocol mappers is correct
        Assert.IsTrue(listClientScopeProtocolMappersResponse.Response.Count() == 3);

        // Verify that the test protocol mapper is present in the list
        Assert.IsTrue(
            listClientScopeProtocolMappersResponse.Response.Any(mapper => mapper.Name == TestProtocolMapper.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientScopeProtocolMappersResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Assign the retrieved protocol mapper to the test variable for further validation
        TestProtocolMapper =
            listClientScopeProtocolMappersResponse.Response.First(mapper => mapper.Name == TestProtocolMapper.Name);
    }

    /// <summary>
    /// Tests retrieving a specific protocol mapper associated with a Keycloak client scope.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldGetProtocolMappersOfClientScope()
    {
        // Ensure that the test client scope and protocol mapper exist
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get the specific protocol mapper associated with the client scope
        var getClientScopeProtocolMapperResponse = await KeycloakRestClient.ProtocolMappers
            .GetMapperAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id,
                TestProtocolMapper.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(getClientScopeProtocolMapperResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(getClientScopeProtocolMapperResponse.IsError);

        // Ensure that the response contains the expected protocol mapper
        Assert.IsNotNull(getClientScopeProtocolMapperResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getClientScopeProtocolMapperResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Assign the retrieved protocol mapper to the test variable for further validation
        TestProtocolMapper = getClientScopeProtocolMapperResponse.Response;
    }

    /// <summary>
    /// Tests updating a protocol mapper associated with a Keycloak client scope.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldUpdateProtocolMappersOfClientScope()
    {
        // Ensure that the test client scope and protocol mapper exist
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Create a Faker instance to generate a random word for the protocol mapper name
        var faker = new Faker();

        // Update the protocol mapper name
        var protocolMapper = TestProtocolMapper;
        protocolMapper.Name = faker.Random.Word().ToLower(CultureInfo.CurrentCulture)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        // Send the request to update the protocol mapper associated with the client scope
        var updateClientScopeProtocolMapperResponse = await KeycloakRestClient.ProtocolMappers
            .UpdateMapperAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id,
                protocolMapper.Id, protocolMapper).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(updateClientScopeProtocolMapperResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(updateClientScopeProtocolMapperResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(updateClientScopeProtocolMapperResponse.Response);

        // Validate monitoring metrics for the update request
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateClientScopeProtocolMapperResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Tests deleting a protocol mapper associated with a Keycloak client scope.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldDeleteProtocolMappersOfClientScope()
    {
        // Ensure that the test client scope and protocol mapper exist
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the protocol mapper associated with the client scope
        var deleteClientScopeProtocolMapperResponse = await KeycloakRestClient.ProtocolMappers
            .DeleteMapperAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id,
                TestProtocolMapper.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteClientScopeProtocolMapperResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteClientScopeProtocolMapperResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteClientScopeProtocolMapperResponse.Response);

        // Validate monitoring metrics for the delete request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteClientScopeProtocolMapperResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);

        // Set the test protocol mapper to null after successful deletion
        TestProtocolMapper = null;
    }

    /// <summary>
    /// Tests listing protocol mappers associated with a Keycloak client scope filtered by protocol name.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldListProtocolMappersOfClientScopeByProtocolName()
    {
        // Ensure that the test client scope exists
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list protocol mappers filtered by protocol name
        var listProtocolMappersOfClientScopeByProtocolNameResponse = await KeycloakRestClient.ProtocolMappers
            .ListMappersByProtocolNameAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, KcProtocol.OpenidConnect).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(listProtocolMappersOfClientScopeByProtocolNameResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(listProtocolMappersOfClientScopeByProtocolNameResponse.IsError);

        // Ensure that the response contains a list of protocol mappers
        Assert.IsNotNull(listProtocolMappersOfClientScopeByProtocolNameResponse.Response);

        // Verify that the expected number of protocol mappers is correct
        Assert.IsTrue(listProtocolMappersOfClientScopeByProtocolNameResponse.Response.Count() == 2);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            listProtocolMappersOfClientScopeByProtocolNameResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Tests the creation of a Keycloak client and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task I_CreateClient() => TestClient = await CreateAndGetClientAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the addition of protocol mappers to an existing Keycloak client.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldAddProtocolMappersToClient()
    {
        // Ensure that the test client exists
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Generate protocol mappers for testing
        var protocolMappers = KcProtocolMapperMocks.Generate(2);

        // Send the request to add protocol mappers to the client
        var addProtocolMappersToClientResponse = await KeycloakRestClient.ProtocolMappers.AddClientMappersAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, protocolMappers)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addProtocolMappersToClientResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addProtocolMappersToClientResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addProtocolMappersToClientResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addProtocolMappersToClientResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests that attempting to add an empty list of protocol mappers to a Keycloak
    /// </summary>
    [TestMethod]
    public async Task JA_ShouldNotAddEmptyProtocolMappersToClient()
    {
        // Ensure that the test client exists
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to add an empty list of protocol mappers to the client
        var addProtocolMappersToClientResponse = await KeycloakRestClient.ProtocolMappers.AddClientMappersAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
            new List<KcProtocolMapper>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addProtocolMappersToClientResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addProtocolMappersToClientResponse.IsError);

        // Ensure that the response content is null as expected
        Assert.IsNull(addProtocolMappersToClientResponse.Response);

        // Ensure that monitoring metrics are not returned for the error response
        Assert.IsNull(addProtocolMappersToClientResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests the addition of a single protocol mapper to an existing Keycloak client.
    /// </summary>
    [TestMethod]
    public async Task K_ShouldAddProtocolMapperToClient()
    {
        // Ensure that the test client exists
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Generate a single protocol mapper for testing
        var protocolMapper = KcProtocolMapperMocks.Generate();

        // Assign the protocol mapper to the test variable for further validation
        TestProtocolMapper = protocolMapper;

        // Send the request to add the protocol mapper to the client
        var addProtocolMapperToClientResponse = await KeycloakRestClient.ProtocolMappers.AddClientMapperAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, protocolMapper)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addProtocolMapperToClientResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addProtocolMapperToClientResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addProtocolMapperToClientResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addProtocolMapperToClientResponse.MonitoringMetrics,
            HttpStatusCode.Created, HttpMethod.Post);
    }

    /// <summary>
    /// Tests listing all protocol mappers associated with an existing Keycloak client.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldListProtocolMappersOfClient()
    {
        // Ensure that the test client and protocol mapper exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list all protocol mappers associated with the client
        var listClientProtocolMappersResponse = await KeycloakRestClient.ProtocolMappers
            .ListClientMappersAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(listClientProtocolMappersResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(listClientProtocolMappersResponse.IsError);

        // Ensure that the response contains a list of protocol mappers
        Assert.IsNotNull(listClientProtocolMappersResponse.Response);

        // Verify that the expected number of protocol mappers is correct
        Assert.IsTrue(listClientProtocolMappersResponse.Response.Count() == 3);

        // Verify that the test protocol mapper is present in the list
        Assert.IsTrue(
            listClientProtocolMappersResponse.Response.Any(mapper => mapper.Name == TestProtocolMapper.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientProtocolMappersResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Assign the retrieved protocol mapper to the test variable for further validation
        TestProtocolMapper =
            listClientProtocolMappersResponse.Response.First(mapper => mapper.Name == TestProtocolMapper.Name);
    }

    /// <summary>
    /// Tests retrieving a specific protocol mapper associated with an existing Keycloak client.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldGetProtocolMappersOfClient()
    {
        // Ensure that the test client and protocol mapper exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get the specific protocol mapper associated with the client
        var getClientProtocolMapperResponse = await KeycloakRestClient.ProtocolMappers
            .GetClientMapperAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestProtocolMapper.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(getClientProtocolMapperResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(getClientProtocolMapperResponse.IsError);

        // Ensure that the response contains the expected protocol mapper
        Assert.IsNotNull(getClientProtocolMapperResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getClientProtocolMapperResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Assign the retrieved protocol mapper to the test variable for further validation
        TestProtocolMapper = getClientProtocolMapperResponse.Response;
    }

    /// <summary>
    /// Tests updating a protocol mapper associated with an existing Keycloak client.
    /// </summary>
    [TestMethod]
    public async Task O_ShouldUpdateProtocolMappersOfClient()
    {
        // Ensure that the test client and protocol mapper exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Create a Faker instance to generate a random word for the protocol mapper name
        var faker = new Faker();

        // Update the protocol mapper name
        var protocolMapper = TestProtocolMapper;
        protocolMapper.Name = faker.Random.Word().ToLower(CultureInfo.CurrentCulture)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        // Send the request to update the protocol mapper associated with the client
        var updateClientProtocolMapperResponse = await KeycloakRestClient.ProtocolMappers
            .UpdateClientMapperAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                protocolMapper.Id, protocolMapper).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(updateClientProtocolMapperResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(updateClientProtocolMapperResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(updateClientProtocolMapperResponse.Response);

        // Validate monitoring metrics for the update request
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateClientProtocolMapperResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Tests deleting a protocol mapper associated with an existing Keycloak client.
    /// </summary>
    [TestMethod]
    public async Task P_ShouldDeleteProtocolMappersOfClient()
    {
        // Ensure that the test client and protocol mapper exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestProtocolMapper);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the protocol mapper associated with the client
        var deleteClientProtocolMapperResponse = await KeycloakRestClient.ProtocolMappers
            .DeleteClientMapperAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestProtocolMapper.Id).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteClientProtocolMapperResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteClientProtocolMapperResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteClientProtocolMapperResponse.Response);

        // Validate monitoring metrics for the delete request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteClientProtocolMapperResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);

        // Set the test protocol mapper to null after successful deletion
        TestProtocolMapper = null;
    }

    /// <summary>
    /// Tests listing protocol mappers associated with an existing Keycloak client filtered by protocol name.
    /// </summary>
    [TestMethod]
    public async Task Q_ShouldListProtocolMappersOfClientByProtocolName()
    {
        // Ensure that the test client exists
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list protocol mappers filtered by protocol name
        var listProtocolMappersOfClientByProtocolNameResponse = await KeycloakRestClient.ProtocolMappers
            .ListClientMappersByProtocolNameAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClient.Id, KcProtocol.OpenidConnect).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(listProtocolMappersOfClientByProtocolNameResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(listProtocolMappersOfClientByProtocolNameResponse.IsError);

        // Ensure that the response contains a list of protocol mappers
        Assert.IsNotNull(listProtocolMappersOfClientByProtocolNameResponse.Response);

        // Verify that the expected number of protocol mappers is correct
        Assert.IsTrue(listProtocolMappersOfClientByProtocolNameResponse.Response.Count() == 2);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            listProtocolMappersOfClientByProtocolNameResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Verifies that a client scope can be successfully deleted from the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldDeleteClientScope()
    {
        // Ensure that the test client scope is not null
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the client scope
        var deleteClientScopeTest = await KeycloakRestClient.ClientScopes
            .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(deleteClientScopeTest);

        // Assert that the response does not indicate an error
        Assert.IsFalse(deleteClientScopeTest.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteClientScopeTest.MonitoringMetrics,
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
