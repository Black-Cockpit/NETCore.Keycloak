using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NETCore.Keycloak.Client.Authorization.Requirements;
using NETCore.Keycloak.Client.Authorization.Store;

namespace NETCore.Keycloak.Client.Authorization.PolicyProviders;

/// <summary>
/// Keycloak policy provider.
/// It builds policies based on their names.
/// <example>
/// [Authorize(Policy = "resource#scope")]
/// </example>
/// </summary>
public class KcProtectedResourcePolicyProvider : DefaultAuthorizationPolicyProvider
{
    /// <summary>
    /// Protected resources store. <see cref="KcProtectedResourceStore"/>
    /// </summary>
    private readonly KcProtectedResourceStore _protectedResourceStore;

    /// <summary>
    /// Cache authorization policies
    /// </summary>
    private readonly IDictionary<string, AuthorizationPolicy> _cachedPolices =
        new ConcurrentDictionary<string, AuthorizationPolicy>();

    /// <summary>
    /// Construct provider
    /// </summary>
    /// <param name="serviceProvider">Service provider. <see cref="IServiceProvider"/></param>
    /// <param name="options">Authorization options</param>
    public KcProtectedResourcePolicyProvider(IServiceProvider serviceProvider,
        IOptions<AuthorizationOptions> options) : base(options)
    {
        using var scope = serviceProvider.CreateScope();
        _protectedResourceStore = scope.ServiceProvider.GetRequiredService<KcProtectedResourceStore>();
    }

    /// <inheritdoc cref="IAuthorizationPolicyProvider.GetPolicyAsync"/>>
    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if ( string.IsNullOrWhiteSpace(policyName) )
        {
            throw new ArgumentNullException(policyName, $"{nameof(policyName)} is required");
        }

        // Get policy name
        var registeredPolicy = await base.GetPolicyAsync(policyName).ConfigureAwait(false);

        // Check if policy name match the format resource#scope
        if ( policyName.Contains('#', StringComparison.Ordinal) && registeredPolicy is not null )
        {
            return registeredPolicy;
        }

        // Initialize policy builder
        var builder = new AuthorizationPolicyBuilder();

        // Split policy name to identify the resource name and scope
        var kcPermission = policyName.Split('#');

        if ( kcPermission is not { Length: 2 } )
        {
            return null;
        }

        // Try get cached policy
        _ = _cachedPolices.TryGetValue(policyName, out var cachedPolicy);

        if ( cachedPolicy != null )
        {
            return cachedPolicy;
        }

        // Add authorization requirement
        var authorizationRequirement = new KcAuthorizationRequirement(_protectedResourceStore, kcPermission[0],
            kcPermission[1]);

        _ = builder.AddRequirements(authorizationRequirement);

        var policy = builder.Build();

        // Cache built policy
        _cachedPolices.Add(policyName, policy);

        return policy;
    }
}
