using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using NETCore.Keycloak.Client.Authentication;
using NETCore.Keycloak.Client.Authentication.Claims;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Tests.Abstraction;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthentication;

/// <summary>
/// Tests for the <see cref="KcAuthenticationExtension"/> class, focusing on
/// the behavior of the Keycloak authentication extension methods.
/// </summary>
[TestClass]
[TestCategory("Final")]
public class KcAuthenticationExtensionTests : KcTestingModule
{
    /// <summary>
    /// Represents the service collection used to register services during tests.
    /// </summary>
    private IServiceCollection _services;

    /// <summary>
    /// Initializes the test environment by creating a new instance of
    /// <see cref="IServiceCollection"/> to ensure test isolation.
    /// </summary>
    [TestInitialize]
    public void Setup() => _services = new ServiceCollection();

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationExtension.AddKeycloakAuthentication"/> method
    /// correctly uses the default authentication scheme when a null or empty scheme is provided.
    /// </summary>
    [TestMethod]
    public void ShouldUseDefaultAuthenticationSchemeWhenSchemeIsNull()
    {
        // Act
        var builder = _services.AddKeycloakAuthentication(
            null,
            config =>
            {
                config.Url = TestEnvironment.BaseUrl;
                config.Issuer = TestEnvironment.BaseUrl;
                config.Realm = TestEnvironment.TestingRealm.Name;
                config.Resource = "test-resource";
            });

        // Assert
        Assert.IsNotNull(builder, "AuthenticationBuilder should not be null.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationExtension.AddKeycloakAuthentication"/> method
    /// throws a <see cref="KcException"/> when the Keycloak configuration delegate is null.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(KcException))]
    public void ShouldThrowExceptionWhenKeycloakConfigIsNull() =>
        _services.AddKeycloakAuthentication("test-scheme", null);

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationExtension.AddKeycloakAuthentication"/> method
    /// correctly registers an implementation of <see cref="IClaimsTransformation"/>
    /// and that it resolves to <see cref="KcRolesClaimsTransformer"/>.
    /// </summary>
    [TestMethod]
    public void ShouldRegisterClaimsTransformation()
    {
        // Act
        // Add Keycloak authentication services with valid configuration.
        _ = _services.AddKeycloakAuthentication(
            "test-scheme",
            config =>
            {
                config.Url = TestEnvironment.BaseUrl; // Base URL of the Keycloak server.
                config.Issuer = TestEnvironment.BaseUrl; // Issuer URL of the Keycloak server.
                config.Realm = "test-realm"; // Name of the Keycloak realm.
                config.Resource = "test-resource"; // Protected resource name.
            });

        // Build the service provider from the configured services.
        var provider = _services.BuildServiceProvider();

        // Assert
        // Verify that IClaimsTransformation is registered in the service provider.
        var claimsTransformer = provider.GetService<IClaimsTransformation>();
        Assert.IsNotNull(claimsTransformer, "IClaimsTransformation should be registered.");

        // Verify that the registered IClaimsTransformation is of type KcRolesClaimsTransformer.
        Assert.IsInstanceOfType(claimsTransformer, typeof(KcRolesClaimsTransformer),
            "Claims transformer should be of type KcRolesClaimsTransformer.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationExtension.AddKeycloakAuthentication"/> method
    /// correctly configures the <see cref="JwtBearerOptions"/> when provided with a valid configuration
    /// and custom setup options.
    /// </summary>
    [TestMethod]
    public void ShouldConfigureJwtBearerOptionsCorrectly()
    {
        // Act
        // Add Keycloak authentication services with valid configuration and custom JWT Bearer options.
        _ = _services.AddKeycloakAuthentication(
            "test-scheme",
            config =>
            {
                config.Url = TestEnvironment.BaseUrl; // Base URL of the Keycloak server.
                config.Issuer = TestEnvironment.BaseUrl; // Issuer URL of the Keycloak server.
                config.Realm = "test-realm"; // Name of the Keycloak realm.
                config.Resource = "test-resource"; // Protected resource name.
            },
            SetupJwtBearerOptions()); // Provide custom JWT Bearer options.

        // Build the service provider from the configured services.
        var provider = _services.BuildServiceProvider();

        // Assert
        // Verify that the AuthenticationSchemeProvider is registered in the service provider.
        var authenticationSchemeProvider = provider.GetService<IAuthenticationSchemeProvider>();
        Assert.IsNotNull(authenticationSchemeProvider, "AuthenticationSchemeProvider should be registered.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationExtension.AddKeycloakAuthentication"/> method
    /// correctly integrates JWT Bearer authentication by ensuring the <see cref="IAuthenticationHandlerProvider"/>
    /// is registered in the service provider.
    /// </summary>
    [TestMethod]
    public void ShouldIntegrateJwtBearerAuthentication()
    {
        // Act
        // Add Keycloak authentication services with valid configuration.
        _ = _services.AddKeycloakAuthentication(
            "test-scheme",
            config =>
            {
                config.Url = TestEnvironment.BaseUrl; // Base URL of the Keycloak server.
                config.Issuer = TestEnvironment.BaseUrl; // Issuer URL of the Keycloak server.
                config.Realm = "test-realm"; // Name of the Keycloak realm.
                config.Resource = "test-resource"; // Protected resource name.
            });

        // Build the service provider from the configured services.
        var provider = _services.BuildServiceProvider();

        // Assert
        // Verify that the IAuthenticationHandlerProvider is registered in the service provider.
        var authentication = provider.GetService<IAuthenticationHandlerProvider>();
        Assert.IsNotNull(authentication, "AuthenticationHandlerProvider should be registered.");
    }

    /// <summary>
    /// Configures custom options for JWT Bearer authentication by setting specific properties
    /// on the <see cref="JwtBearerOptions"/> instance.
    /// </summary>
    /// <returns>
    /// A delegate that sets <see cref="JwtBearerOptions.SaveToken"/> to <c>true</c>,
    /// indicating that the bearer token should be saved for later use.
    /// </returns>
    private static Action<JwtBearerOptions> SetupJwtBearerOptions() =>
        options => options.SaveToken = true;
}
