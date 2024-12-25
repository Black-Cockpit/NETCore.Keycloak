namespace NETCore.Keycloak;

/// <summary>
/// Represents an exception specific to Keycloak operations.
/// </summary>
[Serializable]
public class KcException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KcException"/> class.
    /// </summary>
    public KcException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KcException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public KcException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
