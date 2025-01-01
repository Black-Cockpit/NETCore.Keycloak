using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Contains unit tests for the <see cref="KcFilter"/> class, focusing on its ability to construct query strings.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcFilterTests
{
    /// <summary>
    /// Validates that the <see cref="KcFilter.BuildQuery"/> method constructs the correct query string when all properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldBuildQueryCorrectly()
    {
        // Arrange
        var filter = new KcFilter
        {
            Max = 50,
            BriefRepresentation = true,
            First = 10,
            Search = "TestSearch"
        };

        // Expected query string
        var expectedQuery = "?max=50&briefRepresentation=true&first=10&search=TestSearch";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcFilter.BuildQuery"/> method handles default values correctly when no optional properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldHandleDefaultValues()
    {
        // Arrange
        var filter = new KcFilter();

        // Expected query string
        const string expectedQuery = "?max=100";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcFilter.BuildQuery"/> method constructs a query string when only some properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldHandlePartialFilterProperties()
    {
        // Arrange
        var filter = new KcFilter
        {
            Max = 20,
            Search = "PartialSearch"
        };

        // Expected query string
        const string expectedQuery = "?max=20&search=PartialSearch";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcFilter.BuildQuery"/> method includes the pagination offset when specified.
    /// </summary>
    [TestMethod]
    public void ShouldIncludePaginationOffset()
    {
        // Arrange
        var filter = new KcFilter
        {
            Max = 10,
            First = 5
        };

        // Expected query string
        const string expectedQuery = "?max=10&first=5";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }
}
