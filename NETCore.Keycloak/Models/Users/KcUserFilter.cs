using System.Globalization;
using System.Text;
using NETCore.Keycloak.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Users;

/// <summary>
/// Represents a filter for querying users in Keycloak.
/// </summary>
public class KcUserFilter : KcFilter
{
    /// <summary>
    /// Gets or sets the email filter. Matches a string contained in the email or the complete email if the "exact" parameter is true.
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the email has been verified.
    /// </summary>
    [JsonProperty("emailVerified")]
    public bool? EmailVerified { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is enabled.
    /// </summary>
    [JsonProperty("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the parameters "last", "first", "email", and "username" must match exactly.
    /// </summary>
    [JsonProperty("exact")]
    public bool? Exact { get; set; }

    /// <summary>
    /// Gets or sets the first name filter. Matches a string contained in the first name or the complete first name if the "exact" parameter is true.
    /// </summary>
    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the alias of an identity provider linked to the user.
    /// </summary>
    [JsonProperty("idpAlias")]
    public string IdpAlias { get; set; }

    /// <summary>
    /// Gets or sets the user ID at an identity provider linked to the user.
    /// </summary>
    [JsonProperty("idpUserId")]
    public string IdpUserId { get; set; }

    /// <summary>
    /// Gets or sets the last name filter. Matches a string contained in the last name or the complete last name if the "exact" parameter is true.
    /// </summary>
    [JsonProperty("lastName")]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets a query for searching custom attributes in the format 'key1:value1 key2:value2'.
    /// </summary>
    [JsonProperty("q")]
    public string Q { get; set; }

    /// <summary>
    /// Gets or sets the username filter. Matches a string contained in the username or the complete username if the "exact" parameter is true.
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// Builds the query string based on the filter properties.
    /// </summary>
    /// <returns>
    /// A string containing the query parameters to be appended to a URL.
    /// </returns>
    public new string BuildQuery()
    {
        var builder = new StringBuilder($"?max={Max}");

        // Add brief representation filter
        if ( BriefRepresentation != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&briefRepresentation={BriefRepresentation.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add email filter
        if ( !string.IsNullOrWhiteSpace(Email) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&email={Email}");
        }

        // Add email verification filter
        if ( EmailVerified != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&emailVerified={EmailVerified.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add enabled status filter
        if ( Enabled != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&enabled={Enabled.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add exact match filter
        if ( Exact != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&exact={Exact.ToString().ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add pagination offset
        if ( First != null )
        {
            _ = builder.Append(CultureInfo.CurrentCulture,
                $"&first={string.Create(CultureInfo.CurrentCulture, $"{First}").ToLower(CultureInfo.CurrentCulture)}");
        }

        // Add first name filter
        if ( !string.IsNullOrWhiteSpace(FirstName) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&firstName={FirstName}");
        }

        // Add identity provider alias filter
        if ( !string.IsNullOrWhiteSpace(IdpAlias) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&idpAlias={IdpAlias}");
        }

        // Add identity provider user ID filter
        if ( !string.IsNullOrWhiteSpace(IdpUserId) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&idpUserId={IdpUserId}");
        }

        // Add last name filter
        if ( !string.IsNullOrWhiteSpace(LastName) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&lastName={LastName}");
        }

        // Add custom attribute query
        if ( !string.IsNullOrWhiteSpace(Q) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&q={Q}");
        }

        // Add general search filter
        if ( !string.IsNullOrWhiteSpace(Search) )
        {
            _ = builder.Append(CultureInfo.CurrentCulture, $"&search={Search}");
        }

        // Add username filter and return the final query
        return string.IsNullOrWhiteSpace(Username)
            ? builder.ToString()
            : builder.Append(CultureInfo.CurrentCulture, $"&username={Username}").ToString();
    }
}
