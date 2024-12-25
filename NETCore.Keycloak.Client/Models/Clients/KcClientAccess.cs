using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Represents the access permissions for a Keycloak client.
/// </summary>
public class KcClientAccess
{
    /// <summary>
    /// Gets or sets a value indicating whether the client has view access.
    /// </summary>
    /// <value>
    /// <c>true</c> if view access is enabled; otherwise, <c>false</c>. Can be <c>null</c> if not specified.
    /// </value>
    [JsonProperty("view")]
    public bool? View { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the client has configure access.
    /// </summary>
    /// <value>
    /// <c>true</c> if configure access is enabled; otherwise, <c>false</c>. Can be <c>null</c> if not specified.
    /// </value>
    [JsonProperty("configure")]
    public bool? Configure { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the client has manage access.
    /// </summary>
    /// <value>
    /// <c>true</c> if manage access is enabled; otherwise, <c>false</c>. Can be <c>null</c> if not specified.
    /// </value>
    [JsonProperty("manage")]
    public bool? Manage { get; set; }
}
