using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NETCore.Keycloak.Client.Authentication.Claims;
using NETCore.Keycloak.Client.Constants;

namespace NETCore.Keycloak.Client.Authentication;

/// <summary>
/// Provides extension methods to configure Keycloak-based authentication in an .NET Core application.
/// </summary>
public static class KcAuthenticationExtension
{
    /// <summary>
    /// Adds Keycloak authentication services to the specified <see cref="IServiceCollection"/>.
    /// This method integrates JWT Bearer authentication and configures Keycloak-specific
    /// options such as token validation and claims transformation.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which authentication services are added.
    /// </param>
    /// <param name="authenticationScheme">
    /// The name of the authentication scheme. If null or empty, a default scheme defined in
    /// <see cref="KcDefaults.AuthenticationScheme"/> will be used.
    /// </param>
    /// <param name="keycloakConfig">
    /// An <see cref="Action{T}"/> to configure the <see cref="KcAuthenticationConfiguration"/> instance.
    /// This must set required properties such as <see cref="KcAuthenticationConfiguration.Url"/>,
    /// <see cref="KcAuthenticationConfiguration.Realm"/>, and <see cref="KcAuthenticationConfiguration.Issuer"/>.
    /// </param>
    /// <param name="configureOptions">
    /// An optional <see cref="Action{T}"/> to further customize <see cref="JwtBearerOptions"/> for the authentication scheme.
    /// </param>
    /// <returns>
    /// An <see cref="AuthenticationBuilder"/> that can be used to further configure authentication.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="keycloakConfig"/> is null or does not set required properties.
    /// </exception>
    public static AuthenticationBuilder AddKeycloakAuthentication(
        this IServiceCollection services,
        string authenticationScheme,
        Action<KcAuthenticationConfiguration> keycloakConfig,
        Action<JwtBearerOptions> configureOptions = null)
    {
        // Set the default authentication scheme if none is provided.
        authenticationScheme = !string.IsNullOrWhiteSpace(authenticationScheme)
            ? authenticationScheme
            : KcDefaults.AuthenticationScheme;

        // Initialize Keycloak configuration and apply the provided configuration delegate.
        var kcOptions = new KcAuthenticationConfiguration();
        keycloakConfig?.Invoke(kcOptions);

        // Validate the Keycloak configuration to ensure required properties are set.
        kcOptions.Validate();

        // Define token validation parameters based on the Keycloak configuration.
        var validationParameters = new TokenValidationParameters
        {
            ClockSkew = kcOptions.TokenClockSkew, // Allowable clock skew for token validation.
            ValidateAudience = true, // Validate the audience claim.
            ValidAudiences = kcOptions.ValidAudiences, // Specify valid audience values.
            ValidateIssuer = true, // Validate the issuer claim.
            NameClaimType = kcOptions.NameClaimType, // Map the name claim type.
            RoleClaimType = kcOptions.RoleClaimType, // Map the role claim type.
            ValidIssuer = kcOptions.ValidIssuer, // Specify the expected issuer.
            RequireExpirationTime = true, // Ensure tokens have an expiration time.
            ValidateLifetime = true // Validate token expiration.
        };

        // Register a custom claims transformation service for Keycloak role claims.
        _ = services.AddTransient<IClaimsTransformation>(_ =>
            new KcRolesClaimsTransformer(
                kcOptions.RoleClaimType,
                kcOptions.RolesSource,
                kcOptions.Resource));

        // Add authentication and configure the JWT Bearer scheme.
        return services.AddAuthentication(authenticationScheme)
            .AddJwtBearer(authenticationScheme, opts =>
            {
                // Configure JWT Bearer options based on the Keycloak configuration.
                opts.Authority = kcOptions.Authority; // Set the authority URL.
                opts.Audience = kcOptions.Resource; // Set the expected audience.
                opts.TokenValidationParameters = validationParameters; // Apply token validation parameters.
                opts.RequireHttpsMetadata = kcOptions.RequireSsl; // Enforce HTTPS if required.
                opts.SaveToken = false; // Do not save the token by default.
                configureOptions?.Invoke(opts); // Apply additional custom options, if provided.
            });
    }
}
