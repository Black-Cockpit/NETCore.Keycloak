# Authentication Module

The Authentication module provides functionality for handling authentication flows with Keycloak, including client credentials flow, resource owner password flow, and token management.

## Response Types

All authentication operations return either `KcResponse<T>` or `KcOperationResponse<T>` where T is the specific response type for the operation. For detailed information about response types and error handling, see [Response Types](response-types.md).

For information about monitoring and metrics available in responses, see [Monitoring and Metrics](monitoring.md).

## Token Types

The module supports various token types for different authentication scenarios:

- **Access Tokens**: Short-lived tokens for accessing protected resources
- **Refresh Tokens**: Long-lived tokens for obtaining new access tokens
- **ID Tokens**: Contains user identity information (OpenID Connect)
- **Request Party Tokens (RPT)**: Tokens with additional permissions/entitlements

## Authentication Operations

### Client Credentials Flow

```csharp
var response = await keycloakClient.Auth.GetClientCredentialsTokenAsync(
    "your-realm",
    new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    });

if (!response.IsError)
{
    var token = response.Response;
    // Use token.AccessToken, token.RefreshToken, etc.
}
```

### Resource Owner Password Flow

```csharp
var response = await keycloakClient.Auth.GetResourceOwnerPasswordTokenAsync(
    "your-realm",
    new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    },
    "username",
    "password");

if (!response.IsError)
{
    var token = response.Response;
    // Use token.AccessToken, token.RefreshToken, etc.
}
```

### Password Validation

```csharp
var response = await keycloakClient.Auth.ValidatePasswordAsync(
    "your-realm",
    new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    },
    "username",
    "password");

if (!response.IsError)
{
    var isValid = response.Response;
    // Handle password validation result
}
```

### Token Refresh

```csharp
var response = await keycloakClient.Auth.RefreshAccessTokenAsync(
    "your-realm",
    "refresh-token",
    "client-id");

if (!response.IsError)
{
    var newToken = response.Response;
    // Use newToken.AccessToken, newToken.RefreshToken, etc.
}
```

### Token Revocation

```csharp
// Revoke access token
var revokeAccessResponse = await keycloakClient.Auth.RevokeAccessTokenAsync(
    "your-realm",
    new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    },
    "access-token");

// Revoke refresh token
var revokeRefreshResponse = await keycloakClient.Auth.RevokeRefreshTokenAsync(
    "your-realm",
    new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    },
    "refresh-token");
```

### Request Party Token (RPT)

Obtain a token with additional permissions:

```csharp
var response = await keycloakClient.Auth.GetRequestPartyTokenAsync(
    "your-realm",
    new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    },
    "audience");

if (!response.IsError)
{
    var rptToken = response.Response;
    // Use the RPT token
}
```

## Models

### KcIdentityProviderToken

The main token model returned by authentication operations:

```csharp
public class KcIdentityProviderToken
{
    // Access token for API requests
    public string AccessToken { get; set; }

    // Token expiration time in seconds
    public long? ExpiresIn { get; set; }

    // Refresh token for obtaining new access tokens
    public string RefreshToken { get; set; }

    // Token type (usually "Bearer")
    public string TokenType { get; set; }

    // OpenID Connect ID token
    public string IdToken { get; set; }

    // Keycloak session state
    public string SessionState { get; set; }

    // Token scope
    public string Scope { get; set; }
}
```

### KcAccessToken Claims

Common claims available in the access token:

```csharp
public class KcAccessToken
{
    // User ID
    public string Subject { get; set; }

    // Username
    public string PreferredUsername { get; set; }

    // User email
    public string Email { get; set; }

    // Email verification status
    public bool? EmailVerified { get; set; }

    // User roles
    public string[] RealmRoles { get; set; }

    // Resource roles
    public Dictionary<string, string[]> ResourceRoles { get; set; }

    // Token scope
    public string Scope { get; set; }
}
```

## Token Security Best Practices

1. **Token Storage**:
   - Never store tokens in local storage
   - Use secure HTTP-only cookies for web applications
   - Store tokens in memory for native applications

2. **Token Handling**:
   - Always validate tokens before using them
   - Implement token refresh logic before expiration
   - Revoke tokens when they are no longer needed

3. **Token Validation**:
   - Verify token signature using Keycloak's public key
   - Validate issuer (iss) and audience (aud) claims
   - Check token expiration (exp) and not-before (nbf) claims

## Error Handling

All operations return a `KcResponse<T>` or `KcOperationResponse<T>` that includes error information and monitoring metrics. Here's how to handle errors:

```csharp
try
{
    var response = await keycloakClient.Auth.GetClientCredentialsTokenAsync(
        realm,
        credentials);

    if (response.IsError)
    {
        // Handle error using response.Exception
        logger.LogError($"Authentication failed: {response.Exception.Message}");
        
        // Handle specific error cases
        if (response.Exception is KcAuthenticationException)
        {
            // Handle authentication-specific errors
        }
    }
    else
    {
        // Process successful response
        var token = response.Response;
    }
}
catch (Exception ex)
{
    // Handle unexpected errors
    logger.LogError($"Unexpected error during authentication: {ex.Message}");
}
