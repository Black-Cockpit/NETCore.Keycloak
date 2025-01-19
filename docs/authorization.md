# Authorization Module

The Authorization module provides functionality for implementing and managing authorization in your application using Keycloak's User-Managed Access (UMA 2.0) protocol and Requesting Party Token (RPT) for fine-grained authorization.

## Response Types

All authorization operations return either `KcResponse<T>` or `KcOperationResponse<T>` where T is the specific response type for the operation. For detailed information about response types and error handling, see [Response Types](response-types.md).

For information about monitoring and metrics available in responses, see [Monitoring and Metrics](monitoring.md).

## Authorization Architecture

The module implements UMA 2.0 authorization using the following components:

1. **Protected Resource Store**: Maintains the registry of protected resources and their scopes
2. **Policy Provider**: Builds authorization policies based on resource and scope requirements
3. **Bearer Authorization Handler**: Validates tokens and enforces access policies
4. **Realm Admin Token Handler**: Manages admin tokens for privileged operations

## Setup

### Basic Configuration

To enable Keycloak authorization in your application, you need to add two components:

1. The basic authorization handler:
```csharp
services.AddKeycloakAuthorization();
```

2. The protected resource protection with your custom stores:
```csharp
services.AddKeycloakProtectedResourcesPolicies<YourResourceStore, YourRealmConfigStore>();
```

This setup will:
- Register the `KcBearerAuthorizationHandler` as a singleton
- Add the HTTP context accessor for token extraction
- Register your custom protected resource store
- Register your custom realm admin configuration store
- Set up the realm admin token handler
- Configure the protected resource policy provider

## Authorization Flow

The authorization process follows these steps:

1. **Token Extraction**:
   - Extracts the Bearer token from the Authorization header
   - Validates the token format and scheme

2. **Realm Extraction**:
   - Extracts the base URL and realm from the token's issuer claim
   - Validates that the realm information is present

3. **Resource Validation**:
   - Fetches protected resources for the current realm
   - Verifies the requested resource exists in that realm

4. **Session Validation**:
   - Validates the user's session is active
   - Uses realm admin token for validation

5. **RPT Token Request**:
   - Requests a Requesting Party Token (RPT) for the resource
   - Includes the required scope in the request
   - Validates the RPT response

### Authorization Handler

The `KcBearerAuthorizationHandler` implements this flow:

```csharp
public class KcBearerAuthorizationHandler : AuthorizationHandler<KcAuthorizationRequirement>
{
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IKcRealmAdminTokenHandler _realmAdminTokenHandler;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        KcAuthorizationRequirement requirement)
    {
        // Validate context and requirement
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        // Extract and validate Bearer token
        var token = ExtractBearerToken();
        if (token == null)
        {
            context.Fail();
            return;
        }

        // Extract realm information
        var (baseUrl, realm) = TryExtractRealm(token);
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(realm))
        {
            context.Fail();
            return;
        }

        // Find protected resource in the current realm
        var protectedResource = requirement.ProtectedResourceStore
            .GetRealmProtectedResources()
            .FirstOrDefault(r => r.Realm == realm)
            ?.ProtectedResourceName;

        if (protectedResource == null)
        {
            context.Fail();
            return;
        }

        // Initialize Keycloak client
        var keycloakClient = new KeycloakClient(baseUrl, _logger);

        // Validate user session
        await ValidateUserSession(context, keycloakClient, realm);

        // Request RPT token with required scope
        var rptResponse = await keycloakClient.Auth.GetRequestPartyTokenAsync(
            realm,
            token,
            protectedResource,
            [requirement.ToString()]);

        if (rptResponse.IsError)
        {
            _logger.LogError(
                "Access to {Resource} resource {Requirement} permission is denied",
                protectedResource,
                requirement);
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}

## Store Implementations

### Protected Resource Store

Create a custom store that inherits from `KcProtectedResourceStore` to define which resources are protected in each realm:

```csharp
public class YourResourceStore : KcProtectedResourceStore
{
    public override ICollection<KcRealmProtectedResources> GetRealmProtectedResources()
    {
        return new List<KcRealmProtectedResources>
        {
            new()
            {
                Realm = "your-realm",
                ProtectedResourceName = "orders-api"
            },
            new()
            {
                Realm = "your-realm",
                ProtectedResourceName = "user-management-service"
            }
        };
    }
}
```

The `KcRealmProtectedResources` class maps resources to realms:
- `Realm`: The Keycloak realm where the resource is registered
- `ProtectedResourceName`: The unique name of the protected resource

### Realm Admin Configuration Store

Create a custom store that inherits from `KcRealmAdminConfigurationStore` to provide admin credentials for each realm:

```csharp
public class YourRealmConfigStore : KcRealmAdminConfigurationStore
{
    public override ICollection<KcRealmAdminConfiguration> GetRealmsAdminConfiguration()
    {
        return new List<KcRealmAdminConfiguration>
        {
            new()
            {
                Realm = "your-realm",
                ClientId = "admin-client",
                ClientSecret = "admin-secret"
            }
        };
    }
}
```

The `KcRealmAdminConfiguration` class provides admin access details:
- `Realm`: The Keycloak realm name
- `ClientId`: The admin client ID with realm management permissions
- `ClientSecret`: The admin client secret

These credentials are used by the `KcRealmAdminTokenHandler` to:
1. Obtain admin tokens for realm operations
2. Validate user sessions
3. Manage protected resources

## Usage

### Protecting Resources with Policies

Use the policy-based authorization with the format `resource#scope` where the resource name must match a protected resource name registered in your `KcProtectedResourceStore`:

