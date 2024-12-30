using NETCore.Keycloak.Client.Models.Auth;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Models;

/// <summary>
/// Represents the test environment configuration for Keycloak integration tests.
/// This class maps to a JSON configuration file used to set up the Keycloak environment,
/// including server details, administrative credentials, and testing realm configuration.
/// </summary>
public class KcTestEnvironment
{
    /// <summary>
    /// Gets or sets the base URL of the Keycloak server.
    /// Example: "http://localhost:8080"
    /// </summary>
    [JsonProperty("baseUrl")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the name of the authentication realm in Keycloak.
    /// Typically, this is the master realm used for administrative tasks.
    /// Example: "master"
    /// </summary>
    [JsonProperty("auth_realm")]
    public string AuthRealm { get; set; }

    /// <summary>
    /// Gets or sets the client ID used for authentication in the Keycloak master realm.
    /// Example: "admin-cli"
    /// </summary>
    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the administrative credentials for the master realm.
    /// Includes login details required for administrative operations.
    /// </summary>
    [JsonProperty("user")]
    public KcUserLogin Admin { get; set; }

    /// <summary>
    /// Gets or sets the configuration for the testing realm in Keycloak.
    /// This includes details such as the realm name, clients, and test users.
    /// </summary>
    [JsonProperty("testing_realm")]
    public KcTestRealm TestingRealm { get; set; }
}
