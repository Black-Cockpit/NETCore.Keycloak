using Microsoft.AspNetCore.Authorization;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.Authorization.Requirements;

/// <summary>
/// Keycloak authorization requirement
/// </summary>
public class KcAuthorizationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Keycloak protected resources
    /// <see cref="KcProtectedResource"/>
    /// </summary>
    public KcProtectedResourceStore ProtectedResourceStore { get; }

    /// <summary>
    /// Resource name
    /// </summary>
    public string Resource { get; }

    /// <summary>
    /// Resource scope
    /// </summary>
    private string Scope { get; }

    /// <summary>
    /// Constructs requirement
    /// </summary>
    /// <param name="protectedResourceStore">Protected resources store. <see cref="KcProtectedResourceStore"/></param>
    /// <param name="resource"></param>
    /// <param name="scope"></param>
    public KcAuthorizationRequirement(KcProtectedResourceStore protectedResourceStore, string resource,
        string scope)
    {
        ProtectedResourceStore = protectedResourceStore;
        Resource = resource;
        Scope = scope;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Resource}#{Scope}";
}
