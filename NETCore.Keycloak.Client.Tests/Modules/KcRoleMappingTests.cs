using System.Net;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Contains tests for validating the Keycloak role mapping API functionalities under expected scenarios.
/// </summary>
[TestClass]
[TestCategory("Final")]
public class KcRoleMappingTests : KcTestingModule
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
                    Environment.GetEnvironmentVariable($"{nameof(KcRoleMappingTests)}_KC_ROLE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcRoleMappingTests)}_KC_ROLE",
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
                    Environment.GetEnvironmentVariable($"{nameof(KcRoleMappingTests)}_KC_GROUP") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcRoleMappingTests)}_KC_GROUP",
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
                    Environment.GetEnvironmentVariable($"{nameof(KcRoleMappingTests)}_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcRoleMappingTests)}_KCUSER",
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
        Assert.IsNotNull(KeycloakRestClient.RoleMappings);
        Assert.IsNotNull(KeycloakRestClient.Users);
    }

    /// <summary>
    /// Tests the creation of a realm role in the Keycloak realm and assigns it to the <see cref="TestRole"/> property.
    /// </summary>
    [TestMethod]
    public async Task A_CreateRole() =>
        TestRole = await CreateAndGetRealmRoleAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a group in the Keycloak realm and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task B_CreateGroup() => TestGroup = await CreateAndGetGroupAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests adding realm role mappings to a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldAddGroupRealmRoleMappings()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map realm roles to the group
        var mapRolesToGroupResponse = await KeycloakRestClient.RoleMappings.AddGroupRealmRoleMappingsAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id, [
                TestRole
            ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapRolesToGroupResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapRolesToGroupResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapRolesToGroupResponse.Response);

        // Validate monitoring metrics for the role mapping request
        KcCommonAssertion.AssertResponseMonitoringMetrics(mapRolesToGroupResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests that attempting to map an empty collection of realm roles
    /// to a group in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldNotMapEmptyRolesColletionToGroup()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map an empty collection of realm roles to the group
        var mapRolesToGroupResponse = await KeycloakRestClient.RoleMappings.AddGroupRealmRoleMappingsAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id, new List<KcRole>())
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapRolesToGroupResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapRolesToGroupResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapRolesToGroupResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were mapped
        Assert.IsNull(mapRolesToGroupResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests listing the realm roles mapped to a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldListGroupMappedRoles()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list realm roles mapped to the group
        var listGroupMappedRolesResponse = await KeycloakRestClient.RoleMappings
            .ListGroupRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listGroupMappedRolesResponse);
        Assert.IsFalse(listGroupMappedRolesResponse.IsError);

        // Ensure that the response contains a list of mapped realm roles
        Assert.IsNotNull(listGroupMappedRolesResponse.Response);

        // Verify that the expected role is present in the mapped realm roles
        Assert.IsTrue(listGroupMappedRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listGroupMappedRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving the realm roles and client roles mapped to a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldGetGroupMappedRoles()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get all role mappings (realm and client roles) for the group
        var getGroupMappedRolesResponse = await KeycloakRestClient.RoleMappings
            .GetGroupRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getGroupMappedRolesResponse);
        Assert.IsFalse(getGroupMappedRolesResponse.IsError);

        // Ensure that the response contains a list of realm role mappings
        Assert.IsNotNull(getGroupMappedRolesResponse.Response);
        Assert.IsNotNull(getGroupMappedRolesResponse.Response.RealmMappings);

        // Verify that the expected realm role is present in the mapped roles
        Assert.IsTrue(getGroupMappedRolesResponse.Response.RealmMappings.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getGroupMappedRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting realm role mappings from a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldDeleteGroupRoleMappings()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the specified realm role mappings from the group
        var deleteGroupRoleMappings = await KeycloakRestClient.RoleMappings
            .DeleteGroupRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestGroup.Id, [
                    TestRole
                ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteGroupRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteGroupRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteGroupRoleMappings.Response);

        // Validate monitoring metrics for the role deletion request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteGroupRoleMappings.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests that attempting to delete an empty collection of realm role mappings
    /// from a group in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldNotDeleteGroupEmptyCollectionRoleMappings()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete an empty collection of realm role mappings from the group
        var deleteGroupRoleMappings = await KeycloakRestClient.RoleMappings
            .DeleteGroupRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestGroup.Id, new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteGroupRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteGroupRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteGroupRoleMappings.Response);

        // Ensure that no monitoring metrics are returned, as no roles were deleted
        Assert.IsNull(deleteGroupRoleMappings.MonitoringMetrics);
    }

    /// <summary>
    /// Tests retrieving available realm roles for a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldGetGroupAvailableRoles()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list available realm roles for the group
        var getGroupAvailableRolesResponse = await KeycloakRestClient.RoleMappings
            .ListGroupAvailableRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestGroup.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getGroupAvailableRolesResponse);
        Assert.IsFalse(getGroupAvailableRolesResponse.IsError);

        // Ensure that the response contains a list of available realm roles
        Assert.IsNotNull(getGroupAvailableRolesResponse.Response);

        // Verify that the expected role is present in the available realm roles
        Assert.IsTrue(getGroupAvailableRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getGroupAvailableRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving the composite roles assigned to a group in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldGetGroupCompositeRoles()
    {
        // Ensure that the test role and group exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list effective (composite) realm roles for the group
        var getGroupCompositeClientRolesResponse = await KeycloakRestClient.RoleMappings
            .ListGroupEffectiveRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestGroup.Id, new KcFilter
                {
                    Search = TestGroup.Name
                }).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getGroupCompositeClientRolesResponse);
        Assert.IsFalse(getGroupCompositeClientRolesResponse.IsError);

        // Ensure that the response contains a list of composite roles
        Assert.IsNotNull(getGroupCompositeClientRolesResponse.Response);

        // Verify that no composite roles are returned (as expected)
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
    /// Tests adding realm role mappings to a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldAddUserRoleMappings()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map the specified realm role to the user
        var mapRolesToUserResponse = await KeycloakRestClient.RoleMappings.AddUserRealmRoleMappingsAsync(
            TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id, [
                TestRole
            ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapRolesToUserResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapRolesToUserResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapRolesToUserResponse.Response);

        // Validate monitoring metrics for the role mapping request
        KcCommonAssertion.AssertResponseMonitoringMetrics(mapRolesToUserResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Tests that attempting to map an empty collection of realm roles
    /// to a user in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldNotMapEmptyRolesColletionToUser()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to map an empty collection of realm roles to the user
        var mapRolesToUserResponse = await KeycloakRestClient.RoleMappings.AddUserRealmRoleMappingsAsync(
                TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id, new List<KcRole>())
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(mapRolesToUserResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(mapRolesToUserResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(mapRolesToUserResponse.Response);

        // Ensure that no monitoring metrics are returned, as no roles were mapped
        Assert.IsNull(mapRolesToUserResponse.MonitoringMetrics);
    }

    /// <summary>
    /// Tests listing the realm roles mapped to a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task O_ShouldListUserMappedRoles()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list realm roles mapped to the user
        var listUserMappedRolesResponse = await KeycloakRestClient.RoleMappings
            .ListUserRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(listUserMappedRolesResponse);
        Assert.IsFalse(listUserMappedRolesResponse.IsError);

        // Ensure that the response contains a list of mapped realm roles
        Assert.IsNotNull(listUserMappedRolesResponse.Response);

        // Verify that the expected role is present in the mapped realm roles
        Assert.IsTrue(listUserMappedRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listUserMappedRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving the realm roles and client roles mapped to a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task P_ShouldGetUserMappedRoles()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to get all role mappings (realm and client roles) for the user
        var getUserMappedRolesResponse = await KeycloakRestClient.RoleMappings
            .GetUserRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestUser.Id)
            .ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getUserMappedRolesResponse);
        Assert.IsFalse(getUserMappedRolesResponse.IsError);

        // Ensure that the response contains a list of role mappings
        Assert.IsNotNull(getUserMappedRolesResponse.Response);
        Assert.IsNotNull(getUserMappedRolesResponse.Response.RealmMappings);

        // Verify that the expected realm role is present in the mapped roles
        Assert.IsTrue(getUserMappedRolesResponse.Response.RealmMappings.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserMappedRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests deleting realm role mappings from a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task Q_ShouldDeleteUserRoleMappings()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the specified realm role mappings from the user
        var deleteUserRoleMappings = await KeycloakRestClient.RoleMappings
            .DeleteUserRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestUser.Id, [
                    TestRole
                ]).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteUserRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteUserRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteUserRoleMappings.Response);

        // Validate monitoring metrics for the role deletion request
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteUserRoleMappings.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Tests that attempting to delete an empty collection of realm role mappings
    /// from a user in the Keycloak realm does not perform any operation.
    /// </summary>
    [TestMethod]
    public async Task R_ShouldNotDeleteUserEmptyCollectionRoleMappings()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete an empty collection of realm role mappings from the user
        var deleteUserRoleMappings = await KeycloakRestClient.RoleMappings
            .DeleteUserRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestUser.Id, new List<KcRole>()).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(deleteUserRoleMappings);

        // Ensure the response does not indicate an error
        Assert.IsFalse(deleteUserRoleMappings.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(deleteUserRoleMappings.Response);

        // Ensure that no monitoring metrics are returned, as no roles were deleted
        Assert.IsNull(deleteUserRoleMappings.MonitoringMetrics);
    }

    /// <summary>
    /// Tests retrieving available realm roles for a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task V_ShouldGetUserAvailableRoles()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list available realm roles for the user
        var getUserAvailableRolesResponse = await KeycloakRestClient.RoleMappings
            .ListUserAvailableRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestUser.Id).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getUserAvailableRolesResponse);
        Assert.IsFalse(getUserAvailableRolesResponse.IsError);

        // Ensure that the response contains a list of available realm roles
        Assert.IsNotNull(getUserAvailableRolesResponse.Response);

        // Verify that the expected role is present in the available realm roles
        Assert.IsTrue(getUserAvailableRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserAvailableRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Tests retrieving the composite roles assigned to a user in the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task W_ShouldGetUserCompositeRoles()
    {
        // Ensure that the test role and user exist
        Assert.IsNotNull(TestRole);
        Assert.IsNotNull(TestUser);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list effective (composite) realm roles for the user
        var getUserCompositeClientRolesResponse = await KeycloakRestClient.RoleMappings
            .ListUserEffectiveRealmRoleMappingsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestUser.Id, new KcFilter
                {
                    Search = TestGroup.Name
                }).ConfigureAwait(false);

        // Ensure that the response is not null and does not indicate an error
        Assert.IsNotNull(getUserCompositeClientRolesResponse);
        Assert.IsFalse(getUserCompositeClientRolesResponse.IsError);

        // Ensure that the response contains a list of composite roles
        Assert.IsNotNull(getUserCompositeClientRolesResponse.Response);

        // Verify that at least one composite role is returned
        Assert.IsTrue(getUserCompositeClientRolesResponse.Response.Any());

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserCompositeClientRolesResponse.MonitoringMetrics,
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
            HttpStatusCode.NoContent,
            HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to delete a group in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZA_ShouldDeleteGroup()
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
    /// Verifies that a realm role can be successfully deleted by its name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZB_ShouldDeleteRealmRoleByName()
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
}
