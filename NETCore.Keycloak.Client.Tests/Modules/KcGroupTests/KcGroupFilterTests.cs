using NETCore.Keycloak.Client.Models.Groups;

namespace NETCore.Keycloak.Client.Tests.Modules.KcGroupTests;

/// <summary>
/// Contains unit tests for the <see cref="KcGroupFilter"/> class, focusing on its ability to construct query strings.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcGroupFilterTests
{
    /// <summary>
    /// Validates that the <see cref="KcGroupFilter.BuildQuery"/> method constructs the correct query string when all properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldBuildQueryCorrectly()
    {
        // Arrange
        var filter = new KcGroupFilter
        {
            Max = 10,
            BriefRepresentation = true,
            First = 5,
            Q = "key1:value1 key2:value2",
            Search = "TestGroup",
            Exact = true,
            Top = false
        };

        // Expected query string
        const string expectedQuery =
            "?max=10&briefRepresentation=true&first=5&q=key1:value1 key2:value2&search=TestGroup&exact=true&top=false";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcGroupFilter.BuildQuery"/> method handles an empty filter by returning a default query string.
    /// </summary>
    [TestMethod]
    public void ShouldHandleEmptyFilterProperties()
    {
        // Arrange
        var filter = new KcGroupFilter();

        // Expected query string
        const string expectedQuery = "?max=100";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcGroupFilter.BuildQuery"/> method constructs a query string when only some properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldHandlePartialFilterProperties()
    {
        // Arrange
        var filter = new KcGroupFilter
        {
            Max = 20,
            Search = "PartialTest",
            Exact = false
        };

        // Expected query string
        const string expectedQuery = "?max=20&search=PartialTest&exact=false";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }
}
