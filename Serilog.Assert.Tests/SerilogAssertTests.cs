using Serilog.Events;

namespace SerilogAssert.Tests;

[TestFixture]
public class SerilogAssertTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Assert_WhenTrue_DoesNotLog()
    {
        log.Log.Assert(true, "state");

        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Assert_WhenFalse_LogsError()
    {
        log.Log.Assert(false, "state");

        Assert.That(log.Entries, Has.Count.EqualTo(1));
        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(log.Last.Message, Does.Contain("Assertion Failed"));
    }

    [Test]
    public void Assert_WhenFalse_IncludesLoggableState()
    {
        log.Log.Assert(false, "context-value");

        Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Assert_WhenFalse_NullState_LogsEmpty()
    {
        log.Log.Assert(false);

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void AssertNotNull_WhenNotNull_ReturnsValue()
    {
        var result = log.Log.AssertNotNull("hello");

        Assert.That(result, Is.EqualTo("hello"));
        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNull_WhenNull_ReturnsNullAndLogs()
    {
        string? value = null;
        var result = log.Log.AssertNotNull(value);

        Assert.That(result, Is.EqualTo(value));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void AssertNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.Log.AssertNotNull(value, "custom message");

        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
