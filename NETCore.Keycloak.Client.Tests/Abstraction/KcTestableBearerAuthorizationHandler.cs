using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NETCore.Keycloak.Client.Authorization.Handlers;
using NETCore.Keycloak.Client.Authorization.Requirements;

namespace NETCore.Keycloak.Client.Tests.Abstraction;

/// <summary>
/// A testable implementation of the <see cref="KcBearerAuthorizationHandler"/> that exposes
/// the protected <see cref="KcBearerAuthorizationHandler.HandleRequirementAsync"/> method for testing purposes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="KcTestableBearerAuthorizationHandler"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The service provider used to resolve scoped dependencies, such as
/// <see cref="IHttpContextAccessor"/> and <see cref="IKcRealmAdminTokenHandler"/>.
/// </param>
public class KcTestableBearerAuthorizationHandler(IServiceProvider serviceProvider) : KcBearerAuthorizationHandler(serviceProvider)
{
    /// <summary>
    /// Exposes the <see cref="KcBearerAuthorizationHandler.HandleRequirementAsync"/> method
    /// for unit testing purposes.
    /// </summary>
    /// <param name="context">The <see cref="AuthorizationHandlerContext"/> for the current authorization request.</param>
    /// <param name="requirement">The <see cref="KcAuthorizationRequirement"/> being evaluated.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task TestHandleRequirementAsync(AuthorizationHandlerContext context,
        KcAuthorizationRequirement requirement) =>
        await HandleRequirementAsync(context, requirement).ConfigureAwait(false);
}
