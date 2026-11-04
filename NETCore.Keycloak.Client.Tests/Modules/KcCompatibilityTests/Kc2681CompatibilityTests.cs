using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NETCore.Keycloak.Client.Tests.Modules.KcCompatibilityTests;

/// <summary>
/// Exercises the existing client against Keycloak 26.8.1 with isolated realms and real authorization decisions.
/// Raw REST calls only prepare fixtures for operations not exposed by the client.
/// </summary>
[TestClass]
[TestCategory("Keycloak2681")]
public sealed class Kc2681CompatibilityTests : KcTestingModule, IDisposable
{
    private readonly HttpClient _setupClient = new();
    private string _realm;
    private string _adminToken;
    private bool _realmCreated;
    private bool _fgapV2;
    private const string Password = "Test-Only_2681!Password";
    private static readonly string[] RedirectUris = ["http://localhost/unused-test-callback"];
    private static readonly string[] GrantTypes = ["password", "client_credentials", "refresh_token"];
    private static readonly string[] ViewAndManageScopes = ["view", "manage"];
    private static readonly string[] ResetPasswordScopes = ["reset-password"];
    private static readonly string[] ViewScopes = ["view"];
    private static readonly string[] ReadScopes = ["read"];

    /// <summary>
    /// Creates a uniquely named realm without modifying the shared integration realm or other installations.
    /// </summary>
    [TestInitialize]
    public async Task SetUpAsync()
    {
        if ( !Version.TryParse(TestEnvironment.KcVersion, out var version) || version < new Version(26, 8, 1) )
        {
            Assert.Inconclusive("This regression suite requires Keycloak 26.8.1 or later.");
        }
        Assert.IsNotNull(TestEnvironment.Admin, "The test configuration must contain master realm admin credentials.");
        var token = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(TestEnvironment.AuthRealm,
            new KcClientCredentials { ClientId = TestEnvironment.ClientId }, TestEnvironment.Admin)
            .ConfigureAwait(false);
        Assert.IsFalse(token.IsError, token.ErrorMessage);
        _adminToken = token.Response.AccessToken;
        var info = await SetupAsync(HttpMethod.Get, "admin/serverinfo").ConfigureAwait(false);
        _fgapV2 = info["profileInfo"]?["disabledFeatures"]?.Values<string>()
            .Contains("ADMIN_FINE_GRAINED_AUTHZ") == true;
        _realm = $"netcore_2681_{Guid.NewGuid():N}";
        _ = await SetupAsync(HttpMethod.Post, "admin/realms", new
        {
            realm = _realm,
            enabled = true,
            organizationsEnabled = true,
            adminPermissionsEnabled = _fgapV2,
            revokeRefreshToken = true,
            refreshTokenMaxReuse = 0
        }, HttpStatusCode.Created).ConfigureAwait(false);
        _realmCreated = true;
    }

    /// <summary>
    /// Deletes only the realm created by this test, including when an assertion fails.
    /// </summary>
    [TestCleanup]
    public async Task CleanUpAsync()
    {
        try
        {
            if ( _realmCreated )
            {
                _ = await SetupAsync(HttpMethod.Delete, $"admin/realms/{_realm}", null, HttpStatusCode.NoContent)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            Dispose();
        }
    }

    /// <summary>
    /// Releases the fixture HTTP client, including when initialization fails.
    /// </summary>
    public void Dispose()
    {
        _setupClient.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Checks the running server rather than trusting only the configured image tag.
    /// </summary>
    [TestMethod]
    public async Task ShouldRunThePinnedServerVersion()
    {
        var server = await SetupAsync(HttpMethod.Get, "admin/serverinfo").ConfigureAwait(false);
        Assert.AreEqual(TestEnvironment.KcVersion, server["systemInfo"]?["version"]?.Value<string>());
    }

    /// <summary>
    /// Uses only the registered authentication method for token, refresh, validation and revocation operations.
    /// </summary>
    /// <param name="basic">Whether this client accepts only client_secret_basic.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task ShouldRespectRegisteredClientAuthenticationMethod(bool basic)
    {
        var credentials = await CreateClientAsync(basic).ConfigureAwait(false);
        _ = await CreateUserAsync("alice").ConfigureAwait(false);
        var login = new KcUserLogin { Username = "alice", Password = Password };
        var service = await KeycloakRestClient.Auth.GetClientCredentialsTokenAsync(_realm, credentials)
            .ConfigureAwait(false);
        Assert.IsFalse(service.IsError, service.ErrorMessage);
        var token = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(_realm, credentials, login)
            .ConfigureAwait(false);
        Assert.IsFalse(token.IsError, token.ErrorMessage);
        var refresh = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials,
            token.Response.RefreshToken).ConfigureAwait(false);
        Assert.IsFalse(refresh.IsError, refresh.ErrorMessage);
        var revoke = await KeycloakRestClient.Auth.RevokeRefreshTokenAsync(_realm, credentials,
            refresh.Response.RefreshToken).ConfigureAwait(false);
        Assert.IsFalse(revoke.IsError, revoke.ErrorMessage);
        var replay = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials,
            refresh.Response.RefreshToken).ConfigureAwait(false);
        Assert.IsTrue(replay.IsError);
        Assert.AreEqual(HttpStatusCode.BadRequest, replay.MonitoringMetrics.StatusCode);
        var validation = await KeycloakRestClient.Auth.ValidatePasswordAsync(_realm, credentials, login)
            .ConfigureAwait(false);
        Assert.IsTrue(validation.Response);
        Assert.IsFalse(validation.IsError);
        var wrong = new KcClientCredentials
        {
            ClientId = credentials.ClientId,
            Secret = credentials.Secret,
            AuthenticationMethod = basic
                ? KcClientAuthenticationMethod.ClientSecretPost : KcClientAuthenticationMethod.ClientSecretBasic
        };
        var rejected = await KeycloakRestClient.Auth.GetClientCredentialsTokenAsync(_realm, wrong)
            .ConfigureAwait(false);
        Assert.IsTrue(rejected.IsError, "The server must actually enforce the configured authentication method.");
        Assert.AreEqual(HttpStatusCode.Unauthorized, rejected.MonitoringMetrics.StatusCode);
    }

