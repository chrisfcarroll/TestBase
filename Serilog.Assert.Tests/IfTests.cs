using Serilog.Events;

namespace SerilogAssert.Tests;

/// <summary>
/// Tests for loggableState-style If methods (preferred style).
/// </summary>
[TestFixture]
public class IfTests
{
    TestLogger _tl = null!;
    ILogger _log = null!;

    [SetUp]
    public void SetUp()
    {
        _tl = new TestLogger();
        _log = _tl.Log;
    }

    // ── If base ─────────────────────────────────────────────────────────

    [Test]
    public void If_WhenTrue_Logs()
    {
        var result = _log.If(true, "some state");

        Assert.That(result, Is.True);
        Assert.That(_tl.Entries, Has.Count.EqualTo(1));
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Information));
        Assert.That(_tl.Last.Message, Does.Contain("some state"));
    }

    [Test]
    public void If_WhenFalse_DoesNotLog()
    {
        var result = _log.If(false, "some state");

        Assert.That(result, Is.False);
        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void If_NullState_LogsEmpty()
    {
        _log.If(true);

        Assert.That(_tl.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void If_RespectsLogLevel()
    {
        _log.If(true, "state", LogEventLevel.Fatal);

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Fatal));
    }

    [Test]
    public void If_IncludesStateName_WhenDistinctFromAction()
    {
        _log.If(true, "myValue", stateName: "myLabel");

        Assert.That(_tl.Last.Message, Does.Contain("myLabel:"));
        Assert.That(_tl.Last.Message, Does.Contain("myValue"));
    }

    // ── WarnIf ──────────────────────────────────────────────────────────

    [Test]
    public void WarnIf_WhenTrue_LogsAtWarning()
    {
        var result = _log.WarnIf(true, "warning state");

        Assert.That(result, Is.True);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Warning));
        Assert.That(_tl.Last.Message, Does.Contain("warning state"));
    }

    [Test]
    public void WarnIf_WhenFalse_DoesNotLog()
    {
        Assert.That(_log.WarnIf(false, "x"), Is.False);
        Assert.That(_tl.Entries, Is.Empty);
    }

    // ── ErrorIf ─────────────────────────────────────────────────────────

    [Test]
    public void ErrorIf_WhenTrue_LogsAtError()
    {
        var result = _log.ErrorIf(true, "err state");

        Assert.That(result, Is.True);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void ErrorIf_WhenFalse_DoesNotLog()
    {
        Assert.That(_log.ErrorIf(false, "x"), Is.False);
        Assert.That(_tl.Entries, Is.Empty);
    }

    // ── InformationIf ───────────────────────────────────────────────────

    [Test]
    public void InformationIf_WhenTrue_LogsAtInformation()
    {
        _log.InformationIf(true, "info state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Information));
    }

    // ── DebugIf ─────────────────────────────────────────────────────────

    [Test]
    public void DebugIf_WhenTrue_LogsAtDebug()
    {
        _log.DebugIf(true, "debug state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Debug));
    }

    // ── VerboseIf ───────────────────────────────────────────────────────

    [Test]
    public void VerboseIf_WhenTrue_LogsAtVerbose()
    {
        _log.VerboseIf(true, "verbose state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Verbose));
    }

    // ── ExceptionIf ─────────────────────────────────────────────────────

    [Test]
    public void ExceptionIf_WhenTrue_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        var result = _log.ExceptionIf(true, ex, "ctx");

        Assert.That(result, Is.True);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(_tl.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void ExceptionIf_WhenFalse_DoesNotLog()
    {
        _log.ExceptionIf(false, new Exception("x"), "ctx");

        Assert.That(_tl.Entries, Is.Empty);
    }

    // ── ExceptionThenThrowIf ────────────────────────────────────────────

    [Test]
    public void ExceptionThenThrowIf_WhenTrue_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.ExceptionThenThrowIf(true, ex, "ctx"));
        Assert.That(_tl.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void ExceptionThenThrowIf_WhenFalse_DoesNothing()
    {
        _log.ExceptionThenThrowIf(false, new Exception("x"));

        Assert.That(_tl.Entries, Is.Empty);
    }

    // ── Return value semantics ──────────────────────────────────────────

    [Test]
    public void If_ReturnsTrueWhenConditionTrue()
    {
        Assert.That(_log.If(true), Is.True);
    }

    [Test]
    public void If_ReturnsFalseWhenConditionFalse()
    {
        Assert.That(_log.If(false), Is.False);
    }

    // ── ILoggable / ToLoggableState integration ─────────────────────────

    [Test]
    public void If_UsesToLoggableState()
    {
        var obj = new HasToLoggableState();
        _log.If(true, obj);

        Assert.That(_tl.Last.Message, Does.Contain("custom-loggable"));
    }

    class HasToLoggableState
    {
        public object ToLoggableState() => "custom-loggable";
    }
}
