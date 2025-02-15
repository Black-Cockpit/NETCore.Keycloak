# 🔐 Keycloak Client for .NET Core

<div align="center">
  <img src="assets/kc_logo.svg" alt="Keycloak .NET Core Client Logo" width="200">
</div>

🚀 A powerful and feature-rich .NET Core client library for Keycloak that simplifies integration with Keycloak's authentication and authorization services. This enterprise-ready library provides a comprehensive implementation of Keycloak's REST API, with full support for OpenID Connect, OAuth 2.0, and User-Managed Access (UMA 2.0) protocols.

***
---

<div align="center">

[![NuGet version](https://img.shields.io/nuget/v/NETCore.Keycloak.Client.svg)](https://www.nuget.org/packages/NETCore.Keycloak.Client/)
[![NuGet downloads](https://img.shields.io/nuget/dt/NETCore.Keycloak.Client.svg)](https://www.nuget.org/packages/NETCore.Keycloak.Client/)
[![GitHub Build Status](https://github.com/Black-Cockpit/NETCore.Keycloak/actions/workflows/build.yml/badge.svg)](https://github.com/Black-Cockpit/NETCore.Keycloak/actions/workflows/build.yml)
[![GitHub Stars](https://img.shields.io/github/stars/Black-Cockpit/NETCore.Keycloak.svg)](https://github.com/Black-Cockpit/NETCore.Keycloak/stargazers)
[![License](https://img.shields.io/github/license/Black-Cockpit/NETCore.Keycloak.svg)](LICENSE)

---

</div>

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

## 📚 Table of Contents

- [🔐 Keycloak Client for .NET Core](#-keycloak-client-for-net-core)
  - [⚙️ Requirements](#️-requirements)
  - [✅ Version Compatibility](#-version-compatibility)
  - [🌟 Key Features](#-key-features)
  - [📚 Table of Contents](#-table-of-contents)
  - [💻 Installation](#-installation)
  - [🚀 Getting Started](#-getting-started)
    - [📋 Prerequisites](#-prerequisites)
    - [🔧 Basic Setup](#-basic-setup)
  - [📖 Basic Usage](#-basic-usage)
  - [📚 Documentation](#-documentation)
  - [🧪 Testing](#-testing)
    - [📋 Test Documentation](#-test-documentation)
    - [🔬 Key Testing Features](#-key-testing-features)
    - [⚡ Running Tests](#-running-tests)
  - [🤝 Contributing](#-contributing)
  - [📄 License](#-license)

## 💻 Installation

To integrate the Keycloak client library into your .NET Core application, simply add the NuGet package:

```bash
Install-Package NETCore.Keycloak
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
    new KcUserFilter { MaxResults = 10 });
```

## 📚 Documentation

Explore our comprehensive documentation for each module:

- 🔐 [API Authentication](docs/api-authentication.md)
  - JWT Bearer Authentication
  - Role Claims Transformation
  - Security Best Practices

- 🛡️ [Authorization](docs/authorization.md)
  - UMA 2.0 Authorization
  - Policy Enforcement
  - Protected Resources

- 📊 [Response Types](docs/response-types.md)
  - Type-safe responses
  - Error handling
  - Response Models

- 📈 [Monitoring and Metrics](docs/monitoring.md)
  - Performance tracking
  - Health checks
  - System diagnostics

- 🔑 [Authentication Management](docs/authentication.md)
  - Token lifecycle
  - Multiple auth flows
  - Security features

- 👥 [User Management](docs/users.md)
  - User operations
  - Role management
  - Group handling

- ⚙️ [Client Management](docs/clients.md)
  - Configuration
  - Service accounts
  - Client scopes

## 🧪 Testing

Our library includes an extensive test suite ensuring reliability across multiple Keycloak versions (20.x through 26.x). The testing infrastructure leverages Docker and Ansible for automated setup and execution.

### 📋 Test Documentation

- 📘 [Test Suite Guide](NETCore.Keycloak.Client.Tests/README.md)
  - Test patterns
  - Setup instructions
  - Mock data structure

- 🔧 [Ansible Setup Guide](NETCore.Keycloak.Client.Tests/ansible/README.md)
  - Environment setup
  - Configuration management
  - Container orchestration

### 🔬 Key Testing Features

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

### ⚡ Running Tests

```bash
# Install test environment dependencies
cd NETCore.Keycloak.Client.Tests
make install_virtual_env

# Run tests for all supported versions
dotnet cake e2e_test.cake
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details on how to submit pull requests, report issues, and contribute to the project.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
⭐ Star us on GitHub | 📫 Report Issues | 📚 Read the Docs
