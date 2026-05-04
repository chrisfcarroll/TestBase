using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

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

        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Postcondition_WhenFalse_LogsError()
    {
        log.Postcondition(false, "state");

        Assert.That(log.Entries, Has.Count.EqualTo(1));
        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(log.Last.Message, Does.Contain("Postcondition Failed"));
    }

    [Test]
    public void Postcondition_WhenFalse_IncludesLoggableState()
    {
        log.Postcondition(false, "context-value");

        Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Postcondition_WhenFalse_NullState_LogsEmpty()
    {
        log.Postcondition(false);

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void PostconditionNotNull_WhenNotNull_ReturnsTrue()
    {
        var result = log.PostconditionNotNull("hello");

        Assert.That(result, Is.EqualTo("hello"));
        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNull_WhenNull_ReturnsFalseAndLogs()
    {
        string? value = null;
        var result = log.PostconditionNotNull(value);

        Assert.That(result, Is.EqualTo(value));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void PostconditionNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.PostconditionNotNull(value, "custom message");

        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
