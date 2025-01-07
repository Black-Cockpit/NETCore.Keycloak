using System.Net;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Contains tests for validating the Keycloak scope mapping API functionalities under expected scenarios.
/// </summary>
[TestClass]
[TestCategory("Final")]
public class KcScopeMappingTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak client scope used for testing.
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
                        $"{nameof(KcScopeMappingTests)}_KC_CLIENT_SCOPE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcScopeMappingTests)}_KC_CLIENT_SCOPE",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Holds the current state of the Keycloak role being tested.
    /// This property serializes/deserializes the role information to/from an environment variable.
    /// </summary>
    private static KcRole TestRole
    {
        get
        {
            try
            {
                // Retrieve and deserialize the role object from the environment variable.
                return JsonConvert.DeserializeObject<KcRole>(
                    Environment.GetEnvironmentVariable($"{nameof(KcScopeMappingTests)}_KC_ROLE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcScopeMappingTests)}_KC_ROLE",
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
                    Environment.GetEnvironmentVariable($"{nameof(KcScopeMappingTests)}_KC_CLIENT") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcScopeMappingTests)}_KC_CLIENT",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the Keycloak REST client components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        Assert.IsNotNull(KeycloakRestClient.ScopeMappings);
        Assert.IsNotNull(KeycloakRestClient.ClientScopes);
        Assert.IsNotNull(KeycloakRestClient.Roles);
        Assert.IsNotNull(KeycloakRestClient.Clients);
    }

    /// <summary>
    /// Tests the creation of a Keycloak client and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task A_CreateClient() => TestClient = await CreateAndGetClientAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a Keycloak client scope and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task B_CreateClientScope() =>
        TestClientScope = await CreateAndGetClientScopeAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a Keycloak client role and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task C_CreateClientRole() =>
        TestRole = await CreateAndGetClientRoleAsync(TestContext, TestClient).ConfigureAwait(false);

    /// <summary>
    /// Tests adding client roles to a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldAddClientRolesToScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map the specified client role to the client scope
        var addClientRoleToScopeResponse = await KeycloakRestClient.ScopeMappings.AddClientRolesToScopeAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id, TestClient.Id, [
                TestRole
            ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addClientRoleToScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addClientRoleToScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addClientRoleToScopeResponse.Response);

        // Validate monitoring metrics for the role mapping request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addClientRoleToScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests adding an empty collection of client roles to a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldAddEmptyClientRolesCollectionToScope()
    {
        // Ensure that the test client and client scope exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map an empty collection of client roles to the client scope
        var addClientRoleToScopeResponse = await KeycloakRestClient.ScopeMappings.AddClientRolesToScopeAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id, TestClient.Id,
            new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addClientRoleToScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addClientRoleToScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addClientRoleToScopeResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were added
        Assert.IsNull(addClientRoleToScopeResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests listing the client roles associated with a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldListClientRolesAssociatedToScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list client roles associated with the client scope
        var listClientRolesAssociatedToScopeResponse = await KeycloakRestClient.ScopeMappings
            .ListClientRolesAssociatedToScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listClientRolesAssociatedToScopeResponse);
        Assert.IsFalse(listClientRolesAssociatedToScopeResponse.IsError);

        // Ensure that the response contains a list of associated client roles
        Assert.IsNotNull(listClientRolesAssociatedToScopeResponse.Response);

        // Verify that the expected role is present in the list of associated client roles
        Assert.IsTrue(listClientRolesAssociatedToScopeResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientRolesAssociatedToScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests removing client roles from a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldRemoveClientRolesFromScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to remove the specified client role from the client scope
        var removeClientRolesFromScopeResponse = await KeycloakRestClient.ScopeMappings
            .RemoveClientRolesFromScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, TestClient.Id, [
                    TestRole
                ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(removeClientRolesFromScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(removeClientRolesFromScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(removeClientRolesFromScopeResponse.Response);

        // Validate monitoring metrics for the role removal request
        KcCommonAssertion.AssertResponseMonitoringMetrics(removeClientRolesFromScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests that attempting to remove an empty collection of client roles
    /// from a client scope in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldNotRemoveEmptyClientClientRolesCollectionFromScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to remove an empty collection of client roles from the client scope
        var removeClientRolesFromScopeResponse = await KeycloakRestClient.ScopeMappings
            .RemoveClientRolesFromScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, TestClient.Id, new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(removeClientRolesFromScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(removeClientRolesFromScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(removeClientRolesFromScopeResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were removed
        Assert.IsNull(removeClientRolesFromScopeResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests listing the client roles available for a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldListClientRolesAvailableForScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list client roles available for assignment to the client scope
        var listClientRolesAvailableForScopeResponse = await KeycloakRestClient.ScopeMappings
            .ListClientRolesAvailableForScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listClientRolesAvailableForScopeResponse);
        Assert.IsFalse(listClientRolesAvailableForScopeResponse.IsError);

        // Ensure that the response contains a list of available client roles
        Assert.IsNotNull(listClientRolesAvailableForScopeResponse.Response);

        // Verify that the expected role is present in the list of available client roles
        Assert.IsTrue(listClientRolesAvailableForScopeResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientRolesAvailableForScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests listing the composite client roles associated with a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldListCompositeClientRolesAssociatedToScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list composite client roles associated with the client scope
        var listCompositeClientRolesAssociatedToScopeResponse = await KeycloakRestClient.ScopeMappings
            .ListCompositeClientRolesAssociatedToScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listCompositeClientRolesAssociatedToScopeResponse);
        Assert.IsFalse(listCompositeClientRolesAssociatedToScopeResponse.IsError);

        // Ensure that the response contains a list of composite client roles
        Assert.IsNotNull(listCompositeClientRolesAssociatedToScopeResponse.Response);

        // Verify that the list of composite client roles is empty
        Assert.IsFalse(listCompositeClientRolesAssociatedToScopeResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            listCompositeClientRolesAssociatedToScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting a client role from the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldDeleteClientRole()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the client role
        var deleteClientRoleResponse = await KeycloakRestClient.Roles
            .DeleteClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClient.Id, TestRole.Name).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteClientRoleResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteClientRoleResponse.IsError);

        // Validate monitoring metrics for the delete request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteClientRoleResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests the creation of a realm role in the Keycloak realm and assigns it to the <see cref="TestRole"/> property.
    /// </summary>
    [TestMethod]
    public async Task K_CreateRole() =>
        TestRole = await CreateAndGetRealmRoleAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests adding realm roles to a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldAddRolesToScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map the specified realm role to the client scope
        var addRolesToScopeResponse = await KeycloakRestClient.ScopeMappings.AddRolesToScopeAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id, [
                TestRole
            ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addRolesToScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addRolesToScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addRolesToScopeResponse.Response);

        // Validate monitoring metrics for the role mapping request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addRolesToScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests that attempting to add an empty collection of realm roles to a client
    /// scope in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldNotAddEmptyRolesCollectionToScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map an empty collection of realm roles to the client scope
        var addRolesToScopeResponse = await KeycloakRestClient.ScopeMappings.AddRolesToScopeAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id, new List<KcRole>())
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(addRolesToScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(addRolesToScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(addRolesToScopeResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were added
        Assert.IsNull(addRolesToScopeResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests listing the realm roles associated with a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task O_ShouldListRolesAssociatedToScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list realm roles associated with the client scope
        var listRolesAssociatedToScopeResponse = await KeycloakRestClient.ScopeMappings
            .ListRolesAssociatedToScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listRolesAssociatedToScopeResponse);
        Assert.IsFalse(listRolesAssociatedToScopeResponse.IsError);

        // Ensure that the response contains a list of associated roles
        Assert.IsNotNull(listRolesAssociatedToScopeResponse.Response);

        // Verify that the expected role is present in the list of associated roles
        Assert.IsTrue(listRolesAssociatedToScopeResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listRolesAssociatedToScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests removing realm roles from a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task P_ShouldRemoveRolesFromScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to remove the specified realm role from the client scope
        var removeRolesFromScopeResponse = await KeycloakRestClient.ScopeMappings
            .RemoveRolesFromScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, [
                    TestRole
                ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(removeRolesFromScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(removeRolesFromScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(removeRolesFromScopeResponse.Response);

        // Validate monitoring metrics for the role removal request
        KcCommonAssertion.AssertResponseMonitoringMetrics(removeRolesFromScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests that attempting to remove an empty collection of realm roles from a client
    /// scope in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task Q_ShouldNotRemoveEmptyRolesCollectionFromScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to remove an empty collection of roles from the client scope
        var removeRolesFromScopeResponse = await KeycloakRestClient.ScopeMappings
            .RemoveRolesFromScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id, new List<KcRole>())
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(removeRolesFromScopeResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(removeRolesFromScopeResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(removeRolesFromScopeResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were removed
        Assert.IsNull(removeRolesFromScopeResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests listing the realm roles available for a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task R_ShouldListRolesAvailableForScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list realm roles available for assignment to the client scope
        var listRolesAvailableForScopeResponse = await KeycloakRestClient.ScopeMappings
            .ListRolesAvailableForScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listRolesAvailableForScopeResponse);
        Assert.IsFalse(listRolesAvailableForScopeResponse.IsError);

        // Ensure that the response contains a list of available roles
        Assert.IsNotNull(listRolesAvailableForScopeResponse.Response);

        // Verify that the expected role is present in the list of available roles
        Assert.IsTrue(listRolesAvailableForScopeResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listRolesAvailableForScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests listing the composite realm roles associated with a client scope in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task V_ShouldListCompositeRolesAssociatedToScope()
    {
        // Ensure that the test client, client scope, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestClientScope);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list composite realm roles associated with the client scope
        var listCompositeRolesAssociatedToScopeResponse = await KeycloakRestClient.ScopeMappings
            .ListCompositeRolesAssociatedToScopeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientScope.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listCompositeRolesAssociatedToScopeResponse);
        Assert.IsFalse(listCompositeRolesAssociatedToScopeResponse.IsError);

        // Ensure that the response contains a list of composite realm roles
        Assert.IsNotNull(listCompositeRolesAssociatedToScopeResponse.Response);

        // Verify that the list of composite roles is empty as expected
        Assert.IsFalse(listCompositeRolesAssociatedToScopeResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesAssociatedToScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
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
    /// Verifies that a realm role can be successfully deleted by its name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZA_ShouldDeleteRealmRoleByName()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the realm role by its name
        var deleteRoleResponse = await KeycloakRestClient.Roles
            .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(deleteRoleResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(deleteRoleResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteRoleResponse.MonitoringMetrics,
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
