using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

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

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void Assert_WhenFalse_LogsError()
    {
        log.Assert(false, "state");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Assertion Failed"));
    }

    [Test]
    public void Assert_WhenFalse_IncludesLoggableState()
    {
        log.Assert(false, "context-value");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Assert_WhenFalse_NullState_LogsEmpty()
    {
        log.Assert(false);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void AssertNotNull_WhenNotNull_ReturnsTrue()
    {
        var result = log.AssertNotNull("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNull_WhenNull_ReturnsFalseAndLogs()
    {
        string? value = null;
        var result = log.AssertNotNull(value);

        NUnit.Framework.Assert.That(result, Is.EqualTo(value));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void AssertNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        log.AssertNotNull(value, "custom message");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }
}
