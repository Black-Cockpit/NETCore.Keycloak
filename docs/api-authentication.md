# API Authentication

This document describes how to implement API authentication using the NETCore.Keycloak.Client library. The library provides JWT Bearer token authentication with Keycloak, including role claim transformation and flexible configuration options.

## Setup

### Basic Configuration

Add Keycloak authentication to your services with the following configuration:

```csharp
services.AddKeycloakAuthentication(
    authenticationScheme: "Bearer", // Optional, defaults to "Bearer"
    keycloakConfig: options =>
    {
        options.Url = "http://localhost:8080/";          // Keycloak base URL
        options.Issuer = "http://localhost:8080/";       // Keycloak issuer URL (usually same as base URL)
        options.Realm = "your-realm";                    // Your Keycloak realm
        options.ClientId = "your-client";                // Your client ID
        options.ClientSecret = "your-client-secret";     // Your client secret
        options.RolesSource = KcRolesClaimSource.Realm;  // Where to source role claims from
        options.RoleClaimType = "roles";                 // Claim type for roles
    });
```

### JWT Bearer Options

You can customize the JWT Bearer options:

```csharp
services.AddKeycloakAuthentication(
    "Bearer",
    keycloakConfig: options => { /* ... */ },
    configureOptions: bearerOptions =>
    {
        bearerOptions.RequireHttpsMetadata = false;  // For development only
        bearerOptions.SaveToken = true;
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
```

## Role Claims Transformation

The library includes a `KcRolesClaimsTransformer` that transforms Keycloak role claims into a format suitable for your application:

```csharp
public class KcRolesClaimsTransformer : IClaimsTransformation
{
    private readonly string _roleClaimType;
    private readonly KcRolesClaimSource _roleSource;
    private readonly string _audience;

    public KcRolesClaimsTransformer(
        string roleClaimType,
        KcRolesClaimSource roleSource,
        string audience)
    {
        _roleClaimType = roleClaimType;
        _roleSource = roleSource;
        _audience = audience;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Transforms Keycloak role claims based on the configured source
        // Returns a new ClaimsPrincipal with transformed claims
    }
}
```

### Role Claim Sources

You can configure where role claims are sourced from using `KcRolesClaimSource`:

- `Realm`: Use realm-level roles
- `ResourceAccess`: Use client-level roles from resource_access claim
- `Both`: Use both realm and client-level roles

## Configuration Options

The `KcAuthenticationConfiguration` class provides the following options:

```csharp
public class KcAuthenticationConfiguration
{
    // Base URL of your Keycloak server
    public string Url { get; set; }

    // Issuer URL (usually same as base URL)
    public string Issuer { get; set; }

    // Your Keycloak realm name
    public string Realm { get; set; }

    // Client ID for authentication
    public string ClientId { get; set; }

    // Client secret (if required)
    public string ClientSecret { get; set; }

    // Source for role claims
    public KcRolesClaimSource RolesSource { get; set; }

    // Claim type for roles
    public string RoleClaimType { get; set; }

    // JWT validation parameters
    public TokenValidationParameters ValidationParameters { get; set; }
}
```

## Usage Examples

### Protecting API Endpoints

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires valid JWT token
public class OrdersController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "orders-viewer")] // Requires specific role
    public IActionResult GetOrders()
    {
        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "orders-creator")]
    public IActionResult CreateOrder()
    {
        return Ok();
    }
}
```

### Accessing Claims

```csharp
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = User.FindAll(_roleClaimType).Select(c => c.Value);
        
        return Ok(new
        {
            UserId = userId,
            Roles = roles
        });
    }
}
```

## Error Handling

The authentication middleware handles various error scenarios:

1. **Token Validation Errors**:
   - Invalid token format
   - Expired tokens
   - Invalid signature
   - Wrong issuer or audience

2. **Role Claim Errors**:
   - Missing role claims
   - Invalid role claim format
   - Role source not found

Example error handling middleware:

```csharp
app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error?.Error is SecurityTokenException)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    Error = "Invalid token",
                    Message = "The provided authentication token is invalid or expired"
                }));
        }
    });
});
```

## Best Practices

1. **Security**:
   - Always use HTTPS in production
   - Keep client secrets secure
   - Regularly rotate client secrets
   - Validate all security-related configuration

2. **Token Handling**:
   - Validate tokens on every request
   - Implement proper token storage
   - Handle token expiration gracefully
   - Use appropriate token lifetimes

3. **Role Management**:
   - Use descriptive role names
   - Implement role-based access control (RBAC)
   - Keep roles granular but manageable
   - Document role requirements

4. **Error Handling**:
   - Provide clear error messages
   - Log authentication failures
   - Implement proper security headers
   - Monitor failed authentication attempts
