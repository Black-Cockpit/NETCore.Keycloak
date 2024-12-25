namespace NETCore.Keycloak.Client;

/// <summary>
/// Represents an exception that occurs when a specified Keycloak user is not found.
/// </summary>
[Serializable]
public class KcUserNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KcUserNotFoundException"/> class with a default message.
    /// </summary>
    public KcUserNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcUserNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KcUserNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcUserNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public KcUserNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
