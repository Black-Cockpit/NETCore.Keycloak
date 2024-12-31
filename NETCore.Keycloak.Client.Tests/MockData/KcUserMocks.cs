using Bogus;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Provides mock data for Keycloak user objects to be used in testing scenarios.
/// </summary>
public static class KcUserMocks
{
    /// <summary>
    /// Generates a mock <see cref="KcUser"/> instance using the provided Faker object for randomized user data.
    /// </summary>
    /// <param name="faker">The <see cref="Faker"/> instance used to generate randomized data.</param>
    /// <returns>A <see cref="KcUser"/> object populated with mock data.</returns>
    /// <exception cref="AssertFailedException">
    /// Thrown if the provided <see cref="Faker"/> instance is null.
    /// </exception>
    public static KcUser GenerateUser(Faker faker)
    {
        // Ensure the faker instance is not null before proceeding.
        Assert.IsNotNull(faker);

        // Generate and return a mock KcUser object with randomized data.
        return new KcUser
        {
            Email = faker.Person.Email,
            Credentials = new List<KcCredentials>
            {
                new()
                {
                    Temporary = false,
                    UserLabel = "User password",
                    Type = "password",
                    Value = KcTestPasswordCreator.Create()
                }
            },
            Enabled = true,
            FirstName = faker.Person.FirstName,
            LastName = faker.Person.LastName,
            RealmRoles = new List<string>
            {
                "kc_client_role_1",
                "kc_client_role_2",
                "kc_client_role_3"
            },
            UserName = faker.Person.Email,
            EmailVerified = false
        };
    }
}
