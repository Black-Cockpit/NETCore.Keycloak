using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.HttpClients.Abstraction;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKeycloakClient"/>
// ReSharper disable once InconsistentNaming
public sealed class KeycloakClient : IKeycloakClient
{
    /// <inheritdoc cref="IKeycloakClient.Auth"/>
    public IKcAuth Auth { get; }

    /// <inheritdoc cref="IKeycloakClient.AttackDetection"/>
    public IKcAttackDetection AttackDetection { get; }

    /// <inheritdoc cref="IKeycloakClient.ClientInitialAccess"/>
    public IKcClientInitialAccess ClientInitialAccess { get; }

    /// <inheritdoc cref="IKeycloakClient.Users"/>
    public IKcUsers Users { get; }

    /// <inheritdoc cref="IKeycloakClient.RoleMappings"/>
    public IKcRoleMappings RoleMappings { get; }

    /// <inheritdoc cref="IKeycloakClient.Roles"/>
    public IKcRoles Roles { get; }

    /// <inheritdoc cref="IKeycloakClient.ClientRoleMappings"/>
    public IKcClientRoleMappings ClientRoleMappings { get; }

    /// <inheritdoc cref="IKeycloakClient.ClientScopes"/>
    public IKcClientScopes ClientScopes { get; }

    /// <inheritdoc cref="IKeycloakClient.Clients"/>
    public IKcClients Clients { get; }

    /// <inheritdoc cref="IKeycloakClient.Groups"/>
    public IKcGroups Groups { get; }

    /// <inheritdoc cref="IKeycloakClient.ProtocolMappers"/>
    public IKcProtocolMappers ProtocolMappers { get; }

    /// <inheritdoc cref="IKeycloakClient.ScopeMappings"/>
    public IKcScopeMappings ScopeMappings { get; }

    /// <inheritdoc cref="IKeycloakClient.Organizations"/>
    public IKcOrganizations Organizations { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakClient"/> class.
    /// Provides access to various Keycloak API services through respective clients.
    /// Uses a default shared <see cref="HttpClient"/> instance to avoid socket exhaustion.
    /// </summary>
    /// <param name="baseUrl">
    /// The base URL of the Keycloak server.
    /// Example: <c>http://localhost:8080</c>.
    /// The trailing slash, if present, will be automatically removed.
    /// </param>
    /// <param name="logger">
    /// An optional logger instance for logging activities.
    /// If not provided, logging will be disabled. See <see cref="ILogger"/>.
    /// </param>
    /// <exception cref="KcException">Thrown if the <paramref name="baseUrl"/> is null, empty, or contains only whitespace.</exception>
    public KeycloakClient(string baseUrl, ILogger logger = null)
        : this(baseUrl, logger, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakClient"/> class with an optional <see cref="IHttpClientFactory"/>.
    /// Provides access to various Keycloak API services through respective clients.
    /// When an <see cref="IHttpClientFactory"/> is provided, it is used to create <see cref="HttpClient"/> instances
    /// with managed handler lifetimes and connection pooling. Otherwise, a default shared <see cref="HttpClient"/> is used.
    /// </summary>
    /// <param name="baseUrl">
    /// The base URL of the Keycloak server.
    /// Example: <c>http://localhost:8080</c>.
    /// The trailing slash, if present, will be automatically removed.
    /// </param>
    /// <param name="logger">
    /// An optional logger instance for logging activities.
    /// If not provided, logging will be disabled. See <see cref="ILogger"/>.
    /// </param>
    /// <param name="httpClientFactory">
    /// An optional <see cref="IHttpClientFactory"/> for creating <see cref="HttpClient"/> instances.
    /// When <c>null</c>, a default shared <see cref="HttpClient"/> with pooled connection lifetime is used.
    /// </param>
    /// <exception cref="KcException">Thrown if the <paramref name="baseUrl"/> is null, empty, or contains only whitespace.</exception>
    public KeycloakClient(string baseUrl, ILogger logger, IHttpClientFactory httpClientFactory)
    {
        if ( string.IsNullOrWhiteSpace(baseUrl) )
        {
            throw new KcException($"{nameof(baseUrl)} is required");
        }

        // Remove the trailing slash from the base URL if it exists.
        baseUrl = baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? baseUrl[..^1]
            : baseUrl;

        // Define the admin API base URL for realm-specific administrative operations.
        var adminUrl = $"{baseUrl}/admin/realms";

        // Initialize various Keycloak API clients with their respective base URLs, logger, and HTTP client factory.
        Auth = new KcAuth($"{baseUrl}/realms", logger, httpClientFactory);
        AttackDetection = new KcAttackDetection(adminUrl, logger, httpClientFactory);
        ClientInitialAccess = new KcClientInitialAccess(adminUrl, logger, httpClientFactory);
        Users = new KcUsers(adminUrl, logger, httpClientFactory);
        Roles = new KcRoles(adminUrl, logger, httpClientFactory);
        ClientRoleMappings = new KcClientRoleMappings(adminUrl, logger, httpClientFactory);
        ClientScopes = new KcClientScopes(adminUrl, logger, httpClientFactory);
        Clients = new KcClients(adminUrl, logger, httpClientFactory);
        Groups = new KcGroups(adminUrl, logger, httpClientFactory);
        ProtocolMappers = new KcProtocolMappers(adminUrl, logger, httpClientFactory);
        ScopeMappings = new KcScopeMappings(adminUrl, logger, httpClientFactory);
        RoleMappings = new KcRoleMappings(adminUrl, logger, httpClientFactory);
        Organizations = new KcOrganizations(adminUrl, logger, httpClientFactory);
    }
}
