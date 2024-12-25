namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents the protected resources associated with a Keycloak realm.
/// </summary>
public class KcRealmProtectedResources
{
    /// <summary>
    /// Gets or sets the name of the Keycloak realm.
    /// </summary>
    /// <value>
    /// A string representing the realm name.
    /// </value>
    public string Realm { get; set; }

    /// <summary>
    /// Gets or sets the name of the protected resource within the realm.
    /// </summary>
    /// <value>
    /// A string representing the protected resource name.
    /// </value>
    public string ProtectedResourceName { get; set; }
}
