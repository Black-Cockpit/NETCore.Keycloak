using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.Tests.MockData;

public class MockKcProtectedResourceStore : KcProtectedResourceStore
{
    public override ICollection<KcRealmProtectedResources> GetRealmProtectedResources() =>
        new List<KcRealmProtectedResources>
        {
            new KcRealmProtectedResources
            {
                Realm = "TestRealm",
                ProtectedResourceName = "TestProtectedResource"
            }
        };
}
