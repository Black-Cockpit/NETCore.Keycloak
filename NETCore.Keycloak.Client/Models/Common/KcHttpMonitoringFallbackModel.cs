using System.Net;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents a fallback model for HTTP monitoring, containing essential metrics and information
/// about a request when a detailed monitoring result is unavailable.
/// </summary>
public class KcHttpMonitoringFallbackModel
{
    /// <summary>
    /// Gets or sets the HTTP status code returned by the API response.
    /// </summary>
    [JsonProperty("statusCode")]
    public HttpStatusCode? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the URL of the API endpoint associated with the request.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method (e.g., GET, POST) used in the request.
    /// </summary>
    [JsonProperty("httpMethod")]
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the duration of the request in milliseconds.
    /// </summary>
    [JsonProperty("requestMilliseconds")]
    public double RequestMilliseconds { get; set; }
}
