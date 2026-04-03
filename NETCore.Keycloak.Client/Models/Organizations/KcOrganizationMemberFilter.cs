using System.Globalization;
using System.Text;
using NETCore.Keycloak.Client.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Organizations;

/// <summary>
/// Represents a filter for querying Keycloak organization members.
/// </summary>
public sealed class KcOrganizationMemberFilter : KcFilter
{
    /// <summary>
    /// Gets or sets a value indicating whether the search parameter must match exactly.
    /// </summary>
    /// <value>
    /// <c>true</c> if the parameters must match exactly; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("exact")]
    public bool? Exact { get; set; }

    /// <summary>
    /// Gets or sets the membership type to filter by.
    /// </summary>
    /// <value>
    /// A string representing the membership type filter.
    /// </value>
    [JsonProperty("membershipType")]
    public string MembershipType { get; set; }

    /// <summary>
    /// Builds the query string based on the filter properties.
    /// </summary>
    /// <returns>
    /// A string containing the query parameters to be appended to a URL.
    /// </returns>
    public new string BuildQuery()
    {
        var builder = new StringBuilder($"?max={Max}");

        // Include pagination offset if specified
        if ( First != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&first={string.Create(CultureInfo.CurrentCulture, $"{First}").ToLower(CultureInfo.CurrentCulture)}");
        }

        // Include general search query if specified
        if ( !string.IsNullOrWhiteSpace(Search) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&search={Search}");
        }

        // Include exact match filter if specified
        if ( Exact != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&exact={Exact.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Include membership type filter if specified
        if ( !string.IsNullOrWhiteSpace(MembershipType) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&membershipType={MembershipType}");
        }

        return builder.ToString();
    }
}
