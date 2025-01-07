using System.Data;
using Moq;
using NETCore.Keycloak.Client.Authorization.Requirements;
using NETCore.Keycloak.Client.Authorization.Store;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Provides unit tests for the <see cref="KcAuthorizationRequirement"/> class.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcAuthorizationRequirementTests
{
    /// <summary>
    /// Tests that the constructor sets the properties correctly.
    /// </summary>
    [TestMethod]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var mockProtectedResourceStore = new Mock<KcProtectedResourceStore>();
        const string resource = "TestResource";
        const string scope = "TestScope";

        // Act
        var requirement = new KcAuthorizationRequirement(mockProtectedResourceStore.Object, resource, scope);

        // Assert
        Assert.IsNotNull(requirement.ProtectedResourceStore);
        Assert.AreEqual(mockProtectedResourceStore.Object, requirement.ProtectedResourceStore);
        Assert.AreEqual("TestResource", requirement.Resource);
    }

    /// <summary>
    /// Tests that the Resource property returns the correct value.
    /// </summary>
    [TestMethod]
    public void Property_Resource_ShouldReturnCorrectValue()
    {
        // Arrange
        var mockProtectedResourceStore = new Mock<KcProtectedResourceStore>();
        const string resource = "TestResource";
        const string scope = "TestScope";

        var requirement = new KcAuthorizationRequirement(mockProtectedResourceStore.Object, resource, scope);

        // Act
        var actualResource = requirement.Resource;

        // Assert
        Assert.AreEqual("TestResource", actualResource);
    }

    /// <summary>
    /// Tests that the ToString method returns the correct formatted string.
    /// </summary>
    [TestMethod]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var mockProtectedResourceStore = new Mock<KcProtectedResourceStore>();
        const string resource = "TestResource";
        const string scope = "TestScope";

        var requirement = new KcAuthorizationRequirement(mockProtectedResourceStore.Object, resource, scope);

        // Act
        var result = requirement.ToString();

        // Assert
        Assert.AreEqual("TestResource#TestScope", result);
    }

    /// <summary>
    /// Tests that the constructor throws an exception if ProtectedResourceStore is null.
    /// </summary>
    [TestMethod]
    public void Constructor_ShouldThrowException_WhenProtectedResourceStoreIsNull() =>
        Assert.ThrowsException<ArgumentNullException>(() =>
            new KcAuthorizationRequirement(null, "TestResource", "TestScope"));

    /// <summary>
    /// Tests that the constructor throws an exception if the Resource is null or empty.
    /// </summary>
    [TestMethod]
    public void Constructor_ShouldThrowException_WhenResourceIsNull()
    {
        // Arrange
        var mockProtectedResourceStore = new Mock<KcProtectedResourceStore>();

        // Act
        _ = Assert.ThrowsException<NoNullAllowedException>(() =>
            new KcAuthorizationRequirement(mockProtectedResourceStore.Object, null, "TestScope"));
    }

    /// <summary>
    /// Tests that the constructor throws an exception if Scope is null or empty.
    /// </summary>
    [TestMethod]
    public void Constructor_ShouldThrowException_WhenScopeIsNull()
    {
        // Arrange
        var mockProtectedResourceStore = new Mock<KcProtectedResourceStore>();

        // Act
        _ = Assert.ThrowsException<NoNullAllowedException>(() =>
            new KcAuthorizationRequirement(mockProtectedResourceStore.Object, "TestResource", null));
    }
}
