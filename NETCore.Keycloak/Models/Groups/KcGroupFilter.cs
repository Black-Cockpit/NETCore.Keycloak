using System.Globalization;
using System.Text;
using NETCore.Keycloak.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Groups;

/// <summary>
/// Represents a filter for querying Keycloak groups.
/// </summary>
public class KcGroupFilter : KcFilter
{
    /// <summary>
    /// Gets or sets a query string for searching custom attributes, formatted as 'key1:value1 key2:value2'.
    /// </summary>
    /// <value>
    /// A string representing the custom attribute search query.
    /// </value>
    [JsonProperty("q")]
    public string Q { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the query parameters must match exactly.
    /// </summary>
    /// <value>
    /// <c>true</c> if the parameters must match exactly; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("exact")]
    public bool? Exact { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to filter for top-level groups only.
    /// </summary>
    /// <value>
    /// <c>true</c> if only top-level groups should be returned; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("top")]
    public bool? Top { get; set; }

    /// <summary>
    /// Builds the query string based on the filter properties.
    /// </summary>
    /// <returns>
    /// A string containing the query parameters to be appended to a URL.
    /// </returns>
    public new string BuildQuery()
    {
        var builder = new StringBuilder($"?max={Max}");

        // Include brief representation if specified
        if ( BriefRepresentation != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&briefRepresentation={BriefRepresentation.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Include pagination offset if specified
        if ( First != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&first={string.Create(CultureInfo.CurrentCulture, $"{First}").ToLower(CultureInfo.CurrentCulture)}");
        }

        // Include custom attribute query if specified
        if ( !string.IsNullOrWhiteSpace(Q) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&q={Q}");
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

        // Include top-level filter if specified
        if ( Top != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&top={Top.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        return builder.ToString();
    }
}
