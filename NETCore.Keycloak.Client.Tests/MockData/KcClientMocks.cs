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
    /// Generates a mock Keycloak public client with randomized attributes for testing.
    /// </summary>
    /// <param name="faker">An instance of the Faker library used to generate randomized data.</param>
    /// <returns>A <see cref="KcClient"/> object with predefined and randomized properties.</returns>
    public static KcClient GeneratePublicClient(Faker faker)
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

    /// <summary>
    /// Generates a mock private <see cref="KcClient"/> instance using the provided <see cref="Faker"/> object for randomized data.
    /// </summary>
    /// <param name="faker">The <see cref="Faker"/> instance used to generate randomized client data.</param>
    /// <returns>A <see cref="KcClient"/> object populated with mock data.</returns>
    /// <exception cref="AssertFailedException">
    /// Thrown if the provided <see cref="Faker"/> instance is null.
    /// </exception>
    public static KcClient GeneratePrivateClient(Faker faker)
    {
        // Ensure the Faker instance is not null before generating data.
        Assert.IsNotNull(faker);

        // Create a private client with randomized details.
        var kClient = new KcClient
        {
            Access = new KcClientAccess
            {
                Configure = true,
                Manage = true,
                View = true
            },
            ClientId = Guid.NewGuid().ToString(), // Unique client identifier.
            Secret = Guid.NewGuid().ToString(), // Secret key for authentication.
            Name = faker.Company.CompanyName(),
            Description = faker.Lorem.Sentence(),
            BaseUrl = faker.Internet.Url(),
            Enabled = true,
            Protocol = KcProtocol.OpenidConnect, // Protocol used by the client.
            PublicClient = false, // Indicates this is a private client.
            ClientAuthenticatorType = KcClientAuthenticatorType.Secret, // Authenticator type set to secret.
            DirectAccessGrantsEnabled = true,
            ServiceAccountsEnabled = true,
            AuthorizationServicesEnabled = true,
            AlwaysDisplayInConsole = false,
            StandardFlowEnabled = false,
            FullScopeAllowed = true,
            AdminUrl = faker.Internet.Url(), // URL for admin management.
            RedirectUris =
            [
                faker.Internet.Url() // URL for redirect after login.
            ],
            RootUrl = faker.Internet.Url(), // Root URL of the client.
            WebOrigins =
            [
                faker.Internet.Url() // Web origins allowed for CORS.
            ],
            FrontChannelLogout = true, // Indicates front-channel logout is enabled.
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
            ],
            AuthorizationSettings = new KcClientResourceServer
            {
                PolicyEnforcementMode = KcPolicyEnforcementMode.Enforcing, // Policy enforcement mode.
                AllowRemoteResourceManagement = true, // Remote resource management enabled.
                Resources =
                [
                    new KcClientResource
                    {
                        Name = "Account", // Resource name.
                        Type = "urn:api:resources:account", // Resource type.
                        Uris =
                        [
                            faker.Internet.Url() // URI associated with the resource.
                        ],
                        Scopes =
                        [
                            new KcClientResourceScope
                            {
                                Name = "Get" // Scope name for permissions.
                            }
                        ],
                        DisplayName = "Account" // Display name for the resource.
                    }
                ]
            }
        };

        // Add custom attributes to the client for additional configuration.
        _ = kClient.Attributes.TryAdd("backchannel.logout.session.required", true);
        _ = kClient.Attributes.TryAdd("client_credentials.use_refresh_token", false);
        _ = kClient.Attributes.TryAdd("display.on.consent.screen", false);
        _ = kClient.Attributes.TryAdd("oauth2.device.authorization.grant.enabled", false);
        _ = kClient.Attributes.TryAdd("backchannel.logout.revoke.offline.tokens", false);
        _ = kClient.Attributes.TryAdd("use.refresh.tokens", true);
        _ = kClient.Attributes.TryAdd("exclude.session.state.from.auth.response", false);

        // Return the fully configured private client object.
        return kClient;
    }
}
