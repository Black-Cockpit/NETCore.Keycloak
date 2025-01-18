# Keycloak Client for .NET Core

A robust Keycloak client library implemented in C# for .NET Core. This library provides models, filters, token handling, and utilities to integrate seamlessly with Keycloak's REST API.

## Features

- Full support for Keycloak's REST API
- Typed models for roles, users, groups, permissions, and tokens
- Filters for querying users, groups, and clients
- Comprehensive token management and validation
- Support for custom claims and certificate configuration
- Authorization handlers for protecting resources
- Client credential management and validation
- Session management and validation
- Attack detection capabilities
- Comprehensive response types with error handling
- Built-in monitoring and metrics

## Table of Contents

- [Installation](#installation)
- [Getting Started](#getting-started)
- [Basic Usage](#basic-usage)
- [Documentation](#documentation)
- [Response Types](#response-types)
- [Monitoring](#monitoring)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

## Installation

To use the Keycloak client library in your .NET Core application, add the NuGet package:

```bash
Install-Package NETCore.Keycloak
```

## Getting Started

### Prerequisites

- .NET Core SDK (version 6.0 or later)
- A running Keycloak instance
- Client credentials and realm configuration for Keycloak

### Basic Setup

1. Add the Keycloak client to your services in `Program.cs` or `Startup.cs`:

```csharp
services.AddKeycloakAuthentication(options =>
{
    options.KeycloakBaseUrl = "http://localhost:8080";
    options.RealmAdminCredentials = new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    };
});
```

## Basic Usage

Here's a quick example of how to use the library:

```csharp
// Create Keycloak client
var keycloakClient = new KeycloakClient("http://localhost:8080");

// Authenticate
var token = await keycloakClient.Auth.GetClientCredentialsTokenAsync(
    "your-realm",
    new KcClientCredentials
    {
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    });

// Use the token for other operations
var users = await keycloakClient.Users.GetAsync(
    "your-realm",
    token.AccessToken,
    new KcUserFilter { MaxResults = 10 });
```

## Documentation

Detailed documentation for each module is available in the docs directory:

- [API Authentication](docs/api-authentication.md)
  - JWT Bearer Authentication
  - Role Claims Transformation
  - Configuration Options
  - Security Best Practices

- [Authorization](docs/authorization.md)
  - UMA 2.0 Authorization
  - Protected Resources
  - Policy Enforcement
  - RPT Token Management

- [Response Types](docs/response-types.md)
  - KcResponse<T>
  - KcOperationResponse<T>
  - Error Handling
  - Response Models

- [Monitoring and Metrics](docs/monitoring.md)
  - Performance Monitoring
  - Error Diagnosis
  - Health Monitoring
  - Integration with Monitoring Systems

- [Authentication Management](docs/authentication.md)
  - Token Management
  - Client Credentials Flow
  - Resource Owner Password Flow
  - Token Validation and Security

- [User Management](docs/users.md)
  - User Operations
  - Session Management
  - Role Mapping
  - Group Management

- [Client Management](docs/clients.md)
  - Client Operations
  - Client Configuration
  - Service Accounts
  - Client Scopes

## Response Types

The library provides a consistent response model for all operations through `KcResponse<T>` and `KcOperationResponse<T>`. These types include:

- Strongly-typed response data
- Error information and exception details
- Built-in monitoring metrics
- Support for single and multi-operation responses

For detailed information about response types and error handling, see [Response Types Documentation](docs/response-types.md).

## Monitoring

Built-in monitoring capabilities help you track the performance and health of your Keycloak integration:

- Request execution times
- HTTP method and status code tracking
- Request URI monitoring
- Support for health checks
- Integration with popular monitoring systems

For comprehensive information about monitoring features and integration examples, see [Monitoring Documentation](docs/monitoring.md).

## Configuration

### Client Configuration Options

```csharp
public class KcClientConfiguration
{
    public string KeycloakBaseUrl { get; set; }
    public string Realm { get; set; }
    public KcClientCredentials ClientCredentials { get; set; }
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool RequireHttpsMetadata { get; set; } = true;
}
```

### Authentication Options

```csharp
services.AddKeycloakAuthentication(options =>
{
    options.KeycloakBaseUrl = Configuration["Keycloak:BaseUrl"];
    options.Realm = Configuration["Keycloak:Realm"];
    options.ClientCredentials = new KcClientCredentials
    {
        ClientId = Configuration["Keycloak:ClientId"],
        ClientSecret = Configuration["Keycloak:ClientSecret"]
    };
    options.RequireHttpsMetadata = true;
});
```

## Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to your branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
