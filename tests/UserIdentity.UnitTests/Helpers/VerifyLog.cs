using Microsoft.Extensions.Logging;

using Moq;

using UserIdentity.Infra.Persistence;

namespace UserIdentity.UnitTests.Helpers;

public static class VerifyLogExtensions
{
    /// <summary>
    /// Verifies that a log message was logged with the specified log level and message.
    /// </summary>
    /// 
    /// <param name="logLevel">The log level to verify.</param>
    /// <param name="message">The expected log message. If null or empty, verifies that any message was logged.</param>
    /// <param name="times">The number of times the log message should have been logged. Defaults to once.</param>
    public static void VerifyLog(this Mock<ILogger<UnitOfWork>> logger, LogLevel logLevel, string? message = "", Func<Times>? times = null)
    {
        times = Times.Once;

        if (string.IsNullOrWhiteSpace(message))
        {
            logger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }

        logger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == message),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}