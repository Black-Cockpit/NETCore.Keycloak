# 🔐 Keycloak Client for .NET Core

🚀 A powerful and feature-rich .NET Core client library for Keycloak that simplifies integration with Keycloak's authentication and authorization services. This enterprise-ready library provides a comprehensive implementation of Keycloak's REST API, with full support for OpenID Connect, OAuth 2.0, and User-Managed Access (UMA 2.0) protocols.

***

## ⚙️ Requirements

| Category     | Supported Versions                                                      |
| ------------ | ----------------------------------------------------------------------- |
| .NET         | 6.0, 7.0, 8.0                                                           |
| Dependencies | ASP.NET Core, Microsoft.Extensions.DependencyInjection, Newtonsoft.Json |

## ✅ Version Compatibility

| Keycloak Version | Support |
| ---------------- | ------- |
| 26.x             | ✅       |
| 25.x             | ✅       |
| 24.x             | ✅       |
| 23.x             | ✅       |
| 22.x             | ✅       |
| 21.x             | ✅       |
| 20.x             | ✅       |

## 🌟 Key Features

- 🔄 Complete Keycloak REST API integration
- 🛡️ Robust security with OpenID Connect and OAuth 2.0
- 📊 Built-in monitoring and performance metrics
- 🔍 Comprehensive error handling and debugging
- 🚦 Automated token management and renewal
- 👥 Advanced user and group management
- 🔑 Multiple authentication flows support
- 📈 Enterprise-grade scalability

## 💻 Installation

To integrate the Keycloak client library into your .NET Core application, simply add the NuGet package:

```bash
Install-Package Keycloak.NETCore.Client
```

## 🚀 Getting Started

### 📋 Prerequisites

- ✳️ .NET Core SDK (version 6.0 or later)
- 🖥️ A running Keycloak instance
- 🔑 Client credentials and realm configuration

### 🔧 Basic Setup

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

## 📖 Basic Usage

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
    new KcUserFilter { Max = 10 });
```
## 📄 License

This project is licensed under the MIT License.
