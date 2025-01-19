using NETCore.Keycloak.Client.Authentication;
using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthentication;

/// <summary>
/// Contains unit tests for the <see cref="KcAuthenticationConfiguration"/> class,
/// validating its default property values and behavior.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcAuthenticationConfigurationTests
{
    /// <summary>
    /// Verifies that the default values of the properties in the
    /// <see cref="KcAuthenticationConfiguration"/> class are set as expected.
    /// </summary>
    [TestMethod]
    public void ShouldSetDefaultValues()
    {
        // Arrange & Act
        // Create an instance of the KcAuthenticationConfiguration class.
        var config = new KcAuthenticationConfiguration();

        // Assert
        // Verify that the default TokenClockSkew is set to 300 seconds.
        Assert.AreEqual(TimeSpan.FromSeconds(300), config.TokenClockSkew,
            "Default TokenClockSkew should be 300 seconds.");

        // Verify that the default RolesSource is set to Realm.
        Assert.AreEqual(KcRolesClaimSource.Realm, config.RolesSource, "Default RolesSource should be Realm.");

        // Verify that the default RoleClaimType is "role".
        Assert.AreEqual("role", config.RoleClaimType, "Default RoleClaimType should be 'role'.");

        // Verify that the default NameClaimType is "preferred_username".
        Assert.AreEqual("preferred_username", config.NameClaimType,
            "Default NameClaimType should be 'preferred_username'.");

        // Verify that ValidAudiences is not null by default.
        Assert.IsNotNull(config.ValidAudiences, "ValidAudiences should not be null.");

        // Verify that ValidAudiences is empty by default.
        Assert.AreEqual(0, config.ValidAudiences.Count(), "ValidAudiences should be empty by default.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationConfiguration.ValidIssuer"/> property
    /// correctly constructs the valid issuer URL when both Issuer and Realm are set.
    /// </summary>
    [TestMethod]
    public void ShouldReturnValidIssuerWhenIssuerAndRealmAreSet()
    {
        // Arrange
        // Create a configuration with valid Issuer and Realm values.
        var config = new KcAuthenticationConfiguration
        {
            Issuer = "http://localhost:8080", // Base URL of the Keycloak server.
            Realm = "test-realm" // Name of the Keycloak realm.
        };

        // Act
        // Retrieve the constructed ValidIssuer.
        var validIssuer = config.ValidIssuer;

        // Assert
        // Verify that ValidIssuer is correctly constructed using the Issuer and Realm.
        Assert.AreEqual("http://localhost:8080/realms/test-realm", validIssuer,
            "ValidIssuer should be constructed correctly.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationConfiguration.ValidIssuer"/> property
    /// returns null when the Realm property is not set.
    /// </summary>
    [TestMethod]
    public void ShouldReturnNullValidIssuerWhenRealmIsNotSet()
    {
        // Arrange
        // Create a configuration with a valid Issuer but a null Realm.
        var config = new KcAuthenticationConfiguration
        {
            Issuer = "http://localhost:8080", // Base URL of the Keycloak server.
            Realm = null // Realm is not set.
        };

        // Act
        // Retrieve the constructed ValidIssuer.
        var validIssuer = config.ValidIssuer;

        // Assert
        // Verify that ValidIssuer is null when Realm is not set.
        Assert.IsNull(validIssuer, "ValidIssuer should be null when Realm is not set.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationConfiguration.ValidIssuer"/> property
    /// returns null when the Issuer property is not set.
    /// </summary>
    [TestMethod]
    public void ShouldReturnNullValidIssuerWhenIssuerIsNotSet()
    {
        // Arrange
        // Create a configuration with a null Issuer and a valid Realm.
        var config = new KcAuthenticationConfiguration
        {
            Issuer = null, // Issuer is not set.
            Realm = "test-realm" // Valid Realm value.
        };

        // Act
        // Retrieve the constructed ValidIssuer.
        var validIssuer = config.ValidIssuer;

        // Assert
        // Verify that ValidIssuer is null when Issuer is not set.
        Assert.IsNull(validIssuer, "ValidIssuer should be null when Issuer is not set.");
    }

    /// <summary>
    /// Verifies that the private <c>NormalizeUrl</c> method in <see cref="KcAuthenticationConfiguration"/>
    /// correctly normalizes URLs by handling trailing slashes, null, and empty inputs.
    /// </summary>
    [TestMethod]
    public void ShouldNormalizeUrlCorrectly()
    {
        // Act & Assert
        // Verify that a URL with a trailing slash is trimmed correctly.
        Assert.AreEqual("http://localhost", InvokeNormalizeUrl("http://localhost/"),
            "Trailing slash should be trimmed.");

        // Verify that a URL without a trailing slash remains unchanged.
        Assert.AreEqual("http://localhost", InvokeNormalizeUrl("http://localhost"),
            "URL without trailing slash should remain unchanged.");

        // Verify that null input returns null.
        Assert.IsNull(InvokeNormalizeUrl(null), "Null URL should return null.");

        // Verify that an empty string input returns null.
        Assert.IsNull(InvokeNormalizeUrl(""), "Empty URL should return null.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcAuthenticationConfiguration.Authority"/> property
    /// correctly returns the same value as the <see cref="KcAuthenticationConfiguration.ValidIssuer"/> property
    /// when both Issuer and Realm are set.
    /// </summary>
    [TestMethod]
    public void AuthorityShouldReturnValidIssuer()
    {
        // Arrange
        // Create a configuration with valid Issuer and Realm values.
        var config = new KcAuthenticationConfiguration
        {
            Issuer = "http://localhost:8080", // Base URL of the Keycloak server.
            Realm = "test-realm" // Name of the Keycloak realm.
        };

        // Act
        // Retrieve the value of the Authority property.
        var authority = config.Authority;

        // Assert
        // Verify that Authority matches the value of ValidIssuer.
        Assert.AreEqual("http://localhost:8080/realms/test-realm", authority, "Authority should match ValidIssuer.");
    }

    /// <summary>
    /// Invokes the private <c>NormalizeUrl</c> method using reflection.
    /// </summary>
    /// <param name="url">The URL to normalize.</param>
    /// <returns>The normalized URL or null if the input is null or empty.</returns>
    private static string InvokeNormalizeUrl(string url)
    {
        // Use reflection to access the private NormalizeUrl method in KcAuthenticationConfiguration.
        var method = typeof(KcAuthenticationConfiguration)
            .GetMethod("NormalizeUrl",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        Assert.IsNotNull(method);

        return ( string ) method.Invoke(null, [
            url
        ]);
    }
}
