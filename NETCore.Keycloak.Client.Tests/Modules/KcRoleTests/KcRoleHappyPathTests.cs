using System.Net;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcRoleTests;

/// <summary>
/// Contains tests for validating the Keycloak role API functionalities under expected scenarios (Happy Path).
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcRoleHappyPathTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak role used for testing in happy path scenarios.
    /// </summary>
    private static KcRole TestRole
    {
        get
        {
            try
            {
                // Retrieve and deserialize the role object from the environment variable.
                return JsonConvert.DeserializeObject<KcRole>(
                    Environment.GetEnvironmentVariable($"{nameof(KcRoleHappyPathTests)}_KC_ROLE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcRoleHappyPathTests)}_KC_ROLE",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak list of composite roles used for testing in happy path scenarios.
    /// </summary>
    private static IEnumerable<KcRole> TestCompositeRoles
    {
        get
        {
            try
            {
                // Retrieve and deserialize the list of role object from the environment variable.
                return JsonConvert.DeserializeObject<IEnumerable<KcRole>>(
                    Environment.GetEnvironmentVariable($"{nameof(KcRoleHappyPathTests)}_KC_COMPOSITE_ROLE") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcRoleHappyPathTests)}_KC_COMPOSITE_ROLE",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak role module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Roles);

    /// <summary>
    /// Verifies that a new realm role can be successfully created in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreateRealmRole()
    {
        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Generate a new realm role with a random name and predefined attributes
        var kcRole = KcRoleMocks.Generate();

        // Ensure that the realm role object is not null
        Assert.IsNotNull(kcRole);

        // Create an ensure the realm role is created
        await CreateRealmRoleAsync(TestContext, kcRole).ConfigureAwait(false);

        // Assign the created role to the TestRole property
        TestRole = kcRole;
    }

    /// <summary>
    /// Verifies that realm roles can be successfully listed and that the test role exists in the list of roles in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldListRealmRoles()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list realm roles, filtering by the test role's name
        var listRolesResponse = await KeycloakRestClient.Roles
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcFilter
            {
                Search = TestRole.Name
            }).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(listRolesResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(listRolesResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(listRolesResponse.Response);

        // Assert that the test role is present in the response
        Assert.IsTrue(listRolesResponse.Response.Any(role => role.Name == TestRole.Name));

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(listRolesResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Update the TestRole property with the corresponding role from the response
        TestRole = listRolesResponse.Response.First(role => role.Name == TestRole.Name);
    }

    /// <summary>
    /// Verifies that a specific realm role can be retrieved by its name from the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldGetRealmRoleByName()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve the realm role by its name
        var getRoleResponse = await KeycloakRestClient.Roles
            .GetAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(getRoleResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(getRoleResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(getRoleResponse.Response);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(getRoleResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Update the TestRole property with the retrieved role
        TestRole = getRoleResponse.Response;
    }

    /// <summary>
    /// Verifies the existence of a realm role in the Keycloak system for multiple scenarios.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldVerifyIfRoleExists()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Define test scenarios with role names and expected existence results
        var roleNames = new List<Tuple<string, bool>>
        {
            new(TestRole.Name, true),
            new("NonExistingRole", false)
        };

        foreach ( var (roleName, expectedResult) in roleNames )
        {
            // Send the request to check if the realm role exists
            var isRoleExistsResponse = await KeycloakRestClient.Roles
                .IsRolesExistsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, roleName)
                .ConfigureAwait(false);

            // Assert that the response from the operation is not null
            Assert.IsNotNull(isRoleExistsResponse);

            // Assert that the response does not indicate an error
            Assert.IsFalse(isRoleExistsResponse.IsError);

            // Assert that the response contains valid data
            Assert.IsNotNull(isRoleExistsResponse.Response);

            // Assert that the response matches the expected result
            Assert.IsTrue(isRoleExistsResponse.Response == expectedResult);

            // Validate that no monitoring metrics are included in the response
            Assert.IsNull(isRoleExistsResponse.MonitoringMetrics);
        }
    }

    /// <summary>
    /// Verifies that a specific realm role can be retrieved by its unique identifier (ID) from the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldGetRealmRoleById()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve the realm role by its ID
        var getRoleResponse = await KeycloakRestClient.Roles
            .GetByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Id)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(getRoleResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(getRoleResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(getRoleResponse.Response);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(getRoleResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Update the TestRole property with the retrieved role
        TestRole = getRoleResponse.Response;
    }

    /// <summary>
    /// Verifies that a realm role can be successfully updated by its name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldUpdateRealmRoleByName()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Clone the test role for modification
        var kcRole = TestRole;

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Update the name of the role
        kcRole.Name = $"{TestRole.Name}_2";

        // Send the request to update the realm role by its name
        var updateRoleResponse = await KeycloakRestClient.Roles
            .UpdateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name, kcRole)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(updateRoleResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(updateRoleResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateRoleResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);

        // Update the TestRole property with the modified role
        TestRole = kcRole;
    }

    /// <summary>
    /// Verifies that a realm role can be successfully updated by its unique identifier (ID) in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldUpdateRealmRoleById()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Clone the test role for modification
        var kcRole = TestRole;

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Update the name of the role
        kcRole.Name = $"{TestRole.Name}_3";

        // Send the request to update the realm role by its ID
        var updateRoleResponse = await KeycloakRestClient.Roles
            .UpdateByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, kcRole.Id, kcRole)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(updateRoleResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(updateRoleResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateRoleResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);

        // Update the TestRole property with the modified role
        TestRole = kcRole;
    }

    /// <summary>
    /// Validates the functionality to add composite roles to a role by name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldAddCompositeToRoleByName()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Generate two mock roles to add as composite roles.
        var compositeRoles = KcRoleMocks.Generate(2).ToList();
        Assert.IsTrue(compositeRoles.Count == 2);

        // Create the composite roles in the Keycloak system.
        foreach ( var compositeRole in compositeRoles )
        {
            await CreateRealmRoleAsync(TestContext, compositeRole).ConfigureAwait(false);
        }

        // Retrieve the list of roles from Keycloak to confirm the composite roles were created.
        var compositeRolesResponse = (await ListRealmRolesAsync(TestContext).ConfigureAwait(false))
            ?.Where(role => compositeRoles.Any(mockRole => mockRole.Name == role.Name)).ToList();
        Assert.IsNotNull(compositeRolesResponse);
        Assert.IsTrue(compositeRolesResponse.Count == 2);

        // Update the test composite roles with the retrieved roles.
        TestCompositeRoles = compositeRolesResponse;

        // Retrieve an access token for the realm admin to perform the composite addition operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to add composite roles to the specified role by name.
        var addCompositeResponse = await KeycloakRestClient.Roles
            .AddCompositeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name,
                TestCompositeRoles).ConfigureAwait(false);

        // Validate the response from the composite addition operation.
        Assert.IsNotNull(addCompositeResponse);
        Assert.IsFalse(addCompositeResponse.IsError);

        // Validate the monitoring metrics for the successful composite addition request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(addCompositeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Validates the functionality to add composite roles to a role by ID in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldAddCompositeToRoleById()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Ensure the composite roles are initialized and contain exactly two roles.
        Assert.IsNotNull(TestCompositeRoles);
        Assert.IsTrue(TestCompositeRoles.Count() == 2);

        // Retrieve an access token for the realm admin to perform the composite addition operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to add composite roles to the specified role by ID.
        var addCompositeResponse = await KeycloakRestClient.Roles
            .AddCompositeByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Id,
                TestCompositeRoles).ConfigureAwait(false);

        // Validate the response from the composite addition operation.
        Assert.IsNotNull(addCompositeResponse);
        Assert.IsFalse(addCompositeResponse.IsError);

        // Validate the monitoring metrics for the successful composite addition request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(addCompositeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Validates the functionality to list composite roles of a role by name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldListCompositeRolesByName()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Ensure the composite roles are initialized.
        Assert.IsNotNull(TestCompositeRoles);

        // Retrieve an access token for the realm admin to perform the composite role listing operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to list composite roles for the specified role by name.
        var listCompositeRolesResponse = await KeycloakRestClient.Roles
            .ListCompositeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name)
            .ConfigureAwait(false);

        // Validate the response from the composite role listing operation.
        Assert.IsNotNull(listCompositeRolesResponse);
        Assert.IsFalse(listCompositeRolesResponse.IsError);
        Assert.IsNotNull(listCompositeRolesResponse.Response);

        // Ensure that the retrieved composite roles match the expected composite roles.
        Assert.IsFalse(listCompositeRolesResponse.Response.Select(role => role.Name)
            .Except(TestCompositeRoles.Select(role => role.Name)).Any());

        // Validate the monitoring metrics for the successful composite role listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to list composite roles of a role by ID in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task K_ShouldListCompositeRolesById()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Ensure the composite roles are initialized.
        Assert.IsNotNull(TestCompositeRoles);

        // Retrieve an access token for the realm admin to perform the composite role listing operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to list composite roles for the specified role by ID.
        var listCompositeRolesResponse = await KeycloakRestClient.Roles
            .ListCompositeByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Id)
            .ConfigureAwait(false);

        // Validate the response from the composite role listing operation.
        Assert.IsNotNull(listCompositeRolesResponse);
        Assert.IsFalse(listCompositeRolesResponse.IsError);
        Assert.IsNotNull(listCompositeRolesResponse.Response);

        // Ensure that the retrieved composite roles match the expected composite roles.
        Assert.IsFalse(listCompositeRolesResponse.Response.Select(role => role.Name)
            .Except(TestCompositeRoles.Select(role => role.Name)).Any());

        // Validate the monitoring metrics for the successful composite role listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the test composite roles with the retrieved composite roles.
        TestCompositeRoles = listCompositeRolesResponse.Response;
    }

    /// <summary>
    /// Validates the functionality to retrieve realm-level composite roles of a role by name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task L_ShouldGetRealmLevelCompositesByRoleName()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Ensure the composite roles are initialized.
        Assert.IsNotNull(TestCompositeRoles);

        // Retrieve an access token for the realm admin to perform the realm-level composite role retrieval operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve realm-level composite roles for the specified role by name.
        var listCompositeRolesResponse = await KeycloakRestClient.Roles
            .GetRealmLevelCompositesAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name)
            .ConfigureAwait(false);

        // Validate the response from the realm-level composite role retrieval operation.
        Assert.IsNotNull(listCompositeRolesResponse);
        Assert.IsFalse(listCompositeRolesResponse.IsError);
        Assert.IsNotNull(listCompositeRolesResponse.Response);

        // Ensure that the retrieved composite roles match the expected composite roles.
        Assert.IsFalse(listCompositeRolesResponse.Response.Select(role => role.Name)
            .Except(TestCompositeRoles.Select(role => role.Name)).Any());

        // Validate the monitoring metrics for the successful realm-level composite role retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve realm-level composite roles of a role by ID in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task N_ShouldGetRealmLevelCompositesByRoleId()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Ensure the composite roles are initialized.
        Assert.IsNotNull(TestCompositeRoles);

        // Retrieve an access token for the realm admin to perform the realm-level composite role retrieval operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve realm-level composite roles for the specified role by ID.
        var listCompositeRolesResponse = await KeycloakRestClient.Roles
            .GetRealmLevelCompositesByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Id)
            .ConfigureAwait(false);

        // Validate the response from the realm-level composite role retrieval operation.
        Assert.IsNotNull(listCompositeRolesResponse);
        Assert.IsFalse(listCompositeRolesResponse.IsError);
        Assert.IsNotNull(listCompositeRolesResponse.Response);

        // Ensure that the retrieved composite roles match the expected composite roles.
        Assert.IsFalse(listCompositeRolesResponse.Response.Select(role => role.Name)
            .Except(TestCompositeRoles.Select(role => role.Name)).Any());

        // Validate the monitoring metrics for the successful realm-level composite role retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listCompositeRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to delete a composite role by name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task O_ShouldDeleteCompositeRoleByName()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Ensure the composite roles are initialized and contain at least two roles.
        Assert.IsNotNull(TestCompositeRoles);
        Assert.IsTrue(TestCompositeRoles.Count() == 2);

        // Retrieve an access token for the realm admin to perform the composite role deletion operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Identify the composite role to delete.
        var roleName = TestCompositeRoles.First().Name;

        // Execute the operation to delete the specified composite role by name.
        var deleteCompositeRoleResponse = await KeycloakRestClient.Roles
            .DeleteCompositeAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name,
                TestCompositeRoles.Where(role => role.Name == roleName).ToList())
            .ConfigureAwait(false);

        // Validate the response from the composite role deletion operation.
        Assert.IsNotNull(deleteCompositeRoleResponse);
        Assert.IsFalse(deleteCompositeRoleResponse.IsError);

        // Validate the monitoring metrics for the successful composite role deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteCompositeRoleResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);

        // Update the test composite roles by removing the deleted role.
        TestCompositeRoles = TestCompositeRoles.Where(role => role.Name != roleName).ToList();
    }

    /// <summary>
    /// Validates the functionality to delete a composite role by ID in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task P_ShouldDeleteCompositeRoleById()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Ensure the composite roles are initialized and contain exactly one role.
        Assert.IsNotNull(TestCompositeRoles);
        Assert.IsTrue(TestCompositeRoles.Count() == 1);

        // Retrieve an access token for the realm admin to perform the composite role deletion operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the specified composite role by ID.
        var deleteCompositeRoleResponse = await KeycloakRestClient.Roles
            .DeleteCompositeByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Id,
                TestCompositeRoles)
            .ConfigureAwait(false);

        // Validate the response from the composite role deletion operation.
        Assert.IsNotNull(deleteCompositeRoleResponse);
        Assert.IsFalse(deleteCompositeRoleResponse.IsError);

        // Validate the monitoring metrics for the successful composite role deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteCompositeRoleResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates the functionality to retrieve groups associated with a role by name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Q_ShouldGetRoleGroupsByName()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Retrieve an access token for the realm admin to perform the role group retrieval operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve groups associated with the specified role name.
        var getRoleGroupsResponse = await KeycloakRestClient.Roles
            .GetGroupsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name)
            .ConfigureAwait(false);

        // Validate the response from the role group retrieval operation.
        Assert.IsNotNull(getRoleGroupsResponse);
        Assert.IsFalse(getRoleGroupsResponse.IsError);
        Assert.IsNotNull(getRoleGroupsResponse.Response);

        // Ensure that no groups are associated with the role (expected behavior in this scenario).
        Assert.IsFalse(getRoleGroupsResponse.Response.Any());

        // Validate the monitoring metrics for the successful role group retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getRoleGroupsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve authorization permissions associated with a role by name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task R_ShouldGetAuthorizationPermissionsByRoleName()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Retrieve an access token for the realm admin to perform the authorization permission retrieval operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve authorization permissions for the specified role name.
        var getAuthorizationPermissionManagementResponse = await KeycloakRestClient.Roles
            .GetAuthorizationPermissionsAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name)
            .ConfigureAwait(false);

        // Validate the response from the authorization permission retrieval operation.
        Assert.IsNotNull(getAuthorizationPermissionManagementResponse);
        Assert.IsFalse(getAuthorizationPermissionManagementResponse.IsError);
        Assert.IsNotNull(getAuthorizationPermissionManagementResponse.Response);

        // Ensure that authorization permissions are not enabled (expected behavior in this scenario).
        Assert.IsFalse(getAuthorizationPermissionManagementResponse.Response.Enabled);

        // Validate the monitoring metrics for the successful authorization permission retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            getAuthorizationPermissionManagementResponse.MonitoringMetrics, HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve authorization permissions associated with a role by ID in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task V_ShouldGetAuthorizationPermissionsByRoleId()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Retrieve an access token for the realm admin to perform the authorization permission retrieval operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve authorization permissions for the specified role ID.
        var getAuthorizationPermissionManagementResponse = await KeycloakRestClient.Roles
            .GetAuthorizationPermissionsByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestRole.Id)
            .ConfigureAwait(false);

        // Validate the response from the authorization permission retrieval operation.
        Assert.IsNotNull(getAuthorizationPermissionManagementResponse);
        Assert.IsFalse(getAuthorizationPermissionManagementResponse.IsError);
        Assert.IsNotNull(getAuthorizationPermissionManagementResponse.Response);

        // Ensure that authorization permissions are not enabled (expected behavior in this scenario).
        Assert.IsFalse(getAuthorizationPermissionManagementResponse.Response.Enabled);

        // Validate the monitoring metrics for the successful authorization permission retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            getAuthorizationPermissionManagementResponse.MonitoringMetrics, HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve users associated with a role by name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task W_ShouldGetUsersInRoleByName()
    {
        // Ensure the test role is initialized before proceeding.
        Assert.IsNotNull(TestRole);

        // Retrieve an access token for the realm admin to perform the user retrieval operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve users associated with the specified role name.
        var getUsersInRoleResponse = await KeycloakRestClient.Roles
            .GetUserInRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Name)
            .ConfigureAwait(false);

        // Validate the response from the user retrieval operation.
        Assert.IsNotNull(getUsersInRoleResponse);
        Assert.IsFalse(getUsersInRoleResponse.IsError);
        Assert.IsNotNull(getUsersInRoleResponse.Response);

        // Ensure that no users are associated with the role (expected behavior in this scenario).
        Assert.IsFalse(getUsersInRoleResponse.Response.Any());

        // Validate the monitoring metrics for the successful user retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUsersInRoleResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Verifies that a realm role can be successfully deleted by its name in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldDeleteRealmRoleByName()
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
    /// Verifies that attempting to delete a realm role by its ID returns a 404 Not Found response when the role no longer exists in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task ZA_ShouldNotDeleteRealmRoleById()
    {
        // Ensure that the test role is not null
        Assert.IsNotNull(TestRole);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to delete the realm role by its ID
        var deleteRoleResponse = await KeycloakRestClient.Roles
            .DeleteByIdAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestRole.Id)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(deleteRoleResponse);

        // Assert that the response indicates an error (404 Not Found)
        Assert.IsTrue(deleteRoleResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteRoleResponse.MonitoringMetrics,
            HttpStatusCode.NotFound, HttpMethod.Delete, true);
    }
}
