# 🔐 Keycloak Client for .NET Core

<div align="center">
  <img src="assets/kc_logo.svg" alt="Keycloak .NET Core Client Logo" width="200">
</div>

🚀 A powerful and feature-rich .NET Core client library for Keycloak that simplifies integration with Keycloak's authentication and authorization services. This enterprise-ready library provides a comprehensive implementation of Keycloak's REST API, with full support for OpenID Connect, OAuth 2.0, and User-Managed Access (UMA 2.0) protocols.

***
---

<div align="center">

[![GitHub Build Status](https://github.com/Black-Cockpit/NETCore.Keycloak/actions/workflows/build_test_analyze.yml/badge.svg)](https://github.com/Black-Cockpit/NETCore.Keycloak/actions/workflows/build.yml)
[![NuGet version](https://img.shields.io/nuget/v/Keycloak.NETCore.Client.svg)](https://www.nuget.org/packages/Keycloak.NETCore.Client/)
[![NuGet downloads](https://img.shields.io/nuget/dt/Keycloak.NETCore.Client.svg)](https://www.nuget.org/packages/Keycloak.NETCore.Client/)
[![GitHub Stars](https://img.shields.io/github/stars/Black-Cockpit/NETCore.Keycloak)](https://github.com/Black-Cockpit/NETCore.Keycloak/stargazers)
[![CodeFactor](https://www.codefactor.io/repository/github/black-cockpit/netcore.keycloak/badge)](https://www.codefactor.io/repository/github/black-cockpit/netcore.keycloak)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FBlack-Cockpit%2FNETCore.Keycloak.svg?type=shield&issueType=security)](https://app.fossa.com/projects/git%2Bgithub.com%2FBlack-Cockpit%2FNETCore.Keycloak?ref=badge_shield&issueType=security)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Black-Cockpit_NETCore.Keycloak&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=Black-Cockpit_NETCore.Keycloak)
[![License](https://img.shields.io/github/license/Black-Cockpit/NETCore.Keycloak)](LICENSE)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FBlack-Cockpit%2FNETCore.Keycloak.svg?type=shield&issueType=license)](https://app.fossa.com/projects/git%2Bgithub.com%2FBlack-Cockpit%2FNETCore.Keycloak?ref=badge_shield&issueType=license)

---

</div>

## ⚙️ Requirements

| Category     | Supported Versions                                                      |
| ------------ | ----------------------------------------------------------------------- |
| .NET         | 8.0, 9.0, 10.0                                                          |
| Dependencies | ASP.NET Core, Microsoft.Extensions.DependencyInjection, Newtonsoft.Json |

## ✅ Version Compatibility

| Keycloak Version | Support |
| ---------------- | ------- |
| 26.x             | ✅       |
| 25.x             | ✅       |
| 24.x             | ✅       |

> **Note:** Keycloak versions 20.x through 23.x and .NET 6.0/7.0 were supported in previous releases (v1.x). If you need support for these older versions, please use [v1.0.2](https://www.nuget.org/packages/Keycloak.NETCore.Client/1.0.2).

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
Install-Package Keycloak.NETCore.Client
```

## 🚀 Getting Started

### 📋 Prerequisites

- ✳️ .NET SDK (version 8.0 or later)
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

## 👥 Contributors

Thanks to all the people who contribute to this project!

<a href="https://github.com/Black-Cockpit/NETCore.Keycloak/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Black-Cockpit/NETCore.Keycloak" />
</a>

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

We welcome contributions from the community! Please check our [Contributing Guidelines](CONTRIBUTING.md) for details on:

- Branch naming conventions
- Code style and formatting rules
- Pull request process
- Security guidelines

⭐ Star us on GitHub | 📫 Report Issues | 📚 Read the Docs
