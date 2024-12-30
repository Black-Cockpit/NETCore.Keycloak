using NETCore.Keycloak.Client.Models.Auth;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Models;

/// <summary>
/// Represents the configuration for the testing realm in Keycloak integration tests.
/// This class is used to deserialize JSON objects containing details of a Keycloak testing realm,
/// including clients and users associated with the realm.
/// </summary>
public class KcTestRealm
{
    /// <summary>
    /// Gets or sets the name of the testing realm in Keycloak.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the public client credentials for the testing realm.
    /// Public clients do not require a secret for authentication.
    /// </summary>
    [JsonProperty("public_client")]
    public KcClientCredentials PublicClient { get; set; }

    /// <summary>
    /// Gets or sets the private client credentials for the testing realm.
    /// Private clients require a secret for authentication.
    /// </summary>
    [JsonProperty("private_client")]
    public KcClientCredentials PrivateClient { get; set; }

    /// <summary>
    /// Gets or sets the test user configuration associated with the testing realm.
    /// Includes login details and permissions for the user.
    /// </summary>
    [JsonProperty("user")]
    public KcTestUser User { get; set; }
}
