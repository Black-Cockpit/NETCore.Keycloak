using NETCore.Keycloak.Client.Exceptions;

namespace NETCore.Keycloak.Client.Tests.ExceptionsTests;

/// <summary>
/// Test suite for validating the behavior of the <see cref="KcException"/> class.
/// </summary>
[TestClass]
public class KcExceptionTests
{
    /// <summary>
    /// Tests that a <see cref="KcException"/> can be thrown without any message or inner exception.
    /// Verifies that the exception is correctly identified.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcException() => Assert.ThrowsException<KcException>(() => throw new KcException());

    /// <summary>
    /// Tests that a <see cref="KcException"/> with a specified message retains the message correctly.
    /// Verifies that the exception's <see cref="KcException"/> property matches the provided message.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcExceptionWithMessage()
    {
        // Create an exception with a specific message
        var exception = new KcException("KcExceptionWithMessage");

        // Verify the exception message
        Assert.AreEqual(exception.Message, "KcExceptionWithMessage");
    }

    /// <summary>
    /// Tests that a <see cref="KcException"/> can be created with both a message and an inner exception.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcExceptionWithInnerException()
    {
        // Create an exception with a message and an inner exception
        var exception = new KcException("KcExceptionWithInnerException", new ArgumentNullException());

        // Verify the exception message
        Assert.AreEqual(exception.Message, "KcExceptionWithInnerException");

        // Verify the inner exception
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<ArgumentNullException>(exception.InnerException);
    }
}
