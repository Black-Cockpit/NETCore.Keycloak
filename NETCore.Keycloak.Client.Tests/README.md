# NETCore.Keycloak.Client Tests

This directory contains the comprehensive test suite for the NETCore.Keycloak.Client library. The test suite is designed to validate the library's functionality across multiple versions of Keycloak (20.x through 26.x) using automated test environments and continuous integration.

The test suite implements:
- End-to-end testing of all Keycloak client features
- Authentication flows (Client Credentials, Resource Owner Password)
- Authorization mechanisms (Bearer tokens, RPT tokens)
- Client and realm management operations
- Group and role management
- User operations and session handling

Key features of the test infrastructure:
- Automated Keycloak environment setup using Ansible
- Docker-based test environments for each Keycloak version
- Parallel test execution capabilities
- Comprehensive mock data fixtures
- Detailed test reporting

## Test Environment Setup

### Prerequisites

1. **Python Environment**:
   - Python 3.9 or higher
   - pip (Python package installer)
   - virtualenv

2. **Docker**:
   - Docker Engine
   - Docker Compose v2

3. **.NET SDK**:
   - .NET 6.0 SDK
   - .NET 7.0 SDK
   - .NET 8.0 SDK

4. **Build Tools**:
   - Cake (C# Make)
   - Make

### Setting Up Python Environment

The project uses a Makefile to automate the Python environment setup. The following commands are available:

```bash
# Install virtual environment and dependencies
make install_virtual_env

# This will:
# 1. Create a Python virtual environment (keycloak.venv)
# 2. Install required Python packages from requirements.txt
# 3. Install necessary Ansible collections:
#    - community.general
#    - community.postgresql
#    - community.crypto
#    - community.docker
```

## Running Tests

The test suite uses Cake for build automation and includes several types of tests:

### Test Commands

```bash
# Run tests for a specific Keycloak version (replace XX with version 20-26)
dotnet cake e2e_test.cake --kc_major_version=XX

# Run tests for all supported Keycloak versions
dotnet cake e2e_test.cake
```

### Available Cake Tasks

1. **Clean**:
   - Cleans build output directories
   - Ensures clean state before build

2. **Restore**:
   - Restores NuGet packages
   - Depends on Clean task

3. **Build**:
   - Builds the solution
   - Depends on Restore task

4. **Test**:
   - Runs unit tests
   - Depends on Build task
   - Uses normal verbosity

5. **E2E-Tests**:
   - Runs end-to-end tests
   - Supports Keycloak versions 20-26
   - Can target specific version
   - Sets up test environment automatically

### Test Categories

1. **Authentication Tests** (`KcAuthTests/`):
   - Client Credentials Flow (`KcAuthClientCredentialsTests`)
   - Resource Owner Password Flow (`KcAuthResourceOwnerPasswordTests`)
   - Token Refresh Operations (`KcAuthRefreshAccessTokenTests`)
   - RPT Token Requests (`KcAuthRequestPartyToken`)
   - Token Validation (`KcAuthValidatePasswordTests`)
   - Token Revocation (`KcAuthRevokeTokenTests`)

2. **Authentication Configuration** (`KcAuthentication/`):
   - Configuration Settings (`KcAuthenticationConfigurationTests`)
   - Extension Methods (`KcAuthenticationExtensionTests`)
   - Role Claims Transformation (`KcRolesClaimsTransformerTests`)

3. **Authorization Tests** (`KcAuthorizationTests/`):
   - Bearer Authorization Handler (`KcBearerAuthorizationHandlerTests`)
   - Authorization Extensions (`KcAuthorizationExtensionTests`)
   - Authorization Requirements (`KcAuthorizationRequirementTests`)
   - Protected Resource Policies (`KcProtectedResourcePolicyProviderTests`)
   - Realm Admin Configuration (`KcRealmAdminConfigurationTests`)
   - Token Caching (`KcCachedTokenTests`)

4. **Client Management** (`KcClientsTests/`):
   - Basic Client Operations (`KcClientHappyPathTests`)
   - Client Filtering (`KcClientFilterTests`)
   - Service Account Management (`KcClientServiceAccountTests`)
   - Client Token Operations (`KcClientTokenTests`)
   - Private Client Handling (`KcPrivateClientTests`)

5. **Client Scope Tests** (`KcClientScopeTests/`):
   - Scope Management (`KcClientScopeHappyPathTests`)

6. **Group Management** (`KcGroupTests/`):
   - Group Operations (`KcGroupsHappyPathTests`)
   - Group Filtering (`KcGroupFilterTests`)

7. **Support Tests**:
   - Attack Detection (`KcAttackDetectionTests`)
   - Client Role Mapping (`KcClientRoleMappingTests`)
   - Protocol Mappers (`KcProtocolMapperTests`)
   - Common Operations (`KcCommonTests`)
   - Filter Operations (`KcFilterTests`)

### Test Patterns

Each test module follows these patterns:

1. **Happy Path Tests**:
   - Basic CRUD operations
   - Expected workflow scenarios
   - Successful authentication flows
   - Normal authorization paths

2. **Error Case Tests**:
   - Invalid credentials handling
   - Expired token scenarios
   - Missing permission cases
   - Resource not found situations

3. **Filter Tests**:
   - Query parameter validation
   - Search functionality
   - Pagination handling
   - Sorting operations

4. **Integration Tests**:
   - End-to-end workflows
   - Cross-component interactions
   - Real-world scenarios
   - Environment-specific cases

### Mock Data Organization

The `MockData/` directory provides test fixtures:

1. **Authentication Mocks**:
   - JWT Tokens (`KcJwtTokenMock`)
   - Test Passwords (`KcTestPasswordCreator`)

2. **Resource Mocks**:
   - Protected Resources (`KcMockKcProtectedResourceStore`)
   - Realm Configuration (`KcMockKcRealmAdminConfigurationStore`)

3. **Entity Mocks**:
   - Clients (`KcClientMocks`)
   - Users (`KcUserMocks`)
   - Roles (`KcRoleMocks`)
   - Protocol Mappers (`KcProtocolMapperMocks`)

## Test Structure

```
NETCore.Keycloak.Client.Tests/
├── Abstraction/                 # Test abstractions and base classes
│   ├── KcTestableBearerAuthorizationHandler.cs
│   └── KcTestingModule.cs
├── ExceptionsTests/            # Exception handling tests
│   ├── KcExceptionTests.cs
│   ├── KcSessionClosedExceptionTests.cs
│   └── KcUserNotFoundExceptionTests.cs
├── MockData/                   # Mock data and test helpers
│   ├── KcClientMocks.cs
│   ├── KcJwtTokenMock.cs
│   ├── KcProtocolMapperMocks.cs
│   ├── KcRoleMocks.cs
│   └── KcUserMocks.cs
├── Models/                     # Test model definitions
│   ├── KcTestEnvironment.cs
│   ├── KcTestRealm.cs
│   ├── KcTestResource.cs
│   └── KcTestUser.cs
├── Modules/                    # Feature module tests
│   ├── KcAuthTests/           # Authentication tests
│   │   ├── KcAuthClientCredentialsTests.cs
│   │   ├── KcAuthRefreshAccessTokenTests.cs
│   │   ├── KcAuthRequestPartyToken.cs
│   │   └── KcAuthResourceOwnerPasswordTests.cs
│   ├── KcAuthentication/      # Authentication configuration tests
│   │   ├── KcAuthenticationConfigurationTests.cs
│   │   ├── KcAuthenticationExtensionTests.cs
│   │   └── KcRolesClaimsTransformerTests.cs
│   ├── KcAuthorizationTests/  # Authorization handler tests
│   │   ├── KcAuthorizationExtensionTests.cs
│   │   ├── KcBearerAuthorizationHandlerTests.cs
│   │   ├── KcProtectedResourcePolicyProviderTests.cs
│   │   └── KcRealmAdminConfigurationTests.cs
│   ├── KcClientScopeTests/    # Client scope tests
│   │   └── KcClientScopeHappyPathTests.cs
│   ├── KcClientsTests/        # Client management tests
│   │   ├── KcClientFilterTests.cs
│   │   ├── KcClientHappyPathTests.cs
│   │   ├── KcClientServiceAccountTests.cs
│   │   └── KcClientTokenTests.cs
│   └── KcGroupTests/          # Group management tests
│       ├── KcGroupFilterTests.cs
│       └── KcGroupsHappyPathTests.cs
├── ansible/                    # Environment automation
├── cakeScripts/               # Build automation scripts
├── e2e_test.cake             # E2E test orchestration
```

## Makefile Details

The Makefile automates several key aspects of the test environment:

### Virtual Environment Setup
```makefile
install_virtual_env:
    # Creates/recreates Python virtual environment
    # Installs Python dependencies
    # Installs required Ansible collections
```

### Environment Cleanup
```makefile
stop:
    # Stops and removes the current Keycloak environment
    # Cleans up containers and volumes
```