```csharp
[Authorize(Policy = "orders-api#view")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetOrders()
    {
        // Only accessible if user has 'view' scope on 'orders-api' resource
        return Ok();
    }

    [Authorize(Policy = "orders-api#create")]
    [HttpPost]
    public IActionResult CreateOrder()
    {
        // Only accessible if user has 'create' scope on 'orders-api' resource
        return Ok();
    }
}
```

The policy provider (`KcProtectedResourcePolicyProvider`) will:
1. Parse the policy name into resource and scope components
2. Verify the resource exists in the protected resource store for the current realm
3. Create an authorization requirement that validates the RPT token has the required scope for the resource

### Policy Provider Implementation

The `KcProtectedResourcePolicyProvider` maintains a cache of authorization policies and validates that requested resources are properly registered:

```csharp
public class KcProtectedResourcePolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly KcProtectedResourceStore _protectedResourceStore;
    private readonly IDictionary<string, AuthorizationPolicy> _cachedPolices;

    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        // Check if policy is cached
        if (_cachedPolices.TryGetValue(policyName, out var cachedPolicy))
        {
            return cachedPolicy;
        }

        // Parse policy name (format: "resource#scope")
        var parts = policyName.Split('#');
        if (parts.Length != 2)
        {
            return await base.GetPolicyAsync(policyName);
        }

        var resource = parts[0];
        var scope = parts[1];

        // Create and cache new policy
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new KcAuthorizationRequirement(
                _protectedResourceStore,
                resource,
                scope))
            .Build();

        _cachedPolices[policyName] = policy;
        return policy;
    }
}
```

### Authorization Requirement

The `KcAuthorizationRequirement` class defines what resource and scope are required:

```csharp
public class KcAuthorizationRequirement : IAuthorizationRequirement
{
    public KcAuthorizationRequirement(
        KcProtectedResourceStore protectedResourceStore,
        string resource,
        string scope)
    {
        ProtectedResourceStore = protectedResourceStore 
            ?? throw new ArgumentNullException(nameof(protectedResourceStore));
            
        if (string.IsNullOrWhiteSpace(resource))
            throw new NoNullAllowedException(nameof(resource));
            
        if (string.IsNullOrWhiteSpace(scope))
            throw new NoNullAllowedException(nameof(scope));
            
        Resource = resource;
        Scope = scope;
    }

    // The store containing protected resources
    public KcProtectedResourceStore ProtectedResourceStore { get; }

    // The resource being protected
    public string Resource { get; }

    // The required scope
    public string Scope { get; }
}
```

### Programmatic Authorization

For dynamic authorization checks:

```csharp
public class ResourceService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly KcProtectedResourceStore _resourceStore;

    public ResourceService(
        IAuthorizationService authorizationService,
        KcProtectedResourceStore resourceStore)
    {
        _authorizationService = authorizationService;
        _resourceStore = resourceStore;
    }

    public async Task AccessResourceAsync(ClaimsPrincipal user, string resource, string scope)
    {
        var requirement = new KcAuthorizationRequirement(
            _resourceStore,
            resource,
            scope);

        var result = await _authorizationService.AuthorizeAsync(
            user,
            null,
            requirement);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException($"Access denied to {resource}#{scope}");
        }
    }
}
```

## UMA 2.0 and RPT

The module implements the User-Managed Access (UMA 2.0) protocol for authorization. UMA 2.0 is an OAuth-based protocol that enables fine-grained authorization using Requesting Party Tokens (RPT).

