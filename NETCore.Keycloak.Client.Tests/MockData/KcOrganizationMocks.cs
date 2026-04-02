using Bogus;
using NETCore.Keycloak.Client.Models.Organizations;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Provides mock data generation for Keycloak organization objects used in testing scenarios.
/// This class leverages the Faker library to generate realistic data for testing Keycloak organization configurations.
/// </summary>
public static class KcOrganizationMocks
{
    /// <summary>
    /// Generates a mock <see cref="KcOrganization"/> instance with randomized test data.
    /// </summary>
    /// <param name="faker">The <see cref="Faker"/> instance used to generate randomized data.</param>
    /// <returns>A new <see cref="KcOrganization"/> instance populated with test data.</returns>
    /// <exception cref="AssertFailedException">
    /// Thrown if the provided <see cref="Faker"/> instance is null.
    /// </exception>
    public static KcOrganization Generate(Faker faker)
    {
        // Ensure the Faker instance is not null before generating data.
        Assert.IsNotNull(faker);

        // Generate a unique name for the organization.
        var orgName = Guid.NewGuid().ToString().Replace("-", string.Empty, StringComparison.Ordinal);

        // Create an organization with randomized details.
        return new KcOrganization
        {
            Name = orgName,
            Alias = orgName,
            Enabled = true,
            Description = faker.Lorem.Sentence(),
            RedirectUrl = faker.Internet.Url(),
            Attributes = new Dictionary<string, List<string>>
            {
                { "test_attr", ["value1"] }
            },
            Domains =
            [
                new KcOrganizationDomain
                {
                    Name = $"{orgName}.{faker.Internet.DomainSuffix()}",
                    Verified = false
                }
            ]
        };
    }
}
