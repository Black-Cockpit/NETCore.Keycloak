using NETCore.Keycloak.Client.Exceptions;

namespace NETCore.Keycloak.Client.Tests.ExceptionsTests;

/// <summary>
/// Test suite for validating the behavior of the <see cref="KcUserNotFoundException"/> class.
/// </summary>
[TestClass]
public class KcUserNotFoundExceptionTests
{
    /// <summary>
    /// Tests that a <see cref="KcUserNotFoundException"/> can be thrown without any message or inner exception.
    /// Verifies that the exception is correctly identified.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcUserNotFoundException() =>
        Assert.ThrowsException<KcUserNotFoundException>(() => throw new KcUserNotFoundException());

    /// <summary>
    /// Tests that a <see cref="KcUserNotFoundException"/> with a specified message retains the message correctly.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcUserNotFoundExceptionWithMessage()
    {
        // Create an exception with a specific message
        var exception = new KcUserNotFoundException("KcUserNotFoundExceptionWithMessage");

        // Verify the exception message
        Assert.AreEqual(exception.Message, "KcUserNotFoundExceptionWithMessage");
    }

    /// <summary>
    /// Tests that a <see cref="KcUserNotFoundException"/> can be created with both a message and an inner exception.
    /// </summary>
    [TestMethod]
    public void ShouldThrowKcUserNotFoundExceptionWithInnerException()
    {
        // Create an exception with a message and an inner exception
        var exception =
            new KcUserNotFoundException("KcUserNotFoundExceptionWithInnerException", new ArgumentNullException());

        // Verify the exception message
        Assert.AreEqual(exception.Message, "KcUserNotFoundExceptionWithInnerException");

        // Verify the inner exception
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<ArgumentNullException>(exception.InnerException);
    }
}
