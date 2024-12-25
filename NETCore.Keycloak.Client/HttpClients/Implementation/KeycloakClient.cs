using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKeycloakClient"/>
// ReSharper disable once InconsistentNaming
public class KeycloakClient : IKeycloakClient
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

    /// <summary>
    /// Keycloak client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url. Example: http://localhost:8080</param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KeycloakClient(string baseUrl, ILogger logger = null)
    {
        if ( string.IsNullOrWhiteSpace(baseUrl) )
        {
            throw new KcException($"{nameof(baseUrl)} is required");
        }

        // Remove last "/" from base url
        baseUrl = baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? baseUrl.Remove(baseUrl.Length - 1, 1)
            : baseUrl;

        var adminUrl = $"{baseUrl}/admin/realms";

        Auth = new KcAuth($"{baseUrl}/realms", logger);
        AttackDetection = new KcAttackDetection(adminUrl, logger);
        ClientInitialAccess = new KcClientInitialAccess(adminUrl, logger);
        Users = new KcUsers(adminUrl, logger);
        Roles = new KcRoles(adminUrl, logger);
        ClientRoleMappings = new KcClientRoleMappings(adminUrl, logger);
        ClientScopes = new KcClientScopes(adminUrl, logger);
        Clients = new KcClients(adminUrl, logger);
        Groups = new KcGroups(adminUrl, logger);
        ProtocolMappers = new KcProtocolMappers(adminUrl, logger);
        ScopeMappings = new KcScopeMappings(adminUrl, logger);
        RoleMappings = new KcRoleMappings(adminUrl, logger);
    }
}
