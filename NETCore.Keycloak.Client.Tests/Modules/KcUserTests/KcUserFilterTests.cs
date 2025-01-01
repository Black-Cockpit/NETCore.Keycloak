using NETCore.Keycloak.Client.Models.Users;

namespace NETCore.Keycloak.Client.Tests.Modules.KcUserTests;

/// <summary>
/// Contains unit tests for the <see cref="KcUserFilter"/> class, focusing on its ability to construct query strings.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcUserFilterTests
{
    /// <summary>
    /// Validates that the <see cref="KcUserFilter.BuildQuery"/> method constructs the correct query string when all properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldBuildQueryCorrectly()
    {
        // Arrange
        var filter = new KcUserFilter
        {
            Max = 10,
            BriefRepresentation = true,
            Email = "test@example.com",
            EmailVerified = true,
            Enabled = false,
            Exact = true,
            First = 5,
            FirstName = "John",
            IdpAlias = "test-idp",
            IdpUserId = "12345",
            LastName = "Doe",
            Q = "key1:value1 key2:value2",
            Search = "TestSearch",
            Username = "testuser"
        };

        // Expected query string
        var expectedQuery = "?max=10&briefRepresentation=true&email=test@example.com&emailVerified=true&enabled=false" +
                            "&exact=true&first=5&firstName=John&idpAlias=test-idp&idpUserId=12345&lastName=Doe" +
                            "&q=key1:value1 key2:value2&search=TestSearch&username=testuser";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcUserFilter.BuildQuery"/> method handles an empty filter by returning a default query string.
    /// </summary>
    [TestMethod]
    public void ShouldHandleEmptyFilterProperties()
    {
        // Arrange
        var filter = new KcUserFilter();

        // Expected query string
        const string expectedQuery = "?max=100";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcUserFilter.BuildQuery"/> method constructs a query string when only some properties are set.
    /// </summary>
    [TestMethod]
    public void ShouldHandlePartialFilterProperties()
    {
        // Arrange
        var filter = new KcUserFilter
        {
            Max = 20,
            Email = "partial@example.com",
            Enabled = true,
            Username = "partialuser"
        };

        // Expected query string
        const string expectedQuery = "?max=20&email=partial@example.com&enabled=true&username=partialuser";

        // Act
        var queryString = filter.BuildQuery();

        // Assert
        Assert.AreEqual(expectedQuery, queryString);
    }

    /// <summary>
    /// Validates that the <see cref="KcUserFilter.BuildQuery"/> method includes custom attribute queries correctly.
    /// </summary>
    [TestMethod]
    public void ShouldIncludeCustomAttributeQuery()
    {
        // Arrange
        var filter = new KcUserFilter
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
