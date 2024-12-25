using NETCore.Keycloak.Models.KcEnum;
using NETCore.Keycloak.Serialization;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.ClientScope;

/// <summary>
/// Represents a protocol mapper configuration in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_protocolmapperrepresentation"/>
/// </summary>
public class KcProtocolMapper
{
    /// <summary>
    /// Gets or sets the unique identifier for the protocol mapper.
    /// </summary>
    /// <value>
    /// A string representing the protocol mapper's ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the protocol mapper.
    /// </summary>
    /// <value>
    /// A string representing the name of the mapper.
    /// </value>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the protocol associated with the mapper.
    /// </summary>
    /// <value>
    /// A <see cref="KcProtocol"/> indicating the SSO protocol (e.g., OpenID Connect or SAML).
    /// </value>
    [JsonProperty("protocol")]
    [JsonConverter(typeof(KcProtocolConverter))]
    public KcProtocol Protocol { get; set; }

    /// <summary>
    /// Gets or sets the internal name of the protocol mapper.
    /// </summary>
    /// <value>
    /// A string representing the protocol mapper's internal name.
    /// </value>
    [JsonProperty("protocolMapper")]
    public string ProtocolMapper { get; set; }

    /// <summary>
    /// Gets or sets the configuration options for the protocol mapper.
    /// </summary>
    /// <value>
    /// An object representing the protocol mapper's configuration.
    /// </value>
    [JsonProperty("config")]
    public object Config { get; set; }
}
