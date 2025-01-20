using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NETCore.Keycloak.Client.Authentication.Claims;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Models.KcEnum;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Authentication;

/// <summary>
/// Keycloak authentication configuration
/// </summary>
public class KcAuthenticationConfiguration
{
    /// <summary>
    /// Keycloak base url.
    /// <remarks>
    /// Only base url should be provider
    /// </remarks>
    /// <example>
    /// http://localhost:8080/
    /// </example>
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// Keycloak base issuer URL.
    /// <remarks>
    /// Only base url should be provider
    /// </remarks>
    /// <example>
    /// http://localhost:8080/
    /// </example>
    /// </summary>
    [JsonProperty("issuer")]
    public string Issuer { get; set; }

    /// <summary>
    /// Realm name
    /// </summary>
    [JsonProperty("realm")]
    public string Realm { get; set; }

    /// <summary>
    /// Protected resource name <see cref="JwtBearerOptions.Audience"/>
    /// </summary>
    [JsonProperty("resource")]
    public string Resource { get; set; }

    /// <summary>
    /// Jwt token clock skew <see cref="JwtBearerOptions"/>
    /// </summary>
    public TimeSpan TokenClockSkew { get; set; } = TimeSpan.FromSeconds(300);

    /// <summary>
    /// Require ssl
    /// </summary>
    public bool RequireSsl { get; set; }

    /// <summary>
    /// Role claim source <see cref="KcRolesClaimSource"/>
    /// User by <see cref="KcRolesClaimsTransformer"/> to identify keycloak roles source.
    /// </summary>
    public KcRolesClaimSource RolesSource { get; set; } = KcRolesClaimSource.Realm;

    /// <summary>
    /// Role claim type.
    /// User by <see cref="KcRolesClaimsTransformer"/> to transform keycloak roles to the specified name.
    /// <seealso cref="TokenValidationParameters.RoleClaimType"/>
    /// </summary>
    public string RoleClaimType { get; set; } = "role";

    /// <summary>
    /// Name claim type
    /// <see cref="TokenValidationParameters.NameClaimType"/>
    /// </summary>
    public string NameClaimType { get; set; } = "preferred_username";

    /// <summary>
    /// List of valid audiences
    /// </summary>
    public IEnumerable<string> ValidAudiences { get; set; } = new List<string>();

    /// <summary>
    /// Get valid issuer
    /// </summary>
    /// <returns></returns>
    public string ValidIssuer => !string.IsNullOrWhiteSpace(Realm) && !string.IsNullOrWhiteSpace(Issuer)
        ? $"{NormalizeUrl(Issuer)}/realms/{Realm}"
        : null;

    /// <summary>
    /// Get authority
    /// </summary>
    public string Authority => ValidIssuer;

    /// <summary>
    /// Normalize url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static string NormalizeUrl(string url)
    {
        if ( string.IsNullOrWhiteSpace(url) )
        {
            return null;
        }

        var urlNormalized = !url.EndsWith("/", StringComparison.Ordinal) ? url : url.TrimEnd('/');

        return urlNormalized;
    }

    /// <summary>
    /// Validates the configuration properties of the current instance to ensure all
    /// required values are set. Throws a <see cref="KcException"/> if any required
    /// property is missing or invalid.
    /// </summary>
    /// <exception cref="KcException">
    /// Thrown when any of the following properties are null, empty, or contain only whitespace:
    /// <list type="bullet">
    /// <item><description><see cref="Url"/></description></item>
    /// <item><description><see cref="Realm"/></description></item>
    /// <item><description><see cref="Issuer"/></description></item>
    /// </list>
    /// </exception>
    public void Validate()
    {
        // Check if Url is null or whitespace and throw an exception if it is.
        if ( string.IsNullOrWhiteSpace(Url) )
        {
            throw new KcException($"{nameof(Url)} is required");
        }

        // Check if Realm is null or whitespace and throw an exception if it is.
        if ( string.IsNullOrWhiteSpace(Realm) )
        {
            throw new KcException($"{nameof(Realm)} is required");
        }

        // Check if Issuer is null or whitespace and throw an exception if it is.
        if ( string.IsNullOrWhiteSpace(Issuer) )
        {
            throw new KcException($"{nameof(Issuer)} is required");
        }
    }
}
