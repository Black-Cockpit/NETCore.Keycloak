using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Tests.Models;
using NETCore.Keycloak.Client.Tests.Modules;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Abstraction;

/// <summary>
/// Base class for Keycloak testing modules, providing a shared testing environment configuration
/// and utility methods for Keycloak integration tests.
/// This class is intended to be inherited by specific Keycloak test modules to standardize setup and test execution.
/// </summary>
public abstract class KcTestingModule
{
    /// <summary>
    /// Indicates whether the admin token has been initialized.
    /// This ensures that the token is only fetched once during the test lifecycle.---
    /// </summary>
    private bool _isAdminTokenInitialized;

    /// <summary>
    /// Instance of the <see cref="IKeycloakClient"/> used to perform Keycloak authentication operations.
    /// </summary>
    protected IKeycloakClient KeycloakRestClient;

    /// <summary>
    /// Represents the testing environment configuration for Keycloak integration tests.
    /// Loaded from the `Assets/testing_environment.json` file.
    /// </summary>
    protected KcTestEnvironment TestEnvironment = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="KcTestingModule"/> class.
    /// Performs setup operations, including loading configuration, initializing a mock logger,
    /// and setting up the Keycloak client.
    /// </summary>
    protected KcTestingModule()
    {
        // Load the test environment configuration from the base module.
        LoadConfiguration();

        // Initialize the mock logger.
        var mockLogger = new Mock<ILogger>();
        _ = mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Initialize the Keycloak client using the configured base URL and mock logger.
        KeycloakRestClient = new KeycloakClient(TestEnvironment.BaseUrl, mockLogger.Object);
    }

    /// <summary>
    /// Retrieves the realm admin token for a specific context.
    /// If the token is already initialized, it retrieves the token from the environment variable.
    /// Otherwise, it fetches the token using the resource owner password credentials flow.
    /// </summary>
    /// <param name="context">The context name used for managing environment variables.</param>
    /// <returns>A task representing the asynchronous operation, with a result of <see cref="KcIdentityProviderToken"/>.</returns>
    protected async Task<KcIdentityProviderToken> GetRealmAdminTokenAsync(string context)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(context), "The context must not be null or empty.");

        if ( !_isAdminTokenInitialized )
        {
            // Act
            var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
                TestEnvironment.TestingRealm.Name,
                new KcClientCredentials
                {
                    ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
                }, new KcUserLogin
                {
                    Username = TestEnvironment.TestingRealm.User.Username,
                    Password = TestEnvironment.TestingRealm.User.Password
                }).ConfigureAwait(false);

            // Assert
            KcCommonAssertion.AssertIdentityProviderTokenResponse(tokenResponse);

            // Validate monitoring metrics for the successful request.
            KcCommonAssertion.AssertResponseMonitoringMetrics(tokenResponse.MonitoringMetrics, HttpStatusCode.OK,
                HttpMethod.Post);

            // Store the token in the environment variable for reuse.
            Environment.SetEnvironmentVariable($"{context.ToUpperInvariant()}_KC_ADMIN_TOKEN",
                JsonConvert.SerializeObject(tokenResponse.Response));

            _isAdminTokenInitialized = true;

            return tokenResponse.Response;
        }

        try
        {
            // Retrieve the token from the environment variable.
            return JsonConvert.DeserializeObject<KcIdentityProviderToken>(
                Environment.GetEnvironmentVariable($"{context.ToUpperInvariant()}_KC_ADMIN_TOKEN") ??
                string.Empty);
        }
        catch ( Exception e )
        {
            Assert.Fail($"Failed to deserialize admin token: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Creates a new realm role in the Keycloak system asynchronously, using the specified context.
    /// </summary>
    protected async Task CreateRealmRoleAsync(string context, KcRole role)
    {
        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Ensure that the realm role object is not null
        Assert.IsNotNull(role);

        // Send the request to create the realm role
        var createRoleResponse = await KeycloakRestClient.Roles
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, role).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(createRoleResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(createRoleResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(createRoleResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);
    }

    /// <summary>
    /// Retrieves a list of all realm roles in the Keycloak system asynchronously, using the specified context.
    /// </summary>
    protected async Task<IEnumerable<KcRole>> ListRealmRolesAsync(string context)
    {
        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Send the request to list realm roles
        var listRolesResponse = await KeycloakRestClient.Roles
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(listRolesResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(listRolesResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(listRolesResponse.Response);

        return listRolesResponse.Response;
    }

    /// <summary>
    /// Loads the test environment configuration from the `Assets/testing_environment.json` file.
    /// The loaded configuration is deserialized into the <see cref="KcTestEnvironment"/> object.
    /// Ensures that the configuration is not null after loading.
    /// </summary>
    protected void LoadConfiguration()
    {
        using var sr =
            new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Assets/testing_environment.json"));
        TestEnvironment = JsonConvert.DeserializeObject<KcTestEnvironment>(sr.ReadToEnd());

        Assert.IsNotNull(TestEnvironment, "The test environment configuration must not be null.");
    }
}