### Authorization Process

1. **Resource Registration**:
   - Resources are registered in Keycloak and mapped to realms
   - Each resource is identified by a unique name within its realm
   - Resources are defined in your `KcProtectedResourceStore` implementation

2. **Initial Access**:
   - Client makes a request with their Bearer token
   - The authorization handler extracts the realm from the token
   - The handler validates the token and user session

3. **Permission Request**:
   - The handler identifies the protected resource for the current realm
   - It requests an RPT token with the required scope
   - The RPT request includes:
     - The original Bearer token
     - The protected resource name
     - The required scope (e.g., "view", "create")

4. **RPT Validation**:
   - Keycloak validates the permission request
   - If approved, it issues an RPT token
   - The RPT token contains the granted permissions

### RPT Token Flow Example

```csharp
// 1. Client makes request with Bearer token
[Authorize(Policy = "orders-api#view")]
public IActionResult GetOrders()
{
    // The KcBearerAuthorizationHandler will:
    // - Extract the Bearer token
    // - Identify the realm and resource
    // - Request an RPT token
    return Ok();
}

// 2. Handler requests RPT token
var rptResponse = await keycloakClient.Auth.GetRequestPartyTokenAsync(
    realm: "your-realm",
    accessToken: bearerToken,
    resource: "orders-api",
    scopes: ["view"]);

// 3. Handle the RPT response
if (rptResponse.IsError)
{
    // Access denied - missing required permissions
    _logger.LogError(
        "Access to {Resource} resource {Scope} permission is denied",
        "orders-api",
        "view");
    return;
}

// 4. Access granted - RPT token contains required permissions
context.Succeed(requirement);
```

### Error Handling

The RPT request can fail for several reasons:

1. **Invalid Resource**:
   - Resource not found in the realm
   - Resource not properly registered in Keycloak

2. **Permission Denied**:
   - User lacks required permissions
   - Scope not granted for the resource

3. **Token Issues**:
   - Invalid or expired Bearer token
   - Invalid user session

4. **Configuration Issues**:
   - Missing realm admin configuration
   - Invalid admin credentials

Example error handling:

```csharp
try
{
    var rptResponse = await keycloakClient.Auth.GetRequestPartyTokenAsync(
        realm,
        bearerToken,
        protectedResource,
        [scope]);

    if (rptResponse.IsError)
    {
        _logger.LogError(
            "RPT request failed for resource {Resource} with scope {Scope}",
            protectedResource,
            scope);
        return;
    }
}
catch (KcException ex)
{
    _logger.LogError(
        ex,
        "Failed to obtain RPT token for resource {Resource}",
        protectedResource);
    throw;
}
```

## Models

### KcProtectedResource

Represents a resource protected by Keycloak:

```csharp
public class KcProtectedResource
{
    // Resource name/identifier
    public string Name { get; set; }

    // Available scopes for this resource
    public IEnumerable<string> Scopes { get; set; }
}
```

### KcRealmAdminConfiguration

Configuration for realm administration:

```csharp
public class KcRealmAdminConfiguration
{
    // Realm name
    public string Realm { get; set; }

    // Admin client ID
    public string ClientId { get; set; }

    // Admin client secret
    public string ClientSecret { get; set; }
}
```

## Error Handling

The authorization handler handles various error scenarios:

1. **Token Validation**:
   - Invalid tokens
   - Expired tokens
   - Missing required claims

2. **Permission Checks**:
   - Missing resources
   - Insufficient scopes
   - Invalid permission tickets

3. **Admin Operations**:
   - Invalid admin credentials
   - Failed token retrieval
   - Configuration errors

Example error handling:

```csharp
try
{
    var result = await _authorizationService.AuthorizeAsync(
        user,
        null,
        new KcAuthorizationRequirement(_resourceStore, "orders", "view"));

    if (!result.Succeeded)
    {
        _logger.LogWarning(
            "Access denied for user {User} to resource {Resource}",
            user.Identity?.Name,
            "orders#view");
            
        throw new UnauthorizedAccessException();
    }
}
catch (KcException ex)
{
    _logger.LogError(
        ex,
        "Authorization failed for user {User}",
        user.Identity?.Name);
        
    throw;
}
```
### Best Practices

1. **Resource Naming**:
   - Use consistent naming patterns for resources
   - Include the resource type in the name (e.g., "orders-api", "users-service")
   - Keep names unique within each realm

2. **Scope Definition**:
   - Use standard scope names (e.g., "view", "create", "update", "delete")
   - Define granular scopes for fine-grained control
   - Document scope requirements for each resource