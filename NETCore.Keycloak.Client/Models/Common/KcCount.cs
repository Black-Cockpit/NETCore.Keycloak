using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents a model for capturing a count value, typically used in responses from Keycloak APIs.
/// </summary>
public class KcCount
{
    /// <summary>
    /// Gets or sets the count value.
    /// </summary>
    [JsonProperty("count")]
    public int Count { get; set; }
}
