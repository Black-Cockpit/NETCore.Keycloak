using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NETCore.Keycloak.Authentication.Claims;
using NETCore.Keycloak.Constants;

namespace NETCore.Keycloak.Authentication;

/// <summary>
/// Keycloak authentication extension
/// </summary>
public static class KcAuthenticationExtension
{
    /// <summary>
    /// Adds keycloak authentication services.
    /// </summary>
    public static AuthenticationBuilder AddKeycloakAuthentication(
        this IServiceCollection services,
        string authenticationScheme,
        Action<KcAuthenticationConfiguration> keycloakConfig,
        Action<JwtBearerOptions> configureOptions = null)
    {
        // Set default scheme if given authentication scheme is null or empty
        authenticationScheme = !string.IsNullOrWhiteSpace(authenticationScheme)
            ? authenticationScheme
            : KcDefaults.AuthenticationScheme;

        var kcOptions = new KcAuthenticationConfiguration();

        keycloakConfig?.Invoke(kcOptions);

        if ( kcOptions == null )
        {
            throw new KcException($"{nameof(kcOptions)} is required");
        }

        var validationParameters = new TokenValidationParameters
        {
            ClockSkew = kcOptions.TokenClockSkew,
            ValidateAudience = true,
            ValidAudiences = kcOptions.ValidAudiences,
            ValidateIssuer = true,
            NameClaimType = kcOptions.NameClaimType,
            RoleClaimType = kcOptions.RoleClaimType,
            ValidIssuer = kcOptions.ValidIssuer,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };

        _ = services.AddTransient<IClaimsTransformation>(_ =>
            new KcRolesClaimsTransformer(
                kcOptions.RoleClaimType,
                kcOptions.RolesSource, kcOptions.Resource));

        return services.AddAuthentication(authenticationScheme)
            .AddJwtBearer(authenticationScheme, opts =>
            {
                opts.Authority = kcOptions.Authority;
                opts.Audience = kcOptions.Resource;
                opts.TokenValidationParameters = validationParameters;
                opts.RequireHttpsMetadata = kcOptions.RequireSsl;
                opts.SaveToken = false;
                configureOptions?.Invoke(opts);
            });
    }
}
