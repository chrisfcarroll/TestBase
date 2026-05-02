using Serilog.Events;

namespace SerilogAssert.Tests;

[TestFixture]
public class SerilogPreconditionTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Precondition_WhenTrue_DoesNotLog()
    {
        log.Log.Precondition(true, "state");

        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Precondition_WhenFalse_LogsError()
    {
        log.Log.Precondition(false, "state");

        Assert.That(log.Entries, Has.Count.EqualTo(1));
        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(log.Last.Message, Does.Contain("Precondition Failed"));
    }

    [Test]
    public void Precondition_WhenFalse_IncludesLoggableState()
    {
        log.Log.Precondition(false, "context-value");

        Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Precondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Log.Precondition(false);

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void PreconditionNotNull_WhenNotNull_ReturnsValue()
    {
        var result = log.Log.PreconditionNotNull("hello");

        Assert.That(result, Is.EqualTo("hello"));
        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNull_WhenNull_ReturnsNullAndLogs()
    {
        string? value = null;
        var result = log.Log.PreconditionNotNull(value);

        Assert.That(result, Is.EqualTo(value));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PreconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.Log.PreconditionNotNull(value, "custom message");

        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
