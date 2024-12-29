using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents the result of an HTTP request execution, including request and response details, execution metrics,
/// and monitoring fallback information.
/// </summary>
public class KcHttpRequestExecutionResult : IDisposable
{
    /// <summary>
    /// Boolean flag to indicate whether the instance has been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Gets or sets the HTTP response message returned from the executed request.
    /// </summary>
    /// <seealso cref="HttpResponseMessage"/>
    [JsonProperty("responseMessage")]
    public HttpResponseMessage ResponseMessage { get; set; }

    /// <summary>
    /// Gets or sets the HTTP request message that was executed.
    /// </summary>
    /// <seealso cref="HttpRequestMessage"/>
    [JsonProperty("requestMessage")]
    public HttpRequestMessage RequestMessage { get; set; }

    /// <summary>
    /// Gets or sets the total time, in milliseconds, taken to execute the request.
    /// </summary>
    [JsonProperty("requestMilliseconds")]
    public double RequestMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets the monitoring fallback model used to track issues such as timeouts or SSL handshake errors.
    /// </summary>
    /// <seealso cref="KcHttpMonitoringFallbackModel"/>
    [JsonProperty("monitoringFallback")]
    public KcHttpMonitoringFallbackModel MonitoringFallback { get; set; }

    /// <summary>
    /// Gets or sets the exception, if any, that occurred during the request execution.
    /// </summary>
    [JsonProperty("exception")]
    public Exception Exception { get; set; }

    /// <summary>
    /// Disposes resources used by the instance, including the request and response messages.
    /// </summary>
    /// <param name="disposing">A value indicating whether the method is called from the Dispose method (true) or from the finalizer (false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if ( _isDisposed )
        {
            return;
        }

        if ( disposing )
        {
            ResponseMessage?.Dispose(); // Dispose the response message
            RequestMessage?.Dispose(); // Dispose the request message
        }

        _isDisposed = true;
    }

    /// <summary>
    /// Disposes resources used by the instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true); // Call Dispose with true indicating managed resources should be disposed
        GC.SuppressFinalize(this); // Suppress finalization to avoid redundant disposal
    }

    /// <summary>
    /// Finalizer to clean up unmanaged resources if Dispose is not called.
    /// </summary>
    ~KcHttpRequestExecutionResult()
    {
        Dispose(false); // Call Dispose with false indicating only unmanaged resources should be disposed
    }
}
