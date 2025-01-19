using NETCore.Keycloak.Client.Models.Roles;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Provides methods to generate mock instances of <see cref="KcRole"/> for testing purposes.
/// </summary>
public static class KcRoleMocks
{
    /// <summary>
    /// Generates a single mock <see cref="KcRole"/> instance with randomized data.
    /// </summary>
    /// <remarks>
    /// The generated role includes:
    /// - A random, lowercase name with spaces removed.
    /// - Predefined attributes, including a "test" attribute with a single value "0".
    /// </remarks>
    /// <returns>A mock <see cref="KcRole"/> instance.</returns>
    public static KcRole Generate()
    {
        // Generate a new realm role with a random name and predefined attributes
        var kcRole = new KcRole
        {
            Name = Guid.NewGuid().ToString().Replace("-", string.Empty, StringComparison.Ordinal),
            Attributes = new Dictionary<string, IEnumerable<string>>
            {
                {
                    "test", ["0"]
                }
            }
        };

        return kcRole;
    }

    /// <summary>
    /// Generates a collection of mock <see cref="KcRole"/> instances with randomized data.
    /// </summary>
    /// <param name="count">The number of roles to generate.</param>
    /// <returns>A collection of <see cref="KcRole"/> instances.</returns>
    public static IEnumerable<KcRole> Generate(int count)
    {
        var roles = new List<KcRole>();

        for ( var i = 0; i < count; i++ )
        {
            roles.Add(Generate());
        }

        return roles;
    }
}
