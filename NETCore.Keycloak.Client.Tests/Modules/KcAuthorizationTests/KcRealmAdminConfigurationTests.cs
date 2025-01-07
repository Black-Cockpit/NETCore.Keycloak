using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Unit tests for the <see cref="KcRealmAdminConfiguration"/> class.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcRealmAdminConfigurationTests
{
    /// <summary>
    /// Validates that no exception is thrown when all properties are correctly set.
    /// </summary>
    [TestMethod]
    public void Validate_ShouldPassWhenAllPropertiesAreSet()
    {
        // Arrange
        var configuration = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = "https://keycloak.example.com",
            Realm = "example-realm",
            ClientId = "example-client-id",
            RealmAdminCredentials = new KcUserLogin
            {
                Username = "admin",
                Password = "admin-password"
            }
        };

        // Act & Assert
        configuration.Validate();
    }

    /// <summary>
    /// Validates that an exception is thrown when <see cref="KcRealmAdminConfiguration.KeycloakBaseUrl"/> is null or empty.
    /// </summary>
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void Validate_ShouldThrowException_WhenKeycloakBaseUrlIsInvalid(string invalidBaseUrl)
    {
        // Arrange
        var configuration = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = invalidBaseUrl,
            Realm = "example-realm",
            ClientId = "example-client-id",
            RealmAdminCredentials = new KcUserLogin
            {
                Username = "admin",
                Password = "admin-password"
            }
        };

        // Act & Assert
        var exception = Assert.ThrowsException<KcException>(configuration.Validate);
        Assert.AreEqual("KeycloakBaseUrl is required", exception.Message);
    }

    /// <summary>
    /// Validates that an exception is thrown when <see cref="KcRealmAdminConfiguration.RealmAdminCredentials"/> is null.
    /// </summary>
    [TestMethod]
    public void Validate_ShouldThrowException_WhenRealmAdminCredentialsAreNull()
    {
        // Arrange
        var configuration = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = "https://keycloak.example.com",
            Realm = "example-realm",
            ClientId = "example-client-id",
            RealmAdminCredentials = null
        };

        // Act & Assert
        var exception = Assert.ThrowsException<KcException>(configuration.Validate);
        Assert.AreEqual("RealmAdminCredentials is required", exception.Message);
    }

    /// <summary>
    /// Validates that the configuration is valid even if <see cref="KcRealmAdminConfiguration.Realm"/> and <see cref="KcRealmAdminConfiguration.ClientId"/> are not set.
    /// </summary>
    [TestMethod]
    public void Validate_ShouldPass_WhenRealmAndClientIdAreNullOrEmpty()
    {
        // Arrange
        var configuration = new KcRealmAdminConfiguration
        {
            KeycloakBaseUrl = "https://keycloak.example.com",
            Realm = null,
            ClientId = null,
            RealmAdminCredentials = new KcUserLogin
            {
                Username = "admin",
                Password = "admin-password"
            }
        };

        // Act & Assert
        configuration.Validate(); // Should not throw an exception
    }
}
