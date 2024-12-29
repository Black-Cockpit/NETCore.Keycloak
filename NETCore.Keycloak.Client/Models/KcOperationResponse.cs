using NETCore.Keycloak.Client.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models;

/// <summary>
/// Represents a Keycloak operation response, extending <see cref="KcBaseResponse{T}"/> to handle
/// additional monitoring metrics for multiple API calls in a single operation.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class KcOperationResponse<T> : KcBaseResponse<T>
{
    /// <summary>
    /// Gets or sets the collection of monitoring metrics associated with the API operation.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcHttpApiMonitoringMetrics"/> instances containing monitoring data
    /// such as execution time, HTTP methods, and status codes for individual API requests involved in the operation,
    /// or <c>null</c> if no metrics are available.
    /// </value>
    [JsonProperty("monitoringMetrics")]
    public ICollection<KcHttpApiMonitoringMetrics> MonitoringMetrics { get; } =
        new List<KcHttpApiMonitoringMetrics>();
}
