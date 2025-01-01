using NETCore.Keycloak.Client.Models.Clients;

namespace NETCore.Keycloak.Client.Tests.Modules.KcClientsTests;

/// <summary>
/// Contains unit tests for the <see cref="KcClientFilter"/> class, focusing on its ability to construct query strings.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcClientFilterTests
{
    /// <summary>
    /// Validates that the <see cref="KcClientFilter.BuildQuery"/> method constructs the correct query string when all properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldBuildQueryCorrectly()
    {
        // Arrange
        var filter = new KcClientFilter
        {
            Max = 10,
            First = 5,
            Search = "TestClient",
            Q = "key1:value1 key2:value2",
            ViewableOnly = true
        };

        // Expected query string
        var expectedQuery = "?max=10&first=5&search=TestClient&q=key1:value1 key2:value2&viewableOnly=true";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcClientFilter.BuildQuery"/> method handles an empty filter by returning a default query string.
    /// </summary>
    [TestMethod]
    public void ShouldHandleEmptyFilterProperties()
    {
        // Arrange
        var filter = new KcClientFilter();

        // Expected query string
        const string expectedQuery = "?max=100";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcClientFilter.BuildQuery"/> method constructs a query string when only some properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldHandlePartialFilterProperties()
    {
        // Arrange
        var filter = new KcClientFilter
        {
            Max = 20,
            Search = "PartialClient",
            ViewableOnly = false
        };

        // Expected query string
        const string expectedQuery = "?max=20&search=PartialClient&viewableOnly=false";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcClientFilter.BuildQuery"/> method includes only the custom attribute query when set.
    /// </summary>
    [TestMethod]
    public void ShouldIncludeCustomAttributeQuery()
    {
        // Arrange
        var filter = new KcClientFilter
        {
            Max = 0,
            Q = "customKey:customValue"
        };

        // Expected query string
        const string expectedQuery = "?max=0&q=customKey:customValue";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }
}
