using NETCore.Keycloak.Client.Models.Auth;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Models;

/// <summary>
/// Represents a test user in Keycloak integration tests.
/// Inherits from <see cref="KcUserLogin"/> to include login credentials
/// and extends functionality with assigned permissions.
/// </summary>
public class KcTestUser : KcUserLogin
{
    /// <summary>
    /// Gets or sets the permissions assigned to the test user.
    /// Each permission is represented by a <see cref="KcTestResource"/>, which includes the resource name and its associated scopes.
    /// </summary>
    [JsonProperty("permissions")]
    public IEnumerable<KcTestResource> Permissions { get; set; }
}
