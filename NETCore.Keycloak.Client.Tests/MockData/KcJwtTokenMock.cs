using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NETCore.Keycloak.Client.Tests.MockData;

/// <summary>
/// Provides functionality to generate mock JWT tokens for testing purposes.
/// </summary>
public static class KcJwtTokenMock
{
    /// <summary>
    /// Generates a hardcoded mock JWT token.
    /// </summary>
    /// <remarks>
    /// This method generates a JWT token with predefined issuer, audience, signing key, claims, and expiration time.
    /// It is designed for testing scenarios where a consistent token is required.
    /// </remarks>
    /// <returns>A string representing the signed JWT token.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if required parameters (e.g., claims, signing key) are null or empty.
    /// </exception>
    public static string CreateMockJwtToken()
    {
        // Hardcoded token details
        const string issuer = "https://mock-issuer.com";
        const string audience = "mock-audience";
        var signingKey = KcTestPasswordCreator.Create(32);
        const int expiryMinutes = 60;

        // Hardcoded claims
        var claims = new[]
        {
            new Claim("sub", "mock-user-id"), new Claim("sid", "mock-session-id"), new Claim("iss", issuer),
            new Claim("aud", audience), new Claim("custom-claim", "custom-value")
        };

        // Define the token signing key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create the JWT token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            SigningCredentials = credentials
        };

        // Generate the token using the JwtSecurityTokenHandler
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the serialized token
        return tokenHandler.WriteToken(token);
    }
}
