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
   - Python 3.12 or higher (for the current Ansible requirements)
   - pip (Python package installer)
   - virtualenv

2. **Docker**:
   - Docker Engine
   - Docker Compose v2

3. **.NET SDK**:
   - .NET 10.0 SDK

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

### Independent coverage unit tests

The `CoverageUnit` category runs without Docker or `Assets/testing_environment.json`:

```bash
dotnet test NETCore.Keycloak.Client.Tests/NETCore.Keycloak.Client.Tests.csproj \
  --configuration Release --filter 'TestCategory=CoverageUnit' --collect 'XPlat Code Coverage'
```

These tests resolve real named JWT options, validate signed-token issuer/audience/expiry rules,
exercise dynamic policies and concurrency, and advance an injected `TimeProvider` to verify
admin token renewal and coalescing of concurrent renewals without sleeping. Fake HTTP responses cover transport/encoding failures,
optional OAuth parameters and JSON wire contracts. Tests use explicit protocol fixtures rather
than setting arbitrary properties through reflection.
独立单测无需 Keycloak 配置，重点覆盖认证授权边界、令牌续期与序列化契约；完整覆盖率仍以全量测试报告为准。

The authorization handlers optionally resolve `IHttpClientFactory` and `TimeProvider` from DI.
Without registrations, the existing shared HTTP client and system clock are used.
The refresh cache uses `refresh_expires_in`, and its safety margin never exceeds half of a
short token's lifetime. Public method signatures are unchanged.

The contract tests also protect resource `_id` mapping (with legacy `id` input accepted),
enum converter registration/null values, session-query exception diagnostics and revocation
transport failures. On Windows, run coverage serially or use separate `--artifacts-path`
directories; another test host loading the instrumented output can prevent Coverlet from
restoring that assembly and invalidate the coverage report.

Local verification on 2026-11-03, .NET 10 Release and Keycloak 26.8.1:

| Measurement | Before | After |
| --- | --- | --- |
| Full v1 run | 380 passed, no skips | 434 passed, no skips |
| Additional independent tests | — | 54 passed without integration credentials |
| Applicable v2 regressions | 31 passed | 31 passed |
| Line coverage, Cobertura rate | 92.75% | 99.65% |
| Branch coverage | 70.59% | 83.72% |
| Unexecuted source lines | 255 | 12 |

The 12 remaining lines are internal argument/null guards, the defensive null token-response
return, repeated-disposal handling and the finalizer path. No reflection into private methods,
forced garbage collection or coverage exclusions were added to inflate the result.
本轮新增 54 项独立单测；剩余 12 行为内部校验、防御性返回与资源释放路径，不代表仍有未执行的 HTTP 方法。

### Run the complete .NET 10 test suite

The complete suite includes integration tests that create, update, and delete Keycloak resources.
Use a dedicated test environment prepared with the Ansible/Cake setup described below.
The generated `Assets/testing_environment.json` must exist in this test project before building;
the project copies it to the test output directory. This file contains local credentials and is
intentionally excluded from Git. A missing file causes integration test classes to fail during
initialization, before their test methods run.

From the repository root, run:

```powershell
dotnet test NETCore.Keycloak.Client.Tests/NETCore.Keycloak.Client.Tests.csproj --framework net10.0 --configuration Release --logger trx
```

In Visual Studio, rebuild the test project after generating or updating the configuration, then
run the `NETCore.Keycloak.Client.Tests (net10.0)` group. Run complete test classes: the integration
scenarios with `A_` through `Z_` method names share resources created by earlier steps.

The Keycloak 26.8.1 suite exercises online session lookup/deletion, Bearer allow/deny decisions,
and public-client management permissions without the previous five version-based skips.
`offline_access` is an **optional** client scope: request it explicitly for offline tokens.
Making it a default scope causes password grants to remove their online session after issuing
an offline token, which invalidates assumptions in online session tests.

### Keycloak 26.8.1 compatibility / 兼容性验证

The target is the existing .NET client surface. The suite does not claim to test every server
feature, UI, external identity provider, LDAP deployment or cluster topology.
本轮完整验证已有客户端功能，包含真实服务端的成功、认证失败和权限拒绝场景。

