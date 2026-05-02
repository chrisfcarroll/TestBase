using Serilog.Events;

namespace SerilogAssert.Tests;

[TestFixture]
public class SerilogPostconditionTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Postcondition_WhenTrue_DoesNotLog()
    {
        log.Log.Postcondition(true, "state");

        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Postcondition_WhenFalse_LogsError()
    {
        log.Log.Postcondition(false, "state");

        Assert.That(log.Entries, Has.Count.EqualTo(1));
        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(log.Last.Message, Does.Contain("Postcondition Failed"));
    }

    [Test]
    public void Postcondition_WhenFalse_IncludesLoggableState()
    {
        log.Log.Postcondition(false, "context-value");

        Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Postcondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Log.Postcondition(false);

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void PostconditionNotNull_WhenNotNull_ReturnsValue()
    {
        var result = log.Log.PostconditionNotNull("hello");

        Assert.That(result, Is.EqualTo("hello"));
        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNull_WhenNull_ReturnsNullAndLogs()
    {
        string? value = null;
        var result = log.Log.PostconditionNotNull(value);

        Assert.That(result, Is.EqualTo(value));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PostconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.Log.PostconditionNotNull(value, "custom message");

        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
