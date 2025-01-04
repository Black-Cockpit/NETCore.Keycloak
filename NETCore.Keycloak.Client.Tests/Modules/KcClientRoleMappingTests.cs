using System.Net;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Contains tests for validating the Keycloak client role mapping API functionalities under expected scenarios.
/// </summary>
[TestClass]
[TestCategory("Final")]
public class KcClientRoleMappingTests : KcTestingModule
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
                    Environment.GetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KC_ROLE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KC_ROLE",
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
                    Environment.GetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KC_CLIENT") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KC_CLIENT",
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
                    Environment.GetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KC_GROUP") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KC_GROUP",
            JsonConvert.SerializeObject(value));
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
                    Environment.GetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientRoleMappingTests)}_KCUSER",
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
        Assert.IsNotNull(KeycloakRestClient.ClientRoleMappings);
        Assert.IsNotNull(KeycloakRestClient.Users);
    }

    /// <summary>
    /// Tests the creation of a Keycloak client and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task A_CreateClient() => TestClient = await CreateAndGetClientAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a client role for a predefined private client in the Keycloak realm and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task B_CreateClientRole() =>
        TestRole = await CreateAndGetClientRoleAsync(TestContext, TestClient).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a group in the Keycloak realm and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task C_CreateGroup() => TestGroup = await CreateAndGetGroupAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests mapping client roles to a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldMapClientRolesToGroup()
    {
        // Ensure that the test client, role, and group exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map client roles to the group
        var mapClientRoleToGroupResponse = await KeycloakRestClient.ClientRoleMappings.MapClientRolesToGroupAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id, TestClient.Id, [
                TestRole
            ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapClientRoleToGroupResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapClientRoleToGroupResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapClientRoleToGroupResponse.Response);

        // Validate monitoring metrics for the role mapping request
        KcCommonAssertion.AssertResponseMonitoringMetrics(mapClientRoleToGroupResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests that attempting to map an empty collection of client roles to a group in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldNotMapEmptyClientRolesCollectionToGroup()
    {
        // Ensure that the test client, role, and group exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map an empty collection of client roles to the group
        var mapClientRoleToGroupResponse = await KeycloakRestClient.ClientRoleMappings.MapClientRolesToGroupAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClient.Id, TestGroup.Id,
            new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapClientRoleToGroupResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapClientRoleToGroupResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapClientRoleToGroupResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were mapped
        Assert.IsNull(mapClientRoleToGroupResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests retrieving client roles mapped to a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldGetGroupMappedClientRoles()
    {
        // Ensure that the test client, role, and group exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get client roles mapped to the group
        var getGroupMappedClientRolesResponse = await KeycloakRestClient.ClientRoleMappings
            .GetGroupMappedClientRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id,
                TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getGroupMappedClientRolesResponse);
        Assert.IsFalse(getGroupMappedClientRolesResponse.IsError);

        // Ensure that the response contains a list of mapped client roles
        Assert.IsNotNull(getGroupMappedClientRolesResponse.Response);

        // Verify that the expected role is present in the mapped client roles
        Assert.IsTrue(getGroupMappedClientRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getGroupMappedClientRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting client role mappings from a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldDeleteGroupClientRoleMappings()
    {
        // Ensure that the test client, role, and group exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete client role mappings from the group
        var deleteGroupClientRoleMappings = await KeycloakRestClient.ClientRoleMappings
            .DeleteGroupClientRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestGroup.Id, TestClient.Id, [
                    TestRole
                ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteGroupClientRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteGroupClientRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteGroupClientRoleMappings.Response);

        // Validate monitoring metrics for the delete request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteGroupClientRoleMappings.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests that attempting to delete an empty collection of client role mappings
    /// from a group in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldNotDeleteGroupEmptyClientRoleCollectionMappings()
    {
        // Ensure that the test client, role, and group exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete an empty collection of client role mappings from the group
        var deleteGroupClientRoleMappings = await KeycloakRestClient.ClientRoleMappings
            .DeleteGroupClientRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestGroup.Id, TestClient.Id, new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteGroupClientRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteGroupClientRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteGroupClientRoleMappings.Response);

        // Ensure that no monitoring metrics are returned, as no roles were mapped for deletion
        Assert.IsNull(deleteGroupClientRoleMappings.MonitoringMetrics);
    }

    /// <summary>
    /// Tests retrieving available client roles for a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldGetGroupAvailableClientRoles()
    {
        // Ensure that the test client, role, and group exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get available client roles for the group
        var getGroupAvailableClientRolesResponse = await KeycloakRestClient.ClientRoleMappings
            .GetGroupAvailableClientRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id,
                TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getGroupAvailableClientRolesResponse);
        Assert.IsFalse(getGroupAvailableClientRolesResponse.IsError);

        // Ensure that the response contains a list of available client roles
        Assert.IsNotNull(getGroupAvailableClientRolesResponse.Response);

        // Verify that the expected role is present in the available client roles
        Assert.IsTrue(getGroupAvailableClientRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getGroupAvailableClientRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving composite client roles for a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldGetGroupCompositeClientRoles()
    {
        // Ensure that the test client, role, and group exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get composite client roles for the group
        var getGroupCompositeClientRolesResponse = await KeycloakRestClient.ClientRoleMappings
            .GetGroupCompositeClientRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id,
                TestClient.Id, new KcFilter
                {
                    Search = TestGroup.Name
                }).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getGroupCompositeClientRolesResponse);
        Assert.IsFalse(getGroupCompositeClientRolesResponse.IsError);

        // Ensure that the response contains a list of composite client roles
        Assert.IsNotNull(getGroupCompositeClientRolesResponse.Response);

        // Verify that no composite client roles are returned (as expected)
        Assert.IsFalse(getGroupCompositeClientRolesResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getGroupCompositeClientRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Creates a new test user in the Keycloak realm and assigns it to the <see cref="TestUser"/> property.
    /// </summary>
    [TestMethod]
    public async Task K_CreateTestUser() =>
        TestUser = await CreateAndGetRealmUserAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests mapping client roles to a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldMapClientRolesToUser()
    {
        // Ensure that the test client, user, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map client roles to the user
        var mapClientRoleToUserResponse = await KeycloakRestClient.ClientRoleMappings.MapClientRolesToUserAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id, TestClient.Id, [
                TestRole
            ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapClientRoleToUserResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapClientRoleToUserResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapClientRoleToUserResponse.Response);

        // Validate monitoring metrics for the role mapping request
        KcCommonAssertion.AssertResponseMonitoringMetrics(mapClientRoleToUserResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests that attempting to map an empty collection of client roles to a user in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldNotMapEmptyClientRolesCollectionToUser()
    {
        // Ensure that the test client, user, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map an empty collection of client roles to the user
        var mapClientRoleToUserResponse = await KeycloakRestClient.ClientRoleMappings.MapClientRolesToUserAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id, TestClient.Id,
            new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapClientRoleToUserResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapClientRoleToUserResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapClientRoleToUserResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were mapped
        Assert.IsNull(mapClientRoleToUserResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests retrieving client roles mapped to a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task O_ShouldGetUserMappedClientRoles()
    {
        // Ensure that the test client, user, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get client roles mapped to the user
        var getUserMappedClientRolesResponse = await KeycloakRestClient.ClientRoleMappings
            .GetUserMappedClientRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id,
                TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getUserMappedClientRolesResponse);
        Assert.IsFalse(getUserMappedClientRolesResponse.IsError);

        // Ensure that the response contains a list of mapped client roles
        Assert.IsNotNull(getUserMappedClientRolesResponse.Response);

        // Verify that the expected role is present in the mapped client roles
        Assert.IsTrue(getUserMappedClientRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserMappedClientRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting client role mappings from a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task P_ShouldDeleteUserClientRoleMappings()
    {
        // Ensure that the test client, user, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete client role mappings from the user
        var deleteUserClientRoleMappings = await KeycloakRestClient.ClientRoleMappings
            .DeleteUserClientRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestUser.Id, TestClient.Id, [
                    TestRole
                ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteUserClientRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteUserClientRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteUserClientRoleMappings.Response);

        // Validate monitoring metrics for the delete request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteUserClientRoleMappings.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests that attempting to delete an empty collection of client role mappings
    /// from a user in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task Q_ShouldNotDeleteUserEmptyClientRoleCollectionMappings()
    {
        // Ensure that the test client, user, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete an empty collection of client role mappings from the user
        var deleteUserClientRoleMappings = await KeycloakRestClient.ClientRoleMappings
            .DeleteUserClientRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestUser.Id, TestClient.Id, new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteUserClientRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteUserClientRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteUserClientRoleMappings.Response);

        // Ensure that no monitoring metrics are returned, as no roles were mapped for deletion
        Assert.IsNull(deleteUserClientRoleMappings.MonitoringMetrics);
    }

    /// <summary>
    /// Tests retrieving available client roles for a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task R_ShouldGetUserAvailableClientRoles()
    {
        // Ensure that the test client, user, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get available client roles for the user
        var getUserAvailableClientRolesResponse = await KeycloakRestClient.ClientRoleMappings
            .GetUserAvailableClientRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id,
                TestClient.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getUserAvailableClientRolesResponse);
        Assert.IsFalse(getUserAvailableClientRolesResponse.IsError);

        // Ensure that the response contains a list of available client roles
        Assert.IsNotNull(getUserAvailableClientRolesResponse.Response);

        // Verify that the expected role is present in the available client roles
        Assert.IsTrue(getUserAvailableClientRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserAvailableClientRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving composite client roles for a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task V_ShouldGetGroupCompositeClientRoles()
    {
        // Ensure that the test client, user, and role exist
        Assert.IsNotNull(TestClient);
        Assert.IsNotNull(TestUser);
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get composite client roles for the user
        var getUserCompositeClientRolesResponse = await KeycloakRestClient.ClientRoleMappings
            .GetUserCompositeClientRolesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id,
                TestClient.Id, new KcFilter
                {
                    Search = TestGroup.Name
                }).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getUserCompositeClientRolesResponse);
        Assert.IsFalse(getUserCompositeClientRolesResponse.IsError);

        // Ensure that the response contains a list of composite client roles
        Assert.IsNotNull(getUserCompositeClientRolesResponse.Response);

        // Verify that no composite client roles are returned (as expected)
        Assert.IsFalse(getUserCompositeClientRolesResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserCompositeClientRolesResponse.MonitoringMetrics,
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

    /// <summary>
    /// Validates the functionality to delete a group in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZB_ShouldDeleteGroup()
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

    /// <summary>
    /// Validates the functionality to delete a user in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZC_ShouldDeleteUser()
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
            HttpStatusCode.NoContent,
            HttpMethod.Delete);
    }
}
