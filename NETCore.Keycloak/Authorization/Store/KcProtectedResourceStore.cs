using NETCore.Keycloak.Models.Common;

namespace NETCore.Keycloak.Authorization.Store;

/// <summary>
/// Protected resources store.
/// </summary>
public abstract class KcProtectedResourceStore
{
    /// <summary>
    /// Get realm protected resources
    /// </summary>
    /// <returns></returns>
    public abstract ICollection<KcRealmProtectedResources> GetRealmProtectedResources();
}
