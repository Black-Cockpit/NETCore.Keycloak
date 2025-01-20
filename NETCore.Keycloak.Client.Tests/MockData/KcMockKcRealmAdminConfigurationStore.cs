using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Tests.Models;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Mock implementation of the <see cref="KcRealmAdminConfigurationStore"/> for testing purposes.
/// Provides predefined administrative configuration data for Keycloak realms to simulate
/// the behavior of the actual store.
/// </summary>
public class KcMockKcRealmAdminConfigurationStore : KcRealmAdminConfigurationStore
{
    /// <summary>
    /// Retrieves a predefined collection of administrative configurations for Keycloak realms.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="KcRealmAdminConfiguration"/> containing test data
    /// with a single entry for a test realm, including its Keycloak base URL, credentials,
    /// and client ID.
    /// </returns>
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
