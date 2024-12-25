namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents a protected resource in Keycloak.
/// </summary>
public class KcProtectedResource
{
    /// <summary>
    /// Gets or sets the name of the protected resource.
    /// </summary>
    /// <value>
    /// A string representing the resource name.
    /// </value>
    public string Resource { get; set; }
}
