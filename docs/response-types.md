# Response Types

All operations in the Keycloak .NET Client return either `KcResponse<T>` or `KcOperationResponse<T>` where T is the specific response type for the operation.

## KcResponse<T>

The base response type for single API operations:

```csharp
public class KcResponse<T>
{
    // The actual response data
    public T Response { get; set; }
    
    // Indicates if the request resulted in an error
    public bool IsError { get; set; }
    
    // Exception details if an error occurred
    public Exception Exception { get; set; }
    
    // Monitoring metrics for the request
    public KcHttpApiMonitoringMetrics MonitoringMetrics { get; set; }
}
```

## KcOperationResponse<T>

Used for operations that involve multiple API calls:

```csharp
public class KcOperationResponse<T> : KcBaseResponse<T>
{
    // Collection of monitoring metrics for multiple API calls
    public ICollection<KcHttpApiMonitoringMetrics> MonitoringMetrics { get; }
}
```

For detailed information about monitoring metrics and how to use them for debugging and performance monitoring, see [Monitoring and Metrics](monitoring.md).

## Error Handling

All operations should be handled by checking the `IsError` property and accessing the error details when needed:

```csharp
try
{
    var response = await keycloakClient.SomeOperation();

    if (response.IsError)
    {
        // Handle error using response.Exception
        logger.LogError($"Operation failed: {response.Exception.Message}");
        
        // Handle specific error cases
        if (response.Exception is KcAuthenticationException)
        {
            // Handle authentication-specific errors
        }
    }
    else
    {
        // Process successful response
        var result = response.Response;
    }
}
catch (Exception ex)
{
    // Handle unexpected errors
    logger.LogError($"Unexpected error: {ex.Message}");
}
