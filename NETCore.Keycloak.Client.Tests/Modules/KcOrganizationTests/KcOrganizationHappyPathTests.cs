using System.Net;
using Bogus;
using NETCore.Keycloak.Client.Models.Organizations;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcOrganizationTests;

/// <summary>
/// Contains integration tests for Keycloak organization management operations.
/// Tests the full lifecycle of organizations including creation, listing, retrieval, updating, counting, and deletion.
/// Organizations are available in Keycloak 26 and above.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcOrganizationHappyPathTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak organization used for testing in happy path scenarios.
    /// </summary>
    private static KcOrganization TestOrganization
    {
        get
        {
            try
            {
                // Retrieve and deserialize the organization object from the environment variable.
                return JsonConvert.DeserializeObject<KcOrganization>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcOrganizationHappyPathTests)}_KC_ORGANIZATION") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcOrganizationHappyPathTests)}_KC_ORGANIZATION",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak organizations module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Organizations);

    /// <summary>
    /// Validates the functionality to create an organization in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreateOrganization()
    {
        // Skip on Keycloak versions below 26 — organizations are not supported.
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        // Retrieve an access token for the realm admin to perform the organization creation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Generate a mock organization.
        var faker = new Faker();
        var kcOrganization = KcOrganizationMocks.Generate(faker);

        // Execute the operation to create the organization.
        var createOrganizationResponse = await KeycloakRestClient.Organizations.CreateAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            kcOrganization).ConfigureAwait(false);

        // Validate the response from the organization creation operation.
        Assert.IsNotNull(createOrganizationResponse);
        Assert.IsFalse(createOrganizationResponse.IsError);

        // Validate the monitoring metrics for the successful organization creation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(createOrganizationResponse.MonitoringMetrics,
            HttpStatusCode.Created, HttpMethod.Post);

        // List organizations to retrieve the created organization with its ID.
        var listOrganizationsResponse = await KeycloakRestClient.Organizations
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcOrganizationFilter
            {
                Exact = true,
                Search = kcOrganization.Name
            }).ConfigureAwait(false);

        // Validate the list response.
        Assert.IsNotNull(listOrganizationsResponse);
        Assert.IsFalse(listOrganizationsResponse.IsError);
        Assert.IsNotNull(listOrganizationsResponse.Response);

        // Update the test organization with the first matching organization from the response.
        TestOrganization = listOrganizationsResponse.Response.First();
    }

    /// <summary>
    /// Validates the functionality to list organizations in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldListOrganizations()
    {
        // Skip on Keycloak versions below 26 — organizations are not supported.
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        // Ensure the test organization is initialized before proceeding.
        Assert.IsNotNull(TestOrganization);

        // Retrieve an access token for the realm admin to perform the organization listing.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to list organizations.
        var listOrganizationsResponse = await KeycloakRestClient.Organizations
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken).ConfigureAwait(false);

        // Validate the response from the organization listing operation.
        Assert.IsNotNull(listOrganizationsResponse);
        Assert.IsFalse(listOrganizationsResponse.IsError);
        Assert.IsNotNull(listOrganizationsResponse.Response);

        // Ensure that at least one organization exists in the results.
        Assert.IsTrue(listOrganizationsResponse.Response.Any());

        // Validate the monitoring metrics for the successful organization listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listOrganizationsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to count organizations in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldCountOrganizations()
    {
        // Skip on Keycloak versions below 26 — organizations are not supported.
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        // Ensure the test organization is initialized before proceeding.
        Assert.IsNotNull(TestOrganization);

        // Retrieve an access token for the realm admin to perform the organization count operation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to count organizations.
        var countOrganizationsResponse = await KeycloakRestClient.Organizations
            .CountAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken).ConfigureAwait(false);

        // Validate the response from the organization count operation.
        Assert.IsNotNull(countOrganizationsResponse);
        Assert.IsFalse(countOrganizationsResponse.IsError);

        // Ensure that the count is greater than zero.
        Assert.IsTrue(countOrganizationsResponse.Response > 0);

        // Validate the monitoring metrics for the successful organization count request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(countOrganizationsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates the functionality to retrieve a specific organization in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldGetOrganization()
    {
        // Skip on Keycloak versions below 26 — organizations are not supported.
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        // Ensure the test organization is initialized before proceeding.
        Assert.IsNotNull(TestOrganization);

        // Retrieve an access token for the realm admin to perform the organization retrieval.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to retrieve the organization by its ID.
        var getOrganizationResponse = await KeycloakRestClient.Organizations
            .GetAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestOrganization.Id)
            .ConfigureAwait(false);

        // Validate the response from the organization retrieval operation.
        Assert.IsNotNull(getOrganizationResponse);
        Assert.IsFalse(getOrganizationResponse.IsError);
        Assert.IsNotNull(getOrganizationResponse.Response);

        // Ensure the response is of the expected organization type.
        Assert.IsInstanceOfType<KcOrganization>(getOrganizationResponse.Response);

        // Ensure the retrieved organization name matches the expected value.
        Assert.AreEqual(TestOrganization.Name, getOrganizationResponse.Response.Name);

        // Validate the monitoring metrics for the successful organization retrieval request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getOrganizationResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the test organization with the retrieved organization details.
        TestOrganization = getOrganizationResponse.Response;
    }

    /// <summary>
    /// Verifies that an organization can be successfully updated in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldUpdateOrganization()
    {
        // Skip on Keycloak versions below 26 — organizations are not supported.
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        // Ensure the test organization is initialized before proceeding.
        Assert.IsNotNull(TestOrganization);

        // Retrieve an access token for the realm admin to perform the organization update.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Modify the organization description.
        var kcOrganization = TestOrganization;
        kcOrganization.Description = "Updated organization description";

        // Execute the operation to update the organization.
        var updateOrganizationResponse = await KeycloakRestClient.Organizations
            .UpdateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestOrganization.Id,
                kcOrganization).ConfigureAwait(false);

        // Validate the response from the organization update operation.
        Assert.IsNotNull(updateOrganizationResponse);
        Assert.IsFalse(updateOrganizationResponse.IsError);

        // Validate the monitoring metrics for the successful organization update request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateOrganizationResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);

        // Update the test organization with the modified details.
        TestOrganization = kcOrganization;
    }

    /// <summary>
    /// Validates the functionality to delete an organization in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldDeleteOrganization()
    {
        // Skip on Keycloak versions below 26 — organizations are not supported.
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        // Ensure the test organization is initialized before proceeding.
        Assert.IsNotNull(TestOrganization);

        // Retrieve an access token for the realm admin to perform the organization deletion.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the specified organization.
        var deleteOrganizationResponse = await KeycloakRestClient.Organizations
            .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestOrganization.Id)
            .ConfigureAwait(false);

        // Validate the response from the organization deletion operation.
        Assert.IsNotNull(deleteOrganizationResponse);
        Assert.IsFalse(deleteOrganizationResponse.IsError);

        // Validate the monitoring metrics for the successful organization deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteOrganizationResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }
}
