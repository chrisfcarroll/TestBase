using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

[TestFixture]
public class LogIfTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    // ── LogIf base ──────────────────────────────────────────────────────

    [Test]
    public void LogIf_WhenTrue_Logs()
    {
        var result = log.LogIf(true, "some state");

        Assert.That(result, Is.True);
        Assert.That(log.Entries, Has.Count.EqualTo(1));
        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
        Assert.That(log.Last.Message, Does.Contain("some state"));
    }

    [Test]
    public void LogIf_WhenFalse_DoesNotLog()
    {
        var result = log.LogIf(false, "some state");

        Assert.That(result, Is.False);
        Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void LogIf_NullState_LogsEmpty()
    {
        log.LogIf(true);

        Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void LogIf_RespectsLogLevel()
    {
        log.LogIf(true, "state", logLevel:LogLevel.Critical);

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Critical));
    }

    [Test]
    public void LogIf_IncludesStateName_WhenDistinctFromAction()
    {
        log.LogIf(true, "myValue", helpfulLabel: "myLabel");

        Assert.That(log.Last.Message, Does.Contain("myLabel:"));
        Assert.That(log.Last.Message, Does.Contain("myValue"));
    }

    // ── LogWarnIf ───────────────────────────────────────────────────────

    [Test]
    public void LogWarnIf_WhenTrue_LogsAtWarning()
    {
        var result = log.LogWarnIf(true, "warning state");

        Assert.That(result, Is.True);
        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Warning));
        Assert.That(log.Last.Message, Does.Contain("warning state"));
    }

    [Test]
    public void LogWarnIf_WhenFalse_DoesNotLog()
    {
        Assert.That(log.LogWarnIf(false, "x"), Is.False);
        Assert.That(log.Entries, Is.Empty);
    }

    // ── LogErrorIf ──────────────────────────────────────────────────────

    [Test]
    public void LogErrorIf_WhenTrue_LogsAtError()
    {
        var result = log.LogErrorIf(true, "err state");

        Assert.That(result, Is.True);
        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogErrorIf_WhenFalse_DoesNotLog()
    {
        Assert.That(log.LogErrorIf(false, "x"), Is.False);
        Assert.That(log.Entries, Is.Empty);
    }

    // ── LogInformationIf ────────────────────────────────────────────────

    [Test]
    public void LogInformationIf_WhenTrue_LogsAtInformation()
    {
        log.LogIf(true, "info state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    // ── LogDebugIf ──────────────────────────────────────────────────────

    [Test]
    public void LogDebugIf_WhenTrue_LogsAtDebug()
    {
        log.LogDebugIf(true, "debug state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    // ── LogTraceIf ──────────────────────────────────────────────────────

    [Test]
    public void LogTraceIf_WhenTrue_LogsAtTrace()
    {
        log.LogTraceIf(true, "trace state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Trace));
    }

    // ── LogExceptionIf ──────────────────────────────────────────────────

    [Test]
    public void LogExceptionIf_WhenTrue_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        var result = log.LogExceptionIf(true, ex, "ctx");

        Assert.That(result, Is.True);
        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void LogExceptionIf_WhenFalse_DoesNotLog()
    {
        log.LogExceptionIf(false, new Exception("x"), "ctx");

        Assert.That(log.Entries, Is.Empty);
    }

    // ── LogExceptionThenThrowIf ─────────────────────────────────────────

    [Test]
    public void LogExceptionThenThrowIf_WhenTrue_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            log.LogExceptionAndThrowIf(true, ex, "ctx"));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogExceptionThenThrowIf_WhenFalse_DoesNothing()
    {
        log.LogExceptionAndThrowIf(false, new Exception("x"));

        Assert.That(log.Entries, Is.Empty);
    }

    // ── Return value semantics ──────────────────────────────────────────

    [Test]
    public void LogIf_ReturnsTrueWhenConditionTrue()
    {
        Assert.That(log.LogIf(true), Is.True);
    }

    [Test]
    public void LogIf_ReturnsFalseWhenConditionFalse()
    {
        Assert.That(log.LogIf(false), Is.False);
    }

    // ── ILoggable / ToLoggableState integration ─────────────────────────

    [Test]
    public void LogIf_UsesToLoggableState()
    {
        var loggableValue = new HasToLoggableState();
        log.LogIf(true, loggableValue);

        Assert.That(log.Last.Message, Does.Contain("custom-loggable"));
    }

    class HasToLoggableState
    {
        // ReSharper disable UnusedMember.Local
        public object ToLoggableState() => "custom-loggable";
        // ReSharper restore UnusedMember.Local
    }
}
