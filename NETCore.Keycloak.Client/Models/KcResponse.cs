using NETCore.Keycloak.Client.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models;

/// <summary>
/// Represents a generic Keycloak request response, encapsulating the result of an API call.
/// Inherits from <see cref="KcBaseResponse{T}"/> to include common properties for API responses.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class KcResponse<T> : KcBaseResponse<T>
{
    /// <summary>
    /// Gets or sets the monitoring metrics associated with the API request.
    /// </summary>
    /// <value>
    /// An instance of <see cref="KcHttpApiMonitoringMetrics"/> containing monitoring data,
    /// including details such as execution time, HTTP method, and status code,
    /// or <c>null</c> if monitoring metrics are not available for the request.
    /// </value>
    [JsonProperty("monitoringMetrics")]
    public KcHttpApiMonitoringMetrics MonitoringMetrics { get; set; }
}
