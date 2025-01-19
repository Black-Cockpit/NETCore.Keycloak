using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Provides mock data generation for Keycloak protocol mappers using Faker to create realistic test data.
/// </summary>
public static class KcProtocolMapperMocks
{
    /// <summary>
    /// Generates a single Keycloak protocol mapper with random values for testing purposes.
    /// </summary>
    public static KcProtocolMapper Generate() =>
        new()
        {
            Name = Guid.NewGuid().ToString().Replace("-", string.Empty, StringComparison.Ordinal),
            Protocol = KcProtocol.OpenidConnect,
            ProtocolMapper = "oidc-usermodel-attribute-mapper",
            Config = new Dictionary<string, string>
            {
                {
                    "aggregate.attrs", "false"
                },
                {
                    "userinfo.token.claim", "false"
                }
            }
        };

    /// <summary>
    /// Generates a collection of Keycloak protocol mappers with random values for testing purposes.
    /// </summary>
    public static IEnumerable<KcProtocolMapper> Generate(int count)
    {
        var list = new List<KcProtocolMapper>();

        for ( var i = 0; i < count; i++ )
        {
            list.Add(Generate());
        }

        return list;
    }
}
