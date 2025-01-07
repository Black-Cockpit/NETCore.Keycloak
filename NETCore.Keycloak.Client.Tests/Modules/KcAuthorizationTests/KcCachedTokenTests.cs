using NETCore.Keycloak.Client.Authorization.Handlers;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthorizationTests;

/// <summary>
/// Unit tests for the <see cref="KcCachedToken"/> class.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcCachedTokenTests
{
    /// <summary>
    /// Verifies that <see cref="KcCachedToken.IsExpired"/> returns <c>false</c>
    /// when the token has not yet expired.
    /// </summary>
    [TestMethod]
    public void ShouldReturnFalse_WhenTokenIsNotExpired()
    {
        // Arrange: Create a token with a 60-second expiry time.
        var cachedToken = new KcCachedToken
        {
            Value = "sample-valid-token",
            Expiry = 60 // 60 seconds expiry time
        };

        // Act: Check if the token is expired.
        var isExpired = cachedToken.IsExpired;

        // Assert: Verify that the token is not expired.
        Assert.IsFalse(isExpired, "Token should not be expired immediately after being cached.");
    }

    /// <summary>
    /// Verifies that <see cref="KcCachedToken.IsExpired"/> returns <c>true</c>
    /// when the token has already expired.
    /// </summary>
    [TestMethod]
    public void ShouldReturnTrue_WhenTokenHasExpired()
    {
        // Arrange: Create a token with a negative expiry time to simulate an already expired token.
        var cachedToken = new KcCachedToken
        {
            Value = "sample-expired-token",
            Expiry = -10 // Expiry time in the past (negative value)
        };

        // Act: Check if the token is expired.
        var isExpired = cachedToken.IsExpired;

        // Assert: Verify that the token is marked as expired.
        Assert.IsTrue(isExpired, "Token should be expired when the expiry time is in the past.");
    }

    /// <summary>
    /// Verifies that <see cref="KcCachedToken.IsExpired"/> returns <c>true</c>
    /// when the expiry time is set to zero seconds.
    /// </summary>
    [TestMethod]
    public void ShouldHandleZeroExpiryTime_AsExpired()
    {
        // Arrange: Create a token with zero expiry time.
        var cachedToken = new KcCachedToken
        {
            Value = "sample-zero-expiry-token",
            Expiry = 0 // Expiry time is 0 seconds
        };

        // Act: Check if the token is expired.
        var isExpired = cachedToken.IsExpired;

        // Assert: Verify that the token is marked as expired.
        Assert.IsTrue(isExpired, "Token should be immediately expired when expiry time is zero.");
    }

    /// <summary>
    /// Verifies that <see cref="KcCachedToken.IsExpired"/> returns <c>false</c>
    /// when the token's expiry time is in the future.
    /// </summary>
    [TestMethod]
    public void ShouldReturnFalse_WhenExpiryIsFuture()
    {
        // Arrange: Create a token with a 1-hour expiry time.
        var cachedToken = new KcCachedToken
        {
            Value = "sample-future-token",
            Expiry = 3600 // 3600 seconds (1 hour)
        };

        // Simulate a short delay to verify near-future validity.
        Thread.Sleep(100); // 100 milliseconds delay

        // Act: Check if the token is expired.
        var isExpired = cachedToken.IsExpired;

        // Assert: Verify that the token is not expired.
        Assert.IsFalse(isExpired, "Token should not be expired within its valid duration.");
    }

    /// <summary>
    /// Verifies that the <see cref="KcCachedToken.Value"/> property correctly stores and returns the token value.
    /// </summary>
    [TestMethod]
    public void ShouldStoreCorrectTokenValue()
    {
        // Arrange: Create a token with a specific value and expiry time.
        const string tokenValue = "sample-stored-token";
        var cachedToken = new KcCachedToken
        {
            Value = tokenValue,
            Expiry = 300 // 300 seconds (5-minute expiry time)
        };

        // Act: Retrieve the stored token value.
        var storedValue = cachedToken.Value;

        // Assert: Verify that the stored token value matches the expected value.
        Assert.AreEqual(tokenValue, storedValue,
            "The token value stored in the cache should match the expected value.");
    }
}
