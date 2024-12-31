using System.Net;
using Bogus;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcClientsTests;

/// <summary>
/// Test suite for verifying the happy path scenarios of Keycloak client management.
/// This class ensures that clients can be created, listed, retrieved, and deleted without errors.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcClientHappyPathTests : KcTestingModule
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
                    Environment.GetEnvironmentVariable($"{nameof(KcClientHappyPathTests)}_KC_CLIENT") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientHappyPathTests)}_KC_CLIENT",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak client credentials used for testing.
    /// </summary>
    private static KcCredentials TestClientSecret
    {
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientHappyPathTests)}_KC_SECRET",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak client scope used for testing.
    /// </summary>
    private static KcClientScope TestClientScope
    {
        get
        {
            try
            {
                // Retrieve and deserialize the client scope from the environment variable.
                return JsonConvert.DeserializeObject<KcClientScope>(
                    Environment.GetEnvironmentVariable($"{nameof(KcClientHappyPathTests)}_KC_CLIENT_SCOPE") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientHappyPathTests)}_KC_CLIENT_SCOPE",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the test environment and ensures that the Keycloak client module is available.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Clients);

    /// <summary>
    /// Validates that a Keycloak client can be successfully created.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreateClient()
    {
        // Retrieve the realm admin token for authentication.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);

        Assert.IsNotNull(accessToken);

        // Use Faker to generate realistic test data for the Keycloak client.
        var faker = new Faker();

        // Define the Keycloak client object with properties and attributes.
        var kClient = KcClientMocks.GenerateClient(faker);

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
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);

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
    /// Validates that a specific client can be retrieved by its ID.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldGetClientById()
    {
        Assert.IsNotNull(TestClient);

        // Retrieve the realm admin token for authentication.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);

        Assert.IsNotNull(accessToken);

        // Retrieve the client by its unique ID.
        var clientResponse = await KeycloakRestClient.Clients
            .GetAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        Assert.IsNotNull(clientResponse);
        Assert.IsFalse(clientResponse.IsError);
        Assert.IsNotNull(clientResponse.Response);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        TestClient = clientResponse.Response;
    }

    /// <summary>
    /// Validates the functionality to update a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldUpdateClient()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the update.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Update the name property of the test client.
        TestClient.Name = "UpdatedName";

        // Execute the client update operation.
        var updateClientResponse = await KeycloakRestClient.Clients
            .UpdateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, TestClient)
            .ConfigureAwait(false);

        // Validate the response from the update operation.
        Assert.IsNotNull(updateClientResponse);
        Assert.IsFalse(updateClientResponse.IsError);

        // Validate the monitoring metrics for the successful update request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateClientResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Validates the functionality to generate a new secret for a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldGenerateNewSecretForClient()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the secret generation.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to generate a new secret for the test client.
        var generateNewSecretResponse = await KeycloakRestClient.Clients
            .GenerateNewSecretAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the secret generation operation.
        Assert.IsNotNull(generateNewSecretResponse);
        Assert.IsFalse(generateNewSecretResponse.IsError);
        Assert.IsNotNull(generateNewSecretResponse.Response);

        // Validate the monitoring metrics for the successful secret generation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(generateNewSecretResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Post);

        // Update the test client secret with the newly generated secret.
        TestClientSecret = generateNewSecretResponse.Response;
    }

    /// <summary>
    /// Validates the functionality to retrieve the secret of a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldGetClientSecret()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the secret retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve the client's secret.
        var clientSecretResponse = await KeycloakRestClient.Clients
            .GetSecretAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the secret retrieval operation.
        Assert.IsNotNull(clientSecretResponse);
        Assert.IsFalse(clientSecretResponse.IsError);
        Assert.IsNotNull(clientSecretResponse.Response);

        // Validate the monitoring metrics for the successful secret retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientSecretResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the test client secret with the newly generated secret.
        TestClientSecret = clientSecretResponse.Response;
    }

    /// <summary>
    /// Validates the scenario where a rotated secret for a client is not available for a public client.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldNotGetClientRotatedSecret()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to attempt to retrieve a rotated secret.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to attempt retrieval of the client's rotated secret.
        var clientRotatedSecretResponse = await KeycloakRestClient.Clients
            .GetRotatedSecretAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response for the scenario where no rotated secret exists.
        Assert.IsNotNull(clientRotatedSecretResponse);
        Assert.IsTrue(clientRotatedSecretResponse.IsError);

        // Validate the monitoring metrics for the unsuccessful rotated secret retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientRotatedSecretResponse.MonitoringMetrics,
            HttpStatusCode.NotFound, HttpMethod.Get, true);

        // Update the test client secret with the response, if applicable (likely null).
        TestClientSecret = clientRotatedSecretResponse.Response;
    }

    /// <summary>
    /// Validates the scenario where an attempt to invalidate a rotated secret for a public client fails in the Keycloak.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldNotInvalidateClientRotatedSecret()
    {
        // Retrieve an access token for the realm admin to attempt secret invalidation.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to attempt invalidation of the client's rotated secret.
        var invalidateClientRotatedSecretResponse = await KeycloakRestClient.Clients
            .InvalidateRotatedSecretAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                Guid.NewGuid().ToString())
            .ConfigureAwait(false);

        // Validate the response for the scenario where secret invalidation fails.
        Assert.IsNotNull(invalidateClientRotatedSecretResponse);
        Assert.IsTrue(invalidateClientRotatedSecretResponse.IsError);

        // Validate the monitoring metrics for the unsuccessful rotated secret invalidation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(invalidateClientRotatedSecretResponse.MonitoringMetrics,
            HttpStatusCode.NotFound, HttpMethod.Delete, true);
    }

    /// <summary>
    /// Validates the functionality to retrieve the default scopes assigned to a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldGetClientDefaultScopes()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the default scopes retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve the default scopes for the client.
        var clientDefaultScopeResponse = await KeycloakRestClient.Clients
            .GetDefaultScopesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the default scopes retrieval operation.
        Assert.IsNotNull(clientDefaultScopeResponse);
        Assert.IsFalse(clientDefaultScopeResponse.IsError);
        Assert.IsNotNull(clientDefaultScopeResponse.Response);
        Assert.IsTrue(clientDefaultScopeResponse.Response.Any());

        // Validate that the retrieved default scopes match the expected client default scopes.
        Assert.IsFalse(clientDefaultScopeResponse.Response.Select(scope => scope.Name)
            .Except(TestClient.DefaultClientScopes).Any());

        // Validate the monitoring metrics for the successful default scopes retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientDefaultScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the test client scope with the first retrieved default scope.
        TestClientScope = clientDefaultScopeResponse.Response.First();
    }

    /// <summary>
    /// Validates the functionality to delete a default scope assigned to a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldDeleteClientDefaultScope()
    {
        // Ensure the test client and test client scope are initialized before proceeding.
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);

        // Retrieve an access token for the realm admin to perform the default scope deletion.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the specified default scope from the client.
        var deleteClientDefaultScopeResponse = await KeycloakRestClient.Clients.DeleteDefaultScopesAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, TestClientScope.Id)
            .ConfigureAwait(false);

        // Validate the response from the default scope deletion operation.
        Assert.IsNotNull(deleteClientDefaultScopeResponse);
        Assert.IsFalse(deleteClientDefaultScopeResponse.IsError);

        // Validate the monitoring metrics for the successful default scope deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteClientDefaultScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to add a default scope to a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldAddClientDefaultScope()
    {
        // Ensure the test client and test client scope are initialized before proceeding.
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);

        // Retrieve an access token for the realm admin to perform the default scope addition.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to add the specified default scope to the client.
        var addClientDefaultScopeResponse = await KeycloakRestClient.Clients.AddDefaultScopesAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, TestClientScope.Id)
            .ConfigureAwait(false);

        // Validate the response from the default scope addition operation.
        Assert.IsNotNull(addClientDefaultScopeResponse);
        Assert.IsFalse(addClientDefaultScopeResponse.IsError);

        // Validate the monitoring metrics for the successful default scope addition request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(addClientDefaultScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Validates the functionality to retrieve protocol mappers associated with a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task K_ShouldGetProtocolMappers()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the protocol mappers retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve protocol mappers associated with the client.
        var protocolMappersResponse = await KeycloakRestClient.Clients
            .GetProtocolMappersAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the protocol mappers retrieval operation.
        Assert.IsNotNull(protocolMappersResponse);
        Assert.IsFalse(protocolMappersResponse.IsError);
        Assert.IsNotNull(protocolMappersResponse.Response);
        Assert.IsTrue(protocolMappersResponse.Response.Any());

        // Validate the monitoring metrics for the successful protocol mappers retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(protocolMappersResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve authorization management permissions for a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldGetAuthorizationManagementPermission()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the authorization permissions retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve authorization management permissions for the client.
        var authorizationPermissionResponse = await KeycloakRestClient.Clients
            .GetAuthorizationManagementPermissionAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the authorization permissions retrieval operation.
        Assert.IsNotNull(authorizationPermissionResponse);
        Assert.IsFalse(authorizationPermissionResponse.IsError);
        Assert.IsNotNull(authorizationPermissionResponse.Response);

        // Validate that authorization management permissions are not enabled for a public client.
        Assert.IsFalse(authorizationPermissionResponse.Response.Enabled);

        // Validate the monitoring metrics for the successful authorization permissions retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(authorizationPermissionResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the scenario where setting authorization management permissions for a public client is not allowed in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldNotSetAuthorizationManagementPermissions()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the authorization permissions setting attempt.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Attempt to set authorization management permissions for the test client.
        var setClientAuthorizationPermissionManagementResponse = await KeycloakRestClient.Clients
            .SetAuthorizationManagementPermissionAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClient.Id, new KcPermissionManagement
                {
                    Enabled = true,
                    Resource = "Profile"
                })
            .ConfigureAwait(false);

        // Validate the response for the scenario where the setting of permissions fails for a public client.
        Assert.IsNotNull(setClientAuthorizationPermissionManagementResponse);

        // Ensure the response indicates an error.
        Assert.IsTrue(setClientAuthorizationPermissionManagementResponse.IsError);

        // Validate the monitoring metrics for the unsuccessful authorization permissions setting request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            setClientAuthorizationPermissionManagementResponse.MonitoringMetrics,
            HttpStatusCode.InternalServerError, HttpMethod.Put, true);
    }

    /// <summary>
    /// Validates the functionality to count offline sessions associated with a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Q_ShouldCountOfflineSessions()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the offline sessions count.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to count offline sessions associated with the client.
        var offlineSessionsCountResponse = await KeycloakRestClient.Clients
            .CountOfflineSessionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the offline sessions count operation.
        Assert.IsNotNull(offlineSessionsCountResponse);
        Assert.IsFalse(offlineSessionsCountResponse.IsError);
        Assert.IsNotNull(offlineSessionsCountResponse.Response);

        // Ensure that the count of offline sessions matches the expected value (0).
        Assert.AreEqual(offlineSessionsCountResponse.Response.Count, 0);

        // Validate the monitoring metrics for the successful offline sessions count request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(offlineSessionsCountResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve offline sessions associated with a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task R_ShouldGetOfflineSessions()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the offline sessions retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve offline sessions associated with the client.
        var offlineSessionsResponse = await KeycloakRestClient.Clients
            .GetOfflineSessionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the offline sessions retrieval operation.
        Assert.IsNotNull(offlineSessionsResponse);
        Assert.IsFalse(offlineSessionsResponse.IsError);
        Assert.IsNotNull(offlineSessionsResponse.Response);

        // Ensure that there are no active offline sessions associated with the client.
        Assert.IsFalse(offlineSessionsResponse.Response.Any());

        // Validate the monitoring metrics for the successful offline sessions retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(offlineSessionsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve user sessions associated with a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task V_ShouldGetUsersSessions()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the user sessions retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve user sessions associated with the client.
        var usersSessionsResponse = await KeycloakRestClient.Clients
            .GetUsersSessionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the user sessions retrieval operation.
        Assert.IsNotNull(usersSessionsResponse);
        Assert.IsFalse(usersSessionsResponse.IsError);
        Assert.IsNotNull(usersSessionsResponse.Response);

        // Ensure that there are no active user sessions associated with the client.
        Assert.IsFalse(usersSessionsResponse.Response.Any());

        // Validate the monitoring metrics for the successful user sessions retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(usersSessionsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve optional scopes associated with a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task W_ShouldGetOptionalScopes()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the optional scopes retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve the optional scopes for the client.
        var optionalScopesResponse = await KeycloakRestClient.Clients
            .GetOptionalScopesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the optional scopes retrieval operation.
        Assert.IsNotNull(optionalScopesResponse);
        Assert.IsFalse(optionalScopesResponse.IsError);
        Assert.IsNotNull(optionalScopesResponse.Response);
        Assert.IsTrue(optionalScopesResponse.Response.Any());

        // Validate that the retrieved optional scopes match the expected client optional scopes.
        Assert.IsFalse(optionalScopesResponse.Response.Select(scope => scope.Name)
            .Except(TestClient.OptionalClientScopes).Any());

        // Validate the monitoring metrics for the successful optional scopes retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(optionalScopesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the test client scope with the first retrieved optional scope.
        TestClientScope = optionalScopesResponse.Response.First();
    }

    /// <summary>
    /// Validates the functionality to delete an optional scope from a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task X_ShouldDeleteOptionalScope()
    {
        // Ensure the test client and test client scope are initialized before proceeding.
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);

        // Retrieve an access token for the realm admin to perform the optional scope deletion.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the specified optional scope from the client.
        var deleteOptionalScopeResponse = await KeycloakRestClient.Clients
            .DeleteOptionalScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestClientScope.Id)
            .ConfigureAwait(false);

        // Validate the response from the optional scope deletion operation.
        Assert.IsNotNull(deleteOptionalScopeResponse);
        Assert.IsFalse(deleteOptionalScopeResponse.IsError);

        // Validate the monitoring metrics for the successful optional scope deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteOptionalScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to add an optional scope to a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Y_ShouldAddOptionalScope()
    {
        // Ensure the test client and test client scope are initialized before proceeding.
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);

        // Retrieve an access token for the realm admin to perform the optional scope addition.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to add the specified optional scope to the client.
        var addOptionalScopeResponse = await KeycloakRestClient.Clients
            .AddOptionalScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestClientScope.Id)
            .ConfigureAwait(false);

        // Validate the response from the optional scope addition operation.
        Assert.IsNotNull(addOptionalScopeResponse);
        Assert.IsFalse(addOptionalScopeResponse.IsError);

        // Validate the monitoring metrics for the successful optional scope addition request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(addOptionalScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Validates the functionality to retrieve a registration access token for a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldGetRegistrationAccessToken()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the registration access token retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve the registration access token for the client.
        var registrationAccessTokenResponse = await KeycloakRestClient.Clients
            .GetRegistrationAccessTokenAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the registration access token retrieval operation.
        Assert.IsNotNull(registrationAccessTokenResponse);
        Assert.IsFalse(registrationAccessTokenResponse.IsError);
        Assert.IsNotNull(registrationAccessTokenResponse.Response);

        // Ensure the response is of the expected type.
        Assert.IsInstanceOfType<KcClient>(registrationAccessTokenResponse.Response);

        // Validate the monitoring metrics for the successful registration access token retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(registrationAccessTokenResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Post);
    }

    /// <summary>
    /// Validates the functionality to count active sessions associated with a client in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZA_ShouldCountSessions()
    {
        // Ensure the test client is initialized before proceeding.
        Assert.IsNotNull(TestClient);

        // Retrieve an access token for the realm admin to perform the session count retrieval.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to count active sessions associated with the client.
        var sessionCountResponse = await KeycloakRestClient.Clients
            .CountSessionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id)
            .ConfigureAwait(false);

        // Validate the response from the session count operation.
        Assert.IsNotNull(sessionCountResponse);
        Assert.IsFalse(sessionCountResponse.IsError);
        Assert.IsNotNull(sessionCountResponse.Response);

        // Ensure that the count of active sessions matches the expected value (0).
        Assert.AreEqual(sessionCountResponse.Response.Count, 0);

        // Validate the monitoring metrics for the successful session count retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(sessionCountResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates that a specific client can be deleted by its ID.
    /// </summary>
    [TestMethod]
    public async Task ZB_ShouldDeleteClient()
    {
        Assert.IsNotNull(TestClient);

        // Retrieve the realm admin token for authentication.
        var accessToken = await GetRealmAdminToken(TestContext).ConfigureAwait(false);

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
