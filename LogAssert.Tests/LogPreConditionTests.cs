using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

[TestFixture]
public class LogPreconditionTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Precondition_WhenTrue_DoesNotLog()
    {
        log.Precondition(true, "state");

        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Precondition_WhenFalse_LogsError()
    {
        log.Precondition(false, "state");

        Assert.That(log.Entries, Has.Count.EqualTo(1));
        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(log.Last.Message, Does.Contain("Precondition Failed"));
    }

    [Test]
    public void Precondition_WhenFalse_IncludesLoggableState()
    {
        log.Precondition(false, "context-value");

        Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Precondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Precondition(false);

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void PreconditionNotNull_WhenNotNull_ReturnsTrue()
    {
        var result = log.PreconditionNotNull("hello");

        Assert.That(result, Is.EqualTo("hello"));
        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNull_WhenNull_ReturnsFalseAndLogs()
    {
        string? value = null;
        var result = log.PreconditionNotNull(value);

        Assert.That(result, Is.EqualTo(value));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PreconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.PreconditionNotNull(value, "custom message");

        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
