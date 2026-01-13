using System.Text.Json.Serialization;

namespace NETCore.Keycloak.Client.Models.Organizations;

/// <summary>
/// Represents an organization resource in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_organizationrepresentation"/>
/// </summary>
public sealed class KcOrganization
{
    /// <summary>
    /// Gets or sets the organization id.
    /// </summary>
    /// <value>
    /// A string representing the unique identifier of the organization.
    /// </value>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the organization name.
    /// </summary>
    /// <value>
    /// A string representing the display name of the organization.
    /// </value>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the organization alias.
    /// </summary>
    /// <value>
    /// A string representing an alternate identifier or alias for the organization.
    /// </value>
    [JsonPropertyName("alias")]
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the organization is enabled.
    /// </summary>
    /// <value>
    /// A nullable boolean that is true when the organization is enabled, false when disabled,
    /// or null if the enabled state is not set.
    /// </value>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets the organization description.
    /// </summary>
    /// <value>
    /// A string containing a human-readable description for the organization.
    /// </value>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the redirect URL for the organization.
    /// </summary>
    /// <value>
    /// A string representing the redirect URL associated with the organization (for example, after login or registration flows).
    /// </value>
    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Custom attributes associated with the organization.
    /// </summary>
    /// <value>
    /// A dictionary where the key is the attribute name and the value is a list of values for that attribute (Keycloak style).
    /// </value>
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>> Attributes { get; set; }

    /// <summary>
    /// Gets or sets the organization domains.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcOrganizationDomain"/> representing domains associated with the organization.
    /// </value>
    [JsonPropertyName("domains")]
    public List<KcOrganizationDomain> Domains { get; set; } = new();
}
