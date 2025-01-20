using System.Globalization;
using System.Net;
using Bogus;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcGroupTests;

/// <summary>
/// Contains tests for validating the Keycloak group API functionalities under expected scenarios (Happy Path).
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcGroupsHappyPathTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak group used for testing in happy path scenarios.
    /// </summary>
    private static KcGroup TestGroup
    {
        get
        {
            try
            {
                // Retrieve and deserialize the group object from the environment variable.
                return JsonConvert.DeserializeObject<KcGroup>(
                    Environment.GetEnvironmentVariable($"{nameof(KcGroupsHappyPathTests)}_KC_GROUP") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcGroupsHappyPathTests)}_KC_GROUP",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak group module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Groups);

    /// <summary>
    /// Validates the functionality to create a group in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreateAGroup()
    {
        // Retrieve an access token for the realm admin to perform the group creation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Generate a mock group.
        var kcGroup = new KcGroup
        {
            Name = Guid.NewGuid().ToString().Replace("-", string.Empty, StringComparison.Ordinal),
            Attributes = new Dictionary<string, IEnumerable<string>>
            {
                {
                    "test", [
                        "0"
                    ]
                }
            }
        };

        // Execute the operation to create the group.
        var createGroupResponse = await KeycloakRestClient.Groups.CreateAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            kcGroup).ConfigureAwait(false);

        // Validate the response from the group creation operation.
        Assert.IsNotNull(createGroupResponse);
        Assert.IsFalse(createGroupResponse.IsError);

        // Validate the monitoring metrics for the successful group creation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(createGroupResponse.MonitoringMetrics,
            HttpStatusCode.Created, HttpMethod.Post);

        // Update the test group with the created group.
        TestGroup = kcGroup;
    }

    /// <summary>
    /// Validates the functionality to list groups in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldListGroups()
    {
        // Ensure the test group is initialized before proceeding.
        Assert.IsNotNull(TestGroup);

        // Retrieve an access token for the realm admin to perform the group listing.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to list groups matching the specified filter criteria.
        var listGroupsResponse = await KeycloakRestClient.Groups
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcGroupFilter
            {
                Exact = true,
                Search = TestGroup.Name
            }).ConfigureAwait(false);

        // Validate the response from the group listing operation.
        Assert.IsNotNull(listGroupsResponse);
        Assert.IsFalse(listGroupsResponse.IsError);
        Assert.IsNotNull(listGroupsResponse.Response);

        // Ensure that the test group is included in the results.
        Assert.IsTrue(listGroupsResponse.Response.Any(group => group.Name == TestGroup.Name));

        // Validate the monitoring metrics for the successful group listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listGroupsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the test group with the first matching group from the response.
        TestGroup = listGroupsResponse.Response.First();
    }

    /// <summary>
    /// Validates the functionality to count groups in the Keycloak system based on specific filter criteria.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldCountGroups()
    {
        // Retrieve an access token for the realm admin to perform the group count operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to count groups matching the specified filter criteria.
        var groupsCountResponse = await KeycloakRestClient.Groups.CountAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            new KcGroupFilter
            {
                Exact = true,
                Search = TestGroup.Name
            }).ConfigureAwait(false);

        // Validate the response from the group count operation.
        Assert.IsNotNull(groupsCountResponse);
        Assert.IsFalse(groupsCountResponse.IsError);
        Assert.IsNotNull(groupsCountResponse.Response);

        // Ensure that the group count matches the expected value (1).
        Assert.IsTrue(groupsCountResponse.Response.Count == 1);

        // Validate the monitoring metrics for the successful group count request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(groupsCountResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve a specific group in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldGetAGroup()
    {
        // Ensure the test group is initialized before proceeding.
        Assert.IsNotNull(TestGroup);

        // Retrieve an access token for the realm admin to perform the group retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve the group by its ID.
        var getGroupResponse = await KeycloakRestClient.Groups
            .GetAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id)
            .ConfigureAwait(false);

        // Validate the response from the group retrieval operation.
        Assert.IsNotNull(getGroupResponse);
        Assert.IsFalse(getGroupResponse.IsError);
        Assert.IsNotNull(getGroupResponse.Response);

        // Ensure the response is of the expected group type.
        Assert.IsInstanceOfType<KcGroup>(getGroupResponse.Response);

        // Validate the monitoring metrics for the successful group retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getGroupResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        // Update the test group with the retrieved group details.
        TestGroup = getGroupResponse.Response;
    }

    /// <summary>
    /// Verifies that a group can be successfully updated in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldUpdateAGroup()
    {
        // Ensure that the test group is not null
        Assert.IsNotNull(TestGroup);

        var kcGroup = TestGroup;

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Add a new attribute to the test group
        kcGroup.Attributes.Add("test2", ["0"]);

        // Send the update request for the group
        var updateGroupResponse = await KeycloakRestClient.Groups
            .UpdateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id, TestGroup)
            .ConfigureAwait(false);

        // Assert that the response from the update operation is not null
        Assert.IsNotNull(updateGroupResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(updateGroupResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateGroupResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);

        TestGroup = kcGroup;
    }

    /// <summary>
    /// Verifies that child groups can be successfully created and added to a parent group in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldCreateGroupChildren()
    {
        // Ensure that the test group is not null
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Create a faker instance to generate random data
        var faker = new Faker();

        // Add a new child group with a randomly generated name to the parent group's subgroups
        TestGroup.Subgroups = new List<KcGroup>
        {
            new()
            {
                Name = faker.Random.Word().ToLower(CultureInfo.CurrentCulture)
                    .Replace(" ", string.Empty, StringComparison.Ordinal)
            }
        };

        // Send the request to create or set the child group
        var setOrCreateChildrenResponse = await KeycloakRestClient.Groups
            .SetOrCreateChildAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id, TestGroup)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(setOrCreateChildrenResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(setOrCreateChildrenResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(setOrCreateChildrenResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Verifies that the authorization permission management settings of a group can be retrieved successfully in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldGetGroupAuthorizationPermissionManagement()
    {
        // Ensure that the test group is not null
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve the group's authorization permission management settings
        var getGroupAuthorizationPermissionManagementResponse = await KeycloakRestClient.Groups
            .GetAuthorizationManagementPermissionAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestGroup.Id).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(getGroupAuthorizationPermissionManagementResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(getGroupAuthorizationPermissionManagementResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(getGroupAuthorizationPermissionManagementResponse.Response);

        // Assert that the authorization permission management is not enabled
        Assert.IsFalse(getGroupAuthorizationPermissionManagementResponse.Response.Enabled);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(
            getGroupAuthorizationPermissionManagementResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Verifies that the members of a group can be retrieved successfully from the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task J_ShouldGetGroupMembers()
    {
        // Ensure that the test group is not null
        Assert.IsNotNull(TestGroup);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve the members of the group
        var groupMembersResponse = await KeycloakRestClient.Groups
            .GetMembersAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestGroup.Id)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(groupMembersResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(groupMembersResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(groupMembersResponse.Response);

        // Assert that the group has no members
        Assert.IsFalse(groupMembersResponse.Response.Any());

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(groupMembersResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to delete a group in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldDeleteGroup()
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
}
