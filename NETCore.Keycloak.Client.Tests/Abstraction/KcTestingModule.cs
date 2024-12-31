using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
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
    protected readonly IKeycloakClient KeycloakRestClient;

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
    protected async Task<KcIdentityProviderToken> GetRealmAdminToken(string context)
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
