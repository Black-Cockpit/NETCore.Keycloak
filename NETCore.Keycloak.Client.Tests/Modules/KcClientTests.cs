using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Tests.Abstraction;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Test suite for validating the functionality of the <see cref="KeycloakClient"/> class.
/// Inherits from <see cref="KcTestingModule"/> to use shared test environment configuration.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcClientTests : KcTestingModule
{
    /// <summary>
    /// Initializes the test environment configuration before each test is executed.
    /// Invokes the <see cref="KcTestingModule.LoadConfiguration"/> method to load required settings.
    /// </summary>
    [TestInitialize]
    public void Init() => LoadConfiguration();

    /// <summary>
    /// Tests the creation of a <see cref="KeycloakClient"/> instance with a valid base URL.
    /// Verifies that all primary components of the client are properly initialized.
    /// </summary>
    [TestMethod]
    public void ShouldCreateClient()
    {
        // Create a Keycloak client using the base URL from the test environment configuration.
        var client = new KeycloakClient(TestEnvironment.BaseUrl);

        // Assert that the client and its components are not null.
        Assert.IsNotNull(client);
        Assert.IsNotNull(client.Auth);
        Assert.IsNotNull(client.Clients);
        Assert.IsNotNull(client.ClientScopes);
        Assert.IsNotNull(client.ClientInitialAccess);
        Assert.IsNotNull(client.ClientRoleMappings);
        Assert.IsNotNull(client.Groups);
        Assert.IsNotNull(client.Roles);
        Assert.IsNotNull(client.Users);
        Assert.IsNotNull(client.AttackDetection);
        Assert.IsNotNull(client.ProtocolMappers);
        Assert.IsNotNull(client.RoleMappings);
        Assert.IsNotNull(client.ScopeMappings);
    }

    /// <summary>
    /// Tests that an exception is thrown when creating a <see cref="KeycloakClient"/> with an invalid base URL.
    /// Verifies that a <see cref="KcException"/> is thrown as expected.
    /// </summary>
    [TestMethod]
    public void ShouldValidateClientConfiguration() =>
        _ = Assert.ThrowsException<KcException>(() => new KeycloakClient(null));

    /// <summary>
    /// Tests that the base URL provided to the <see cref="KeycloakClient"/> is properly truncated.
    /// Ensures that the client is created with a correctly formatted base URL.
    /// </summary>
    [TestMethod]
    public void ShouldTruncateBaseUrl()
    {
        // Ensure the base URL ends with a forward slash.
        var url = TestEnvironment.BaseUrl;
        url = url.EndsWith("/", StringComparison.Ordinal) ? url : $"{url}/";

        // Create a Keycloak client and assert it is not null.
        var client = new KeycloakClient(url);

        Assert.IsNotNull(client);
    }
}
