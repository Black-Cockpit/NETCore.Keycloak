namespace NETCore.Keycloak.Authorization.Handlers;

/// <summary>
/// Keycloak caching token
/// </summary>
internal sealed class KcCachedToken
{
    /// <summary>
    /// Token value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Caching time
    /// </summary>
    private DateTime CachingTime { get; } = DateTime.UtcNow;

    /// <summary>
    /// Cache expiry
    /// </summary>
    public long Expiry { get; set; }

    /// <summary>
    /// Check if the cache is expired
    /// </summary>
    /// <returns></returns>
    public bool IsExpired => (CachingTime.AddSeconds(Expiry) - DateTime.UtcNow).TotalSeconds <= 0;
}
