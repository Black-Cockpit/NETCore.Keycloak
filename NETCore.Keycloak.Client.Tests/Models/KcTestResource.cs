using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Models;

/// <summary>
/// Represents a Keycloak resource used in testing scenarios.
/// This class is used to deserialize JSON objects containing resource information, including its name and associated scopes.
/// </summary>
public class KcTestResource
{
    /// <summary>
    /// Gets or sets the name of the resource.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the scopes associated with the resource.
    /// Each scope defines a permission or action applicable to the resource.
    /// </summary>
    [JsonProperty("scopes")]
    public IEnumerable<string> Scopes { get; set; }
}
