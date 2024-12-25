using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Represents a protocol mapper configuration in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_protocolmapperrepresentation"/>
/// </summary>
public class KcClientProtocolMapper
{
    /// <summary>
    /// Gets or sets the unique identifier for the protocol mapper instance.
    /// </summary>
    /// <value>
    /// A string, typically a UUID, representing the internal ID of the protocol mapper instance.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the protocol mapper.
    /// </summary>
    /// <value>
    /// A string representing the protocol mapper name.
    /// </value>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the protocol associated with the mapper.
    /// Allowed values are "openid-connect" or "saml".
    /// </summary>
    /// <value>
    /// A string specifying the protocol type.
    /// </value>
    [JsonProperty("protocol")]
    public string Protocol { get; set; }

    /// <summary>
    /// Gets or sets the internal name of the protocol mapper type used by Keycloak.
    /// </summary>
    /// <value>
    /// A string representing the internal protocol mapper type name.
    /// </value>
    [JsonProperty("protocolMapper")]
    public string ProtocolMapper { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether user consent is required for the mapper to be active.
    /// </summary>
    /// <value>
    /// <c>true</c> if consent is required; otherwise, <c>false</c>. Can be <c>null</c> if not specified.
    /// </value>
    [JsonProperty("consentRequired")]
    public bool? ConsentRequired { get; set; }

    /// <summary>
    /// Gets or sets the human-readable text for user consent.
    /// </summary>
    /// <value>
    /// A string representing the consent text shown to the user.
    /// </value>
    [JsonProperty("consentText")]
    public string ConsentText { get; set; }

    /// <summary>
    /// Gets or sets the configuration options for the protocol mapper.
    /// </summary>
    /// <value>
    /// An object containing key-value pairs of configuration options.
    /// </value>
    [JsonProperty("config")]
    public object Config { get; set; }
}
