using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents a filter used for querying Keycloak resources.
/// </summary>
public class KcFilter
{
    /// <summary>
    /// Gets or sets a value indicating whether brief representations of the resources should be returned.
    /// Default value is <c>false</c>.
    /// </summary>
    /// <value>
    /// <c>true</c> if brief representations are requested; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("briefRepresentation")]
    public bool? BriefRepresentation { get; set; }

    /// <summary>
    /// Gets or sets the pagination offset.
    /// </summary>
    /// <value>
    /// An integer representing the starting point of the pagination, or <c>null</c> if not specified.
    /// </value>
    [JsonProperty("first")]
    public int? First { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to return. Defaults to 100.
    /// </summary>
    /// <value>
    /// An integer representing the maximum size of the result set.
    /// </value>
    [JsonProperty("max")]
    public int Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets a search string to filter resources by their name or first matching property.
    /// </summary>
    /// <value>
    /// A string used for searching, or <c>null</c> if no search term is specified.
    /// </value>
    [JsonProperty("search")]
    public string Search { get; set; }

    /// <summary>
    /// Builds the query string based on the filter properties.
    /// </summary>
    /// <returns>
    /// A string containing the query parameters to be appended to a URL.
    /// </returns>
    public string BuildQuery()
    {
        // Initialize the query builder with the maximum results parameter
        var builder = new StringBuilder($"?max={Max}");

        // Add brief representation parameter if specified
        if ( BriefRepresentation != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&briefRepresentation={BriefRepresentation.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add pagination offset if specified
        if ( First != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&first={string.Create(CultureInfo.CurrentCulture, $"{First}").ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add search string if specified and not null or whitespace
        if ( !string.IsNullOrWhiteSpace(Search) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&search={Search}");
        }

        // Return the constructed query string
        return builder.ToString();
    }
}
