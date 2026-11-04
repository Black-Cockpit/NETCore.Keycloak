using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Tests.MockData;

namespace NETCore.Keycloak.Client.Tests.UtilsTests;

/// <summary>
/// Verifies password and JWT behavior across dependency upgrades.
/// </summary>
[TestClass]
public class KcDependencyCompatibilityTests
{
    /// <summary>
    /// Keeps the password lengths used by user fixtures and JWT signing keys.
    /// </summary>
    [TestMethod]
    [DataRow(16)]
    [DataRow(32)]
    public void ShouldGeneratePasswordsWithRequestedLength(int length)
    {
        var password = KcTestPasswordCreator.Create(length);

        Assert.AreEqual(length, password.Length);
        Assert.IsTrue(password.All(character => character is >= '!' and <= '~'));
    }

    /// <summary>
    /// Keeps disabled character groups out of generated passwords.
    /// </summary>
    [TestMethod]
    [DataRow(true, false, false, false)]
    [DataRow(false, true, false, false)]
    [DataRow(false, false, true, false)]
    [DataRow(false, false, false, true)]
    public void ShouldRespectEnabledPasswordCharacters(bool lowerCase, bool upperCase, bool digits, bool specialAscii)
    {
        var password = KcTestPasswordCreator.Create(16, lowerCase, upperCase, digits, specialAscii);

        Assert.AreEqual(16, password.Length);
        Assert.IsTrue(password.All(character =>
            (lowerCase && char.IsAsciiLetterLower(character)) ||
            (upperCase && char.IsAsciiLetterUpper(character)) ||
            (digits && char.IsAsciiDigit(character)) ||
            (specialAscii && character is >= '!' and <= '~' && !char.IsAsciiLetterOrDigit(character))));
    }

    /// <summary>
    /// Exercises signing and parsing the JWT format used by test fixtures.
    /// </summary>
    [TestMethod]
    public void ShouldReadSessionIdFromGeneratedJwt()
    {
        var token = new KcIdentityProviderToken { AccessToken = KcJwtTokenMock.CreateMockJwtToken() };

        Assert.AreEqual("mock-session-id", token.GetSessionId());
    }

    /// <summary>
    /// Keeps invalid and empty tokens from producing a session ID.
    /// </summary>
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("not-a-jwt")]
    public void ShouldReturnNullSessionIdForInvalidTokens(string accessToken)
    {
        var token = new KcIdentityProviderToken { AccessToken = accessToken };

        Assert.IsNull(token.GetSessionId());
    }
}
