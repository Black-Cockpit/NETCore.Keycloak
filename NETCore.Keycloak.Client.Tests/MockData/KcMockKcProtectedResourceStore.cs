using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Mock implementation of the <see cref="KcProtectedResourceStore"/> for testing purposes.
/// Provides predefined protected resource data to simulate the behavior of the actual store.
/// </summary>
public class KcMockKcProtectedResourceStore : KcProtectedResourceStore
{
    /// <summary>
    /// Retrieves a predefined collection of protected resources for a test realm.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="KcRealmProtectedResources"/> containing test data
    /// with a single entry for "TestRealm" and its associated "TestProtectedResource".
    /// </returns>
    public override ICollection<KcRealmProtectedResources> GetRealmProtectedResources() =>
        new List<KcRealmProtectedResources>
        {
            new()
            {
                Realm = "TestRealm",
                ProtectedResourceName = "TestProtectedResource"
            }
        };
}
