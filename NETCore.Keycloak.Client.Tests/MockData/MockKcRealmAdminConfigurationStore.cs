using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Tests.Models;

namespace NETCore.Keycloak.Client.Tests.MockData;

public class MockKcRealmAdminConfigurationStore : KcRealmAdminConfigurationStore
{
    public override ICollection<KcRealmAdminConfiguration> GetRealmsAdminConfiguration() =>
        new List<KcRealmAdminConfiguration>
        {
            new()
            {
                KeycloakBaseUrl = "https://realm.keycloak.io/",
                Realm = "realm",
                RealmAdminCredentials = new KcTestUser
                {
                    Username = "admin",
                    Password = "password"
                },
                ClientId = "client"
            }
        };
}
