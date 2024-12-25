using System.Runtime.Serialization;

namespace NETCore.Keycloak.Models.KcEnum;

/// <summary>
/// Supported Keycloak protocols for authentication and authorization.
/// </summary>
public enum KcProtocol
{
    /// <summary>
    /// 'OpenID Connect' protocol enables clients to verify the identity of an end-user based on the authentication performed by an authorization server.
    /// It is widely used for modern web and mobile authentication scenarios.
    /// </summary>
    [EnumMember(Value = "openid-connect")]
    OpenidConnect,

    /// <summary>
    /// 'SAML' (Security Assertion Markup Language) protocol supports web-based authentication and authorization,
    /// including cross-domain single sign-on (SSO), leveraging security tokens with assertions to exchange information securely.
    /// </summary>
    [EnumMember(Value = "saml")]
    Saml
}
