using System.Net;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Contains tests for validating the Keycloak client initial access token functionalities.
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcClientInitialAccessTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak client initial access token used for testing.
    /// </summary>
    private static KcClientInitialAccessModel TestClientInitialAccess
    {
        get
        {
            try
            {
                // Retrieve and deserialize the client initial access token object from the environment variable.
                return JsonConvert.DeserializeObject<KcClientInitialAccessModel>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcClientInitialAccessTests)}_KC_CLIENT_INITIAL_ACCESS") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcClientInitialAccessTests)}_KC_CLIENT_INITIAL_ACCESS",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak client initial access module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.ClientInitialAccess);

    /// <summary>
    /// Validates the functionality to create a client initial access token in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task A_ShouldCreateClientInitialAccess()
    {
        // Retrieve an access token for the realm admin to perform the client initial access token creation.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to create a client initial access token.
        var createClientInitialAccessResponse = await KeycloakRestClient.ClientInitialAccess
            .CreateInitialAccessTokenAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                new KcCreateClientInitialAccess
                {
                    Count = 1,
                    Expiration = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                }).ConfigureAwait(false);

        // Validate the response from the client initial access token creation operation.
        Assert.IsNotNull(createClientInitialAccessResponse);
        Assert.IsFalse(createClientInitialAccessResponse.IsError);
        Assert.IsNotNull(createClientInitialAccessResponse.Response);

        // Ensure the response is of the expected type.
        Assert.IsInstanceOfType<KcClientInitialAccessModel>(createClientInitialAccessResponse.Response);

        // Validate the monitoring metrics for the successful token creation request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(createClientInitialAccessResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Post);

        // Update the test client initial access with the created token.
        TestClientInitialAccess = createClientInitialAccessResponse.Response;
    }

    /// <summary>
    /// Validates the functionality to list client initial access tokens in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldListClientInitialAccess()
    {
        // Retrieve an access token for the realm admin to perform the client initial access token listing.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to list client initial access tokens.
        var listClientInitialAccessResponse = await KeycloakRestClient.ClientInitialAccess
            .GetInitialAccessAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken)
            .ConfigureAwait(false);

        // Validate the response from the client initial access token listing operation.
        Assert.IsNotNull(listClientInitialAccessResponse);
        Assert.IsFalse(listClientInitialAccessResponse.IsError);
        Assert.IsNotNull(listClientInitialAccessResponse.Response);

        // Ensure the response contains at least one token.
        Assert.IsTrue(listClientInitialAccessResponse.Response.Any());

        // Validate the monitoring metrics for the successful token listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientInitialAccessResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Update the test client initial access with the first retrieved token.
        TestClientInitialAccess = listClientInitialAccessResponse.Response.First();
    }

    /// <summary>
    /// Validates the functionality to delete a client initial access token in the Keycloak system.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldDeleteClientInitialAccess()
    {
        // Ensure the test client initial access is initialized before proceeding.
        Assert.IsNotNull(TestClientInitialAccess);

        // Retrieve an access token for the realm admin to perform the client initial access token deletion.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to delete the specified client initial access token.
        var deleteClientInitialAccessResponse = await KeycloakRestClient.ClientInitialAccess
            .DeleteInitialAccessTokenAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken,
                TestClientInitialAccess.Id).ConfigureAwait(false);

        // Validate the response from the client initial access token deletion operation.
        Assert.IsNotNull(deleteClientInitialAccessResponse);
        Assert.IsFalse(deleteClientInitialAccessResponse.IsError);

        // Validate the monitoring metrics for the successful token deletion request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(deleteClientInitialAccessResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Delete);
    }
}
