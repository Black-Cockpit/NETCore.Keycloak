using System.Net;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcRoleTests;

/// <summary>
/// Contains tests for Keycloak client role operations.
/// </summary>
[TestClass]
[TestCategory("Combined")]
public class KcClientRoleTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak role used for testing.
    /// </summary>
    private static KcRole TestRole
    {
        get
        {
            try
            {
                // Retrieve and deserialize the role object from the environment variable.
                return JsonConvert.DeserializeObject<KcRole>(
                    Environment.GetEnvironmentVariable($"{nameof(KcClientRoleTests)}_KC_ROLE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientRoleTests)}_KC_ROLE",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak list of composite roles used for testing.
    /// </summary>
    private static IEnumerable<KcRole> TestCompositeRoles
    {
        get
        {
            try
            {
                // Retrieve and deserialize the list of role object from the environment variable.
                return JsonConvert.DeserializeObject<IEnumerable<KcRole>>(
                    Environment.GetEnvironmentVariable($"{nameof(KcClientRoleTests)}_KC_COMPOSITE_ROLE") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientRoleTests)}_KC_COMPOSITE_ROLE",
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
                    Environment.GetEnvironmentVariable($"{nameof(KcClientRoleTests)}_KC_CLIENT") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientRoleTests)}_KC_CLIENT",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the Keycloak REST client components before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
        Assert.IsNotNull(KeycloakRestClient.Roles);
        Assert.IsNotNull(KeycloakRestClient.Clients);
    }

    /// <summary>
    /// Tests the creation of a Keycloak client and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task A_CreateClient() => TestClient = await CreateAndGetClientAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a client role for a predefined private client in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldCreateClientRole()
    {
        // Ensure that the test client exists
        Assert.IsNotNull(TestClient);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Generate a new client role for testing
        TestRole = KcRoleMocks.Generate();

        // Ensure that the generated role is not null
        Assert.IsNotNull(TestRole);

        // Send the request to create the client role
        var clientRoleResponse = await KeycloakRestClient.Roles
            .CreateClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, TestRole)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(clientRoleResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(clientRoleResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(clientRoleResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientRoleResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);
    }

    /// <summary>
    /// Tests listing client roles associated with a predefined private client in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldListClientRoles()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list client roles filtered by the role's name
        var listClientRolesResponse = await KeycloakRestClient.Roles
            .ListClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, new KcFilter
            {
                Search = TestRole.Name
            }).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(listClientRolesResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(listClientRolesResponse.IsError);

        // Ensure that the response contains a list of client roles
        Assert.IsNotNull(listClientRolesResponse.Response);

        // Ensure that the test role is present in the response
        Assert.IsTrue(listClientRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientRolesResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Assign the retrieved role to the test variable for further validation
        TestRole = listClientRolesResponse.Response.First(role => role.Name == TestRole.Name);
    }

    /// <summary>
    /// Tests retrieving a specific client role associated with a predefined private client in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldGetClientRole()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get the specific client role
        var getClientRoleResponse = await KeycloakRestClient.Roles
            .GetClientRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestRole.Name).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(getClientRoleResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(getClientRoleResponse.IsError);

        // Ensure that the response contains the requested client role
        Assert.IsNotNull(getClientRoleResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getClientRoleResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Assign the retrieved role to the test variable for further validation
        TestRole = getClientRoleResponse.Response;
    }

    /// <summary>
    /// Tests the verification of the existence of client roles associated with a predefined
    /// private client in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldVerifyIfClientRoleExists()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Define test scenarios with role names and expected existence results
        var roleNames = new List<Tuple<string, bool>>
        {
            new(TestRole.Name, true), // Existing role
            new("NonExistingRole", false) // Non-existing role
        };

        foreach ( var (roleName, expectedResult) in roleNames )
        {
            // Send the request to verify if the client role exists
            var isClientRoleExistsResponse = await KeycloakRestClient.Roles
                .IsClientRoleExistsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                    roleName).ConfigureAwait(false);

            // Ensure that the response is not null
            Assert.IsNotNull(isClientRoleExistsResponse);

            // Ensure the response does not indicate an error
            Assert.IsFalse(isClientRoleExistsResponse.IsError);

            // Ensure that the response contains a result indicating the existence of the role
            Assert.IsNotNull(isClientRoleExistsResponse.Response);

            // Verify that the result matches the expected value
            Assert.IsTrue(isClientRoleExistsResponse.Response == expectedResult);

            // Ensure that no monitoring metrics are returned for this type of request
            Assert.IsNull(isClientRoleExistsResponse.MonitoringMetrics);
        }
    }

    /// <summary>
    /// Tests updating a client role associated with a predefined private client in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldUpdateClientRole()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Clone the test role for modification
        var kcRole = TestRole;

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Modify the role name
        kcRole.Name = $"{TestRole.Name}_2";

        // Send the request to update the client role
        var updateClientRoleResponse = await KeycloakRestClient.Roles
            .UpdateClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestRole.Name, kcRole).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(updateClientRoleResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(updateClientRoleResponse.IsError);

        // Validate monitoring metrics for the update request
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateClientRoleResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);

        // Assign the updated role to the test variable for further validation
        TestRole = kcRole;
    }

    /// <summary>
    /// Tests adding composite roles to a client role in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldAddCompositeToClientRole()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Generate two composite roles for testing
        var compositeRoles = KcRoleMocks.Generate(2).ToList();
        Assert.IsTrue(compositeRoles.Count == 2);

        // Create the composite roles in the realm
        foreach ( var compositeRole in compositeRoles )
        {
            await CreateRealmRoleAsync(TestContext, compositeRole).ConfigureAwait(false);
        }

        // Retrieve the list of roles from Keycloak to confirm the composite roles were created
        var compositeRolesResponse = (await ListRealmRolesAsync(TestContext).ConfigureAwait(false))
            ?.Where(role => compositeRoles.Any(mockRole => mockRole.Name == role.Name)).ToList();
        Assert.IsNotNull(compositeRolesResponse);
        Assert.IsTrue(compositeRolesResponse.Count == 2);

        // Update the test composite roles with the retrieved roles
        TestCompositeRoles = compositeRolesResponse;

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to add composite roles to the client role
        var addCompositeResponse = await KeycloakRestClient.Roles
            .AddClientRoleToCompositeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestRole.Name, TestCompositeRoles).ConfigureAwait(false);

        // Validate the response from the composite addition operation
        Assert.IsNotNull(addCompositeResponse);
        Assert.IsFalse(addCompositeResponse.IsError);

        // Validate the monitoring metrics for the successful composite addition request
        KcCommonAssertion.AssertResponseMonitoringMetrics(addCompositeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests retrieving the composite roles associated with a client role in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldGetClientCompositeRole()
    {
        // Ensure that the test client, role, and composite roles exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestCompositeRoles);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve composite roles associated with the client role
        var listCompositeRolesResponse = await KeycloakRestClient.Roles
            .GetClientCompositeRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestRole.Name).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listCompositeRolesResponse);
        Assert.IsFalse(listCompositeRolesResponse.IsError);

        // Ensure that the response contains a list of composite roles
        Assert.IsNotNull(listCompositeRolesResponse.Response);

        // Verify that the retrieved roles match the expected composite roles
        Assert.IsFalse(listCompositeRolesResponse.Response.Select(role => role.Name)
            .Except(TestCompositeRoles.Select(role => role.Name)).Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving client-level composite roles by name in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task HA_ShouldGetClientLevelCompositesByName()
    {
        // Ensure that the test client, role, and composite roles exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestCompositeRoles);

        // Ensure that the composite roles collection is not empty
        Assert.IsTrue(TestCompositeRoles.Any());

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve client-level composite roles by the first composite role's name
        var listCompositeRolesResponse = await KeycloakRestClient.Roles
            .GetClientLevelCompositesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestCompositeRoles.First().Name, TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listCompositeRolesResponse);
        Assert.IsFalse(listCompositeRolesResponse.IsError);

        // Ensure that the response contains a list of roles
        Assert.IsNotNull(listCompositeRolesResponse.Response);

        // Verify that the expected role is not present in the retrieved composite roles (as expected)
        Assert.IsFalse(listCompositeRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving client-level composite roles by ID in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task HB_ShouldGetClientLevelCompositesById()
    {
        // Ensure that the test client, role, and composite roles exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestCompositeRoles);

        // Ensure that the composite roles collection is not empty
        Assert.IsTrue(TestCompositeRoles.Any());

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve client-level composite roles by the first composite role's ID
        var listCompositeRolesResponse = await KeycloakRestClient.Roles
            .GetClientLevelCompositesByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestCompositeRoles.First().Id, TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listCompositeRolesResponse);
        Assert.IsFalse(listCompositeRolesResponse.IsError);

        // Ensure that the response contains a list of roles
        Assert.IsNotNull(listCompositeRolesResponse.Response);

        // Verify that the expected role is not present in the retrieved composite roles (as expected)
        Assert.IsFalse(listCompositeRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting a client role and its associated composite roles from the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldDeleteClientCompositeRole()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the client role and its composite roles
        var deleteClientCompositeRoleResponse = await KeycloakRestClient.Roles
            .DeleteClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClient.Id, TestRole.Name).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteClientCompositeRoleResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteClientCompositeRoleResponse.IsError);

        // Validate monitoring metrics for the delete request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteClientCompositeRoleResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests the creation of a client role for a predefined private client in the Keycloak realm and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task J_CreateClientRole() =>
        TestRole = await CreateAndGetClientRoleAsync(TestContext, TestClient).ConfigureAwait(false);

    /// <summary>
    /// Tests retrieving the groups associated with a client role in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task K_ShouldGetRoleGroupsInClientRole()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get groups associated with the client role
        var getRoleGroupsResponse = await KeycloakRestClient.Roles
            .GetGroupsInClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestRole.Name).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getRoleGroupsResponse);
        Assert.IsFalse(getRoleGroupsResponse.IsError);

        // Ensure that the response contains a list of groups
        Assert.IsNotNull(getRoleGroupsResponse.Response);

        // Verify that no groups are associated with the client role
        Assert.IsFalse(getRoleGroupsResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getRoleGroupsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving the authorization permissions for a client role in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldGetClientRoleAuthorizationPermissions()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get the authorization permissions for the client role
        var getAuthorizationPermissionManagementResponse = await KeycloakRestClient.Roles
            .GetClientRoleAuthorizationPermissionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClient.Id, TestRole.Name).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getAuthorizationPermissionManagementResponse);
        Assert.IsFalse(getAuthorizationPermissionManagementResponse.IsError);

        // Ensure that the response contains the authorization permissions
        Assert.IsNotNull(getAuthorizationPermissionManagementResponse.Response);

        // Verify that the authorization permissions are disabled by default
        Assert.IsFalse(getAuthorizationPermissionManagementResponse.Response.Enabled);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            getAuthorizationPermissionManagementResponse.MonitoringMetrics, HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving the users assigned to a client role in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldGetUsersInClientRole()
    {
        // Ensure that the test client and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get users assigned to the client role
        var getUsersInRoleResponse = await KeycloakRestClient.Roles
            .GetUsersInClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id,
                TestRole.Name).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getUsersInRoleResponse);
        Assert.IsFalse(getUsersInRoleResponse.IsError);

        // Ensure that the response contains a list of users
        Assert.IsNotNull(getUsersInRoleResponse.Response);

        // Verify that no users are assigned to the client role
        Assert.IsFalse(getUsersInRoleResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUsersInRoleResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting a client role from the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldDeleteClientRole()
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
