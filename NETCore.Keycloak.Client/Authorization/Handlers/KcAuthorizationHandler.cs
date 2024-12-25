using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.Authorization.Requirements;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.Authorization.Handlers;

/// <summary>
/// Keycloak authorization handler
/// </summary>
public class KcAuthorizationHandler : AuthorizationHandler<KcAuthorizationRequirement>
{
    /// <summary>
    /// Logger <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Http accessor: Used to extract the authorization token. <see cref="IHttpContextAccessor"/>
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Realm admin handler. <see cref="IKcRealmAdminTokenHandler"/>
    /// </summary>
    private readonly IKcRealmAdminTokenHandler _realmAdminTokenHandler;

    /// <summary>
    /// Constructs handler
    /// </summary>
    /// <param name="serviceProvider">Service provider, <see cref="IServiceProvider"/></param>
    public KcAuthorizationHandler(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        _logger = scope.ServiceProvider.GetService<ILogger>();
        _httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        _realmAdminTokenHandler = scope.ServiceProvider.GetRequiredService<IKcRealmAdminTokenHandler>();
    }

    /// <inheritdoc/>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        KcAuthorizationRequirement requirement)
    {
        if ( requirement == null )
        {
            throw new ArgumentNullException(nameof(requirement),
                $"{nameof(requirement)} is required");
        }

        if ( context == null )
        {
            throw new ArgumentNullException(nameof(context), $"{nameof(context)} is required");
        }

        // Check if user is authenticated
        if ( context.User.Identity?.IsAuthenticated ?? false )
        {
            // Get authorization data
            var authorizationData =
                _httpContextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Split(" ");

            // Check if authorization is JWT Bearer scheme
            if ( authorizationData == null || authorizationData.Length < 2 ||
                 authorizationData[0] != JwtBearerDefaults.AuthenticationScheme )
            {
                return; // Should not fail as the context of the handler is only for Bearer tokens.
            }

            // Extract Bearer token
            if ( authorizationData.ElementAtOrDefault(1) is var identityToken &&
                 string.IsNullOrWhiteSpace(identityToken) )
            {
                context.Fail();
            }

            // Extract realm name and keycloak base URL from issuer claim
            var (baseUrl, realm) = TryExtractRealm(identityToken);

            // Should fail if issuer is null or realm is null
            if ( string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(realm) )
            {
                context.Fail();
                return;
            }

            // Fetch protected resource
            if ( requirement.ProtectedResourceStore.GetRealmProtectedResources()
                    .FirstOrDefault(resource => resource.Realm == realm)?.ProtectedResourceName is
                { } protectedResource )
            {
                // Initialize keycloak client instance
                var keycloakClient = new KeycloakClient(baseUrl, _logger);

                // Validate user session
                await ValidateUserSession(context, keycloakClient, realm).ConfigureAwait(false);

                // Request party token and verify if the user has access to protected resource
                if ( await keycloakClient.Auth.GetRequestPartyTokenAsync(realm, identityToken,
                        protectedResource, [
                            requirement.ToString()
                        ]).ConfigureAwait(false) is var rptResponse && rptResponse.IsError )
                {
                    if ( _logger != null )
                    {
                        KcLoggerMessages.Error(_logger,
                            $"Access to {protectedResource} resource {requirement} permission is denied",
                            rptResponse.Exception);
                    }

                    context.Fail();
                    return;
                }

                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }

    /// <summary>
    /// Get token issuer url
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    private (string, string) TryExtractRealm(string accessToken)
    {
        if ( string.IsNullOrWhiteSpace(accessToken) )
        {
            return (null, null);
        }

        try
        {
            // Read JWT token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            // Extract issuer claim
            if ( token.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value is var issuer &&
                 string.IsNullOrWhiteSpace(issuer) )
            {
                return (null, null);
            }

            // Read issuer URL
            var urlData = new Uri(issuer);

            // Extract realm name and base URL
            return ($"{urlData.Scheme}://{urlData.Authority}",
                urlData.AbsolutePath.Replace("/realms/", string.Empty, StringComparison.Ordinal));
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable extract issuer from token", e);
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Validate user session
    /// </summary>
    /// <param name="context"></param>
    /// <param name="keycloakClient"></param>
    /// <param name="realm"></param>
    /// <exception cref="KcUserNotFoundException"></exception>
    /// <exception cref="KcSessionClosedException"></exception>
    private async Task ValidateUserSession(AuthorizationHandlerContext context, KeycloakClient keycloakClient,
        string realm)
    {
        // Ensure realm is not null.
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new ArgumentNullException(nameof(realm), $"{nameof(realm)} is required");
        }

        // Get realm admin token
        var adminToken = await _realmAdminTokenHandler.TryGetAdminTokenAsync(realm).ConfigureAwait(false);

        // Try extract subject from user claims
        if ( context.User.Claims
                 .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value is var userId &&
             string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcUserNotFoundException("Unable to extract user subject");
        }

        // Check if user is deleted
        if ( await keycloakClient.Users.GetAsync(realm, adminToken, userId).ConfigureAwait(false) is var
                userResponse && userResponse.IsError )
        {
            throw new KcUserNotFoundException($"User {userId} not found, {userResponse.ErrorMessage}",
                userResponse.Exception);
        }

        // Try extract session id from user claims
        if ( context.User.Claims
                 .FirstOrDefault(claim => claim.Type == "sid")?.Value is var sessionId &&
             string.IsNullOrWhiteSpace(sessionId) )
        {
            throw new KcSessionClosedException("Unable to extract session id");
        }

        // Check if user has sessions
        if ( await keycloakClient.Users.SessionsAsync(realm, adminToken, userId).ConfigureAwait(false) is var
                sessionsResponse && sessionsResponse.IsError )
        {
            throw new KcSessionClosedException($"No active session found for user {userId}",
                userResponse.Exception);
        }

        // Check if session is always active
        if ( sessionsResponse.Response.All(session => session.Id != sessionId) )
        {
            throw new KcSessionClosedException($"Session {sessionId} not found for user {userId}",
                userResponse.Exception);
        }
    }
}
