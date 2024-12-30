using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Models;

/// <summary>
/// Represents the configuration for the testing realm in Keycloak integration tests.
/// This class maps to a JSON object containing the details of a Keycloak testing realm.
/// </summary>
public class KcTestRealm
{
    /// <summary>
    /// The name of the testing realm in Keycloak.
    /// Example: "net_core_keycloak_tests"
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The client ID associated with the testing realm.
    /// Used for authenticating applications in the test environment.
    /// Example: "app_client"
    /// </summary>
    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    /// <summary>
    /// The username of the administrator for the testing realm.
    /// Example: "net_core_keycloak_tests_admin"
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// The password for the administrator of the testing realm.
    /// Example: "6W)+P6E7ek@CdklQ#S$0Swlu$4#P&YuK"
    /// </summary>
    [JsonProperty("password")]
    public string Password { get; set; }
}
