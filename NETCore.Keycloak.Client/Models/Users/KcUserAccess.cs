using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Users;

/// <summary>
/// Keycloak user access representation
/// </summary>
public class KcUserAccess
{
    /// <summary>
    /// View access
    /// </summary>
    [JsonProperty("view")]
    public bool? View { get; set; }

    /// <summary>
    /// Manage group membership access
    /// </summary>
    [JsonProperty("manageGroupMembership")]
    public bool? ManageGroupMembership { get; set; }

    /// <summary>
    /// Map role access
    /// </summary>
    [JsonProperty("mapRoles")]
    public bool? MapRoles { get; set; }

    /// <summary>
    /// Impersonate access
    /// </summary>
    [JsonProperty("impersonate")]
    public bool? Impersonate { get; set; }

    /// <summary>
    /// Manage access
    /// </summary>
    [JsonProperty("manage")]
    public bool? Manage { get; set; }
}
