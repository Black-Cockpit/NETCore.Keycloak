namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents the protected resources and their associated information within a Keycloak realm.
/// </summary>
/// <remarks>
/// This class encapsulates details about a secured resource, including its association with a specific Keycloak realm.
/// Protected resources typically represent APIs, endpoints, or services that require authorization.
/// </remarks>
public class KcRealmProtectedResources
{
    /// <summary>
    /// Gets or sets the name of the Keycloak realm to which the protected resource belongs.
    /// </summary>
    /// <value>
    /// A string representing the unique identifier or name of the realm, such as "example-realm".
    /// </value>
    public string Realm { get; set; }

    /// <summary>
    /// Gets or sets the name of the protected resource associated with the specified realm.
    /// </summary>
    /// <value>
    /// A string representing the name of the resource, such as "orders-api" or "user-management-service".
    /// </value>
    /// <remarks>
    /// This property identifies the secured resource that is registered under the specified realm.
    /// The resource name is typically used in authorization checks and API protection.
    /// </remarks>
    public string ProtectedResourceName { get; set; }
}
