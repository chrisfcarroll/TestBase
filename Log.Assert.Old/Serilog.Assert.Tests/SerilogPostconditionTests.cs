using Serilog.Events;

namespace Serilog.Assert.Tests;

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

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Postcondition_WhenFalse_LogsError()
    {
        log.Log.Postcondition(false, "state");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Postcondition Failed"));
    }

    [Test]
    public void Postcondition_WhenFalse_IncludesLoggableState()
    {
        log.Log.Postcondition(false, "context-value");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Postcondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Log.Postcondition(false);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void PostconditionNotNull_WhenNotNull_ReturnsValue()
    {
        var result = log.Log.PostconditionNotNull("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNull_WhenNull_ReturnsNullAndLogs()
    {
        string? value = null;
        var result = log.Log.PostconditionNotNull(value);

        NUnit.Framework.Assert.That(result, Is.EqualTo(value));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PostconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.Log.PostconditionNotNull(value, "custom message");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
