using System.Net;
using System.Net.Http.Headers;
using Moq;
using NETCore.Keycloak.Client.HttpClients.Implementation;

namespace NETCore.Keycloak.Client.Tests.Modules.KcAuthTests;

/// <summary>
/// Verifies that RPT requests keep each caller's bearer token isolated.
/// </summary>
[TestClass]
public class KcAuthRequestPartyTokenUnitTests
{
    /// <summary>
    /// Uses the current caller's token without changing shared client defaults.
    /// </summary>
    [TestMethod]
    public async Task ShouldKeepBearerTokensScopedToEachRequest()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "factory-default");
        var factory = new Mock<IHttpClientFactory>();
        _ = factory.Setup(value => value.CreateClient(It.IsAny<string>())).Returns(httpClient);
        var client = new KeycloakClient("https://keycloak.test", null, factory.Object);

        var first = await client.Auth.GetRequestPartyTokenAsync("test-realm", "first-token", "auth_client")
            .ConfigureAwait(false);
        var second = await client.Auth.GetRequestPartyTokenAsync("test-realm", "second-token", "auth_client")
            .ConfigureAwait(false);

        Assert.IsFalse(first.IsError, first.ErrorMessage);
        Assert.IsFalse(second.IsError, second.ErrorMessage);
        CollectionAssert.AreEqual(new[] { "Bearer first-token" }, handler.AuthorizationHeaders[0]);
        CollectionAssert.AreEqual(new[] { "Bearer second-token" }, handler.AuthorizationHeaders[1]);
        Assert.AreEqual("factory-default", httpClient.DefaultRequestHeaders.Authorization?.Parameter);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<string[]> AuthorizationHeaders { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            AuthorizationHeaders.Add(request.Headers.GetValues("Authorization").ToArray());
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"rpt-token\",\"token_type\":\"Bearer\"}"),
                RequestMessage = request
            });
        }
    }
}
