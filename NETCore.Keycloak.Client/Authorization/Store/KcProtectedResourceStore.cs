using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.Authorization.Store;

/// <summary>
/// Represents an abstract store for managing and retrieving protected resources within a Keycloak realm.
/// </summary>
/// <remarks>
/// The <see cref="KcProtectedResourceStore"/> class provides a blueprint for implementations that need to access
/// and retrieve protected resources, such as APIs or resource endpoints, that are secured by Keycloak.
/// </remarks>
public abstract class KcProtectedResourceStore
{
    /// <summary>
    /// Retrieves a collection of protected resources defined in the Keycloak realm.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="KcRealmProtectedResources"/> objects, where each object represents a secured resource,
    /// including its name, type, and associated permissions (scopes).
    /// </returns>
    /// <remarks>
    /// This method is intended to be overridden by subclasses to provide specific implementations for fetching
    /// protected resources from the underlying store or configuration.
    /// </remarks>
    public abstract ICollection<KcRealmProtectedResources> GetRealmProtectedResources();
}
