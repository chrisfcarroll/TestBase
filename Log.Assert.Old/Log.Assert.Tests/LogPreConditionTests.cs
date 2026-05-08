using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

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

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Precondition_WhenFalse_LogsError()
    {
        log.Precondition(false, "state");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Precondition Failed"));
    }

    [Test]
    public void Precondition_WhenFalse_IncludesLoggableState()
    {
        log.Precondition(false, "context-value");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Precondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Precondition(false);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void PreconditionNotNull_WhenNotNull_ReturnsTrue()
    {
        var result = log.PreconditionNotNull("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNull_WhenNull_ReturnsFalseAndLogs()
    {
        string? value = null;
        var result = log.PreconditionNotNull(value);

        NUnit.Framework.Assert.That(result, Is.EqualTo(value));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PreconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.PreconditionNotNull(value, "custom message");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
