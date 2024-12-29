using System.Diagnostics;
using NETCore.Keycloak.Client.Models.Common;

namespace NETCore.Keycloak.Client.HttpClients;

/// <summary>
/// Provides functionality for executing HTTP requests with monitoring and error handling support.
/// </summary>
public static class KcHttpRequestExecutor
{
    /// <summary>
    /// Executes an HTTP request and captures execution metrics, including fallback data in case of exceptions.
    /// </summary>
    /// <param name="request">The function representing the HTTP request to be executed.</param>
    /// <param name="monitoringFallbackModel">
    /// The monitoring fallback model used to track execution metrics and errors.
    /// <see cref="KcHttpMonitoringFallbackModel"/>
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a <see cref="KcHttpMonitoringFallbackModel"/>
    /// with the result of the HTTP request and its associated metrics.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="request"/> or <paramref name="monitoringFallbackModel"/> is null.
    /// </exception>
    public static async Task<KcHttpRequestExecutionResult> ExecuteRequest(
        Func<Task<HttpResponseMessage>> request,
        KcHttpMonitoringFallbackModel monitoringFallbackModel)
    {
        // Ensure the request function is not null
        ArgumentNullException.ThrowIfNull(request);

        // Ensure the monitoring fallback model is not null
        ArgumentNullException.ThrowIfNull(monitoringFallbackModel);

        // Start a timer to measure request execution time
        var timer = Stopwatch.StartNew();

        try
        {
            // Execute the HTTP request
            var response = await request.Invoke().ConfigureAwait(false);

            // Stop the timer after the request completes successfully
            timer.Stop();

            // Record the elapsed time in the monitoring fallback model
            monitoringFallbackModel.RequestMilliseconds = timer.ElapsedMilliseconds;

            // Return the execution result with the captured metrics and response data
            return new KcHttpRequestExecutionResult
            {
                RequestMilliseconds = timer.ElapsedMilliseconds,
                ResponseMessage = response,
                RequestMessage = response.RequestMessage,
                MonitoringFallback = monitoringFallbackModel
            };
        }
        catch ( Exception e )
        {
            // Stop the timer if an exception occurs
            timer.Stop();

            // Record the elapsed time in the monitoring fallback model
            monitoringFallbackModel.RequestMilliseconds = timer.ElapsedMilliseconds;

            // Return the execution result with the exception details and fallback metrics
            return new KcHttpRequestExecutionResult
            {
                RequestMilliseconds = timer.ElapsedMilliseconds,
                Exception = e,
                MonitoringFallback = monitoringFallbackModel
            };
        }
    }
}