| Existing surface / 已有功能 | Verification / 验证内容 |
| --- | --- |
| Authentication | Public clients; confidential Basic/Post registration and enforcement; password validation; refresh and revocation |
| Users and sessions | CRUD, credentials, groups, online/offline session separation and deletion |
| Clients, client scopes and initial access | CRUD, secrets, scopes, service accounts, sessions and initial access tokens |
| Roles and groups | Realm/client roles, composites, members, role-to-group visibility and privileged group protection |
| Role mappings, client role mappings, scope mappings and protocol mappers | Existing positive workflows plus invalid-token and least-privilege rejection |
| Organizations | Existing CRUD, domains, filtering and serialization; authentication and permission boundaries |
| Attack detection | Existing login failure operations and authentication/permission boundaries |
| UMA/RPT and ASP.NET integration | Multiple permission fields, request-local bearer headers, resource-owner isolation, Bearer allow/deny and claims/policy tests |

The dedicated `Keycloak2681` regressions create a unique `netcore_2681_*` realm per test and delete
it afterwards. They use master admin credentials only for fixture preparation and cleanup;
the operation under test uses the intended user or delegated administrator token.
Original `A_`–`Z_` tests still use the dedicated shared integration realm.
Do not run either suite against production.

From this test directory on a dedicated Linux test host (or WSL with Docker configured):

```bash
make prepare_keycloak_26_environment KC_VERSION=26.8.1 KC_FGAP_VERSION=v1
dotnet test NETCore.Keycloak.Client.Tests.csproj --configuration Release \
  --logger 'trx;LogFileName=full.trx' --results-directory obj/verification/v1 \
  --collect 'XPlat Code Coverage'
python verify_test_results.py obj/verification/v1 --require-http-method-coverage
make stop

make prepare_keycloak_26_environment KC_VERSION=26.8.1 KC_FGAP_VERSION=v2
dotnet test NETCore.Keycloak.Client.Tests.csproj --configuration Release \
  --filter 'TestCategory=Keycloak2681&TestCategory!=LegacyAdminPermissions' --logger 'trx;LogFileName=fgap-v2.trx' \
  --results-directory obj/verification/v2
python verify_test_results.py obj/verification/v2
make stop
```

The full v1 profile also exercises the client scope-mapping aliases, role-container evaluation,
node registration/deletion, empty adapter topology checks, revocation propagation, composite-role
removal and legacy `/management/permissions` APIs. The coverage gate requires execution of every
declared method in all 13 client interfaces, including async state machines; it does not imply 100% line
or branch coverage. 未执行的已有 HTTP 方法会导致 CI 失败，避免仅凭部分测试通过宣称完整验证。

The legacy `/management/permissions` APIs are FGAP v1 APIs;
enabling v2 does not convert them into FGAP v2 configuration APIs. The v2 profile repeats
the dedicated authentication/security regressions and applicable API round trips, including
allowing profile management while explicitly denying password reset. Only the explicit
`LegacyAdminPermissions` category is excluded from this profile. It is not a replacement for the full v1 run.
完整 v1 测试与 v2 安全回归互为补充；CI 要求两者都通过，且不允许失败或跳过。

`.github/workflows/keycloak_2681.yml` runs both profiles independently of Sonar credentials,
uses PostgreSQL through the standard Ansible setup, and publishes TRX/Cobertura artifacts.
Local dev-mode/H2 runs do not establish PostgreSQL or cluster compatibility by themselves.

For an already prepared second server, `KC_TEST_BASE_URL` overrides only the server URL loaded
from the ignored configuration file. The server version is checked through `/admin/serverinfo`.
The override does not provision shared `A_`–`Z_` fixtures: use the standard setup for a full run,
or select `TestCategory=Keycloak2681` for the independent regressions.

