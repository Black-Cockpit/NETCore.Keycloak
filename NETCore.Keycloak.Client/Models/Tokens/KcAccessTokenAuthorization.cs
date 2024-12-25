using NETCore.Keycloak.Client.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Tokens;

/// <summary>
/// Represents the authorization details within a Keycloak access token.
/// </summary>
public class KcAccessTokenAuthorization
{
    /// <summary>
    /// Gets or sets the list of permissions included in the access token.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcPermission"/> representing the access token's permissions.
    /// </value>
    [JsonProperty("permissions")]
    public IEnumerable<KcPermission> Permissions { get; set; }
}
