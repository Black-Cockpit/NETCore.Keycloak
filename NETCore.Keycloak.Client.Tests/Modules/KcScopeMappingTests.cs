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
[TestCategory("Combined")]
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
    /// Tests the creation of a Keycloak client scope and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task A_CreateClientScope() =>
        TestClientScope = await CreateAndGetClientScopeAsync(TestContext).ConfigureAwait(false);

    /// <summary>
    /// Tests the creation of a Keycloak realm role and verifies that it is created successfully.
    /// </summary>
    [TestMethod]
    public async Task B_CreateRole() => TestRole = await CreateAndGetRealmRoleAsync(TestContext).ConfigureAwait(false);

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
}
