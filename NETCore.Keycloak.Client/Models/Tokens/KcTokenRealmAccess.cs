using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Tokens;

/// <summary>
/// Represents the realm access details of a Keycloak token.
/// </summary>
public class KcTokenRealmAccess
{
    /// <summary>
    /// Gets or sets the roles assigned to the Keycloak token within the realm.
    /// </summary>
    /// <value>
    /// A collection of strings representing the roles associated with the token.
    /// </value>
    [JsonProperty("roles")]
    public IEnumerable<string> Roles { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the caller's identity should be verified.
    /// </summary>
    /// <value>
    /// A nullable boolean that, if set to <c>true</c>, specifies that the caller's identity must be verified.
    /// </value>
    [JsonProperty("verify_caller")]
    public bool? VerifyCaller { get; set; }
}
