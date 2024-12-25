using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Serialization;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.ClientScope;

/// <summary>
/// keycloak client scope representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_clientscoperepresentation"/>
/// </summary>
public class KcClientScope
{
    /// <summary>
    /// Key cloak client scope representation.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Client scope name.
    /// Name of the client scope. Must be unique in the realm.
    /// Name should not contain space characters as it is used as value of scope parameter.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Description of the client scope
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// This client scope is supplying SSO protocol configuration.
    /// <see cref="KcProtocol"/>
    /// </summary>
    [JsonProperty("protocol")]
    [JsonConverter(typeof(KcProtocolConverter))]
    public KcProtocol Protocol { get; set; } = KcProtocol.OpenidConnect;

    /// <summary>
    /// Client scope attributes
    /// </summary>
    [JsonProperty("attributes")]
    public object Attributes { get; set; }

    /// <summary>
    /// List of protocol mappers
    /// </summary>
    [JsonProperty("protocolMappers")]
    public IEnumerable<KcProtocolMapper> ProtocolMappers { get; set; }
}
