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
/// Authorization handler for Keycloak that validates user sessions and permissions for accessing protected resources.
/// </summary>
public class KcBearerAuthorizationHandler : AuthorizationHandler<KcAuthorizationRequirement>
{
    /// <summary>
    /// Logger for logging Keycloak authorization events.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// HTTP context accessor used to extract authorization tokens from the current HTTP request.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Realm admin token handler responsible for retrieving admin tokens for validating user sessions.
    /// </summary>
    private readonly IKcRealmAdminTokenHandler _realmAdminTokenHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="KcBearerAuthorizationHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to create scoped services.</param>
    public KcBearerAuthorizationHandler(IServiceProvider serviceProvider)
    {
        // Create a service scope for resolving scoped dependencies.
        using var scope = serviceProvider.CreateScope();

        // Resolve the logger instance.
        _logger = scope.ServiceProvider.GetService<ILogger>();

        // Resolve the HTTP context accessor for accessing request data.
        _httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

        // Resolve the realm admin token handler for retrieving admin tokens.
        _realmAdminTokenHandler = scope.ServiceProvider.GetRequiredService<IKcRealmAdminTokenHandler>();
    }

    /// <summary>
    /// Handles the authorization requirement by validating the user session and checking permissions for protected resources.
    /// </summary>
    /// <param name="context">The authorization handler context.</param>
    /// <param name="requirement">The authorization requirement defining the protected resource and permissions.</param>
    /// <exception cref="ArgumentNullException">Thrown if the context or requirement is null.</exception>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        KcAuthorizationRequirement requirement)
    {
        // Ensure that authorization context is not null
        ArgumentNullException.ThrowIfNull(context, "Authorization context is required.");

        // Ensure that authorization requirement is not null.
        ArgumentNullException.ThrowIfNull(requirement, "Authorization requirement is required.");

        // Check if the user is authenticated
        if ( context.User.Identity?.IsAuthenticated ?? false )
        {
            // Extract the authorization header value
            var authorizationData =
                _httpContextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Split(" ");

            // Ensure the authorization header uses the Bearer scheme and contains a token
            if ( authorizationData == null || authorizationData.Length < 2 ||
                 authorizationData[0] != JwtBearerDefaults.AuthenticationScheme )
            {
                return; // No action if not Bearer scheme
            }

            // Extract and validate the Bearer token
            var identityToken = authorizationData.ElementAtOrDefault(1);
            if ( string.IsNullOrWhiteSpace(identityToken) )
            {
                context.Fail();
                return;
            }

            // Extract the base URL and realm name from the token's issuer claim
            var (baseUrl, realm) = TryExtractRealm(identityToken);

            // Fail the context if the base URL or realm name is missing
            if ( string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(realm) )
            {
                context.Fail();
                return;
            }

            // Fetch protected resources and check for authorization
            var protectedResource = requirement.ProtectedResourceStore.GetRealmProtectedResources()
                .FirstOrDefault(resource => resource.Realm == realm)?.ProtectedResourceName;

            if ( protectedResource != null )
            {
                // Initialize the Keycloak client
                var keycloakClient = new KeycloakClient(baseUrl, _logger);

                // Validate the user session
                await ValidateUserSession(context, keycloakClient, realm).ConfigureAwait(false);

                // Request party token and check for access to the protected resource
                var rptResponse = await keycloakClient.Auth.GetRequestPartyTokenAsync(realm, identityToken,
                        protectedResource, [requirement.ToString()])
                    .ConfigureAwait(false);

                if ( rptResponse.IsError )
                {
                    // Log an error if the request party token (RPT) request failed
                    KcLoggerMessages.Error(_logger,
                        $"Access to {protectedResource} resource {requirement} permission is denied",
                        rptResponse.Exception);
                    context.Fail();
                    return;
                }

                // Succeed the authorization context if access is granted
                context.Succeed(requirement);
                return;
            }

            // Fail the context if the protected resource is not found
            context.Fail();
        }
    }

    /// <summary>
    /// Extracts the base URL and realm name from the token's issuer claim.
    /// </summary>
    /// <param name="accessToken">The JWT access token.</param>
    /// <returns>A tuple containing the base URL and realm name.</returns>
    private (string, string) TryExtractRealm(string accessToken)
    {
        if ( string.IsNullOrWhiteSpace(accessToken) )
        {
            return (null, null);
        }

        try
        {
            // Read the JWT token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            // Extract the issuer claim
            var issuer = token.Claims.FirstOrDefault(claim => claim.Type == "iss")?.Value;
            if ( string.IsNullOrWhiteSpace(issuer) )
            {
                return (null, null);
            }

            // Parse the issuer URL to extract the base URL and realm name
            var urlData = new Uri(issuer);
            return ($"{urlData.Scheme}://{urlData.Authority}",
                urlData.AbsolutePath.Replace("/realms/", string.Empty, StringComparison.Ordinal));
        }
        catch ( Exception e )
        {
            // Log an error if unable to extract the issuer
            KcLoggerMessages.Error(_logger, "Unable to extract issuer from token", e);
            return (null, null);
        }
    }

    /// <summary>
    /// Validates the user's session to ensure the session is active and valid.
    /// </summary>
    /// <param name="context">The authorization handler context.</param>
    /// <param name="keycloakClient">The Keycloak client for interacting with Keycloak APIs.</param>
    /// <param name="realm">The realm name for validating the session.</param>
    /// <exception cref="ArgumentNullException">Thrown if the realm is null or empty.</exception>
    /// <exception cref="KcUserNotFoundException">Thrown if the user cannot be found.</exception>
    /// <exception cref="KcSessionClosedException">Thrown if the user session is not active.</exception>
    private async Task ValidateUserSession(AuthorizationHandlerContext context, KeycloakClient keycloakClient,
        string realm)
    {
        // Ensure the realm name is provided
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new ArgumentNullException(nameof(realm), "Realm is required.");
        }

        // Retrieve the admin token for the specified realm
        var adminToken = await _realmAdminTokenHandler.TryGetAdminTokenAsync(realm).ConfigureAwait(false);

        // Extract the user ID from the claims
        var userId = context.User.Claims
            .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcUserNotFoundException("Unable to extract user subject.");
        }

        // Check if the user exists in Keycloak
        var userResponse = await keycloakClient.Users.GetAsync(realm, adminToken, userId).ConfigureAwait(false);
        if ( userResponse.IsError )
        {
            throw new KcUserNotFoundException($"User {userId} not found. Error: {userResponse.ErrorMessage}",
                userResponse.Exception);
        }

        // Extract the session ID from the claims
        var sessionId = context.User.Claims.FirstOrDefault(claim => claim.Type == "sid")?.Value;
        if ( string.IsNullOrWhiteSpace(sessionId) )
        {
            throw new KcSessionClosedException("Unable to extract session ID.");
        }

        // Retrieve active sessions for the user
        var sessionsResponse =
            await keycloakClient.Users.SessionsAsync(realm, adminToken, userId).ConfigureAwait(false);
        if ( sessionsResponse.IsError )
        {
            throw new KcSessionClosedException($"No active session found for user {userId}.",
                userResponse.Exception);
        }

        // Ensure the session ID exists among active sessions
        if ( sessionsResponse.Response.All(session => session.Id != sessionId) )
        {
            throw new KcSessionClosedException($"Session {sessionId} not found for user {userId}.",
                userResponse.Exception);
        }
    }
}
