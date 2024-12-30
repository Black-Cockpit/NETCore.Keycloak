using System.Net;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Tokens;

namespace NETCore.Keycloak.Client.Tests.Modules;

/// <summary>
/// Provides common assertion methods for validating API monitoring metrics in Keycloak tests.
/// </summary>
public static class KcCommonAssertion
{
    /// <summary>
    /// Asserts the properties of a <see cref="KcHttpApiMonitoringMetrics"/> object
    /// against expected values to ensure the integrity of API monitoring data.
    /// </summary>
    /// <param name="monitoringMetrics">The monitoring metrics object to validate.</param>
    /// <param name="expectedStatusCode">The expected HTTP status code of the response.</param>
    /// <param name="expectedMethod">The expected HTTP method of the request.</param>
    /// <param name="isError">
    /// Indicates whether an error is expected.
    /// If true, the <see cref="KcHttpApiMonitoringMetrics.Error"/> property should not be null or empty.
    /// </param>
    public static void AssertResponseMonitoringMetrics(
        KcHttpApiMonitoringMetrics monitoringMetrics,
        HttpStatusCode? expectedStatusCode,
        HttpMethod expectedMethod,
        bool isError = false)
    {
        // Ensure the monitoring metrics object is not null.
        Assert.IsNotNull(monitoringMetrics);

        // Validate the HTTP method is not null and matches the expected method.
        Assert.IsNotNull(monitoringMetrics.HttpMethod);
        Assert.AreEqual(monitoringMetrics.HttpMethod, expectedMethod);

        // Validate the error state matches the isError flag.
        Assert.IsTrue(string.IsNullOrWhiteSpace(monitoringMetrics.Error) == !isError);

        // Validate the status code matches the expected status code.
        Assert.AreEqual(monitoringMetrics.StatusCode, expectedStatusCode);

        // Ensure the URL is not null.
        Assert.IsNotNull(monitoringMetrics.Url);

        // Ensure the request duration in milliseconds is not null.
        Assert.IsNotNull(monitoringMetrics.RequestMilliseconds);
    }

    /// <summary>
    /// Asserts the properties of a <see cref="KcResponse{T}"/> object containing an identity provider token.
    /// Ensures that the token response is valid and all critical properties are properly initialized.
    /// </summary>
    /// <param name="tokenResponse">
    /// The response object containing the identity provider token to validate.
    /// Must not be null and must have no errors.
    /// </param>
    /// <exception cref="AssertFailedException">
    /// Thrown if any of the assertions fail, indicating an invalid token response.
    /// </exception>
    public static void AssertIdentityProviderTokenResponse(KcResponse<KcIdentityProviderToken> tokenResponse)
    {
        // Ensure the token response object is not null.
        Assert.IsNotNull(tokenResponse);

        // Validate that the response does not indicate an error.
        Assert.IsFalse(tokenResponse.IsError);

        // Ensure the token details within the response are not null.
        Assert.IsNotNull(tokenResponse.Response);

        // Validate that the access token is not null or empty.
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.Response.AccessToken));

        // Validate that the refresh token is not null or empty.
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.Response.RefreshToken));

        // Ensure the scope of the token is defined.
        Assert.IsNotNull(tokenResponse.Response.Scope);

        // Validate that the 'NotBeforePolicy' is properly set.
        Assert.IsNotNull(tokenResponse.Response.NotBeforePolicy);

        // Ensure the token has a defined expiration time.
        Assert.IsNotNull(tokenResponse.Response.ExpiresIn);

        // Validate that the refresh token exists and is properly set.
        Assert.IsNotNull(tokenResponse.Response.RefreshToken);

        // Ensure the token type is defined (e.g., "Bearer").
        Assert.IsNotNull(tokenResponse.Response.TokenType);

        // Validate that the creation timestamp of the token is set.
        Assert.IsNotNull(tokenResponse.Response.CreatedAt);
    }
}
