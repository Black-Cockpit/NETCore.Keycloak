using System.Text.Json.Serialization;

namespace NETCore.Keycloak.Client.Models.Organizations;

/// <summary>
/// Represents an organization resource.
/// </summary>
public sealed class KcOrganization
{
    /// <summary>
    /// Gets or sets the organization id.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the organization name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the organization alias.
    /// </summary>
    [JsonPropertyName("alias")]
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the organization is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets the organization description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the redirect URL for the organization.
    /// </summary>
    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Custom attributes.
    /// Key = attribute name
    /// Value = list of values (Keycloak style)
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>> Attributes { get; set; }

    /// <summary>
    /// Gets or sets the organization domains.
    /// </summary>
    [JsonPropertyName("domains")]
    public List<KcOrganizationDomain> Domains { get; set; } = new();
}
