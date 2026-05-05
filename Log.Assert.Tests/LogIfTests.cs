using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

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

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("some state"));
    }

    [Test]
    public void LogIf_WhenFalse_DoesNotLog()
    {
        var result = log.LogIf(false, "some state");

        NUnit.Framework.Assert.That(result, Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void LogIf_NullState_LogsEmpty()
    {
        log.LogIf(true);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void LogIf_RespectsLogLevel()
    {
        log.LogIf(true, "state", logLevel:LogLevel.Critical);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Critical));
    }

    [Test]
    public void LogIf_IncludesStateName_WhenDistinctFromAction()
    {
        log.LogIf(true, "myValue", helpfulLabel: "myLabel");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myLabel:"));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myValue"));
    }

    // ── LogWarnIf ───────────────────────────────────────────────────────

    [Test]
    public void LogWarnIf_WhenTrue_LogsAtWarning()
    {
        var result = log.LogWarnIf(true, "warning state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Warning));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("warning state"));
    }

    [Test]
    public void LogWarnIf_WhenFalse_DoesNotLog()
    {
        NUnit.Framework.Assert.That(log.LogWarnIf(false, "x"), Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── LogErrorIf ──────────────────────────────────────────────────────

    [Test]
    public void LogErrorIf_WhenTrue_LogsAtError()
    {
        var result = log.LogErrorIf(true, "err state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogErrorIf_WhenFalse_DoesNotLog()
    {
        NUnit.Framework.Assert.That(log.LogErrorIf(false, "x"), Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── LogInformationIf ────────────────────────────────────────────────

    [Test]
    public void LogInformationIf_WhenTrue_LogsAtInformation()
    {
        log.LogIf(true, "info state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    // ── LogDebugIf ──────────────────────────────────────────────────────

    [Test]
    public void LogDebugIf_WhenTrue_LogsAtDebug()
    {
        log.LogDebugIf(true, "debug state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    // ── LogTraceIf ──────────────────────────────────────────────────────

    [Test]
    public void LogTraceIf_WhenTrue_LogsAtTrace()
    {
        log.LogTraceIf(true, "trace state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Trace));
    }

    // ── LogExceptionIf ──────────────────────────────────────────────────

    [Test]
    public void LogExceptionIf_WhenTrue_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        var result = log.LogExceptionIf(true, ex, "ctx");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void LogExceptionIf_WhenFalse_DoesNotLog()
    {
        log.LogExceptionIf(false, new Exception("x"), "ctx");

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── LogExceptionThenThrowIf ─────────────────────────────────────────

    [Test]
    public void LogExceptionThenThrowIf_WhenTrue_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() =>
                                                                     log.LogExceptionAndThrowIf(true, ex, "ctx"));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogExceptionThenThrowIf_WhenFalse_DoesNothing()
    {
        log.LogExceptionAndThrowIf(false, new Exception("x"));

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── Return value semantics ──────────────────────────────────────────

    [Test]
    public void LogIf_ReturnsTrueWhenConditionTrue()
    {
        NUnit.Framework.Assert.That(log.LogIf(true), Is.True);
    }

    [Test]
    public void LogIf_ReturnsFalseWhenConditionFalse()
    {
        NUnit.Framework.Assert.That(log.LogIf(false), Is.False);
    }

    // ── ILoggable / ToLoggableState integration ─────────────────────────

    [Test]
    public void LogIf_UsesToLoggableState()
    {
        var loggableValue = new HasToLoggableState();
        log.LogIf(true, loggableValue);

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("custom-loggable"));
    }

    class HasToLoggableState
    {
        // ReSharper disable UnusedMember.Local
        public object ToLoggableState() => "custom-loggable";
        // ReSharper restore UnusedMember.Local
    }
}
