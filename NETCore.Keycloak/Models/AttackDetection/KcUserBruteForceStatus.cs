using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.AttackDetection;

/// <summary>
/// Represents the brute force detection status for a Keycloak user.
/// </summary>
public class KcUserBruteForceStatus
{
    /// <summary>
    /// Gets or sets the number of failed login attempts for the user.
    /// </summary>
    /// <value>
    /// The total number of failures recorded for the user, or <c>null</c> if not applicable.
    /// </value>
    [JsonProperty("numFailures")]
    public int? NumFailures { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is disabled due to brute force detection.
    /// </summary>
    /// <value>
    /// <c>true</c> if the user is disabled; otherwise, <c>false</c>, or <c>null</c> if not applicable.
    /// </value>
    [JsonProperty("disabled")]
    public bool? Disabled { get; set; }

    /// <summary>
    /// Gets or sets the last recorded IP address associated with a failed login attempt.
    /// </summary>
    /// <value>
    /// The last IP address that caused a failure, or <c>null</c> if no failures are recorded.
    /// </value>
    [JsonProperty("lastIPFailure")]
    public string LastIpFailure { get; set; }

    /// <summary>
    /// Gets or sets the UNIX timestamp of the last recorded login failure.
    /// </summary>
    /// <value>
    /// The UNIX time of the last failure, or <c>null</c> if no failures are recorded.
    /// </value>
    [JsonProperty("lastFailure")]
    public long? LastFailure { get; set; }
}
