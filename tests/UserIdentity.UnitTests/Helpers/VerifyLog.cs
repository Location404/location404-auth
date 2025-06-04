using Microsoft.Extensions.Logging;

using Moq;

using UserIdentity.Infra.Persistence;

namespace UserIdentity.UnitTests.Helpers;

public static class VerifyLogExtensions
{
    /// <summary>zzzzzz
    /// Verifies that a log message was logged with the specified log level and message.
    /// </summary>
    public static void VerifyLog(this Mock<ILogger<UnitOfWork>> logger, LogLevel logLevel, string message, Func<Times> times)
    {
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