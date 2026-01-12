using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.Models.Organizations;

/// <summary>
/// Filter model for organizations queries.
/// </summary>
public sealed class KcOrganizationFilter : KcFilter
{
    /// <summary>
    /// Filter by organization name (partial or exact if Exact = true)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Filter by organization alias
    /// </summary>
    [JsonPropertyName("alias")]
    public string Alias { get; set; }

    /// <summary>
    /// Filter by enabled state
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Filter by domain name
    /// </summary>
    [JsonPropertyName("domain")]
    public string Domain { get; set; }

    /// <summary>
    /// Filter by verified domain
    /// </summary>
    [JsonPropertyName("domainVerified")]
    public bool? DomainVerified { get; set; }

    /// <summary>
    /// Require exact matching for name / alias
    /// </summary>
    [JsonPropertyName("exact")]
    public bool? Exact { get; set; }

    /// <summary>
    /// Free text search (Keycloak standard search param)
    /// </summary>
    [JsonPropertyName("search")]
    public string Search { get; set; }

    /// <summary>
    /// Build query string
    /// </summary>
    public new string BuildQuery()
    {
        var sb = new StringBuilder($"?max={Max}");

        if ( BriefRepresentation.HasValue )
        {
            _ = sb.Append("&briefRepresentation=")
              .Append(BriefRepresentation.Value.ToString().ToLower(CultureInfo.CurrentCulture));
        }

        if ( !string.IsNullOrWhiteSpace(Name) )
        {
            _ = sb.Append("&name=").Append(Name);
        }

        if ( !string.IsNullOrWhiteSpace(Alias) )
        {
            _ = sb.Append("&alias=").Append(Alias);
        }

        if ( Enabled.HasValue )
        {
            _ = sb.Append("&enabled=")
              .Append(Enabled.Value.ToString().ToLower(CultureInfo.CurrentCulture));
        }

        if ( !string.IsNullOrWhiteSpace(Domain) )
        {
            _ = sb.Append("&domain=").Append(Domain);
        }

        if ( DomainVerified.HasValue )
        {
            _ = sb.Append("&domainVerified=")
              .Append(DomainVerified.Value.ToString().ToLower(CultureInfo.CurrentCulture));
        }

        if ( Exact.HasValue )
        {
            _ = sb.Append("&exact=")
              .Append(Exact.Value.ToString().ToLower(CultureInfo.CurrentCulture));
        }

        if ( First.HasValue )
        {
            _ = sb.Append("&first=")
              .Append(First.Value.ToString(CultureInfo.CurrentCulture));
        }

        if ( !string.IsNullOrWhiteSpace(Search) )
        {
            _ = sb.Append("&search=").Append(Search);
        }

        return sb.ToString();
    }
}
