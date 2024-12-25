# Keycloak Client for .NET Core

A robust Keycloak client library implemented in C# for .NET Core. This library provides models, filters, token handling, and utilities to integrate seamlessly with Keycloak's REST API.

## Features

- Full support for Keycloak's REST API.
- Typed models for roles, users, groups, permissions, tokens, and more.
- Filters for querying users, groups, and clients.
- JWT token handling and validation.
- Support for custom claims, token parsing, and certificate configuration.

## Table of Contents

- [Installation](#installation)
- [Getting Started](#getting-started)
- [Usage](#usage)
    - [Authentication](#authentication)
    - [Managing Users](#managing-users)
    - [Working with Tokens](#working-with-tokens)
    - [Permissions and Roles](#permissions-and-roles)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

## Installation

To use the Keycloak client library in your .NET Core application, add the NuGet package (coming soon):

```bash
Install-Package NETCore.Keycloak
```

Alternatively, you can clone the repository and include the project in your solution.

## Getting Started

### Prerequisites

- .NET Core SDK (version 6.0 or later).
- A running Keycloak instance.
- Client credentials and realm configuration for Keycloak.

### Setup

1. Configure your Keycloak :



2. Register the Keycloak client in your `Startup.cs` or `Program.cs`:


## Usage

### Authentication

Authenticate against Keycloak to get an access token:

### Managing Users

Query users with filters:

```csharp
var filter = new KcUserFilter
{
    Username = "testuser",
    EmailVerified = true
};
```

## Configuration


## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -m 'Add new feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a pull request.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

## Acknowledgments

- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [Newtonsoft.Json](https://www.newtonsoft.com/json) for JSON serialization.
- [System.IdentityModel.Tokens.Jwt](https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/) for JWT handling.

---

Feel free to report issues or request features via the [Issues](https://github.com/Black-Cockpit/NETCore.Keycloak/issues) tab.

