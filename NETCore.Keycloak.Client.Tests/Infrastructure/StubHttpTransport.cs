using System.Net;
using System.Text;

namespace NETCore.Keycloak.Client.Tests.Infrastructure;

internal sealed class StubHttpTransport(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond)
    : HttpMessageHandler, IHttpClientFactory
{
    public HttpClient CreateClient(string name) => new(this, disposeHandler: false);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken) => respond(request);

    public static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK) => new(status)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };
}
