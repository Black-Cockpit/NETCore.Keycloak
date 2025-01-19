using Microsoft.Extensions.Logging;
using Moq;
using NETCore.Keycloak.Client.Exceptions;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.Tests.UtilsTests;

/// <summary>
/// Test suite for validating the functionality of the <see cref="KcLoggerMessages"/> class.
/// Ensures that log messages are correctly formatted and logged for various log levels.
/// </summary>
[TestClass]
[TestCategory("Initial")]
public class KcLoggerMessagesTests
{
    /// <summary>
    /// Mock instance of the <see cref="ILogger"/> used for verifying log invocations.
    /// </summary>
    private Mock<ILogger> _mockLogger;

    /// <summary>
    /// Initializes the mock logger before each test and configures it to enable all log levels.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger>();
        _ = _mockLogger.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
    }

    /// <summary>
    /// Verifies that a debug-level log message is correctly formatted and logged.
    /// </summary>
    [TestMethod]
    public void ShouldLogDebugMessage()
    {
        // Arrange
        const string message = "Debug message";
        var exception = new KcException("Debug exception");

        // Verify that the mock logger supports debug-level logging.
        Assert.IsTrue(_mockLogger.Object.IsEnabled(LogLevel.Debug));

        // Act
        KcLoggerMessages.Debug(_mockLogger.Object, message, exception);

        // Assert
        _mockLogger.Verify(logger =>
                logger.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that an error-level log message is correctly formatted and logged.
    /// </summary>
    [TestMethod]
    public void ShouldLogErrorMessage()
    {
        // Arrange
        const string message = "Error message";
        var exception = new KcException("Error exception");

        // Verify that the mock logger supports error-level logging.
        Assert.IsTrue(_mockLogger.Object.IsEnabled(LogLevel.Error));

        // Act
        KcLoggerMessages.Error(_mockLogger.Object, message, exception);

        // Assert
        _mockLogger.Verify(logger =>
                logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that a critical-level log message is correctly formatted and logged.
    /// </summary>
    [TestMethod]
    public void ShouldLogCriticalMessage()
    {
        // Arrange
        const string message = "Critical message";
        var exception = new KcException("Critical exception");

        // Verify that the mock logger supports critical-level logging.
        Assert.IsTrue(_mockLogger.Object.IsEnabled(LogLevel.Critical));

        // Act
        KcLoggerMessages.Critical(_mockLogger.Object, message, exception);

        // Assert
        _mockLogger.Verify(logger =>
                logger.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that an informational log message is correctly formatted and logged.
    /// </summary>
    [TestMethod]
    public void ShouldLogInformationMessage()
    {
        // Arrange
        const string message = "Information message";
        var exception = new KcException("Information exception");

        // Verify that the mock logger supports informational-level logging.
        Assert.IsTrue(_mockLogger.Object.IsEnabled(LogLevel.Information));

        // Act
        KcLoggerMessages.Information(_mockLogger.Object, message, exception);

        // Assert
        _mockLogger.Verify(logger =>
                logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