Coverage includes regressions related to credential mutation permissions, privileged group
membership, role/group visibility, UMA resource-owner boundaries and refresh/revoke races
listed in the [26.8.1 release notes](https://www.keycloak.org/2026/10/keycloak-2681-released).
This is functional regression evidence, not a claim of exhaustive security certification.

To use Basic client authentication, set
`KcClientCredentials.AuthenticationMethod = KcClientAuthenticationMethod.ClientSecretBasic`.
The default remains `ClientSecretPost`, including public clients with no secret.
默认认证方式和现有公开方法签名保持不变；Basic 认证为显式可选项。

### Declared API inventory / 公开接口清单

The current client exposes **13 modules and 178 public asynchronous methods**. Method counts
include convenience operations and alternate forms; they are not counts of unique HTTP routes.
The earlier figure of 167 counted only ordinary Task-returning methods in Cobertura and omitted
11 async state machines. The verification script now checks the declared interfaces and maps
those state machines back to their methods. This corrects the counting/verification scope;
it does not add 11 new APIs.
当前按公开接口声明核对为 178 个方法，包含组合操作和不同形式的封装，不等于 178 条独立服务端路由。

Except for Auth, paths below are relative to `/admin/realms/{realm}`.

| Module | Methods | Available operations | Representative paths |
| --- | ---: | --- | --- |
| Auth | 7 | Client credentials/password grants; password validation; refresh; access/refresh revocation; UMA/RPT with multiple permissions; public clients and optional Basic/Post authentication. | `/realms/{realm}/protocol/openid-connect/token and /revoke` |
| AttackDetection | 2 | Read a user login-failure status; clear one user or all users' failures. | `/attack-detection/brute-force/users` |
| ClientInitialAccess | 3 | Create, list and delete initial access tokens for client registration. | `/clients-initial-access` |
| Users | 18 | Create/read/update/delete/search/count; email existence; credentials and labels; password reset; group membership/count; online sessions, session deletion and user-wide logout. | `/users; /sessions/{sessionId}` |
| Clients | 33 | CRUD/search; secret generation/read/rotation invalidation; default/optional scopes; example access/ID/user-info evaluation; mapper/scope evaluation; legacy permissions; node registration/deletion/checks; online/offline sessions/counts; revocation push; registration token and service-account user. | `/clients; /clients/{id}/evaluate-scopes; /clients/{id}/nodes` |
| ClientScopes | 5 | Create/read/update/delete/list reusable client scopes. | `/client-scopes` |
| Groups | 10 | CRUD/list/count; child group creation/update; members; legacy management permissions. | `/groups` |
| Roles | 38 | Realm/client role CRUD; lookup by name/ID where exposed; existence; composites and realm/client composite subsets; role users/groups; legacy role management permissions. | `/roles; /roles-by-id; /clients/{id}/roles` |
| RoleMappings | 12 | User/group realm-role assignment, removal, assigned/available/effective role lists and combined mapping representations. | `/users/{id}/role-mappings; /groups/{id}/role-mappings` |
| ClientRoleMappings | 10 | Assign/remove client roles to users/groups; query assigned, available and composite client roles. | `/users/{id}/role-mappings/clients/{client}; group equivalent` |
| ScopeMappings | 20 | Assign/remove realm/client roles and query assigned/available/effective scope mappings for both reusable client scopes and individual clients. | `/client-scopes/{id}/scope-mappings; /clients/{id}/scope-mappings` |
| ProtocolMappers | 14 | Single/bulk creation, list/read/update/delete and protocol-filtered lists for reusable client scopes and individual clients. | `/client-scopes/{id}/protocol-mappers; /clients/{id}/protocol-mappers` |
| Organizations | 6 | Create/read/update/delete/list/count organizations; domain fields are read/written as part of organization representations. | `/organizations` |

Additional ASP.NET integration covers named JWT options, issuer/audience/lifetime validation,
realm/client role claims, dynamic `resource#scope` policies, admin token caching/renewal and
Bearer session/RPT authorization. Renewal is coalesced per handler/realm to avoid concurrent
submission of a rotating refresh token; cancellation of one waiter leaves the active renewal intact.

Support boundaries for acceptance:

- Organization invitations, members, organization groups and identity-provider links are not wrapped.
- Realm-wide CRUD/configuration, identity-provider management, authorization-code/PKCE/PAR,
  token introspection, Standard Token Exchange and inbound backchannel logout handlers are not wrapped.
- Legacy `/management/permissions` configuration belongs to FGAP v1. V2 authorization decisions are
  regression-tested; raw REST fixture setup is not a public FGAP v2 administration API.
- Protocol-filtered mapper APIs contain SAML selection, but the SAML branch has not been independently
  integration-tested. This is not a claim of SAML login/brokering support.
- Node registration/deletion and empty adapter-topology checks/pushes are tested. Real remote Java
  adapters, production clusters, LDAP/brokers and a hosted ASP.NET middleware login flow require
  deployment-specific validation. Local dev-mode/H2 evidence does not replace Linux/PostgreSQL CI.
- The two legacy methods named `GetScopedProtocolMappersInContainerAsync` and
  `GetUnScopedProtocolMappersInContainerAsync` evaluate role scope mappings. They retain their
  historical DTO/signatures; tested results include role IDs/names rather than complete role metadata.

The full declared method list follows for traceability.

<details>
<summary>Auth: 7 methods</summary>

- `GetClientCredentialsTokenAsync`
- `GetResourceOwnerPasswordTokenAsync`
- `ValidatePasswordAsync`
- `RefreshAccessTokenAsync`
- `RevokeAccessTokenAsync`
- `RevokeRefreshTokenAsync`
- `GetRequestPartyTokenAsync`

</details>

<details>
<summary>AttackDetection: 2 methods</summary>

- `DeleteUsersLoginFailureAsync`
- `GetUserStatusAsync`

</details>

<details>
<summary>ClientInitialAccess: 3 methods</summary>

- `CreateInitialAccessTokenAsync`
- `GetInitialAccessAsync`
- `DeleteInitialAccessTokenAsync`

</details>

<details>
<summary>Users: 18 methods</summary>

- `CreateAsync`
- `IsUserExistsByEmailAsync`
- `ListUserAsync`
- `CountAsync`
- `GetAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetCredentialsAsync`
- `DeleteCredentialsAsync`
- `UpdateCredentialsLabelAsync`
- `UserGroupsAsync`
- `CountGroupsAsync`
- `AddToGroupAsync`
- `DeleteFromGroupAsync`
- `ResetPasswordAsync`
- `SessionsAsync`
- `DeleteSessionAsync`
- `LogoutFromAllSessionsAsync`

</details>

<details>
<summary>Clients: 33 methods</summary>

- `CreateAsync`
- `ListAsync`
- `GetAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GenerateNewSecretAsync`
- `GetSecretAsync`
- `GetRotatedSecretAsync`
- `InvalidateRotatedSecretAsync`
- `GetDefaultScopesAsync`
- `AddDefaultScopesAsync`
- `DeleteDefaultScopesAsync`
- `GenerateExampleAccessTokenAsync`
- `GenerateExampleIdTokenAsync`
- `GenerateExampleUserInfoAsync`
- `GetProtocolMappersAsync`
- `GetScopedProtocolMappersInContainerAsync`
- `GetUnScopedProtocolMappersInContainerAsync`
- `GetAuthorizationManagementPermissionAsync`
- `SetAuthorizationManagementPermissionAsync`
- `RegisterNodeAsync`
- `DeleteNodeAsync`
- `CountOfflineSessionsAsync`
- `GetOfflineSessionsAsync`
- `GetOptionalScopesAsync`
- `AddOptionalScopeAsync`
- `DeleteOptionalScopeAsync`
- `PushRevocationAsync`
- `GetRegistrationAccessTokenAsync`
- `GetServiceAccountUserAsync`
- `CountSessionsAsync`
- `TestAvailableNodesAsync`
- `GetUsersSessionsAsync`

</details>

<details>
<summary>ClientScopes: 5 methods</summary>

- `CreateAsync`
- `ListAsync`
- `GetAsync`
- `UpdateAsync`
- `DeleteAsync`

</details>

<details>
<summary>Groups: 10 methods</summary>

- `CreateAsync`
- `ListAsync`
- `CountAsync`
- `GetAsync`
- `UpdateAsync`
- `DeleteAsync`
- `SetOrCreateChildAsync`
- `GetAuthorizationManagementPermissionAsync`
- `SetAuthorizationManagementPermissionAsync`
- `GetMembersAsync`

</details>

<details>
<summary>Roles: 38 methods</summary>

- `CreateAsync`
- `ListAsync`
- `GetAsync`
- `IsRolesExistsAsync`
- `GetByIdAsync`
- `UpdateAsync`
- `UpdateByIdAsync`
- `DeleteAsync`
- `DeleteByIdAsync`
- `AddCompositeAsync`
- `AddCompositeByIdAsync`
- `ListCompositeAsync`
- `ListCompositeByIdAsync`
- `DeleteCompositeAsync`
- `DeleteCompositeByIdAsync`
- `GetClientLevelCompositesAsync`
- `GetClientLevelCompositesByIdAsync`
- `GetRealmLevelCompositesAsync`
- `GetRealmLevelCompositesByIdAsync`
- `GetGroupsAsync`
- `GetAuthorizationPermissionsAsync`
- `GetAuthorizationPermissionsByIdAsync`
- `SetAuthorizationPermissionsAsync`
- `SetAuthorizationPermissionsByIdAsync`
- `GetUserInRoleAsync`
- `CreateClientRoleAsync`
- `ListClientRoleAsync`
- `GetClientRolesAsync`
- `IsClientRoleExistsAsync`
- `UpdateClientRoleAsync`
- `DeleteClientRoleAsync`
- `AddClientRoleToCompositeAsync`
- `GetClientCompositeRolesAsync`
- `RemoveClientRoleFromCompositeAsync`
- `GetGroupsInClientRoleAsync`
- `GetClientRoleAuthorizationPermissionsAsync`
- `SetClientRoleAuthorizationPermissionsAsync`
- `GetUsersInClientRoleAsync`

</details>

<details>
<summary>RoleMappings: 12 methods</summary>

- `GetGroupRoleMappingsAsync`
- `AddGroupRealmRoleMappingsAsync`
- `ListGroupRealmRoleMappingsAsync`
- `DeleteGroupRealmRoleMappingsAsync`
- `ListGroupAvailableRealmRoleMappingsAsync`
- `ListGroupEffectiveRealmRoleMappingsAsync`
- `GetUserRoleMappingsAsync`
- `AddUserRealmRoleMappingsAsync`
- `ListUserRealmRoleMappingsAsync`
- `DeleteUserRealmRoleMappingsAsync`
- `ListUserAvailableRealmRoleMappingsAsync`
- `ListUserEffectiveRealmRoleMappingsAsync`

</details>

<details>
<summary>ClientRoleMappings: 10 methods</summary>

- `MapClientRolesToGroupAsync`
- `GetGroupMappedClientRolesAsync`
- `DeleteGroupClientRoleMappingsAsync`
- `GetGroupAvailableClientRolesAsync`
- `GetGroupCompositeClientRolesAsync`
- `MapClientRolesToUserAsync`
- `GetUserMappedClientRolesAsync`
- `DeleteUserClientRoleMappingsAsync`
- `GetUserAvailableClientRolesAsync`
- `GetUserCompositeClientRolesAsync`

</details>

<details>
<summary>ScopeMappings: 20 methods</summary>

- `AddClientRolesToScopeAsync`
- `ListClientRolesAssociatedToScopeAsync`
- `RemoveClientRolesFromScopeAsync`
- `ListClientRolesAvailableForScopeAsync`
- `ListCompositeClientRolesAssociatedToScopeAsync`
- `AddRolesToScopeAsync`
- `ListRolesAssociatedToScopeAsync`
- `RemoveRolesFromScopeAsync`
- `ListRolesAvailableForScopeAsync`
- `ListCompositeRolesAssociatedToScopeAsync`
- `AddClientRolesToClientScopeAsync`
- `ListClientRolesAssociatedToClientScopeAsync`
- `RemoveClientRolesFromClientScopeAsync`
- `ListClientRolesAvailableForClientScopeAsync`
- `ListCompositeClientRolesAssociatedToClientScopeAsync`
- `AddRolesToScopeClientAsync`
- `ListRolesAssociatedToClientScopeAsync`
- `RemoveRolesFromClientScopeAsync`
- `ListRolesAvailableForClientScopeAsync`
- `ListCompositeRolesAssociatedToClientScopeAsync`

</details>

<details>
<summary>ProtocolMappers: 14 methods</summary>

- `AddMappersAsync`
- `AddMapperAsync`
- `ListMappersAsync`
- `GetMapperAsync`
- `UpdateMapperAsync`
- `DeleteMapperAsync`
- `ListMappersByProtocolNameAsync`
- `AddClientMappersAsync`
- `AddClientMapperAsync`
- `ListClientMappersAsync`
- `GetClientMapperAsync`
- `UpdateClientMapperAsync`
- `DeleteClientMapperAsync`
- `ListClientMappersByProtocolNameAsync`

</details>

<details>
<summary>Organizations: 6 methods</summary>

- `CreateAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAsync`
- `ListAsync`
- `CountAsync`

</details>

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
