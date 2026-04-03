using System.Net;
using NETCore.Keycloak.Client.Models.Organizations;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcOrganizationTests;

/// <summary>
/// Contains integration tests for Keycloak organization member management operations.
/// Tests the full lifecycle of organization members including adding, listing, retrieving,
/// counting, getting member organizations, and removing members.
/// Organizations are available in Keycloak 26 and above.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcOrganizationMemberHappyPathTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak organization used for testing member operations.
    /// </summary>
    private static KcOrganization TestOrganization
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<KcOrganization>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcOrganizationMemberHappyPathTests)}_KC_ORGANIZATION") ?? string.Empty);
            }
            catch ( Exception e )
            {
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable(
            $"{nameof(KcOrganizationMemberHappyPathTests)}_KC_ORGANIZATION",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak user used for testing member operations.
    /// </summary>
    private static KcUser TestUser
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcOrganizationMemberHappyPathTests)}_KC_USER") ?? string.Empty);
            }
            catch ( Exception e )
            {
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable(
            $"{nameof(KcOrganizationMemberHappyPathTests)}_KC_USER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.Organizations);

    /// <summary>
    /// Creates an organization and a user for the member tests.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldSetupOrganizationAndUser()
    {
        // Skip on Keycloak versions below 26 — organizations are not supported.
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        // Create an organization and user for member testing.
        var organization = await CreateAndGetOrganizationAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(organization);
        TestOrganization = organization;

        // Create a user to be added as a member.
        var user = await CreateAndGetRealmUserAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(user);
        TestUser = user;
    }

    /// <summary>
    /// Validates adding a user as a member of an organization.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldAddMemberToOrganization()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var addMemberResponse = await KeycloakRestClient.Organizations.AddMemberAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id,
            TestUser.Id).ConfigureAwait(false);

        Assert.IsNotNull(addMemberResponse);
        Assert.IsFalse(addMemberResponse.IsError);

        KcCommonAssertion.AssertResponseMonitoringMetrics(addMemberResponse.MonitoringMetrics,
            HttpStatusCode.Created, HttpMethod.Post);
    }

    /// <summary>
    /// Validates listing members of an organization.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldGetOrganizationMembers()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var getMembersResponse = await KeycloakRestClient.Organizations.GetMembersAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id).ConfigureAwait(false);

        Assert.IsNotNull(getMembersResponse);
        Assert.IsFalse(getMembersResponse.IsError);
        Assert.IsNotNull(getMembersResponse.Response);
        Assert.IsTrue(getMembersResponse.Response.Any());

        KcCommonAssertion.AssertResponseMonitoringMetrics(getMembersResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates listing members with a filter.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldGetOrganizationMembersWithFilter()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var getMembersResponse = await KeycloakRestClient.Organizations.GetMembersAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id,
            new KcOrganizationMemberFilter
            {
                Search = TestUser.Email,
                Exact = true,
                Max = 10
            }).ConfigureAwait(false);

        Assert.IsNotNull(getMembersResponse);
        Assert.IsFalse(getMembersResponse.IsError);
        Assert.IsNotNull(getMembersResponse.Response);
        Assert.IsTrue(getMembersResponse.Response.Any(m => m.Email == TestUser.Email));

        KcCommonAssertion.AssertResponseMonitoringMetrics(getMembersResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates counting members of an organization.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldGetOrganizationMembersCount()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var countResponse = await KeycloakRestClient.Organizations.GetMembersCountAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id).ConfigureAwait(false);

        Assert.IsNotNull(countResponse);
        Assert.IsFalse(countResponse.IsError);
        Assert.IsTrue(countResponse.Response > 0);

        KcCommonAssertion.AssertResponseMonitoringMetrics(countResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates retrieving a specific member by ID.
    /// </summary>
    [TestMethod]
    public async Task F_ShouldGetOrganizationMember()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var getMemberResponse = await KeycloakRestClient.Organizations.GetMemberAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id,
            TestUser.Id).ConfigureAwait(false);

        Assert.IsNotNull(getMemberResponse);
        Assert.IsFalse(getMemberResponse.IsError);
        Assert.IsNotNull(getMemberResponse.Response);
        Assert.IsInstanceOfType<KcUser>(getMemberResponse.Response);
        Assert.AreEqual(TestUser.Email, getMemberResponse.Response.Email);

        KcCommonAssertion.AssertResponseMonitoringMetrics(getMemberResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates retrieving organizations for a member within an organization context.
    /// </summary>
    [TestMethod]
    public async Task G_ShouldGetMemberOrganizations()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var memberOrgsResponse = await KeycloakRestClient.Organizations.GetMemberOrganizationsAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id,
            TestUser.Id).ConfigureAwait(false);

        Assert.IsNotNull(memberOrgsResponse);
        Assert.IsFalse(memberOrgsResponse.IsError);
        Assert.IsNotNull(memberOrgsResponse.Response);
        Assert.IsTrue(memberOrgsResponse.Response.Any(o => o.Id == TestOrganization.Id));

        KcCommonAssertion.AssertResponseMonitoringMetrics(memberOrgsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates retrieving organizations for a user using the top-level endpoint.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldGetUserOrganizations()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var userOrgsResponse = await KeycloakRestClient.Organizations.GetUserOrganizationsAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestUser.Id).ConfigureAwait(false);

        Assert.IsNotNull(userOrgsResponse);
        Assert.IsFalse(userOrgsResponse.IsError);
        Assert.IsNotNull(userOrgsResponse.Response);
        Assert.IsTrue(userOrgsResponse.Response.Any(o => o.Id == TestOrganization.Id));

        KcCommonAssertion.AssertResponseMonitoringMetrics(userOrgsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);
    }

    /// <summary>
    /// Validates removing a member from an organization.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldRemoveMemberFromOrganization()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var removeMemberResponse = await KeycloakRestClient.Organizations.RemoveMemberAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id,
            TestUser.Id).ConfigureAwait(false);

        Assert.IsNotNull(removeMemberResponse);
        Assert.IsFalse(removeMemberResponse.IsError);

        KcCommonAssertion.AssertResponseMonitoringMetrics(removeMemberResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }

    /// <summary>
    /// Validates inviting an existing user to an organization.
    /// This test requires SMTP to be configured in the Keycloak realm,
    /// as the invite endpoint sends an email to the user.
    /// </summary>
    [TestMethod]
    [TestCategory("RequiresSMTP")]
    public async Task J_ShouldInviteExistingUserToOrganization()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        var inviteResponse = await KeycloakRestClient.Organizations.InviteExistingUserAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id,
            TestUser.Id).ConfigureAwait(false);

        Assert.IsNotNull(inviteResponse);

        // The invite endpoint requires SMTP configuration.
        // If SMTP is not configured, Keycloak returns 500 with "Failed to send invite email".
        if ( inviteResponse.IsError &&
             inviteResponse.ErrorMessage?.Contains("Failed to send invite email",
                 StringComparison.OrdinalIgnoreCase) == true )
        {
            Assert.Inconclusive("Skipped — SMTP not configured in Keycloak realm.");
        }

        Assert.IsFalse(inviteResponse.IsError);

        KcCommonAssertion.AssertResponseMonitoringMetrics(inviteResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Cleans up by deleting the test user and organization.
    /// </summary>
    [TestMethod]
    public async Task K_ShouldCleanup()
    {
        if ( GetKcMajorVersion() < 26 )
        {
            Assert.Inconclusive("Skipped — organizations require Keycloak 26 or above.");
        }

        Assert.IsNotNull(TestOrganization);
        Assert.IsNotNull(TestUser);

        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Delete the test user.
        var deleteUserResponse = await KeycloakRestClient.Users.DeleteAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestUser.Id).ConfigureAwait(false);

        Assert.IsNotNull(deleteUserResponse);
        Assert.IsFalse(deleteUserResponse.IsError);

        // Delete the test organization.
        var deleteOrgResponse = await KeycloakRestClient.Organizations.DeleteAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            TestOrganization.Id).ConfigureAwait(false);

        Assert.IsNotNull(deleteOrgResponse);
        Assert.IsFalse(deleteOrgResponse.IsError);
    }
}
