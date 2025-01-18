# Monitoring and Metrics

The Keycloak .NET Client provides built-in monitoring capabilities through metrics that are included with every response. These metrics help track API performance, diagnose issues, and monitor the health of your Keycloak integration.

## Monitoring Metrics Structure

### KcHttpApiMonitoringMetrics

The base metrics class included in every response:

```csharp
public class KcHttpApiMonitoringMetrics
{
    // Time taken for the request in milliseconds
    public long RequestMilliseconds { get; set; }
    
    // HTTP method used (GET, POST, etc.)
    public string Method { get; set; }
    
    // HTTP status code
    public int StatusCode { get; set; }
    
    // Request URL
    public string RequestUri { get; set; }
}
```

## Types of Monitoring

### Single Operation Monitoring

For single API operations, metrics are available through the `MonitoringMetrics` property of `KcResponse<T>`:

```csharp
var response = await keycloakClient.SomeOperation();
var metrics = response.MonitoringMetrics;

// Access metrics
var executionTime = metrics.RequestMilliseconds;
var httpMethod = metrics.Method;
var statusCode = metrics.StatusCode;
var requestUrl = metrics.RequestUri;
```

### Multi-Operation Monitoring

For operations that involve multiple API calls, metrics are available as a collection through `KcOperationResponse<T>`:

```csharp
var response = await keycloakClient.ComplexOperation();
var metricsCollection = response.MonitoringMetrics;

foreach (var metrics in metricsCollection)
{
    // Access metrics for each API call
    logger.LogInformation($"API Call to {metrics.RequestUri} took {metrics.RequestMilliseconds}ms");
}
```

## Using Monitoring Data

### Performance Monitoring

Track API performance and identify bottlenecks:

```csharp
public async Task MonitorOperationPerformance()
{
    var response = await keycloakClient.SomeOperation();
    var metrics = response.MonitoringMetrics;
    
    if (metrics.RequestMilliseconds > 1000) // More than 1 second
    {
        logger.LogWarning($"Slow operation detected: {metrics.Method} {metrics.RequestUri}");
    }
}
```

### Error Diagnosis

Use metrics to diagnose API errors:

```csharp
public async Task DiagnoseOperation()
{
    var response = await keycloakClient.SomeOperation();
    var metrics = response.MonitoringMetrics;
    
    if (response.IsError)
    {
        logger.LogError(
            "Operation failed: Status {StatusCode}, Method {Method}, URL {Url}, Duration {Duration}ms",
            metrics.StatusCode,
            metrics.Method,
            metrics.RequestUri,
            metrics.RequestMilliseconds
        );
    }
}
```

### Health Monitoring

Implement health checks using metrics:

```csharp
public class KeycloakHealthCheck : IHealthCheck
{
    private readonly IKeycloakClient _keycloakClient;
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _keycloakClient.SomeOperation();
            var metrics = response.MonitoringMetrics;
            
            var data = new Dictionary<string, object>
            {
                { "StatusCode", metrics.StatusCode },
                { "ResponseTime", metrics.RequestMilliseconds }
            };
            
            return metrics.StatusCode < 400
                ? HealthCheckResult.Healthy("Keycloak is healthy", data)
                : HealthCheckResult.Unhealthy("Keycloak returned error", null, data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Keycloak health check failed", ex);
        }
    }
}
```

## Best Practices

1. **Regular Monitoring**
   - Implement regular monitoring of API response times
   - Set up alerts for slow operations or high error rates
   - Track trends in performance metrics

2. **Logging Strategy**
   - Log metrics for failed operations with detailed context
   - Include metrics in application monitoring systems
   - Use structured logging for better analysis

3. **Performance Thresholds**
   - Define acceptable performance thresholds
   - Monitor operations exceeding thresholds
   - Investigate and optimize slow operations

4. **Error Tracking**
   - Correlate errors with metrics data
   - Track error rates and patterns
   - Use metrics for troubleshooting

## Integration with Monitoring Systems

### Example: Prometheus Integration

```csharp
public class KeycloakMetricsCollector
{
    private readonly Counter _requestCounter;
    private readonly Histogram _requestDuration;
    
    public KeycloakMetricsCollector()
    {
        _requestCounter = Metrics.CreateCounter(
            "keycloak_api_requests_total",
            "Total number of Keycloak API requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "status_code" }
            });
            
        _requestDuration = Metrics.CreateHistogram(
            "keycloak_api_request_duration_ms",
            "Keycloak API request duration",
            new HistogramConfiguration
            {
                LabelNames = new[] { "method" }
            });
    }
    
    public void RecordMetrics(KcHttpApiMonitoringMetrics metrics)
    {
        _requestCounter.WithLabels(metrics.Method, metrics.StatusCode.ToString()).Inc();
        _requestDuration.WithLabels(metrics.Method).Observe(metrics.RequestMilliseconds);
    }
}
```

### Example: Application Insights Integration

```csharp
public class KeycloakTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is DependencyTelemetry dependency)
        {
            // Add Keycloak metrics to dependency tracking
            dependency.Properties["RequestMilliseconds"] = 
                dependency.Context.Properties["KeycloakRequestMilliseconds"];
            dependency.Properties["Method"] = 
                dependency.Context.Properties["KeycloakMethod"];
        }
    }
}
```
