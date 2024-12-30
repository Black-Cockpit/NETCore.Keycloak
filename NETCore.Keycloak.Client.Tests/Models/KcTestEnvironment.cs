using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Models;

/// <summary>
/// Represents the test environment configuration for Keycloak integration tests.
/// This class maps to a JSON configuration file for setting up the Keycloak environment.
/// </summary>
public class KcTestEnvironment
{
    /// <summary>
    /// The base URL of the Keycloak server.
    /// Example: "http://localhost:8080"
    /// </summary>
    [JsonProperty("baseUrl")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// The name of the authentication realm in Keycloak.
    /// Typically, this is the master realm used for administrative tasks.
    /// Example: "master"
    /// </summary>
    [JsonProperty("auth_realm")]
    public string AuthRealm { get; set; }

    /// <summary>
    /// The client ID used for authentication in the Keycloak master realm.
    /// Example: "admin-cli"
    /// </summary>
    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    /// <summary>
    /// The username of the Keycloak administrator for the master realm.
    /// Example: "admin"
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// The password of the Keycloak administrator for the master realm.
    /// Example: "wF21EC3BQGM2pCE6unA57ZQbw5wTI900"
    /// </summary>
    [JsonProperty("password")]
    public string Password { get; set; }

    /// <summary>
    /// The configuration for the testing realm in Keycloak.
    /// This includes the realm name, client ID, and administrator credentials.
    /// </summary>
    [JsonProperty("testing_realm")]
    public KcTestRealm TestingRealm { get; set; }
}
