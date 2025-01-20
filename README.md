# Keycloak Client for .NET Core

A comprehensive .NET Core client library for Keycloak that provides seamless integration with Keycloak's authentication and authorization services. This library offers a robust implementation of Keycloak's REST API, including support for OpenID Connect, OAuth 2.0, and User-Managed Access (UMA 2.0).

## Requirements

| Category     | Supported Versions                                                      |
| ------------ | ----------------------------------------------------------------------- |
| .NET         | 6.0, 7.0, 8.0                                                           |
| Dependencies | ASP.NET Core, Microsoft.Extensions.DependencyInjection, Newtonsoft.Json |

## Coverage

| Keycloak Version | Support |
| ---------------- | ------- |
| 26.x             | ✅       |
| 25.x             | ✅       |
| 24.x             | ✅       |
| 23.x             | ✅       |
| 22.x             | ✅       |
| 21.x             | ✅       |
| 20.x             | ✅       |

## Features

- Full support for Keycloak's REST API
- Comprehensive response types with error handling
- Built-in monitoring and metrics

## Table of Contents

- [Keycloak Client for .NET Core](#keycloak-client-for-net-core)
  - [Requirements](#requirements)
  - [Coverage](#coverage)
  - [Features](#features)
  - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Basic Setup](#basic-setup)
  - [Basic Usage](#basic-usage)
  - [Documentation](#documentation)
  - [Response Types](#response-types)
  - [Monitoring](#monitoring)
  - [Configuration](#configuration)
    - [Client Configuration Options](#client-configuration-options)
    - [Authentication Options](#authentication-options)
  - [Testing](#testing)
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

## Testing

The library includes a comprehensive test suite that validates functionality across multiple Keycloak versions (20.x through 26.x). The test infrastructure uses Docker and Ansible for automated environment setup and test execution.

### Test Documentation

- [Test Suite Documentation](NETCore.Keycloak.Client.Tests/README.md)
  - Test Categories and Patterns
  - Environment Setup
  - Running Tests
  - Mock Data Organization

- [Ansible Environment Documentation](NETCore.Keycloak.Client.Tests/ansible/README.md)
  - Environment Provisioning
  - Keycloak Configuration
  - Database Setup
  - Container Management

### Key Testing Features

1. **Version Coverage**:
   - Supports Keycloak 20.x through 26.x
   - Automated environment setup per version
   - Parallel version testing

2. **Test Categories**:
   - Authentication flows
   - Authorization mechanisms
   - Client operations
   - Group management
   - User operations

3. **Infrastructure**:
   - Docker-based environments
   - Ansible automation
   - Continuous Integration ready
   - Comprehensive mock data

### Running Tests

```bash
# Install test environment dependencies
cd NETCore.Keycloak.Client.Tests
make install_virtual_env

# Run tests for all supported versions
dotnet cake e2e_test.cake

# Run tests for specific version
dotnet cake e2e_test.cake --kc_major_version=XX  # Replace XX with version (20-26)
```

For detailed testing instructions and environment setup, refer to the [Test Suite Documentation](NETCore.Keycloak.Client.Tests/README.md).

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
