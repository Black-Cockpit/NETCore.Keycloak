using System.Globalization;
using System.Net;
using Bogus;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcClientScopeTests;

/// <summary>
/// Contains tests for validating the Keycloak client scope API functionalities under expected scenarios (Happy Path).
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcClientScopeHappyPathTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak client scope used for testing in happy path scenarios.
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
                        $"{nameof(KcClientScopeHappyPathTests)}_KC_CLIENT_SCOPE") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientScopeHappyPathTests)}_KC_CLIENT_SCOPE",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak client scope module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.ClientScopes);

    /// <summary>
    /// Verifies that a new client scope can be successfully created in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreateClientScope()
    {
        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Create a faker instance to generate random data
        var faker = new Faker();

        // Define the client scope to be created
        var clientScope = new KcClientScope
        {
            Description = faker.Random.Words(3),
            Name = faker.Random.Word().ToLower(CultureInfo.CurrentCulture)
                .Replace(" ", string.Empty, StringComparison.Ordinal),
            Protocol = KcProtocol.OpenidConnect
        };

        // Update the TestClientScope property with the defined client scope
        TestClientScope = clientScope;

        // Send the request to create the client scope
        var createClientScopeResponse = await KeycloakRestClient.ClientScopes
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, clientScope).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(createClientScopeResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(createClientScopeResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(createClientScopeResponse.MonitoringMetrics,
            HttpStatusCode.Created, HttpMethod.Post);
    }

    /// <summary>
    /// Verifies that client scopes can be successfully listed from the Keycloak system,
    /// and checks that a specific test client scope exists within the list.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldListClientScopes()
    {
        // Ensure that the test client scope is not null
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list client scopes
        var listClientScopesResponse = await KeycloakRestClient.ClientScopes
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(listClientScopesResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(listClientScopesResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(listClientScopesResponse.Response);

        // Assert that the test client scope is present in the list
        Assert.IsTrue(listClientScopesResponse.Response.Any(scope => scope.Name == TestClientScope.Name));

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientScopesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the TestClientScope property with the matching scope from the response
        TestClientScope = listClientScopesResponse.Response.First(scope => scope.Name == TestClientScope.Name);
    }

    /// <summary>
    /// Verifies that a specific client scope can be retrieved from the Keycloak system by its unique identifier (ID).
    /// </summary>
    [TestMethod]
    public async Task C_ShouldGetClientScopes()
    {
        // Ensure that the test client scope is not null
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to retrieve the client scope by its ID
        var getClientScopeResponse = await KeycloakRestClient.ClientScopes
            .GetAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(getClientScopeResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(getClientScopeResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(getClientScopeResponse.Response);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(getClientScopeResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the TestClientScope property with the retrieved client scope
        TestClientScope = getClientScopeResponse.Response;
    }

    /// <summary>
    /// Verifies that a client scope can be successfully updated in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task D_ShouldUpdateClientScope()
    {
        // Ensure that the test client scope is not null
        Assert.IsNotNull(TestClientScope);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Create a faker instance to generate random data
        var faker = new Faker();

        // Clone the test client scope and update its name
        var clientScope = TestClientScope;
        clientScope.Name = faker.Random.Word().ToLower(CultureInfo.CurrentCulture)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        // Update the TestClientScope property with the modified scope
        TestClientScope = clientScope;

        // Send the request to update the client scope
        var updateClientScopeResponse = await KeycloakRestClient.ClientScopes
            .UpdateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, TestClientScope.Id, clientScope)
            .ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(updateClientScopeResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(updateClientScopeResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(updateClientScopeResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Put);
    }

    /// <summary>
    /// Verifies that a client scope can be successfully deleted from the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldDeleteClientScope()
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
}
