using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

/// <summary>
/// Tests for loggableState-style LogIf methods (preferred style).
/// </summary>
[TestFixture]
public class LogIfTests
{
    TestLogger _log = null!;

    [SetUp]
    public void SetUp() => _log = new TestLogger();

    // ── LogIf base ──────────────────────────────────────────────────────

    [Test]
    public void LogIf_WhenTrue_Logs()
    {
        var result = _log.LogIf(true, "some state");

        Assert.That(result, Is.True);
        Assert.That(_log.Entries, Has.Count.EqualTo(1));
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Information));
        Assert.That(_log.Last.Message, Does.Contain("some state"));
    }

    [Test]
    public void LogIf_WhenFalse_DoesNotLog()
    {
        var result = _log.LogIf(false, "some state");

        Assert.That(result, Is.False);
        Assert.That(_log.Entries, Is.Empty);
    }

    [Test]
    public void LogIf_NullState_LogsEmpty()
    {
        _log.LogIf(true);

        Assert.That(_log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void LogIf_RespectsLogLevel()
    {
        _log.LogIf(true, "state", LogLevel.Critical);

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Critical));
    }

    [Test]
    public void LogIf_IncludesStateName_WhenDistinctFromAction()
    {
        _log.LogIf(true, "myValue", stateName: "myLabel");

        Assert.That(_log.Last.Message, Does.Contain("myLabel:"));
        Assert.That(_log.Last.Message, Does.Contain("myValue"));
    }

    // ── LogWarnIf ───────────────────────────────────────────────────────

    [Test]
    public void LogWarnIf_WhenTrue_LogsAtWarning()
    {
        var result = _log.LogWarnIf(true, "warning state");

        Assert.That(result, Is.True);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Warning));
        Assert.That(_log.Last.Message, Does.Contain("warning state"));
    }

    [Test]
    public void LogWarnIf_WhenFalse_DoesNotLog()
    {
        Assert.That(_log.LogWarnIf(false, "x"), Is.False);
        Assert.That(_log.Entries, Is.Empty);
    }

    // ── LogErrorIf ──────────────────────────────────────────────────────

    [Test]
    public void LogErrorIf_WhenTrue_LogsAtError()
    {
        var result = _log.LogErrorIf(true, "err state");

        Assert.That(result, Is.True);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogErrorIf_WhenFalse_DoesNotLog()
    {
        Assert.That(_log.LogErrorIf(false, "x"), Is.False);
        Assert.That(_log.Entries, Is.Empty);
    }

    // ── LogInformationIf ────────────────────────────────────────────────

    [Test]
    public void LogInformationIf_WhenTrue_LogsAtInformation()
    {
        _log.LogInformationIf(true, "info state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    // ── LogDebugIf ──────────────────────────────────────────────────────

    [Test]
    public void LogDebugIf_WhenTrue_LogsAtDebug()
    {
        _log.LogDebugIf(true, "debug state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    // ── LogTraceIf ──────────────────────────────────────────────────────

    [Test]
    public void LogTraceIf_WhenTrue_LogsAtTrace()
    {
        _log.LogTraceIf(true, "trace state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Trace));
    }

    // ── LogExceptionIf ──────────────────────────────────────────────────

    [Test]
    public void LogExceptionIf_WhenTrue_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        var result = _log.LogExceptionIf(true, ex, "ctx");

        Assert.That(result, Is.True);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(_log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void LogExceptionIf_WhenFalse_DoesNotLog()
    {
        _log.LogExceptionIf(false, new Exception("x"), "ctx");

        Assert.That(_log.Entries, Is.Empty);
    }

    // ── LogExceptionThenThrowIf ─────────────────────────────────────────

    [Test]
    public void LogExceptionThenThrowIf_WhenTrue_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.LogExceptionThenThrowIf(true, ex, "ctx"));
        Assert.That(_log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogExceptionThenThrowIf_WhenFalse_DoesNothing()
    {
        _log.LogExceptionThenThrowIf(false, new Exception("x"));

        Assert.That(_log.Entries, Is.Empty);
    }

    // ── Return value semantics ──────────────────────────────────────────

    [Test]
    public void LogIf_ReturnsTrueWhenConditionTrue()
    {
        Assert.That(_log.LogIf(true), Is.True);
    }

    [Test]
    public void LogIf_ReturnsFalseWhenConditionFalse()
    {
        Assert.That(_log.LogIf(false), Is.False);
    }

    // ── ILoggable / ToLoggableState integration ─────────────────────────

    [Test]
    public void LogIf_UsesToLoggableState()
    {
        var obj = new HasToLoggableState();
        _log.LogIf(true, obj);

        Assert.That(_log.Last.Message, Does.Contain("custom-loggable"));
    }

    class HasToLoggableState
    {
        public object ToLoggableState() => "custom-loggable";
    }
}
