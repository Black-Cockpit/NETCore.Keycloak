using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Moq;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.HttpClients.Implementation;
using NETCore.Keycloak.Client.Models.Auth;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Verifies OAuth client authentication on every credential-bearing endpoint.
/// </summary>
[TestClass]
public class KcClientAuthenticationTests
{
    /// <summary>
    /// Keeps credentials scoped to requests and uses the selected authentication method for every grant.
    /// </summary>
    /// <param name="basic">Whether credentials belong in the Basic header.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task ShouldAuthenticateTokenRefreshAndRevocationRequests(bool basic)
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "factory-default");
        var factory = new Mock<IHttpClientFactory>();
        _ = factory.Setup(value => value.CreateClient(It.IsAny<string>())).Returns(httpClient);
        var client = new KeycloakClient("https://keycloak.test", null, factory.Object);
        var credentials = new KcClientCredentials
        {
            ClientId = "client :+&",
            Secret = "sëcret :+&",
            AuthenticationMethod = basic
                ? KcClientAuthenticationMethod.ClientSecretBasic
                : KcClientAuthenticationMethod.ClientSecretPost
        };
        var login = new KcUserLogin { Username = "test-user", Password = "test-password" };

        Assert.IsFalse((await client.Auth.GetClientCredentialsTokenAsync("realm", credentials)
            .ConfigureAwait(false)).IsError);
        Assert.IsFalse((await client.Auth.GetResourceOwnerPasswordTokenAsync("realm", credentials, login)
            .ConfigureAwait(false)).IsError);
        Assert.IsFalse((await client.Auth.RefreshAccessTokenAsync("realm", credentials, "refresh-token")
            .ConfigureAwait(false)).IsError);
        Assert.IsFalse((await client.Auth.RevokeAccessTokenAsync("realm", credentials, "access-token")
            .ConfigureAwait(false)).IsError);
        Assert.IsFalse((await client.Auth.RevokeRefreshTokenAsync("realm", credentials, "refresh-token")
            .ConfigureAwait(false)).IsError);
        var validation = await client.Auth.ValidatePasswordAsync("realm", credentials, login).ConfigureAwait(false);
        Assert.IsTrue(validation.Response);
        Assert.AreEqual(8, handler.Requests.Count);

        foreach ( var (authorization, body) in handler.Requests )
        {
            if ( basic )
            {
                Assert.AreEqual("Basic " + Convert.ToBase64String(
                    Encoding.UTF8.GetBytes("client+%3A%2B%26:s%C3%ABcret+%3A%2B%26")), authorization);
                Assert.IsFalse(body.Contains("client_id=", StringComparison.Ordinal));
                Assert.IsFalse(body.Contains("client_secret=", StringComparison.Ordinal));
            }
            else
            {
                StringAssert.Contains(body, "client_id=client+%3A%2B%26", StringComparison.Ordinal);
                StringAssert.Contains(body, "client_secret=s%C3%ABcret+%3A%2B%26", StringComparison.Ordinal);
            }
        }
        Assert.AreEqual("Bearer factory-default", httpClient.DefaultRequestHeaders.Authorization?.ToString());
    }

    /// <summary>
    /// Rejects unknown authentication methods before credentials can be sent using an unintended method.
    /// </summary>
    [TestMethod]
    public void ShouldRejectUnknownAuthenticationMethod()
    {
        var credentials = new KcClientCredentials
        {
            ClientId = "client",
            AuthenticationMethod = ( KcClientAuthenticationMethod ) 99
        };
        _ = Assert.ThrowsExactly<KcException>(credentials.Validate);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<(string Authorization, string Body)> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add((request.Headers.Authorization?.ToString(),
                await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)));
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new StringContent("{\"access_token\":\"access\",\"refresh_token\":\"refresh\"}")
            };
        }
    }
}