    /// <summary>
    /// Keeps online sessions visible and requests offline sessions explicitly.
    /// </summary>
    [TestMethod]
    public async Task ShouldDistinguishOnlineAndOfflineSessions()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        var userId = await CreateUserAsync("alice").ConfigureAwait(false);
        var login = new KcUserLogin { Username = "alice", Password = Password };
        var online = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(_realm, credentials, login)
            .ConfigureAwait(false);
        Assert.IsFalse(online.IsError, online.ErrorMessage);
        var sid = new JwtSecurityTokenHandler().ReadJwtToken(online.Response.AccessToken)
            .Claims.Single(claim => claim.Type == "sid").Value;
        var sessions = await KeycloakRestClient.Users.SessionsAsync(_realm, _adminToken, userId)
            .ConfigureAwait(false);
        Assert.IsFalse(sessions.IsError, sessions.ErrorMessage);
        Assert.IsTrue(sessions.Response.Any(session => session.Id == sid));
        var offline = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(_realm, credentials, login,
            scope: "openid offline_access").ConfigureAwait(false);
        Assert.IsFalse(offline.IsError, offline.ErrorMessage);
        var refresh = new JwtSecurityTokenHandler().ReadJwtToken(offline.Response.RefreshToken);
        Assert.AreEqual("Offline", refresh.Claims.Single(claim => claim.Type == "typ").Value);
        var deleted = await KeycloakRestClient.Users.DeleteSessionAsync(_realm, _adminToken, sid)
            .ConfigureAwait(false);
        Assert.IsFalse(deleted.IsError, deleted.ErrorMessage);
        var revoked = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials,
            online.Response.RefreshToken).ConfigureAwait(false);
        Assert.IsTrue(revoked.IsError);
        var offlineRefresh = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials,
            offline.Response.RefreshToken).ConfigureAwait(false);
        Assert.IsFalse(offlineRefresh.IsError, offlineRefresh.ErrorMessage);
    }

    /// <summary>
    /// Ensures strict refresh token rotation rejects reuse and revoked descendants.
    /// </summary>
    [TestMethod]
    public async Task ShouldRejectReusedAndRevokedRefreshTokensInStrictMode()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        _ = await CreateUserAsync("alice").ConfigureAwait(false);
        var token = await LoginAsync(credentials, "alice").ConfigureAwait(false);
        var first = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials, token.RefreshToken)
            .ConfigureAwait(false);
        Assert.IsFalse(first.IsError, first.ErrorMessage);
        var reused = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials, token.RefreshToken)
            .ConfigureAwait(false);
        Assert.IsTrue(reused.IsError);
        Assert.AreEqual(HttpStatusCode.BadRequest, reused.MonitoringMetrics.StatusCode);

        // A fresh login keeps revocation verification independent of the server's replay response.
        var fresh = await LoginAsync(credentials, "alice").ConfigureAwait(false);
        var rotated = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials, fresh.RefreshToken)
            .ConfigureAwait(false);
        Assert.IsFalse(rotated.IsError, rotated.ErrorMessage);
        var revoked = await KeycloakRestClient.Auth.RevokeRefreshTokenAsync(_realm, credentials,
            rotated.Response.RefreshToken).ConfigureAwait(false);
        Assert.IsFalse(revoked.IsError, revoked.ErrorMessage);
        var replay = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials,
            rotated.Response.RefreshToken).ConfigureAwait(false);
        Assert.IsTrue(replay.IsError);
    }

    /// <summary>
    /// Ensures refresh tokens issued concurrently with revocation cannot continue the revoked session.
    /// </summary>
    [TestMethod]
    public async Task ShouldInvalidateRefreshTokensRacingWithRevocation()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        _ = await CreateUserAsync("alice").ConfigureAwait(false);
        for ( var attempt = 0; attempt < 3; attempt++ )
        {
            var token = await LoginAsync(credentials, "alice").ConfigureAwait(false);
            var refreshTask = KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials, token.RefreshToken);
            var revokeTask = KeycloakRestClient.Auth.RevokeRefreshTokenAsync(_realm, credentials, token.RefreshToken);
            await Task.WhenAll(refreshTask, revokeTask).ConfigureAwait(false);
            var refresh = await refreshTask.ConfigureAwait(false);
            var revoke = await revokeTask.ConfigureAwait(false);
            Assert.IsFalse(revoke.IsError, revoke.ErrorMessage);
            var descendant = refresh.IsError ? token.RefreshToken : refresh.Response.RefreshToken;
            var replay = await KeycloakRestClient.Auth.RefreshAccessTokenAsync(_realm, credentials, descendant)
                .ConfigureAwait(false);
            Assert.IsTrue(replay.IsError, "Revocation must invalidate descendants from a concurrent refresh.");
            Assert.AreEqual(HttpStatusCode.BadRequest, replay.MonitoringMetrics.StatusCode);
        }
    }

    /// <summary>
    /// Prevents manage-users administrators from acquiring realm-admin through privileged group membership.
    /// </summary>
    [TestMethod]
    public async Task ShouldPreventManageUsersPrivilegeEscalationThroughGroups()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        var actor = await CreateUserAsync("delegate").ConfigureAwait(false);
        await AssignAdminRoleAsync(actor, "manage-users").ConfigureAwait(false);
        var management = await GetManagementClientIdAsync().ConfigureAwait(false);
        var root = $"admin/realms/{_realm}";
        _ = await SetupAsync(HttpMethod.Post, $"{root}/groups", new { name = "privileged" }, HttpStatusCode.Created)
            .ConfigureAwait(false);
        _ = await SetupAsync(HttpMethod.Post, $"{root}/groups", new { name = "ordinary" }, HttpStatusCode.Created)
            .ConfigureAwait(false);
        var groups = await SetupAsync(HttpMethod.Get, $"{root}/groups").ConfigureAwait(false);
        var privileged = groups.Single(value => value["name"]?.Value<string>() == "privileged")["id"]?.Value<string>();
        var ordinary = groups.Single(value => value["name"]?.Value<string>() == "ordinary")["id"]?.Value<string>();
        var role = await SetupAsync(HttpMethod.Get, $"{root}/clients/{management}/roles/realm-admin")
            .ConfigureAwait(false);
        _ = await SetupAsync(HttpMethod.Post, $"{root}/groups/{privileged}/role-mappings/clients/{management}",
            new[] { role }, HttpStatusCode.NoContent).ConfigureAwait(false);
        var token = await LoginAsync(credentials, "delegate").ConfigureAwait(false);
        var normal = await KeycloakRestClient.Users.AddToGroupAsync(_realm, token.AccessToken, actor, ordinary)
            .ConfigureAwait(false);
        Assert.IsFalse(normal.IsError, normal.ErrorMessage);
        AssertDenied(await KeycloakRestClient.Users.AddToGroupAsync(_realm, token.AccessToken, actor, privileged)
            .ConfigureAwait(false));
        AssertDenied(await KeycloakRestClient.Users.CreateAsync(_realm, token.AccessToken,
            new KcUser { UserName = "injected_admin", Enabled = true, Groups = ["privileged"] })
            .ConfigureAwait(false));
        var memberships = await KeycloakRestClient.Users.UserGroupsAsync(_realm, _adminToken, actor)
            .ConfigureAwait(false);
        Assert.IsFalse(memberships.IsError, memberships.ErrorMessage);
        Assert.IsFalse(memberships.Response.Any(value => value.Id == privileged));
    }

    /// <summary>
    /// Filters role-to-group listings to groups a delegated administrator can view.
    /// </summary>
    [TestMethod]
    public async Task ShouldHideInaccessibleGroupsInRoleMemberships()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        var actor = await CreateUserAsync("delegate").ConfigureAwait(false);
        await AssignAdminRoleAsync(actor, "view-realm").ConfigureAwait(false);
        await AssignAdminRoleAsync(actor, "query-groups").ConfigureAwait(false);
        var root = $"admin/realms/{_realm}";
        foreach ( var name in new[] { "visible", "hidden" } )
        {
            _ = await SetupAsync(HttpMethod.Post, $"{root}/groups", new { name }, HttpStatusCode.Created)
                .ConfigureAwait(false);
        }
        var groups = await SetupAsync(HttpMethod.Get, $"{root}/groups").ConfigureAwait(false);
        var visible = groups.Single(value => value["name"]?.Value<string>() == "visible")["id"]?.Value<string>();
        var role = new KcRole { Name = "membership_role" };
        var created = await KeycloakRestClient.Roles.CreateAsync(_realm, _adminToken, role).ConfigureAwait(false);
        Assert.IsFalse(created.IsError, created.ErrorMessage);
        var representation = await SetupAsync(HttpMethod.Get, $"{root}/roles/{role.Name}").ConfigureAwait(false);
        foreach ( var group in groups )
        {
            _ = await SetupAsync(HttpMethod.Post, $"{root}/groups/{group["id"]}/role-mappings/realm",
                new[] { representation }, HttpStatusCode.NoContent).ConfigureAwait(false);
        }
        var management = _fgapV2
            ? (await SetupAsync(HttpMethod.Get, $"{root}/clients?clientId=admin-permissions").ConfigureAwait(false))
                .Single()["id"]?.Value<string>()
            : await GetManagementClientIdAsync().ConfigureAwait(false);
        var authz = $"{root}/clients/{management}/authz/resource-server";
        JToken permissions = null;
        if ( !_fgapV2 )
        {
            permissions = await SetupAsync(HttpMethod.Put, $"{root}/groups/{visible}/management/permissions",
                new { enabled = true }).ConfigureAwait(false);
        }
        var policy = await SetupAsync(HttpMethod.Post, $"{authz}/policy/user", new
        {
            name = "visible-group-policy",
            users = new[] { actor },
            logic = "POSITIVE"
        }, HttpStatusCode.Created).ConfigureAwait(false);
        if ( _fgapV2 )
        {
            _ = await SetupAsync(HttpMethod.Post, $"{authz}/permission/scope", new
            {
                name = "visible-group-permission",
                resourceType = "Groups",
                resources = new[] { visible },
                scopes = ViewScopes,
                policies = new[] { policy["id"]?.Value<string>() }
            }, HttpStatusCode.Created).ConfigureAwait(false);
        }
        else
        {
            var viewId = permissions["scopePermissions"]?["view"]?.Value<string>();
            var view = await SetupAsync(HttpMethod.Get, $"{authz}/permission/scope/{viewId}").ConfigureAwait(false);
            view["policies"] = new JArray(policy["id"]?.Value<string>());
            _ = await SetupAsync(HttpMethod.Put, $"{authz}/permission/scope/{viewId}", view, HttpStatusCode.Created)
                .ConfigureAwait(false);
        }
        var all = await KeycloakRestClient.Roles.GetGroupsAsync(_realm, _adminToken, role.Name).ConfigureAwait(false);
        Assert.IsFalse(all.IsError, all.ErrorMessage);
        Assert.AreEqual(2, all.Response.Count());
        var token = await LoginAsync(credentials, "delegate").ConfigureAwait(false);
        var filtered = await KeycloakRestClient.Roles.GetGroupsAsync(_realm, token.AccessToken, role.Name)
            .ConfigureAwait(false);
        Assert.IsFalse(filtered.IsError, filtered.ErrorMessage);
        CollectionAssert.AreEqual(new[] { visible }, filtered.Response.Select(value => value.Id).ToArray());
    }

    /// <summary>
    /// Denies ordinary users all credential mutation paths and verifies the password remains unchanged.
    /// </summary>
    [TestMethod]
    public async Task ShouldDenyUnprivilegedCredentialMutations()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        var userId = await CreateUserAsync("alice").ConfigureAwait(false);
        _ = await CreateUserAsync("bob").ConfigureAwait(false);
        var bob = await LoginAsync(credentials, "bob").ConfigureAwait(false);
        var stored = await KeycloakRestClient.Users.GetCredentialsAsync(_realm, _adminToken, userId)
            .ConfigureAwait(false);
        Assert.IsFalse(stored.IsError, stored.ErrorMessage);
        var password = stored.Response.Single(value => value.Type == "password");
        var deleted = await KeycloakRestClient.Users
            .DeleteCredentialsAsync(_realm, bob.AccessToken, userId, password.Id).ConfigureAwait(false);
        AssertDenied(deleted);
        var updated = await KeycloakRestClient.Users.UpdateAsync(_realm, bob.AccessToken, userId, new KcUser
        {
            Credentials = [new KcCredentials { Type = "password", Value = "Unexpected-New_2681!Password" }]
        }).ConfigureAwait(false);
        AssertDenied(updated);
        var reset = await KeycloakRestClient.Users.ResetPasswordAsync(_realm, bob.AccessToken, userId,
            new KcCredentials { Type = "password", Value = "Unexpected-New_2681!Password", Temporary = false })
            .ConfigureAwait(false);
        AssertDenied(reset);
        _ = await LoginAsync(credentials, "alice").ConfigureAwait(false);
    }

    /// <summary>
    /// Allows delegated access while preserving explicit credential denial in FGAP v2 and view-only access in v1.
    /// </summary>
    [TestMethod]
    public async Task ShouldEnforceDelegatedUserManagementScopes()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        var target = await CreateUserAsync("alice").ConfigureAwait(false);
        var actor = await CreateUserAsync("delegate").ConfigureAwait(false);
        await AssignAdminRoleAsync(actor, "query-users").ConfigureAwait(false);
        var management = _fgapV2
            ? (await SetupAsync(HttpMethod.Get, $"admin/realms/{_realm}/clients?clientId=admin-permissions")
                .ConfigureAwait(false)).Single()["id"]?.Value<string>()
            : await GetManagementClientIdAsync().ConfigureAwait(false);
        var authz = $"admin/realms/{_realm}/clients/{management}/authz/resource-server";
        JToken permissions = null;
        if ( !_fgapV2 )
        {
            permissions = await SetupAsync(HttpMethod.Put, $"admin/realms/{_realm}/users-management-permissions",
                new { enabled = true }).ConfigureAwait(false);
        }
        var policy = await SetupAsync(HttpMethod.Post, $"{authz}/policy/user", new
        {
            name = "delegate-user",
            users = new[] { actor },
            logic = "POSITIVE"
        }, HttpStatusCode.Created).ConfigureAwait(false);
        if ( _fgapV2 )
        {
            _ = await SetupAsync(HttpMethod.Post, $"{authz}/permission/scope", new
            {
                name = "delegate-profile-management",
                resourceType = "Users",
                resources = new[] { target },
                scopes = ViewAndManageScopes,
                policies = new[] { policy["id"]?.Value<string>() },
                decisionStrategy = "UNANIMOUS"
            }, HttpStatusCode.Created).ConfigureAwait(false);
            var denial = await SetupAsync(HttpMethod.Post, $"{authz}/policy/user", new
            {
                name = "deny-delegate-password",
                users = new[] { actor },
                logic = "NEGATIVE"
            }, HttpStatusCode.Created).ConfigureAwait(false);
            _ = await SetupAsync(HttpMethod.Post, $"{authz}/permission/scope", new
            {
                name = "deny-password-reset",
                resourceType = "Users",
                resources = new[] { target },
                scopes = ResetPasswordScopes,
                policies = new[] { denial["id"]?.Value<string>() },
                decisionStrategy = "UNANIMOUS"
            }, HttpStatusCode.Created).ConfigureAwait(false);
        }
        else
        {
            var viewId = permissions["scopePermissions"]?["view"]?.Value<string>();
            var view = await SetupAsync(HttpMethod.Get, $"{authz}/permission/scope/{viewId}").ConfigureAwait(false);
            view["policies"] = new JArray(policy["id"]?.Value<string>());
            _ = await SetupAsync(HttpMethod.Put, $"{authz}/permission/scope/{viewId}", view, HttpStatusCode.Created)
                .ConfigureAwait(false);
        }

        var token = await LoginAsync(credentials, "delegate").ConfigureAwait(false);
        var read = await KeycloakRestClient.Users.GetAsync(_realm, token.AccessToken, target).ConfigureAwait(false);
        Assert.IsFalse(read.IsError, read.ErrorMessage);
        var update = await KeycloakRestClient.Users.UpdateAsync(_realm, token.AccessToken, target,
            new KcUser { FirstName = "Updated" }).ConfigureAwait(false);
        if ( _fgapV2 )
        {
            Assert.IsFalse(update.IsError, update.ErrorMessage);
        }
        else
        {
            AssertDenied(update);
        }

        var stored = await KeycloakRestClient.Users.GetCredentialsAsync(_realm, _adminToken, target)
            .ConfigureAwait(false);
        Assert.IsFalse(stored.IsError, stored.ErrorMessage);
        var password = stored.Response.Single(value => value.Type == "password");
        AssertDenied(await KeycloakRestClient.Users
            .DeleteCredentialsAsync(_realm, token.AccessToken, target, password.Id).ConfigureAwait(false));
        AssertDenied(await KeycloakRestClient.Users.UpdateAsync(_realm, token.AccessToken, target,
            new KcUser { Credentials = [new KcCredentials { Type = "password", Value = "Denied-New_2681!Password" }] })
            .ConfigureAwait(false));
        AssertDenied(await KeycloakRestClient.Users.ResetPasswordAsync(_realm, token.AccessToken, target,
            new KcCredentials { Type = "password", Value = "Denied-New_2681!Password", Temporary = false })
            .ConfigureAwait(false));
        _ = await LoginAsync(credentials, "alice").ConfigureAwait(false);
    }

    /// <summary>
    /// Prevents a token issued in one realm from changing users in another realm.
    /// </summary>
    [TestMethod]
    public async Task ShouldRejectCrossRealmAdminOperations()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        _ = await CreateUserAsync("alice").ConfigureAwait(false);
        var user = await LoginAsync(credentials, "alice").ConfigureAwait(false);
        await AssignAdminRoleAsync(new JwtSecurityTokenHandler().ReadJwtToken(user.AccessToken).Subject, "realm-admin")
            .ConfigureAwait(false);
        user = await LoginAsync(credentials, "alice").ConfigureAwait(false);
        var ownRealm = await KeycloakRestClient.Users.ListUserAsync(_realm, user.AccessToken).ConfigureAwait(false);
        Assert.IsFalse(ownRealm.IsError, ownRealm.ErrorMessage);
        var otherRealm = $"netcore_2681_{Guid.NewGuid():N}";
        _ = await SetupAsync(HttpMethod.Post, "admin/realms", new { realm = otherRealm, enabled = true },
            HttpStatusCode.Created).ConfigureAwait(false);
        try
        {
            var result = await KeycloakRestClient.Users.CreateAsync(otherRealm, user.AccessToken,
                new KcUser { UserName = "cross_realm_user", Enabled = true }).ConfigureAwait(false);
            AssertDenied(result);
            var users = await KeycloakRestClient.Users.ListUserAsync(otherRealm, _adminToken).ConfigureAwait(false);
            Assert.IsFalse(users.IsError, users.ErrorMessage);
            Assert.IsFalse(users.Response.Any(value => value.UserName == "cross_realm_user"));
        }
        finally
        {
            _ = await SetupAsync(HttpMethod.Delete, $"admin/realms/{otherRealm}", null, HttpStatusCode.NoContent)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Confirms invalid tokens cannot enumerate users, clients or organizations.
    /// </summary>
    [TestMethod]
    public async Task ShouldRejectInvalidBearerTokensAcrossAdminModules()
    {
        var users = await KeycloakRestClient.Users.ListUserAsync(_realm, "invalid-token").ConfigureAwait(false);
        Assert.IsTrue(users.IsError);
        Assert.AreEqual(HttpStatusCode.Unauthorized, users.MonitoringMetrics.StatusCode);
        var clients = await KeycloakRestClient.Clients.ListAsync(_realm, "invalid-token").ConfigureAwait(false);
        Assert.IsTrue(clients.IsError);
        Assert.AreEqual(HttpStatusCode.Unauthorized, clients.MonitoringMetrics.StatusCode);
        var organizations = await KeycloakRestClient.Organizations.ListAsync(_realm, "invalid-token")
            .ConfigureAwait(false);
        Assert.IsTrue(organizations.IsError);
        Assert.AreEqual(HttpStatusCode.Unauthorized, organizations.MonitoringMetrics.StatusCode);
    }

    /// <summary>
    /// Checks authentication and least-privilege boundaries on every existing administrative module.
    /// </summary>
    /// <param name="module">The administrative module to exercise.</param>
    [TestMethod]
    [DataRow("Users")]
    [DataRow("Clients")]
    [DataRow("ClientScopes")]
    [DataRow("Groups")]
    [DataRow("Roles")]
    [DataRow("Organizations")]
    [DataRow("AttackDetection")]
    [DataRow("ClientInitialAccess")]
    [DataRow("RoleMappings")]
    [DataRow("ClientRoleMappings")]
    [DataRow("ProtocolMappers")]
    [DataRow("ScopeMappings")]
    public async Task ShouldEnforceAdminModuleTokenBoundaries(string module)
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        var userId = await CreateUserAsync("alice").ConfigureAwait(false);
        var token = await LoginAsync(credentials, "alice").ConfigureAwait(false);
        var management = await GetManagementClientIdAsync().ConfigureAwait(false);
        var scopes = await SetupAsync(HttpMethod.Get, $"admin/realms/{_realm}/client-scopes").ConfigureAwait(false);
        var scopeId = scopes.Single(value => value["name"]?.Value<string>() == "profile")["id"]?.Value<string>();
        foreach ( var accessToken in new[] { "invalid-token", token.AccessToken } )
        {
            var expected = accessToken == "invalid-token" ? HttpStatusCode.Unauthorized : HttpStatusCode.Forbidden;
            var metrics = module switch
            {
                "Users" => await AssertErrorAsync(KeycloakRestClient.Users.ListUserAsync(_realm, accessToken))
                    .ConfigureAwait(false),
                "Clients" => await AssertErrorAsync(KeycloakRestClient.Clients.ListAsync(_realm, accessToken))
                    .ConfigureAwait(false),
                "ClientScopes" => await AssertErrorAsync(KeycloakRestClient.ClientScopes.ListAsync(_realm, accessToken))
                    .ConfigureAwait(false),
                "Groups" => await AssertErrorAsync(KeycloakRestClient.Groups.ListAsync(_realm, accessToken))
                    .ConfigureAwait(false),
                "Roles" => await AssertErrorAsync(KeycloakRestClient.Roles.ListAsync(_realm, accessToken))
                    .ConfigureAwait(false),
                "Organizations" => await AssertErrorAsync(KeycloakRestClient.Organizations
                    .ListAsync(_realm, accessToken)).ConfigureAwait(false),
                "AttackDetection" => await AssertErrorAsync(KeycloakRestClient.AttackDetection
                    .GetUserStatusAsync(_realm, accessToken, userId)).ConfigureAwait(false),
                "ClientInitialAccess" => await AssertErrorAsync(KeycloakRestClient.ClientInitialAccess
                    .GetInitialAccessAsync(_realm, accessToken)).ConfigureAwait(false),
                "RoleMappings" => await AssertErrorAsync(KeycloakRestClient.RoleMappings
                    .GetUserRoleMappingsAsync(_realm, accessToken, userId)).ConfigureAwait(false),
                "ClientRoleMappings" => await AssertErrorAsync(KeycloakRestClient.ClientRoleMappings
                    .GetUserMappedClientRolesAsync(_realm, accessToken, userId, management)).ConfigureAwait(false),
                "ProtocolMappers" => await AssertErrorAsync(KeycloakRestClient.ProtocolMappers
                    .ListMappersAsync(_realm, accessToken, scopeId)).ConfigureAwait(false),
                "ScopeMappings" => await AssertErrorAsync(KeycloakRestClient.ScopeMappings
                    .ListRolesAssociatedToScopeAsync(_realm, accessToken, scopeId)).ConfigureAwait(false),
                _ => throw new ArgumentOutOfRangeException(nameof(module))
            };
            Assert.AreEqual(expected, metrics.StatusCode, $"Unexpected access to {module}.");
        }
    }

    private static async Task<KcHttpApiMonitoringMetrics> AssertErrorAsync<T>(Task<KcResponse<T>> operation)
    {
        var response = await operation.ConfigureAwait(false);
        Assert.IsTrue(response.IsError);
        Assert.IsNull(response.Response, "A denied request must not expose resource data.");
        return response.MonitoringMetrics;
    }

    /// <summary>
    /// Keeps same-named UMA resources owned by different users isolated, including multi-permission requests.
    /// </summary>
    [TestMethod]
    public async Task ShouldKeepUmaPermissionsIsolatedAcrossResourceOwners()
    {
        var credentials = await CreateClientAsync().ConfigureAwait(false);
        var alice = await CreateUserAsync("alice").ConfigureAwait(false);
        var bob = await CreateUserAsync("bob").ConfigureAwait(false);
        const string audience = "resource-server";
        _ = await SetupAsync(HttpMethod.Post, $"admin/realms/{_realm}/clients", new
        {
            clientId = audience,
            protocol = "openid-connect",
            publicClient = false,
            serviceAccountsEnabled = true,
            authorizationServicesEnabled = true
        }, HttpStatusCode.Created).ConfigureAwait(false);
        var clients = await SetupAsync(HttpMethod.Get, $"admin/realms/{_realm}/clients?clientId={audience}")
            .ConfigureAwait(false);
        var authz = $"admin/realms/{_realm}/clients/{clients.Single()["id"]}/authz/resource-server";
        var first = await SetupAsync(HttpMethod.Post, $"{authz}/resource", new
        {
            name = "shared-document",
            owner = new { id = alice },
            ownerManagedAccess = true,
            scopes = new[] { new { name = "read" } }
        }, HttpStatusCode.Created).ConfigureAwait(false);
        var second = await SetupAsync(HttpMethod.Post, $"{authz}/resource", new
        {
            name = "shared-document",
            owner = new { id = bob },
            ownerManagedAccess = true,
            scopes = new[] { new { name = "read" } }
        }, HttpStatusCode.Created).ConfigureAwait(false);
        var policy = await SetupAsync(HttpMethod.Post, $"{authz}/policy/user", new
        {
            name = "allow-alice",
            users = new[] { alice },
            logic = "POSITIVE"
        }, HttpStatusCode.Created).ConfigureAwait(false);
        var firstId = first["_id"]?.Value<string>();
        var secondId = second["_id"]?.Value<string>();
        _ = await SetupAsync(HttpMethod.Post, $"{authz}/permission/scope", new
        {
            name = "read-alice-document",
            resources = new[] { firstId },
            scopes = ReadScopes,
            policies = new[] { policy["id"]?.Value<string>() },
            decisionStrategy = "UNANIMOUS"
        }, HttpStatusCode.Created).ConfigureAwait(false);
        var token = await LoginAsync(credentials, "alice").ConfigureAwait(false);
        var allowed = await KeycloakRestClient.Auth.GetRequestPartyTokenAsync(_realm, token.AccessToken, audience,
            [$"{firstId}#read"]).ConfigureAwait(false);
        Assert.IsFalse(allowed.IsError, allowed.ErrorMessage);
        var denied = await KeycloakRestClient.Auth.GetRequestPartyTokenAsync(_realm, token.AccessToken, audience,
            [$"{secondId}#read"]).ConfigureAwait(false);
        Assert.IsTrue(denied.IsError);
        Assert.AreEqual(HttpStatusCode.Forbidden, denied.MonitoringMetrics.StatusCode);
        var multiple = await KeycloakRestClient.Auth.GetRequestPartyTokenAsync(_realm, token.AccessToken, audience,
            [$"{firstId}#read", $"{secondId}#read"]).ConfigureAwait(false);
        Assert.IsFalse(multiple.IsError, multiple.ErrorMessage);
        var byName = await KeycloakRestClient.Auth.GetRequestPartyTokenAsync(_realm, token.AccessToken, audience,
            ["shared-document#read"]).ConfigureAwait(false);
        Assert.IsFalse(byName.IsError, byName.ErrorMessage);
        foreach ( var response in new[] { multiple, byName } )
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.Response.AccessToken);
            var authorization = JObject.Parse(jwt.Claims.Single(claim => claim.Type == "authorization").Value);
            var permissions = authorization["permissions"]!.ToArray();
            Assert.IsTrue(permissions.Any(value => value["rsid"]?.Value<string>() == firstId));
            Assert.IsFalse(permissions.Any(value => value["rsid"]?.Value<string>() == secondId));
        }
    }

    /// <summary>
    /// Adds, lists, evaluates and removes client-role scope mappings on a client, not a reusable client scope.
    /// </summary>
    [TestMethod]
    public async Task ShouldRoundTripClientRoleScopeMappings()
    {
        var target = await CreateRoleClientAsync().ConfigureAwait(false);
        var container = await CreateRoleClientAsync().ConfigureAwait(false);
        var role = await CreateClientRoleAsync(container, "scoped_role").ConfigureAwait(false);
        var available = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListClientRolesAvailableForClientScopeAsync(_realm, _adminToken, target, container).ConfigureAwait(false));
        Assert.IsTrue(available.Any(value => value.Id == role.Id));
        _ = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .AddClientRolesToClientScopeAsync(_realm, _adminToken, target, container, [role]).ConfigureAwait(false),
            HttpStatusCode.NoContent);
        var mapped = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListClientRolesAssociatedToClientScopeAsync(_realm, _adminToken, target, container).ConfigureAwait(false));
        Assert.IsTrue(mapped.Any(value => value.Id == role.Id));
        var effective = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListCompositeClientRolesAssociatedToClientScopeAsync(_realm, _adminToken, target, container)
            .ConfigureAwait(false));
        Assert.IsTrue(effective.Any(value => value.Id == role.Id));
        _ = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .RemoveClientRolesFromClientScopeAsync(_realm, _adminToken, target, container, [role])
            .ConfigureAwait(false),
            HttpStatusCode.NoContent);
        var removed = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListClientRolesAssociatedToClientScopeAsync(_realm, _adminToken, target, container).ConfigureAwait(false));
        Assert.IsFalse(removed.Any(value => value.Id == role.Id));
    }

    /// <summary>
    /// Adds, lists, evaluates and removes realm-role scope mappings on a client.
    /// </summary>
    [TestMethod]
    public async Task ShouldRoundTripRealmRoleClientScopeMappings()
    {
        var target = await CreateRoleClientAsync().ConfigureAwait(false);
        var role = new KcRole { Name = "scoped_realm_role" };
        _ = AssertSucceeded(await KeycloakRestClient.Roles.CreateAsync(_realm, _adminToken, role)
            .ConfigureAwait(false), HttpStatusCode.Created);
        role = (await SetupAsync(HttpMethod.Get, $"admin/realms/{_realm}/roles/{role.Name}").ConfigureAwait(false))
            .ToObject<KcRole>();
        var available = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListRolesAvailableForClientScopeAsync(_realm, _adminToken, target).ConfigureAwait(false));
        Assert.IsTrue(available.Any(value => value.Id == role.Id));
        _ = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .AddRolesToScopeClientAsync(_realm, _adminToken, target, [role]).ConfigureAwait(false),
            HttpStatusCode.NoContent);
        var mapped = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListRolesAssociatedToClientScopeAsync(_realm, _adminToken, target).ConfigureAwait(false));
        Assert.IsTrue(mapped.Any(value => value.Id == role.Id));
        var effective = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListCompositeRolesAssociatedToClientScopeAsync(_realm, _adminToken, target).ConfigureAwait(false));
        Assert.IsTrue(effective.Any(value => value.Id == role.Id));
        _ = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .RemoveRolesFromClientScopeAsync(_realm, _adminToken, target, [role]).ConfigureAwait(false),
            HttpStatusCode.NoContent);
        var removed = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .ListRolesAssociatedToClientScopeAsync(_realm, _adminToken, target).ConfigureAwait(false));
        Assert.IsFalse(removed.Any(value => value.Id == role.Id));
    }

    /// <summary>
    /// Distinguishes granted and ungranted roles when evaluating a client's role container.
    /// </summary>
    [TestMethod]
    public async Task ShouldEvaluateGrantedAndUngrantedRoleContainers()
    {
        var target = await CreateRoleClientAsync().ConfigureAwait(false);
        var container = await CreateRoleClientAsync().ConfigureAwait(false);
        var granted = await CreateClientRoleAsync(container, "granted_role").ConfigureAwait(false);
        var ungranted = await CreateClientRoleAsync(container, "ungranted_role").ConfigureAwait(false);
        _ = AssertSucceeded(await KeycloakRestClient.ScopeMappings
            .AddClientRolesToClientScopeAsync(_realm, _adminToken, target, container, [granted]).ConfigureAwait(false),
            HttpStatusCode.NoContent);
        var included = AssertSucceeded(await KeycloakRestClient.Clients
            .GetScopedProtocolMappersInContainerAsync(_realm, _adminToken, target, container, scope: "openid profile")
            .ConfigureAwait(false)).ToList();
        Assert.IsTrue(included.Any(value => value.Id == granted.Id));
        Assert.IsFalse(included.Any(value => value.Id == ungranted.Id));
        var excluded = AssertSucceeded(await KeycloakRestClient.Clients
            .GetUnScopedProtocolMappersInContainerAsync(_realm, _adminToken, target, container)
            .ConfigureAwait(false)).ToList();
        Assert.IsTrue(excluded.Any(value => value.Id == ungranted.Id));
        Assert.IsFalse(excluded.Any(value => value.Id == granted.Id));
    }

    /// <summary>
    /// Persists node registration and deletion on a dedicated client without contacting external adapters.
    /// </summary>
    [TestMethod]
    public async Task ShouldRegisterAndDeleteClientNodes()
    {
        var clientId = await CreateRoleClientAsync().ConfigureAwait(false);
        _ = AssertSucceeded(await KeycloakRestClient.Clients.RegisterNodeAsync(_realm, _adminToken, clientId,
            new Dictionary<string, object> { ["node"] = "127.0.0.1" }).ConfigureAwait(false), HttpStatusCode.NoContent);
        var client = AssertSucceeded(await KeycloakRestClient.Clients.GetAsync(_realm, _adminToken, clientId)
            .ConfigureAwait(false));
        Assert.IsNotNull(client.RegisteredNodes);
        Assert.IsTrue(JObject.FromObject(client.RegisteredNodes).ContainsKey("127.0.0.1"));
        _ = AssertSucceeded(await KeycloakRestClient.Clients.DeleteNodeAsync(_realm, _adminToken, clientId, "127.0.0.1")
            .ConfigureAwait(false), HttpStatusCode.NoContent);
        client = AssertSucceeded(await KeycloakRestClient.Clients.GetAsync(_realm, _adminToken, clientId)
            .ConfigureAwait(false));
        Assert.IsTrue(client.RegisteredNodes == null ||
            !JObject.FromObject(client.RegisteredNodes).ContainsKey("127.0.0.1"));
    }

    /// <summary>
    /// Processes empty adapter topologies for node checks and revocation propagation.
    /// </summary>
    [TestMethod]
    public async Task ShouldTestNodesAndPushRevocationWithoutAdapters()
    {
        var clientId = await CreateRoleClientAsync().ConfigureAwait(false);
        var nodes = AssertSucceeded(await KeycloakRestClient.Clients
            .TestAvailableNodesAsync(_realm, _adminToken, clientId).ConfigureAwait(false));
        Assert.IsNotNull(nodes);
        Assert.IsFalse(nodes.FailedRequests?.Any() == true);
        var revocation = AssertSucceeded(await KeycloakRestClient.Clients
            .PushRevocationAsync(_realm, _adminToken, clientId).ConfigureAwait(false));
        Assert.IsNotNull(revocation);
        Assert.IsFalse(revocation.FailedRequests?.Any() == true);
    }

    /// <summary>
    /// Removes all composites through the existing parameterless removal API and verifies server state.
    /// </summary>
    [TestMethod]
    public async Task ShouldRemoveClientRoleComposites()
    {
        var client = await CreateRoleClientAsync().ConfigureAwait(false);
        var composite = await CreateClientRoleAsync(client, "composite_role").ConfigureAwait(false);
        var member = await CreateClientRoleAsync(client, "member_role").ConfigureAwait(false);
        _ = AssertSucceeded(await KeycloakRestClient.Roles.AddClientRoleToCompositeAsync(_realm, _adminToken, client,
            composite.Name, [member]).ConfigureAwait(false), HttpStatusCode.NoContent);
        var before = AssertSucceeded(await KeycloakRestClient.Roles
            .GetClientCompositeRolesAsync(_realm, _adminToken, client, composite.Name).ConfigureAwait(false));
        Assert.IsTrue(before.Any(value => value.Id == member.Id));
        _ = AssertSucceeded(await KeycloakRestClient.Roles
            .RemoveClientRoleFromCompositeAsync(_realm, _adminToken, client, composite.Name).ConfigureAwait(false),
            HttpStatusCode.NoContent);
        var after = AssertSucceeded(await KeycloakRestClient.Roles
            .GetClientCompositeRolesAsync(_realm, _adminToken, client, composite.Name).ConfigureAwait(false));
        Assert.IsFalse(after.Any());
        _ = AssertSucceeded(await KeycloakRestClient.Roles
            .RemoveClientRoleFromCompositeAsync(_realm, _adminToken, client, composite.Name).ConfigureAwait(false),
            HttpStatusCode.NoContent);
        var denied = await KeycloakRestClient.Roles.RemoveClientRoleFromCompositeAsync(_realm, "invalid-token", client,
            composite.Name).ConfigureAwait(false);
        Assert.IsTrue(denied.IsError);
        Assert.AreEqual(HttpStatusCode.Unauthorized, denied.MonitoringMetrics.StatusCode);
    }

    /// <summary>
    /// Round-trips the legacy FGAP v1 management-permission APIs for groups and realm/client roles.
    /// </summary>
    [TestMethod]
    [TestCategory("LegacyAdminPermissions")]
    public async Task ShouldRoundTripLegacyManagementPermissions()
    {
        var root = $"admin/realms/{_realm}";
        _ = await SetupAsync(HttpMethod.Post, $"{root}/groups", new { name = "managed_group" }, HttpStatusCode.Created)
            .ConfigureAwait(false);
        var groups = await SetupAsync(HttpMethod.Get, $"{root}/groups").ConfigureAwait(false);
        var group = groups.Single()["id"]?.Value<string>();
        var enabled = new KcPermissionManagement { Enabled = true };
        var disabled = new KcPermissionManagement { Enabled = false };
        var groupPermission = AssertSucceeded(await KeycloakRestClient.Groups.SetAuthorizationManagementPermissionAsync(
            _realm, _adminToken, group, enabled).ConfigureAwait(false));
        Assert.IsTrue(groupPermission.Enabled);
        var groupRead = AssertSucceeded(await KeycloakRestClient.Groups.GetAuthorizationManagementPermissionAsync(
            _realm, _adminToken, group).ConfigureAwait(false));
        Assert.IsTrue(groupRead.Enabled);
        _ = AssertSucceeded(await KeycloakRestClient.Groups.SetAuthorizationManagementPermissionAsync(
            _realm, _adminToken, group, disabled).ConfigureAwait(false));
        var role = new KcRole { Name = "managed_role" };
        _ = AssertSucceeded(await KeycloakRestClient.Roles.CreateAsync(_realm, _adminToken, role).ConfigureAwait(false),
            HttpStatusCode.Created);
        role = (await SetupAsync(HttpMethod.Get, $"{root}/roles/{role.Name}").ConfigureAwait(false)).ToObject<KcRole>();
        Assert.IsTrue(AssertSucceeded(await KeycloakRestClient.Roles.SetAuthorizationPermissionsAsync(
            _realm, _adminToken, role.Name, enabled).ConfigureAwait(false)).Enabled);
        Assert.IsFalse(AssertSucceeded(await KeycloakRestClient.Roles.SetAuthorizationPermissionsByIdAsync(
            _realm, _adminToken, role.Id, disabled).ConfigureAwait(false)).Enabled);
        var client = await CreateRoleClientAsync().ConfigureAwait(false);
        var clientRole = await CreateClientRoleAsync(client, "managed_client_role").ConfigureAwait(false);
        Assert.IsTrue(AssertSucceeded(await KeycloakRestClient.Roles.SetClientRoleAuthorizationPermissionsAsync(
            _realm, _adminToken, client, clientRole.Name, enabled).ConfigureAwait(false)).Enabled);
        var read = AssertSucceeded(await KeycloakRestClient.Roles.GetClientRoleAuthorizationPermissionsAsync(
            _realm, _adminToken, client, clientRole.Name).ConfigureAwait(false));
        Assert.IsTrue(read.Enabled);
        _ = AssertSucceeded(await KeycloakRestClient.Roles.SetClientRoleAuthorizationPermissionsAsync(
            _realm, _adminToken, client, clientRole.Name, disabled).ConfigureAwait(false));
    }

    private static T AssertSucceeded<T>(KcResponse<T> response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.IsFalse(response.IsError, response.ErrorMessage);
        Assert.AreEqual(expected, response.MonitoringMetrics.StatusCode);
        return response.Response;
    }

    private async Task<string> CreateRoleClientAsync()
    {
        var name = $"roles_{Guid.NewGuid():N}";
        _ = await SetupAsync(HttpMethod.Post, $"admin/realms/{_realm}/clients", new
        {
            clientId = name,
            protocol = "openid-connect",
            publicClient = false,
            fullScopeAllowed = false
        }, HttpStatusCode.Created).ConfigureAwait(false);
        var clients = await SetupAsync(HttpMethod.Get, $"admin/realms/{_realm}/clients?clientId={name}")
            .ConfigureAwait(false);
        return clients.Single()["id"]?.Value<string>();
    }

    private async Task<KcRole> CreateClientRoleAsync(string clientId, string name)
    {
        _ = AssertSucceeded(await KeycloakRestClient.Roles.CreateClientRoleAsync(_realm, _adminToken, clientId,
            new KcRole { Name = name }).ConfigureAwait(false), HttpStatusCode.Created);
        return AssertSucceeded(await KeycloakRestClient.Roles.GetClientRolesAsync(_realm, _adminToken, clientId, name)
            .ConfigureAwait(false));
    }

    private async Task<KcClientCredentials> CreateClientAsync(bool basic = false)
    {
        var clientId = $"client_{Guid.NewGuid():N}";
        // OIDC registration is used so Keycloak persists token_endpoint_auth_method exactly as specified.
        var initial = await KeycloakRestClient.ClientInitialAccess.CreateInitialAccessTokenAsync(_realm, _adminToken,
            new KcCreateClientInitialAccess { Count = 1, Expiration = 60 }).ConfigureAwait(false);
        Assert.IsFalse(initial.IsError, initial.ErrorMessage);
        var registered = await SetupAsync(HttpMethod.Post, $"realms/{_realm}/clients-registrations/openid-connect", new
        {
            client_name = clientId,
            redirect_uris = RedirectUris,
            grant_types = GrantTypes,
            token_endpoint_auth_method = basic ? "client_secret_basic" : "client_secret_post"
        }, HttpStatusCode.Created, initial.Response.Token).ConfigureAwait(false);
        return new KcClientCredentials
        {
            ClientId = registered["client_id"]?.Value<string>(),
            Secret = registered["client_secret"]?.Value<string>(),
            AuthenticationMethod = basic
                ? KcClientAuthenticationMethod.ClientSecretBasic : KcClientAuthenticationMethod.ClientSecretPost
        };
    }

    private async Task<string> CreateUserAsync(string username)
    {
        var response = await KeycloakRestClient.Users.CreateAsync(_realm, _adminToken, new KcUser
        {
            UserName = username,
            Enabled = true,
            FirstName = "Test",
            LastName = "User",
            Email = $"{username}@example.test",
            EmailVerified = true,
            Credentials = [new KcCredentials { Type = "password", Value = Password, Temporary = false }]
        }).ConfigureAwait(false);
        Assert.IsFalse(response.IsError, response.ErrorMessage);
        var users = await KeycloakRestClient.Users.ListUserAsync(_realm, _adminToken).ConfigureAwait(false);
        Assert.IsFalse(users.IsError, users.ErrorMessage);
        return users.Response.Single(value => value.UserName == username).Id;
    }

    private async Task<KcIdentityProviderToken> LoginAsync(KcClientCredentials credentials, string username)
    {
        var token = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(_realm, credentials,
            new KcUserLogin { Username = username, Password = Password }).ConfigureAwait(false);
        Assert.IsFalse(token.IsError, token.ErrorMessage);
        return token.Response;
    }

    private async Task<string> GetManagementClientIdAsync()
    {
        var clients = await SetupAsync(HttpMethod.Get, $"admin/realms/{_realm}/clients?clientId=realm-management")
            .ConfigureAwait(false);
        return clients.Single()["id"]?.Value<string>();
    }

    private async Task AssignAdminRoleAsync(string userId, string role)
    {
        var management = await GetManagementClientIdAsync().ConfigureAwait(false);
        var representation = await SetupAsync(HttpMethod.Get,
            $"admin/realms/{_realm}/clients/{management}/roles/{role}").ConfigureAwait(false);
        _ = await SetupAsync(HttpMethod.Post,
            $"admin/realms/{_realm}/users/{userId}/role-mappings/clients/{management}",
            new[] { representation }, HttpStatusCode.NoContent).ConfigureAwait(false);
    }

    private static void AssertDenied(KcResponse<object> response)
    {
        Assert.IsTrue(response.IsError);
        Assert.AreEqual(HttpStatusCode.Forbidden, response.MonitoringMetrics.StatusCode);
    }

    private async Task<JToken> SetupAsync(HttpMethod method, string path, object body = null,
        HttpStatusCode expected = HttpStatusCode.OK, string accessToken = null)
    {
        using var request = new HttpRequestMessage(method, $"{TestEnvironment.BaseUrl.TrimEnd('/')}/{path}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken ?? _adminToken);
        if ( body != null )
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        }
        using var response = await _setupClient.SendAsync(request).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var result = string.IsNullOrWhiteSpace(content) ? null : JToken.Parse(content);
        Assert.AreEqual(expected, response.StatusCode,
            $"Fixture request failed: {method} {path}; {result?["error_description"]?.Value<string>()}");
        return result;
    }
}
