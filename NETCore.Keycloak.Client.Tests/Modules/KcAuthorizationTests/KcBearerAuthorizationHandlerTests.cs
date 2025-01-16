using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.Authorization.Handlers;
using NETCore.Keycloak.Client.Authorization.Requirements;
using NETCore.Keycloak.Client.Authorization.Store;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Tests.Abstraction;
using NETCore.Keycloak.Client.Tests.MockData;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Tests for <see cref="KcBearerAuthorizationHandler"/>, which is responsible for validating
/// user sessions and permissions for accessing protected resources.
/// </summary>
[TestClass]
[TestCategory("Final")]
public class KcBearerAuthorizationHandlerTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// Used for consistent naming and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = $"{nameof(KcBearerAuthorizationHandlerTests)}";

    /// <summary>
    /// Mock service provider for resolving dependencies such as logging services.
    /// </summary>
    private Mock<IServiceProvider> _mockProvider;

    /// <summary>
    /// Mock for <see cref="IHttpContextAccessor"/>, used to simulate HTTP context in tests.
    /// </summary>
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    /// <summary>
    /// Instance of <see cref="KcBearerAuthorizationHandler"/> under test.
    /// </summary>
    private KcTestableBearerAuthorizationHandler _handler;

    /// <summary>
    /// Represents the password for the test Keycloak user.
    /// </summary>
    private static string TestUserPassword
    {
        // Retrieve the test user's password from the environment variable.
        get => Environment.GetEnvironmentVariable("KCUSER_PASSWORD");

        // Store the test user's password in the environment variable.
        set => Environment.SetEnvironmentVariable("KCUSER_PASSWORD", value);
    }

    /// <summary>
    /// Gets or sets the Keycloak authorized user used for testing.
    /// </summary>
    private static KcUser TestAuthorizedUser
    {
        get
        {
            try
            {
                // Retrieve and deserialize the user object from the environment variable.
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcBearerAuthorizationHandlerTests)}_AUTHORIZED_KCUSER") ??
                    string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcBearerAuthorizationHandlerTests)}_AUTHORIZED_KCUSER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Gets or sets the Keycloak unauthorized user used for testing.
    /// </summary>
    private static KcUser TestUnAuthorizedUser
    {
        get
        {
            try
            {
                // Retrieve and deserialize the user object from the environment variable.
                return JsonConvert.DeserializeObject<KcUser>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcBearerAuthorizationHandlerTests)}_UNAUTHORIZED_KCUSER") ?? string.Empty
                );
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null;
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcBearerAuthorizationHandlerTests)}_UNAUTHORIZED_KCUSER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Initializes the test environment for <see cref="KcBearerAuthorizationHandlerTests"/>.
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        Assert.IsNotNull(KeycloakRestClient.Auth);
        Assert.IsNotNull(KeycloakRestClient.Users);
        Assert.IsNotNull(KeycloakRestClient.RoleMappings);

        // Initialize mock dependencies for the test environment
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        // Set up a mock service scope and factory
        var mockScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockProvider = new Mock<IServiceProvider>();

        // Configure the mock service provider to resolve the IServiceScopeFactory
        _ = _mockProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockScopeFactory.Object);

        // Configure the mock IServiceScopeFactory to return a mock scope
        _ = mockScopeFactory.Setup(factory => factory.CreateScope())
            .Returns(mockScope.Object);

        // Set up a mock logger to simulate logging functionality
        var mockLogger = new Mock<ILogger>();
        _ = mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Configure the mock scope to resolve services
        _ = mockScope.Setup(x => x.ServiceProvider).Returns(_mockProvider.Object);

        // Mock resolution of ILogger from the service provider
        _ = _mockProvider.Setup(x => x.GetService(typeof(ILogger)))
            .Returns(mockLogger.Object);

        // Mock resolution of IHttpContextAccessor from the service provider
        _ = _mockProvider.Setup(sp => sp.GetService(typeof(IHttpContextAccessor)))
            .Returns(_mockHttpContextAccessor.Object);

        // Mock resolution of IKcRealmAdminTokenHandler from the service provider
        _ = _mockProvider.Setup(sp => sp.GetService(typeof(IKcRealmAdminTokenHandler)))
            .Returns(SetUpMockRealmAdminTokenHandler);
    }

    /// <summary>
    /// Tests the creation of an instance of <see cref="KcTestableBearerAuthorizationHandler"/>.
    /// </summary>
    [TestMethod]
    public void A_ShouldCreateInstance()
    {
        // Set up a mock HTTP context with an authorization header
        SetUpMockHttpContextAccessor("Bearer test");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Assert that the handler instance is successfully created
        Assert.IsNotNull(_handler);
    }

    /// <summary>
    /// Validates the behavior of <see cref="KcTestableBearerAuthorizationHandler"/> when processing
    /// an <see cref="AuthorizationHandlerContext"/>.
    /// </summary>
    [TestMethod]
    public async Task B_ShouldValidateAuthorizationHandlerContext()
    {
        // Set up a mock HTTP context with an authorization header
        SetUpMockHttpContextAccessor("Bearer test");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Verify that the handler instance is successfully created
        Assert.IsNotNull(_handler);

        // Verify that TestHandleRequirementAsync throws ArgumentNullException for null arguments
        _ = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _handler.TestHandleRequirementAsync(null, null).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Validates the behavior of <see cref="KcTestableBearerAuthorizationHandler"/> when processing
    /// a <see cref="KcAuthorizationRequirement"/>.
    /// </summary>
    [TestMethod]
    public async Task C_ShouldValidateKcAuthorizationRequirement()
    {
        // Set up a mock HTTP context with an authorization header
        SetUpMockHttpContextAccessor("Bearer test");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Verify that the handler instance is successfully created
        Assert.IsNotNull(_handler);

        // Create a mock AuthorizationHandlerContext with a mock JWT token and authorization requirement
        var mockAuthorizationHandlerContext = CreateAuthorizationHandlerContext(
            KcJwtTokenMock.CreateMockJwtToken(),
            SetUpKcAuthorizationRequirement()
        );

        // Verify that TestHandleRequirementAsync throws ArgumentNullException when the requirement is null
        _ = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                await _handler.TestHandleRequirementAsync(mockAuthorizationHandlerContext, null).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies the creation of both authorized and unauthorized users by assigning attributes
    /// and invoking the user creation logic asynchronously.
    /// </summary>
    [TestMethod]
    public async Task D_CreateUsers()
    {
        // Generate a test password for the user creation process.
        TestUserPassword = KcTestPasswordCreator.Create();

        // Define user attributes for the authorized user.
        var attributes = new Dictionary<string, object>
        {
            {
                "account_owner", 1
            },
            {
                "business_account_owner", 1
            }
        };

        // Create an authorized user with specified attributes and store the result.
        TestAuthorizedUser = await CreateAndGetRealmUserAsync(TestContext, TestUserPassword, attributes)
            .ConfigureAwait(false);

        // Create an unauthorized user without additional attributes and store the result.
        TestUnAuthorizedUser = await CreateAndGetRealmUserAsync(TestContext, TestUserPassword)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that realm roles can be assigned to an authorized user.
    /// </summary>
    [TestMethod]
    public async Task E_ShouldAssignRealmRolesToAuthorizedUser()
    {
        // Retrieve the realm administrator access token.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);

        // Validate that the access token is retrieved successfully.
        Assert.IsNotNull(accessToken);

        // Send a request to fetch the available realm roles for the unauthorized user.
        var getUserAvailableRolesResponse = await KeycloakRestClient.RoleMappings
            .ListUserAvailableRealmRoleMappingsAsync(
                TestEnvironment.TestingRealm.Name,
                accessToken.AccessToken,
                TestUnAuthorizedUser.Id)
            .ConfigureAwait(false);

        // Validate that the response is not null and does not indicate an error.
        Assert.IsNotNull(getUserAvailableRolesResponse);
        Assert.IsFalse(getUserAvailableRolesResponse.IsError);

        // Validate that the response contains a list of available realm roles.
        Assert.IsNotNull(getUserAvailableRolesResponse.Response);
        Assert.IsTrue(getUserAvailableRolesResponse.Response.Any());

        // Validate monitoring metrics for the role listing request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(getUserAvailableRolesResponse.MonitoringMetrics,
            HttpStatusCode.OK, HttpMethod.Get);

        // Filter and identify the specific role to be assigned.
        var roles = getUserAvailableRolesResponse.Response
            .Where(role => role.Name == "kc_client_role_1")
            .ToList();

        // Send a request to map the identified realm role to the authorized user.
        var mapRolesToUserResponse = await KeycloakRestClient.RoleMappings.AddUserRealmRoleMappingsAsync(
                TestEnvironment.TestingRealm.Name,
                accessToken.AccessToken,
                TestAuthorizedUser.Id,
                roles)
            .ConfigureAwait(false);

        // Validate that the role mapping response is not null and does not indicate an error.
        Assert.IsNotNull(mapRolesToUserResponse);
        Assert.IsFalse(mapRolesToUserResponse.IsError);

        // Validate that the response content is null, as expected for this type of request.
        Assert.IsNull(mapRolesToUserResponse.Response);

        // Validate monitoring metrics for the role mapping request.
        KcCommonAssertion.AssertResponseMonitoringMetrics(mapRolesToUserResponse.MonitoringMetrics,
            HttpStatusCode.NoContent, HttpMethod.Post);
    }

    /// <summary>
    /// Validates that access is denied and a <see cref="KcUserNotFoundException"/> is thrown
    /// when the user cannot be found during authorization handling.
    /// </summary>
    [TestMethod]
    public async Task FA_ShouldDenyAccessWithKcUserNotFoundException()
    {
        // Retrieve an access token using resource owner password credentials
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestUnAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Set up the authorization requirement
        var requirement = SetUpKcAuthorizationRequirement();

        // Create an AuthorizationHandlerContext with the retrieved access token
        var context = CreateAuthorizationHandlerContext(tokenResponse.Response.AccessToken, requirement, false);

        // Set up a mock HTTP context with the retrieved access token
        SetUpMockHttpContextAccessor($"Bearer {tokenResponse.Response.AccessToken}");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Verify that TestHandleRequirementAsync throws KcUserNotFoundException for the given context and requirement
        _ = await Assert.ThrowsExceptionAsync<KcUserNotFoundException>(async () =>
                await _handler.TestHandleRequirementAsync(context, requirement).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Validates that access is denied and a <see cref="KcUserNotFoundException"/> is thrown
    /// when the user cannot be found during authorization handling with additional context parameters.
    /// </summary>
    [TestMethod]
    public async Task FB_ShouldDenyAccessWithKcUserNotFoundException()
    {
        // Retrieve an access token using resource owner password credentials
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestUnAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Set up the authorization requirement
        var requirement = SetUpKcAuthorizationRequirement();

        // Create an AuthorizationHandlerContext with the retrieved access token and additional parameters
        var context = CreateAuthorizationHandlerContext(
            tokenResponse.Response.AccessToken,
            requirement,
            mapSubjectClaim: true,
            removeSessionIdClaim: false,
            mockSubject: Guid.NewGuid().ToString()
        );

        // Set up a mock HTTP context with the retrieved access token
        SetUpMockHttpContextAccessor($"Bearer {tokenResponse.Response.AccessToken}");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Verify that TestHandleRequirementAsync throws KcUserNotFoundException for the given context and requirement
        _ = await Assert.ThrowsExceptionAsync<KcUserNotFoundException>(async () =>
                await _handler.TestHandleRequirementAsync(context, requirement).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Validates that access is denied and a <see cref="KcSessionClosedException"/> is thrown
    /// when the user's session is closed during the authorization handling process.
    /// </summary>
    [TestMethod]
    public async Task GA_ShouldDenyAccessWithKcSessionClosedException()
    {
        // Retrieve an access token using resource owner password credentials
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestUnAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Set up the authorization requirement
        var requirement = SetUpKcAuthorizationRequirement();

        // Create an AuthorizationHandlerContext with the retrieved access token and specific parameters
        var context = CreateAuthorizationHandlerContext(
            tokenResponse.Response.AccessToken,
            requirement,
            mapSubjectClaim: true,
            removeSessionIdClaim: true
        );

        // Set up a mock HTTP context with the retrieved access token
        SetUpMockHttpContextAccessor($"Bearer {tokenResponse.Response.AccessToken}");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Verify that TestHandleRequirementAsync throws KcSessionClosedException for the given context and requirement
        _ = await Assert.ThrowsExceptionAsync<KcSessionClosedException>(async () =>
                await _handler.TestHandleRequirementAsync(context, requirement).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Validates that access is denied and a <see cref="KcSessionClosedException"/> is thrown
    /// when the user's session is closed during authorization handling with additional mock data for subject and session ID.
    /// </summary>
    [TestMethod]
    public async Task GB_ShouldDenyAccessWithKcSessionClosedException()
    {
        // Retrieve an access token using resource owner password credentials
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestUnAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Set up the authorization requirement
        var requirement = SetUpKcAuthorizationRequirement();

        // Create an AuthorizationHandlerContext with the retrieved access token, additional parameters, and mock data
        var context = CreateAuthorizationHandlerContext(
            tokenResponse.Response.AccessToken,
            requirement,
            mapSubjectClaim: true,
            removeSessionIdClaim: false,
            mockSubject: null,
            mockSessionId: Guid.NewGuid().ToString()
        );

        // Set up a mock HTTP context with the retrieved access token
        SetUpMockHttpContextAccessor($"Bearer {tokenResponse.Response.AccessToken}");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Verify that TestHandleRequirementAsync throws KcSessionClosedException for the given context and requirement
        _ = await Assert.ThrowsExceptionAsync<KcSessionClosedException>(async () =>
                await _handler.TestHandleRequirementAsync(context, requirement).ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Validates that access is denied for an unauthorized user during the authorization handling process.
    /// </summary>
    [TestMethod]
    public async Task H_ShouldDenyAccessForUnAuthorizedUser()
    {
        // Retrieve an access token using resource owner password credentials for an unauthorized user
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestUnAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Set up the authorization requirement
        var requirement = SetUpKcAuthorizationRequirement();

        // Create an AuthorizationHandlerContext with the retrieved access token
        var context = CreateAuthorizationHandlerContext(
            tokenResponse.Response.AccessToken,
            requirement
        );

        // Set up a mock HTTP context with the retrieved access token
        SetUpMockHttpContextAccessor($"Bearer {tokenResponse.Response.AccessToken}");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Process the authorization context
        await _handler.TestHandleRequirementAsync(context, requirement).ConfigureAwait(false);

        // Verify that the authorization context has not succeeded
        Assert.IsFalse(context.HasSucceeded);
    }

    /// <summary>
    /// Validates that access is allowed for an authorized user during the authorization handling process.
    /// </summary>
    [TestMethod]
    public async Task I_ShouldAllowAccessForAuthorizedUser()
    {
        // Retrieve an access token using resource owner password credentials for an authorized user
        var tokenResponse = await KeycloakRestClient.Auth.GetResourceOwnerPasswordTokenAsync(
            TestEnvironment.TestingRealm.Name,
            new KcClientCredentials
            {
                ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId
            },
            new KcUserLogin
            {
                Username = TestAuthorizedUser.UserName,
                Password = TestUserPassword
            }).ConfigureAwait(false);

        // Set up the authorization requirement
        var requirement = SetUpKcAuthorizationRequirement();

        // Create an AuthorizationHandlerContext with the retrieved access token
        var context = CreateAuthorizationHandlerContext(
            tokenResponse.Response.AccessToken,
            requirement
        );

        // Set up a mock HTTP context with the retrieved access token
        SetUpMockHttpContextAccessor($"Bearer {tokenResponse.Response.AccessToken}");

        // Initialize the KcAuthorizationHandler using the mocked service provider
        _handler = new KcTestableBearerAuthorizationHandler(_mockProvider.Object);

        // Process the authorization context
        await _handler.TestHandleRequirementAsync(context, requirement).ConfigureAwait(false);

        // Verify that the authorization context has succeeded
        Assert.IsTrue(context.HasSucceeded);
    }

    /// <summary>
    /// Verifies that test users are successfully deleted from the Keycloak realm.
    /// </summary>
    [TestMethod]
    public async Task Z_ShouldDeleteTestUsers()
    {
        // Ensure that both test user instances are not null.
        Assert.IsNotNull(TestAuthorizedUser, "TestAuthorizedUser must not be null.");
        Assert.IsNotNull(TestUnAuthorizedUser, "TestUnAuthorizedUser must not be null.");

        // Create a list of users to be deleted.
        var usersList = new List<KcUser>
        {
            TestAuthorizedUser,
            TestUnAuthorizedUser
        };

        // Retrieve an access token for the realm admin to perform the user deletion.
        var accessToken = await GetRealmAdminTokenAsync(TestContext).ConfigureAwait(false);
        Assert.IsNotNull(accessToken, "Access token for realm admin must not be null.");

        // Iterate over the user list and delete each user.
        foreach ( var user in usersList )
        {
            // Execute the user deletion operation using the Keycloak REST client.
            var deleteUserResponse = await KeycloakRestClient.Users
                .DeleteAsync(TestEnvironment.TestingRealm.Name, accessToken.AccessToken, user.Id)
                .ConfigureAwait(false);

            // Validate the deletion response.
            Assert.IsNotNull(deleteUserResponse, $"Delete response for user {user.Id} must not be null.");
            Assert.IsFalse(deleteUserResponse.IsError,
                $"Delete request for user {user.Id} should not return an error.");

            // Validate the monitoring metrics for the deletion request.
            KcCommonAssertion.AssertResponseMonitoringMetrics(deleteUserResponse.MonitoringMetrics,
                HttpStatusCode.NoContent, HttpMethod.Delete);
        }
    }

    /// <summary>
    /// Configures the mock <see cref="IHttpContextAccessor"/> with a specified authorization header.
    /// </summary>
    private void SetUpMockHttpContextAccessor(string authorization)
    {
        // Create a mock HTTP context
        var mockHttpContext = new DefaultHttpContext();

        // Assign the specified authorization header to the mock HTTP request
        mockHttpContext.Request.Headers.Authorization = authorization;

        // Configure the mock IHttpContextAccessor to return the mock HTTP context
        _ = _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext);
    }

    /// <summary>
    /// Creates an <see cref="AuthorizationHandlerContext"/> instance for testing purposes using a JWT token.
    /// </summary>
    /// <param name="jwtToken">The JWT token containing the claims to be used in the authorization context.</param>
    /// <param name="requirement">The <see cref="KcAuthorizationRequirement"/> that defines the resource and scope to be authorized.</param>
    /// <param name="mapSubjectClaim">
    /// Indicates whether the subject claim ("sub") from the JWT should be mapped to a <see cref="ClaimTypes.NameIdentifier"/> claim.
    /// Defaults to <c>true</c>.
    /// </param>
    /// <param name="removeSessionIdClaim">
    /// Indicates whether the session ID claim ("sid") should be removed from the claims. Defaults to <c>false</c>.
    /// </param>
    /// <param name="mockSubject">
    /// An optional mocked subject value to override the subject claim in the JWT. If specified, this value will be added as
    /// a <see cref="ClaimTypes.NameIdentifier"/> claim.
    /// </param>
    /// <param name="mockSessionId">
    /// An optional mocked session ID value to override the session ID claim in the JWT. If specified, this value will be
    /// added as a "sid" claim if <paramref name="removeSessionIdClaim"/> is <c>false</c>.
    /// </param>
    /// <returns>
    /// An <see cref="AuthorizationHandlerContext"/> instance initialized with the specified JWT token, requirement, and mock values.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="jwtToken"/> is null, empty, or consists only of whitespace.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="jwtToken"/> is not a valid JWT.
    /// </exception>
    private static AuthorizationHandlerContext CreateAuthorizationHandlerContext(string jwtToken,
        KcAuthorizationRequirement requirement, bool mapSubjectClaim = true, bool removeSessionIdClaim = false,
        string mockSubject = null, string mockSessionId = null)
    {
        // Validate that the JWT token is provided and not null or empty
        if ( string.IsNullOrWhiteSpace(jwtToken) )
        {
            throw new ArgumentNullException(nameof(jwtToken), "JWT token is required.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt;

        try
        {
            // Parse the JWT token
            jwt = tokenHandler.ReadJwtToken(jwtToken);
        }
        catch ( Exception ex )
        {
            throw new ArgumentException("The provided JWT token is invalid.", nameof(jwtToken), ex);
        }

        // Extract claims from the parsed JWT token
        var userClaims = jwt.Claims.ToList();

        var subject = userClaims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if ( mapSubjectClaim && string.IsNullOrWhiteSpace(mockSubject) && !string.IsNullOrWhiteSpace(subject) )
        {
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, subject));
        }

        if ( mapSubjectClaim && !string.IsNullOrWhiteSpace(mockSubject) )
        {
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, mockSubject));
        }

        switch ( removeSessionIdClaim )
        {
            case true:
                userClaims = userClaims.Where(c => c.Type != "sid").ToList();
                break;
            case false when !string.IsNullOrWhiteSpace(mockSessionId):
                userClaims = userClaims.Where(c => c.Type != "sid").ToList();
                userClaims.Add(new Claim("sid", mockSessionId));
                break;
        }

        // Create a ClaimsPrincipal from the JWT claims
        var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "Bearer"));

        // Return a new AuthorizationHandlerContext initialized with the specified requirement and user
        return new AuthorizationHandlerContext([requirement], user, null);
    }

    /// <summary>
    /// Sets up and returns a <see cref="KcAuthorizationRequirement"/> instance for testing purposes.
    /// </summary>
    /// <remarks>
    /// This method configures a mocked <see cref="KcProtectedResourceStore"/> to simulate the protected resource data
    /// and retrieves resource policy information from the test environment.
    /// </remarks>
    /// <returns>
    /// A configured instance of <see cref="KcAuthorizationRequirement"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no permissions or scopes are defined in the test environment, or if the resource access data is invalid.
    /// </exception>
    private KcAuthorizationRequirement SetUpKcAuthorizationRequirement()
    {
        // Mock the protected resource store and set up its behavior.
        var mockProtectedResourceStore = new Mock<KcProtectedResourceStore>();

        _ = mockProtectedResourceStore.Setup(store => store.GetRealmProtectedResources()).Returns(() =>
            new List<KcRealmProtectedResources>
            {
                new()
                {
                    Realm = TestEnvironment.TestingRealm.Name,
                    ProtectedResourceName = TestEnvironment.TestingRealm.PrivateClient.ClientId
                }
            });

        // Retrieve the first resource policy defined in the test environment.
        var resourcePolicy = TestEnvironment.TestingRealm.User.Permissions.FirstOrDefault()?.Scopes.FirstOrDefault();

        // Ensure a resource policy is available.
        Assert.IsNotNull(resourcePolicy);

        // Split the resource policy into resource name and scope.
        var resourceAccessData = resourcePolicy.Split('#');

        // Validate that the resource access data contains exactly two parts.
        Assert.IsTrue(resourceAccessData.Length == 2,
            "Resource access data must contain both resource name and scope.");

        // Return a new authorization requirement using the mocked resource store and resource access data.
        return new KcAuthorizationRequirement(
            mockProtectedResourceStore.Object,
            resourceAccessData[0], // Resource name
            resourceAccessData[1] // Resource scope
        );
    }

    /// <summary>
    /// Configures a mock <see cref="KcRealmAdminTokenHandler"/> for use in tests.
    /// </summary>
    private KcRealmAdminTokenHandler SetUpMockRealmAdminTokenHandler()
    {
        // Create mocks for required dependencies
        var mockScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockConfigStore = new Mock<KcRealmAdminConfigurationStore>();
        var mockProvider = new Mock<IServiceProvider>();

        // Configure the mock service provider to resolve the IServiceScopeFactory
        _ = mockProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockScopeFactory.Object);

        // Configure the mock IServiceScopeFactory to return a mock scope
        _ = mockScopeFactory.Setup(factory => factory.CreateScope())
            .Returns(mockScope.Object);

        // Set up a mock logger to simulate logging functionality
        var mockLogger = new Mock<ILogger>();
        _ = mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        // Configure the mock scope to resolve services
        _ = mockScope.Setup(x => x.ServiceProvider).Returns(mockProvider.Object);

        // Mock resolution of ILogger from the service provider
        _ = mockProvider.Setup(x => x.GetService(typeof(ILogger)))
            .Returns(mockLogger.Object);

        // Define a test-specific configuration for the Keycloak realm
        var testConfig = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = TestEnvironment.BaseUrl,
            Realm = TestEnvironment.TestingRealm.Name,
            ClientId = TestEnvironment.TestingRealm.PublicClient.ClientId,
            RealmAdminCredentials = new KcUserLogin
            {
                Username = TestEnvironment.TestingRealm.User.Username,
                Password = TestEnvironment.TestingRealm.User.Password
            }
        };

        // Configure the mock configuration store to return the test configuration
        _ = mockConfigStore.Setup(c => c.GetRealmsAdminConfiguration())
            .Returns(new List<KcRealmAdminConfiguration>
            {
                testConfig
            });

        // Return a new instance of KcRealmAdminTokenHandler with the mocked dependencies
        return new KcRealmAdminTokenHandler(mockConfigStore.Object, mockProvider.Object);
    }
}
