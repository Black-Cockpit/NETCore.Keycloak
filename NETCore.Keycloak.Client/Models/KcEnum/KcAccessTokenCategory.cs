using System.Runtime.Serialization;

namespace NETCore.Keycloak.Client.Models.KcEnum;

/// <summary>
/// Represents the different categories of Keycloak access tokens.
/// </summary>
public enum KcAccessTokenCategory
{
    /// <summary>
    /// Represents an internal token used for internal operations within Keycloak.
    /// </summary>
    [EnumMember(Value = "INTERNAL")]
    Internal,

    /// <summary>
    /// Represents an access token used to authorize access to resources.
    /// </summary>
    [EnumMember(Value = "ACCESS")]
    Access,

    /// <summary>
    /// Represents an ID token containing user identity claims.
    /// </summary>
    [EnumMember(Value = "ID")]
    Id,

    /// <summary>
    /// Represents an admin token used for administrative purposes.
    /// </summary>
    [EnumMember(Value = "ADMIN")]
    Admin,

    /// <summary>
    /// Represents a token used for user information retrieval.
    /// </summary>
    [EnumMember(Value = "USERINFO")]
    Userinfo,

    /// <summary>
    /// Represents a logout token used during the logout process.
    /// </summary>
    [EnumMember(Value = "LOGOUT")]
    Logout,

    /// <summary>
    /// Represents a token issued in response to an authorization request.
    /// </summary>
    [EnumMember(Value = "AUTHORIZATION_RESPONSE")]
    AuthorizationResponse
}
