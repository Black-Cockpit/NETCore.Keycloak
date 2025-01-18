# Clients Module

The Clients module provides functionality for managing Keycloak clients, including creation, configuration, and access management.

## Response Types

All client operations return either `KcResponse<T>` or `KcOperationResponse<T>` where T is the specific response type for the operation. For detailed information about response types and error handling, see [Response Types](response-types.md).

For information about monitoring and metrics available in responses, see [Monitoring and Metrics](monitoring.md).

## Client Operations

### Create Client

```csharp
var response = await keycloakClient.Clients.CreateAsync(
    "your-realm",
    "your-access-token",
    new KcClient
    {
        ClientId = "client-id",
        Name = "Client Name",
        Description = "Client Description",
        Enabled = true,
        Protocol = "openid-connect",
        PublicClient = false,
        StandardFlowEnabled = true,
        DirectAccessGrantsEnabled = true
    });

if (!response.IsError)
{
    var client = response.Response;
    // Handle successful client creation
}
```

### Get Client

```csharp
var response = await keycloakClient.Clients.GetAsync(
    "your-realm",
    "your-access-token",
    "client-id");

if (!response.IsError)
{
    var client = response.Response;
    // Access client properties
    var clientId = client.ClientId;
    var name = client.Name;
    var isEnabled = client.Enabled;
}
```

### List Clients

```csharp
var response = await keycloakClient.Clients.ListAsync(
    "your-realm",
    "your-access-token");

if (!response.IsError)
{
    var clients = response.Response;
    foreach (var client in clients)
    {
        // Process each client
        logger.LogInformation($"Client ID: {client.ClientId}, Name: {client.Name}");
    }
}
```

### Update Client

```csharp
var response = await keycloakClient.Clients.UpdateAsync(
    "your-realm",
    "your-access-token",
    "client-id",
    new KcClient
    {
        Name = "Updated Client Name",
        Description = "Updated Description",
        Enabled = true
    });

if (!response.IsError)
{
    // Client updated successfully
    logger.LogInformation("Client updated successfully");
}
```

### Delete Client

```csharp
var response = await keycloakClient.Clients.DeleteAsync(
    "your-realm",
    "your-access-token",
    "client-id");

if (!response.IsError)
{
    // Client deleted successfully
    logger.LogInformation("Client deleted successfully");
}
```

### Get Client Secret

```csharp
var response = await keycloakClient.Clients.GetSecretAsync(
    "your-realm",
    "your-access-token",
    "client-id");

if (!response.IsError)
{
    var secret = response.Response;
    var clientSecret = secret.Value;
}
```

### Get Client Sessions

```csharp
var response = await keycloakClient.Clients.GetUsersSessionsAsync(
    "your-realm",
    "your-access-token",
    "client-id");

if (!response.IsError)
{
    var sessions = response.Response;
    foreach (var session in sessions)
    {
        logger.LogInformation($"Session ID: {session.Id}, User ID: {session.UserId}");
    }
}
```

### Get Offline Sessions

```csharp
var response = await keycloakClient.Clients.GetOfflineSessionsAsync(
    "your-realm",
    "your-access-token",
    "client-id");

if (!response.IsError)
{
    var offlineSessions = response.Response;
    foreach (var session in offlineSessions)
    {
        logger.LogInformation($"Offline Session ID: {session.Id}");
    }
}
```

## Client Scopes

### Get Optional Scopes

```csharp
var response = await keycloakClient.Clients.GetOptionalScopesAsync(
    "your-realm",
    "your-access-token",
    "client-id");

if (!response.IsError)
{
    var scopes = response.Response;
    foreach (var scope in scopes)
    {
        logger.LogInformation($"Scope: {scope.Name}");
    }
}
```

### Add Optional Scope

```csharp
var response = await keycloakClient.Clients.AddOptionalScopeAsync(
    "your-realm",
    "your-access-token",
    "client-id",
    "scope-id");

if (!response.IsError)
{
    // Scope added successfully
    logger.LogInformation("Optional scope added successfully");
}
```

## Error Handling

All operations return a `KcResponse<T>` that includes error information. Here's how to handle errors:

```csharp
try
{
    var response = await keycloakClient.Clients.CreateAsync(
        realm,
        accessToken,
        clientConfig);

    if (response.IsError)
    {
        // Handle error using response.Exception
        logger.LogError($"Client operation failed: {response.Exception.Message}");
        
        // Handle specific error cases
        if (response.Exception is KcClientException)
        {
            // Handle client-specific errors
        }
    }
    else
    {
        // Process successful response
        var result = response.Response;
    }
}
catch (Exception ex)
{
    // Handle unexpected errors
    logger.LogError($"Unexpected error during client operation: {ex.Message}");
}
```

## Models

### KcClient

The main model for client operations:

```csharp
public class KcClient
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public string Protocol { get; set; }
    public bool PublicClient { get; set; }
    public bool StandardFlowEnabled { get; set; }
    public bool ImplicitFlowEnabled { get; set; }
    public bool DirectAccessGrantsEnabled { get; set; }
    public bool ServiceAccountsEnabled { get; set; }
    public List<string> RedirectUris { get; set; }
    public List<string> WebOrigins { get; set; }
    public int NotBefore { get; set; }
    public bool BearerOnly { get; set; }
    public bool ConsentRequired { get; set; }
    public bool StandardFlowEnabled { get; set; }
    public bool ImplicitFlowEnabled { get; set; }
    public bool DirectAccessGrantsEnabled { get; set; }
    public bool ServiceAccountsEnabled { get; set; }
    public bool PublicClient { get; set; }
    public bool FrontchannelLogout { get; set; }
    public string Protocol { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
    public bool FullScopeAllowed { get; set; }
    public int NodeReRegistrationTimeout { get; set; }
    public List<KcProtocolMapper> ProtocolMappers { get; set; }
    public bool SurrogateAuthRequired { get; set; }
    public string ClientAuthenticatorType { get; set; }
}
