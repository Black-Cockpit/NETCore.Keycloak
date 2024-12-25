namespace NETCore.Keycloak;

/// <summary>
/// Represents an exception that occurs when a Keycloak session is closed unexpectedly.
/// </summary>
[Serializable]
public class KcSessionClosedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KcSessionClosedException"/> class with a default message.
    /// </summary>
    public KcSessionClosedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcSessionClosedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KcSessionClosedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcSessionClosedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public KcSessionClosedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
