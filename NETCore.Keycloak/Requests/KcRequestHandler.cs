using System.Net;
using System.Net.Http.Headers;
using NETCore.Keycloak.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NETCore.Keycloak.Requests;

/// <summary>
/// Keycloak request response handler
/// </summary>
public static class KcRequestHandler
{
    /// <summary>
    /// Handle keycloak request response
    /// </summary>
    /// <param name="response">Keycloak response <see cref="HttpResponseMessage"/></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<KcResponse<T>> HandleAsync<T>(HttpResponseMessage response,
        CancellationToken cancellationToken = default) => response?.IsSuccessStatusCode != true
        ? new KcResponse<T>
        {
            IsError = true,
            ErrorMessage = response != null
                ? await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
                : null
        }
        : response.StatusCode == HttpStatusCode.NoContent
            ? new KcResponse<T>()
            : new KcResponse<T>
            {
                Response =
                    JsonConvert.DeserializeObject<T>(
                        await response.Content.ReadAsStringAsync(cancellationToken)
                            .ConfigureAwait(false)),
            };

    /// <summary>
    /// Create http request message
    /// </summary>
    /// <param name="method">Http Method <see cref="HttpMethod"/></param>
    /// <param name="endpoint">Keycloak endpoint</param>
    /// <param name="accessToken">Admin access token</param>
    /// <param name="content">Request body, <see cref="HttpContent"/></param>
    /// <param name="contentType">Request content type header</param>
    /// <returns></returns>
    public static HttpRequestMessage CreateRequest(HttpMethod method, string endpoint,
        string accessToken, HttpContent content = null, string contentType = "application/json")
    {
        var request = new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(endpoint),
            Content = content ?? new StringContent(string.Empty)
        };

        if ( request.Content.Headers.ContentType != null )
        {
            request.Content.Headers.ContentType.MediaType = contentType;
            request.Content.Headers.ContentType.CharSet = null;
        }

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

        if ( !string.IsNullOrWhiteSpace(accessToken) )
        {
            _ = request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
        }

        return request;
    }

    /// <summary>
    /// Body builder
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static StringContent GetBody(object o)
        => new(JsonConvert.SerializeObject(o, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            }
        }));
}
