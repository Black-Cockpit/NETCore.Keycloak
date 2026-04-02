using NETCore.Keycloak.Client.Models.Organizations;

namespace NETCore.Keycloak.Client.Tests.Modules.KcOrganizationTests;

/// <summary>
/// Contains unit tests for the <see cref="KcOrganizationFilter"/> class, focusing on its ability to construct query strings.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcOrganizationFilterTests
{
    /// <summary>
    /// Validates that the <see cref="KcOrganizationFilter.BuildQuery"/> method constructs the correct query string when all properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldBuildQueryCorrectly()
    {
        // Arrange
        var filter = new KcOrganizationFilter
        {
            Max = 10,
            BriefRepresentation = true,
            First = 5,
            Q = "key1:value1 key2:value2",
            Search = "TestOrganization",
            Exact = true
        };

        // Expected query string
        const string expectedQuery =
            "?max=10&briefRepresentation=true&first=5&q=key1:value1 key2:value2&search=TestOrganization&exact=true";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcOrganizationFilter.BuildQuery"/> method handles an empty filter by returning a default query string.
    /// </summary>
    [TestMethod]
    public void ShouldHandleEmptyFilterProperties()
    {
        // Arrange
        var filter = new KcOrganizationFilter();

        // Expected query string
        const string expectedQuery = "?max=100";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcOrganizationFilter.BuildQuery"/> method constructs a query string when only some properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldHandlePartialFilterProperties()
    {
        // Arrange
        var filter = new KcOrganizationFilter
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

    /// <summary>
    /// Validates that the <see cref="KcOrganizationFilter.BuildQuery"/> method includes the custom attribute query parameter when specified.
    /// </summary>
    [TestMethod]
    public void ShouldIncludeCustomAttributeQuery()
    {
        // Arrange
        var filter = new KcOrganizationFilter
        {
            Q = "department:engineering"
        };

        // Expected query string
        const string expectedQuery = "?max=100&q=department:engineering";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }
}
