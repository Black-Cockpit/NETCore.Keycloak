using NETCore.Keycloak.Models.Common;

namespace NETCore.Keycloak.Authorization.Store;

/// <summary>
/// Realm admin configuration store.
/// </summary>
public abstract class KcRealmAdminConfigurationStore
{
    /// <summary>
    /// Get realm protected resources
    /// </summary>
    /// <returns></returns>
    public abstract ICollection<KcRealmAdminConfiguration> GetRealmsAdminConfiguration();
}
