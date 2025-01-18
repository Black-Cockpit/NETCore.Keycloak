using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NETCore.Keycloak.Client.Authorization.Handlers;
using NETCore.Keycloak.Client.Authorization.PolicyProviders;
using NETCore.Keycloak.Client.Authorization.Store;

namespace NETCore.Keycloak.Client.Authorization;

/// <summary>
/// Keycloak authorization extension
/// </summary>
public static class KcAuthorizationExtension
{
    /// <summary>
    /// Add keycloak authorization
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddKeycloakAuthorization(this IServiceCollection services) =>
        services
            .AddSingleton<IAuthorizationHandler, KcBearerAuthorizationHandler>().AddHttpContextAccessor();

    /// <summary>
    /// Add keycloak policy provider
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddKeycloakProtectedResourcesPolicies<T, TV>(
        this IServiceCollection services)
        where T : KcProtectedResourceStore
        where TV : KcRealmAdminConfigurationStore
    {
        _ = services.AddSingleton<KcProtectedResourceStore, T>()
            .AddSingleton<KcRealmAdminConfigurationStore, TV>()
            .AddSingleton<IKcRealmAdminTokenHandler, KcRealmAdminTokenHandler>()
            .AddSingleton<IAuthorizationPolicyProvider, KcProtectedResourcePolicyProvider>(
                provider =>
                    new KcProtectedResourcePolicyProvider(provider,
                        provider.GetRequiredService<IOptions<AuthorizationOptions>>()));
        return services;
    }
}
