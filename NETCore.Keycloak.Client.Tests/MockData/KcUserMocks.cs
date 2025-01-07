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
    /// Generates a mock <see cref="KcUser"/> instance with randomized user data for testing purposes.
    /// </summary>
    /// <remarks>
    /// This method uses the provided <see cref="Faker"/> instance to generate mock values for the user's attributes,
    /// including email, first name, and last name. If a <paramref name="password"/> is not provided, a default test password is generated.
    /// Additional user attributes can be set via the optional <paramref name="attributes"/> parameter.
    /// </remarks>
    /// <param name="faker">The <see cref="Faker"/> instance used to generate randomized user data.</param>
    /// <param name="password">An optional user password. If null or empty, a generated password will be used.</param>
    /// <param name="attributes">An optional dictionary of additional attributes to assign to the user.</param>
    /// <returns>A <see cref="KcUser"/> object populated with mock user data.</returns>
    /// <exception cref="AssertFailedException">
    /// Thrown if the <paramref name="faker"/> parameter is null to ensure valid input during testing.
    /// </exception>
    public static KcUser GenerateUser(Faker faker, string password = null,
        IDictionary<string, object> attributes = null)
    {
        // Ensure the Faker instance is not null before generating user data.
        Assert.IsNotNull(faker, "The Faker instance must not be null.");

        // Generate and return a mock KcUser object with randomized user data.
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
                    Value = !string.IsNullOrEmpty(password)
                        ? password
                        : KcTestPasswordCreator.Create()
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
            EmailVerified = false,
            Attributes = attributes
        };
    }
}
