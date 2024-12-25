using System.Globalization;
using System.Text;
using NETCore.Keycloak.Client.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Represents a filter for querying Keycloak clients.
/// </summary>
public class KcClientFilter : KcFilter
{
    /// <summary>
    /// Gets or sets a value indicating whether to filter clients that can only be viewed in full by an admin.
    /// </summary>
    /// <value>
    /// <c>true</c> to include only viewable clients; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("viewableOnly")]
    public bool? ViewableOnly { get; set; }

    /// <summary>
    /// Gets or sets a query string for searching custom attributes in the format 'key1:value1 key2:value2'.
    /// </summary>
    /// <value>
    /// A string containing the custom attribute search query, or <c>null</c> if not specified.
    /// </value>
    [JsonProperty("q")]
    public string Q { get; set; }

    /// <summary>
    /// Builds the query string based on the filter properties specific to clients.
    /// </summary>
    /// <returns>
    /// A string containing the query parameters to be appended to a URL.
    /// </returns>
    public new string BuildQuery()
    {
        // Initialize the query builder with the maximum results parameter
        var builder = new StringBuilder($"?max={Max}");

        // Add pagination offset if specified
        if ( First != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&first={string.Create(CultureInfo.CurrentCulture, $"{First}").ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add general search string if specified and not null or whitespace
        if ( !string.IsNullOrWhiteSpace(Search) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&search={Search}");
        }

        // Add custom attribute query string if specified
        if ( !string.IsNullOrWhiteSpace(Q) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&q={Q}");
        }

        // Add viewable-only filter if specified
        if ( ViewableOnly != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&viewableOnly={ViewableOnly.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Return the constructed query string
        return builder.ToString();
    }
}
