namespace NETCore.Keycloak.Client.Authorization.Handlers;

/// <summary>
/// Represents a cached Keycloak token along with its expiry information.
/// </summary>
/// <remarks>
/// The <see cref="KcCachedToken"/> class is used to store an access token retrieved from Keycloak,
/// along with its caching timestamp and expiry duration. This helps determine whether the token is still valid or has expired.
/// </remarks>
public sealed class KcCachedToken
{
    /// <summary>
    /// Gets or sets the value of the cached token.
    /// </summary>
    /// <value>
    /// A string representing the JWT token or bearer token used for authentication and authorization.
    /// </value>
    public string Value { get; set; }

    /// <summary>
    /// Gets the UTC time when the token was cached.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the time (in UTC) when the token was stored in the cache.
    /// </value>
    private DateTime CachingTime { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the expiry duration (in seconds) for the cached token.
    /// </summary>
    /// <value>
    /// A long value representing the number of seconds after which the token expires.
    /// </value>
    /// <remarks>
    /// The expiry time is typically provided in the token payload and indicates how long the token is valid.
    /// </remarks>
    public long Expiry { get; set; }

    /// <summary>
    /// Determines whether the cached token has expired.
    /// </summary>
    /// <value>
    /// Returns <c>true</c> if the token has expired; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// The method calculates the difference between the current UTC time and the cached time plus the expiry duration.
    /// If the difference is zero or negative, the token is considered expired.
    /// </remarks>
    public bool IsExpired => (CachingTime.AddSeconds(Expiry) - DateTime.UtcNow).TotalSeconds <= 0;
}
