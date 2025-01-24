using System.Net;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.MockData;
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
    /// Asynchronously creates a Keycloak realm role and retrieves its details.
    /// </summary>
    protected async Task<KcRole> CreateAndGetRealmRoleAsync(string context)
    {
        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Generate a new realm role with a random name and predefined attributes
        var kcRole = KcRoleMocks.Generate();

        // Send the request to create the realm role
        var createRoleResponse = await KeycloakRestClient.Roles
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, kcRole).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(createRoleResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(createRoleResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(createRoleResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);

        // Send the request to list realm roles, filtering by the test role's name
        var listRolesResponse = await KeycloakRestClient.Roles
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcFilter
            {
                Search = kcRole.Name
            }).ConfigureAwait(false);

        // Assert that the response from the operation is not null
        Assert.IsNotNull(listRolesResponse);

        // Assert that the response does not indicate an error
        Assert.IsFalse(listRolesResponse.IsError);

        // Assert that the response contains valid data
        Assert.IsNotNull(listRolesResponse.Response);

        // Assert that the test role is present in the response
        Assert.IsTrue(listRolesResponse.Response.Any(role => role.Name == kcRole.Name));

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(listRolesResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        return listRolesResponse.Response.First(role => role.Name == kcRole.Name);
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
    /// Asynchronously creates a client role in the Keycloak realm and retrieves its details.
    /// </summary>
    protected async Task<KcRole> CreateAndGetClientRoleAsync(string context, KcClient client)
    {
        // Ensure that the test client exists
        Assert.IsNotNull(client);

        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);

        // Ensure that the access token is not null
        Assert.IsNotNull(accessToken);

        // Generate a new client role for testing
        var kcRole = KcRoleMocks.Generate();

        // Ensure that the generated role is not null
        Assert.IsNotNull(kcRole);

        // Send the request to create the client role
        var clientRoleResponse = await KeycloakRestClient.Roles
            .CreateClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, client.Id, kcRole)
            .ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(clientRoleResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(clientRoleResponse.IsError);

        // Ensure that the response content is null as expected for this type of request
        Assert.IsNull(clientRoleResponse.Response);

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientRoleResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);

        // Send the request to list client roles filtered by the role's name
        var listClientRolesResponse = await KeycloakRestClient.Roles
            .ListClientRoleAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, client.Id, new KcFilter
            {
                Search = kcRole.Name
            }).ConfigureAwait(false);

        // Ensure that the response is not null
        Assert.IsNotNull(listClientRolesResponse);

        // Ensure the response does not indicate an error
        Assert.IsFalse(listClientRolesResponse.IsError);

        // Ensure that the response contains a list of client roles
        Assert.IsNotNull(listClientRolesResponse.Response);

        // Ensure that the test role is present in the response
        Assert.IsTrue(listClientRolesResponse.Response.Any(role => role.Name == kcRole.Name));

        // Validate monitoring metrics for the request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientRolesResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        return listClientRolesResponse.Response.First(role => role.Name == kcRole.Name);
    }

    /// <summary>
    /// Creates a new user in the specified Keycloak realm and retrieves the created user based on the provided context.
    /// </summary>
    protected async Task<KcUser> CreateAndGetRealmUserAsync(string context, string password = null,
        IDictionary<string, object> attributes = null)
    {
        // Create a faker instance for generating random data
        var faker = new Faker();

        // Generate a new user using mock data
        var user = KcUserMocks.GenerateUser(faker, password, attributes);

        // Ensure the generated user is not null
        Assert.IsNotNull(user);

        // Retrieve an access token for the realm admin to perform the user creation
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Execute the operation to create the user in the specified realm
        var createUserResponse = await KeycloakRestClient.Users
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, user)
            .ConfigureAwait(false);

        // Validate the response from the user creation operation
        Assert.IsNotNull(createUserResponse);
        Assert.IsFalse(createUserResponse.IsError);

        // Validate the monitoring metrics for the successful user creation request
        KcCommonAssertion.AssertResponseMonitoringMetrics(createUserResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);

        // Execute the operation to list users matching the specified filter criteria
        var listUsersResponse = await KeycloakRestClient.Users.ListUserAsync(
            TestEnvironment.TestingRealm.Name,
            accessToken.AccessToken,
            new KcUserFilter
            {
                Email = user.Email,
                Exact = true
            }).ConfigureAwait(false);

        // Validate the response from the user listing operation
        Assert.IsNotNull(listUsersResponse);
        Assert.IsFalse(listUsersResponse.IsError);
        Assert.IsNotNull(listUsersResponse.Response);

        // Ensure that the response contains exactly one user matching the filter criteria
        Assert.IsTrue(listUsersResponse.Response.Count() == 1);

        // Validate the monitoring metrics for the successful user listing request
        KcCommonAssertion.AssertResponseMonitoringMetrics(listUsersResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        return listUsersResponse.Response.First();
    }

    /// <summary>
    /// Asynchronously creates a Keycloak client with generated data and retrieves the created client details.
    /// </summary>
    protected async Task<KcClient> CreateAndGetClientAsync(string context)
    {
        // Retrieve the realm admin token for authentication.
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Use Faker to generate realistic test data for the Keycloak client.
        var faker = new Faker();

        // Define the Keycloak client object with properties and attributes.
        var kClient = KcClientMocks.GeneratePublicClient(faker);
        Assert.IsNotNull(kClient);

        // Create the Keycloak client via the REST API.
        var clientResponse = await KeycloakRestClient.Clients
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, kClient).ConfigureAwait(false);
        Assert.IsNotNull(clientResponse);
        Assert.IsFalse(clientResponse.IsError);

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientResponse.MonitoringMetrics, HttpStatusCode.Created,
            HttpMethod.Post);

        // List clients matching the created client's name.
        var clientsResponse = await KeycloakRestClient.Clients
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcClientFilter
            {
                Search = kClient.Name
            }).ConfigureAwait(false);
        Assert.IsNotNull(clientsResponse);
        Assert.IsFalse(clientsResponse.IsError);
        Assert.IsNotNull(clientsResponse.Response);
        Assert.IsTrue(clientsResponse.Response.Any(client => client.ClientId == kClient.ClientId));

        // Validate monitoring metrics for the successful request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(clientsResponse.MonitoringMetrics, HttpStatusCode.OK,
            HttpMethod.Get);

        return clientsResponse.Response.FirstOrDefault(client => client.ClientId == kClient.ClientId);
    }

    /// <summary>
    /// Asynchronously creates a Keycloak client scope with generated data and retrieves the created client scope details.
    /// </summary>
    protected async Task<KcClientScope> CreateAndGetClientScopeAsync(string context)
    {
        // Retrieve the realm administrator token
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);
        Assert.IsNotNull(accessToken);

        // Create a faker instance to generate random data
        var faker = new Faker();

        // Define the client scope to be created
        var clientScope = new KcClientScope
        {
            Description = faker.Random.Words(3),
            Name = Guid.NewGuid().ToString().Replace("-", string.Empty, StringComparison.Ordinal),
            Protocol = KcProtocol.OpenidConnect
        };

        // Send the request to create the client scope
        var createClientScopeResponse = await KeycloakRestClient.ClientScopes
            .CreateAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, clientScope).ConfigureAwait(false);
        Assert.IsNotNull(createClientScopeResponse);
        Assert.IsFalse(createClientScopeResponse.IsError);

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(createClientScopeResponse.MonitoringMetrics,
            HttpStatusCode.Created, HttpMethod.Post);

        // Send the request to list client scopes
        var listClientScopesResponse = await KeycloakRestClient.ClientScopes
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken).ConfigureAwait(false);
        Assert.IsNotNull(listClientScopesResponse);
        Assert.IsFalse(listClientScopesResponse.IsError);
        Assert.IsNotNull(listClientScopesResponse.Response);

        // Assert that the test client scope is present in the list
        Assert.IsTrue(listClientScopesResponse.Response.Any(scope => scope.Name == clientScope.Name));

        // Validate the response monitoring metrics
        KcCommonAssertion.AssertResponseMonitoringMetrics(listClientScopesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        return listClientScopesResponse.Response.First(scope => scope.Name == clientScope.Name);
    }

    /// <summary>
    /// Asynchronously creates a group in the Keycloak realm and retrieves its details.
    /// </summary>
    protected async Task<KcGroup> CreateAndGetGroupAsync(string context)
    {
        // Retrieve an access token for the realm admin to perform the group creation.
        var accessToken = await GetRealmAdminTokenAsync(context).ConfigureAwait(false);
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

        // Execute the operation to list groups matching the specified filter criteria.
        var listGroupsResponse = await KeycloakRestClient.Groups
            .ListAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, new KcGroupFilter
            {
                Exact = true,
                Search = kcGroup.Name
            }).ConfigureAwait(false);

        // Validate the response from the group listing operation.
        Assert.IsNotNull(listGroupsResponse);
        Assert.IsFalse(listGroupsResponse.IsError);
        Assert.IsNotNull(listGroupsResponse.Response);

        // Ensure that the test group is included in the results.
        Assert.IsTrue(listGroupsResponse.Response.Any(group => group.Name == kcGroup.Name));

        // Validate the monitoring metrics for the successful group listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(listGroupsResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        return listGroupsResponse.Response.First();
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
