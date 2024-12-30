namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Represents a Keycloak HTTP client that provides access to various Keycloak REST API services.
/// </summary>
public interface IKeycloakClient
{
    /// <summary>
    /// Gets the authentication REST client for managing authentication-related operations.
    /// See <see cref="IKcAuth"/> for detailed operations.
    /// </summary>
    public IKcAuth Auth { get; }

    /// <summary>
    /// Gets the attack detection REST client for monitoring and handling brute-force attacks.
    /// See <see cref="IKcAttackDetection"/> for available operations.
    /// </summary>
    public IKcAttackDetection AttackDetection { get; }

    /// <summary>
    /// Gets the initial access REST client for managing initial access tokens for client registrations.
    /// See <see cref="IKcClientInitialAccess"/> for more details.
    /// </summary>
    public IKcClientInitialAccess ClientInitialAccess { get; }

    /// <summary>
    /// Gets the users REST client for managing user accounts and related operations.
    /// See <see cref="IKcUsers"/> for supported functionality.
    /// </summary>
    public IKcUsers Users { get; }

    /// <summary>
    /// Gets the role mappings REST client for managing role mappings at the client level.
    /// See <see cref="IKcRoleMappings"/> for more details.
    /// </summary>
    public IKcRoleMappings RoleMappings { get; }

    /// <summary>
    /// Gets the roles REST client for managing realm and client roles.
    /// See <see cref="IKcRoles"/> for detailed functionality.
    /// </summary>
    public IKcRoles Roles { get; }

    /// <summary>
    /// Gets the client role mappings REST client for managing role mappings for specific clients.
    /// See <see cref="IKcClientRoleMappings"/> for more details.
    /// </summary>
    public IKcClientRoleMappings ClientRoleMappings { get; }

    /// <summary>
    /// Gets the client scopes REST client for managing client scopes.
    /// See <see cref="IKcClientScopes"/> for available operations.
    /// </summary>
    public IKcClientScopes ClientScopes { get; }

    /// <summary>
    /// Gets the clients REST client for managing clients and client-related configurations.
    /// See <see cref="IKcClients"/> for detailed operations.
    /// </summary>
    public IKcClients Clients { get; }

    /// <summary>
    /// Gets the groups REST client for managing groups and their memberships.
    /// See <see cref="IKcGroups"/> for more details.
    /// </summary>
    public IKcGroups Groups { get; }

    /// <summary>
    /// Gets the protocol mappers REST client for managing protocol mappers.
    /// See <see cref="IKcProtocolMappers"/> for detailed functionality.
    /// </summary>
    public IKcProtocolMappers ProtocolMappers { get; }

    /// <summary>
    /// Gets the scope mappings REST client for managing scope mappings.
    /// See <see cref="IKcScopeMappings"/> for detailed operations.
    /// </summary>
    public IKcScopeMappings ScopeMappings { get; }
}
