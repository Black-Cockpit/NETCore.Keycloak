namespace NETCore.Keycloak.Client.Models.KcEnum;

/// <summary>
/// Represents the source of role claims in Keycloak.
/// </summary>
public enum KcRolesClaimSource
{
    /// <summary>
    /// No role claims are specified.
    /// </summary>
    None,

    /// <summary>
    /// Role claims sourced from the realm.
    /// </summary>
    Realm,

    /// <summary>
    /// Role claims sourced from resource access.
    /// </summary>
    ResourceAccess
}
