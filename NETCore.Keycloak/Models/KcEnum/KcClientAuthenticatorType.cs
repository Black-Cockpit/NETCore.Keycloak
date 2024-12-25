using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.KcEnum;

/// <summary>
/// When using client-secret, the module parameter secret can set it,while for client-jwt, you can use the keys use.jwks.url, jwks.url,
/// and jwt.credential.certificate in the attributes module parameter to configure its behavior.
/// </summary>
public enum KcClientAuthenticatorType
{
    /// <summary>
    /// client-secret authentication type
    /// </summary>
    [EnumMember(Value = "client-secret")] Secret,

    /// <summary>
    /// client-jwt authentication type
    /// </summary>
    [JsonProperty("client-jwt")] Jwt,
}
