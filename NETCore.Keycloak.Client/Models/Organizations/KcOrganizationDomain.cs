using System.Text.Json.Serialization;

namespace NETCore.Keycloak.Client.Models.Organizations;

/// <summary>
/// Represents organization domain information.
/// </summary>
public sealed class KcOrganizationDomain
{
    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the domain is verified.
    /// </summary>
    [JsonPropertyName("verified")]
    public bool? Verified { get; set; }
}
