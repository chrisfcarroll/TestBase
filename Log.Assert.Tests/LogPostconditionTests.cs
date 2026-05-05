using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

[TestFixture]
public class LogPostconditionTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Postcondition_WhenTrue_DoesNotLog()
    {
        log.Postcondition(true, "state");

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Postcondition_WhenFalse_LogsError()
    {
        log.Postcondition(false, "state");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Postcondition Failed"));
    }

    [Test]
    public void Postcondition_WhenFalse_IncludesLoggableState()
    {
        log.Postcondition(false, "context-value");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Postcondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Postcondition(false);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void PostconditionNotNull_WhenNotNull_ReturnsTrue()
    {
        var result = log.PostconditionNotNull("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNull_WhenNull_ReturnsFalseAndLogs()
    {
        string? value = null;
        var result = log.PostconditionNotNull(value);

        NUnit.Framework.Assert.That(result, Is.EqualTo(value));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PostconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.PostconditionNotNull(value, "custom message");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
