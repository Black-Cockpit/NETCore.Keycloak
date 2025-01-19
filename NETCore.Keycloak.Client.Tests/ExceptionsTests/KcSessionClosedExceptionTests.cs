using NETCore.Keycloak.Client.Exceptions;

namespace NETCore.Keycloak.Client.Tests.ExceptionsTests;

/// <summary>
/// Test suite for validating the behavior of the <see cref="KcSessionClosedException"/> class.
/// </summary>
[TestClass]
public class KcSessionClosedExceptionTests
{
    /// <summary>
    /// Tests that a <see cref="KcSessionClosedException"/> can be thrown without any message or inner exception.
    /// Verifies that the exception is correctly identified.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcSessionClosedException() =>
        Assert.ThrowsException<KcSessionClosedException>(() => throw new KcSessionClosedException());

    /// <summary>
    /// Tests that a <see cref="KcSessionClosedException"/> with a specified message retains the message correctly.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcSessionClosedExceptionWithMessage()
    {
        // Create an exception with a specific message
        var exception = new KcSessionClosedException("KcSessionClosedExceptionWithMessage");

        // Verify the exception message
        Assert.AreEqual(exception.Message, "KcSessionClosedExceptionWithMessage");
    }

    /// <summary>
    /// Tests that a <see cref="KcSessionClosedException"/> can be created with both a message and an inner exception.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcSessionClosedExceptionWithInnerException()
    {
        // Create an exception with a message and an inner exception
        var exception =
            new KcSessionClosedException("KcSessionClosedExceptionWithInnerException", new ArgumentNullException());

        // Verify the exception message
        Assert.AreEqual(exception.Message, "KcSessionClosedExceptionWithInnerException");

        // Verify the inner exception
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<ArgumentNullException>(exception.InnerException);
    }
}
