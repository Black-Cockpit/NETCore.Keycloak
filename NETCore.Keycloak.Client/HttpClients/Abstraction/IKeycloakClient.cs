namespace NETCore.Keycloak.Client.HttpClients.Abstraction;

/// <summary>
/// Keycloak http client
/// </summary>
public interface IKeycloakClient
{
    /// <summary>
    /// Authentication rest client. <see cref="IKcAuth"/>
    /// </summary>
    public IKcAuth Auth { get; }

    /// <summary>
    /// Attack detection rest client. <see cref="IKcAttackDetection"/>
    /// </summary>
    public IKcAttackDetection AttackDetection { get; }

    /// <summary>
    /// Initial access rest client. <see cref="IKcClientInitialAccess"/>
    /// </summary>
    public IKcClientInitialAccess ClientInitialAccess { get; }

    /// <summary>
    /// Users rest client. <see cref="IKcUsers"/>
    /// </summary>
    public IKcUsers Users { get; }

    /// <summary>
    /// Client role mapping. <see cref="IKcRoleMappings"/>
    /// </summary>
    public IKcRoleMappings RoleMappings { get; }

    /// <summary>
    /// Roles rest client. <see cref="IKcRoles"/>
    /// </summary>
    public IKcRoles Roles { get; }

    /// <summary>
    /// Role mappings rest client. <see cref="IKcClientRoleMappings"/>
    /// </summary>
    public IKcClientRoleMappings ClientRoleMappings { get; }

    /// <summary>
    /// Client scopes rest client. <see cref="IKcClientScopes"/>
    /// </summary>
    public IKcClientScopes ClientScopes { get; }

    /// <summary>
    /// Clients rest client. <see cref="IKcClients"/>
    /// </summary>
    public IKcClients Clients { get; }

    /// <summary>
    /// Groups rest client. <see cref="IKcGroups"/>
    /// </summary>
    public IKcGroups Groups { get; }

    /// <summary>
    /// Protocol mappers rest client. <see cref="IKcProtocolMappers"/>
    /// </summary>
    public IKcProtocolMappers ProtocolMappers { get; }

    /// <summary>
    /// Scope mappings rest client. <see cref="IKcScopeMappings"/>
    /// </summary>
    public IKcScopeMappings ScopeMappings { get; }
}
