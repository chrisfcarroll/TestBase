using Serilog.Events;

namespace Serilog.Assert.Tests;

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

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Precondition_WhenFalse_LogsError()
    {
        log.Log.Precondition(false, "state");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Precondition Failed"));
    }

    [Test]
    public void Precondition_WhenFalse_IncludesLoggableState()
    {
        log.Log.Precondition(false, "context-value");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Precondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Log.Precondition(false);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void PreconditionNotNull_WhenNotNull_ReturnsValue()
    {
        var result = log.Log.PreconditionNotNull("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNull_WhenNull_ReturnsNullAndLogs()
    {
        string? value = null;
        var result = log.Log.PreconditionNotNull(value);

        NUnit.Framework.Assert.That(result, Is.EqualTo(value));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PreconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.Log.PreconditionNotNull(value, "custom message");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
