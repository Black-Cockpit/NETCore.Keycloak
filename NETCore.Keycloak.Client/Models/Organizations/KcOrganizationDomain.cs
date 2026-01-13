using System.Text.Json.Serialization;

namespace NETCore.Keycloak.Client.Models.Organizations;

/// <summary>
/// Represents an organization domain in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/latest/rest-api/index.html#OrganizationDomainRepresentation"/>
/// </summary>
public sealed class KcOrganizationDomain
{
    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    /// <value>
    /// A string representing the domain name (for example, "example.com").
    /// </value>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the domain is verified.
    /// </summary>
    /// <value>
    /// A nullable boolean indicating whether the domain has been verified by Keycloak. 
    /// True if verified; false if not; null if the verification state is unknown.
    /// </value>
    [JsonPropertyName("verified")]
    public bool? Verified { get; set; }
}
