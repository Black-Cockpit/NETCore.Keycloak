using Bogus;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Provides mock data generation for Keycloak client objects used in testing scenarios.
/// This class leverages the Faker library to generate realistic data for testing Keycloak client configurations.
/// </summary>
public static class KcClientMocks
{
    /// <summary>
    /// Generates a mock Keycloak client with randomized attributes for testing.
    /// </summary>
    /// <param name="faker">An instance of the Faker library used to generate randomized data.</param>
    /// <returns>A <see cref="KcClient"/> object with predefined and randomized properties.</returns>
    public static KcClient GenerateClient(Faker faker)
    {
        Assert.IsNotNull(faker);

        var kClient = new KcClient
        {
            Access = new KcClientAccess
            {
                Configure = true,
                Manage = true,
                View = true
            },
            ClientId = Guid.NewGuid().ToString(),
            Secret = Guid.NewGuid().ToString(),
            Name = faker.Company.CompanyName(),
            Description = faker.Lorem.Sentence(),
            BaseUrl = faker.Internet.Url(),
            Enabled = true,
            Protocol = KcProtocol.OpenidConnect,
            PublicClient = true,
            ClientAuthenticatorType = KcClientAuthenticatorType.Secret,
            DirectAccessGrantsEnabled = true,
            FullScopeAllowed = true,
            AdminUrl = faker.Internet.Url(),
            RedirectUris =
            [
                faker.Internet.Url()
            ],
            RootUrl = faker.Internet.Url(),
            WebOrigins =
            [
                faker.Internet.Url()
            ],
            FrontChannelLogout = true,
            DefaultClientScopes =
            [
                "acr",
                "attributes",
                "audiences",
                "email",
                "offline_access",
                "profile",
                "roles",
                "web-origins"
            ],
            OptionalClientScopes =
            [
                "address",
                "microprofile-jwt",
                "phone"
            ]
        };

        // Add custom attributes to the client.
        _ = kClient.Attributes.TryAdd("backchannel.logout.session.required", true);
        _ = kClient.Attributes.TryAdd("client_credentials.use_refresh_token", false);
        _ = kClient.Attributes.TryAdd("display.on.consent.screen", false);
        _ = kClient.Attributes.TryAdd("oauth2.device.authorization.grant.enabled", false);
        _ = kClient.Attributes.TryAdd("backchannel.logout.revoke.offline.tokens", false);
        _ = kClient.Attributes.TryAdd("use.refresh.tokens", true);
        _ = kClient.Attributes.TryAdd("exclude.session.state.from.auth.response", false);
        return kClient;
    }
}
