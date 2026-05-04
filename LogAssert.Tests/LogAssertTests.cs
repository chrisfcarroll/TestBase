using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

[TestFixture]
public class LogAssertTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Assert_WhenTrue_DoesNotLog()
    {
        log.Assert(true, "state");

        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Assert_WhenFalse_LogsError()
    {
        log.Assert(false, "state");

        Assert.That(log.Entries, Has.Count.EqualTo(1));
        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(log.Last.Message, Does.Contain("Assertion Failed"));
    }

    [Test]
    public void Assert_WhenFalse_IncludesLoggableState()
    {
        log.Assert(false, "context-value");

        Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Assert_WhenFalse_NullState_LogsEmpty()
    {
        log.Assert(false);

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void AssertNotNull_WhenNotNull_ReturnsTrue()
    {
        var result = log.AssertNotNull("hello");

        Assert.That(result, Is.EqualTo("hello"));
        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNull_WhenNull_ReturnsFalseAndLogs()
    {
        string? value = null;
        var result = log.AssertNotNull(value);

        Assert.That(result, Is.EqualTo(value));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void AssertNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.AssertNotNull(value, "custom message");

        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
