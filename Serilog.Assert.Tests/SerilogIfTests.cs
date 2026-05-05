using Serilog.Events;

namespace Serilog.Assert.Tests;

[TestFixture]
public class SerilogIfTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    // ── If base ─────────────────────────────────────────────────────────

    [Test]
    public void If_WhenTrue_Logs()
    {
        var result = log.Log.If(true, "some state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Information));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("some state"));
    }

    [Test]
    public void If_WhenFalse_DoesNotLog()
    {
        var result = log.Log.If(false, "some state");

        NUnit.Framework.Assert.That(result, Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void If_NullState_LogsEmpty()
    {
        log.Log.If(true);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void If_RespectsLogLevel()
    {
        log.Log.If(true, "state", logLevel:LogEventLevel.Fatal);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Fatal));
    }

    [Test]
    public void If_IncludesStateName_WhenDistinctFromAction()
    {
        log.Log.If(true, "myValue", helpfulLabel: "myLabel");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myLabel:"));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myValue"));
    }

    // ── WarnIf ──────────────────────────────────────────────────────────

    [Test]
    public void WarnIf_WhenTrue_LogsAtWarning()
    {
        var result = log.Log.WarnIf(true, "warning state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Warning));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("warning state"));
    }

    [Test]
    public void WarnIf_WhenFalse_DoesNotLog()
    {
        NUnit.Framework.Assert.That(log.Log.WarnIf(false, "x"), Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── ErrorIf ─────────────────────────────────────────────────────────

    [Test]
    public void ErrorIf_WhenTrue_LogsAtError()
    {
        var result = log.Log.ErrorIf(true, "err state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void ErrorIf_WhenFalse_DoesNotLog()
    {
        NUnit.Framework.Assert.That(log.Log.ErrorIf(false, "x"), Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── Information (via If) ────────────────────────────────────────────

    [Test]
    public void If_WhenTrue_LogsAtInformation()
    {
        log.Log.If(true, "info state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Information));
    }

    // ── DebugIf ─────────────────────────────────────────────────────────

    [Test]
    public void DebugIf_WhenTrue_LogsAtDebug()
    {
        log.Log.DebugIf(true, "debug state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Debug));
    }

    // ── VerboseIf ───────────────────────────────────────────────────────

    [Test]
    public void VerboseIf_WhenTrue_LogsAtVerbose()
    {
        log.Log.VerboseIf(true, "verbose state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Verbose));
    }

    // ── ExceptionIf ─────────────────────────────────────────────────────

    [Test]
    public void ExceptionIf_WhenTrue_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        var result = log.Log.ExceptionIf(true, ex, "ctx");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void ExceptionIf_WhenFalse_DoesNotLog()
    {
        log.Log.ExceptionIf(false, new Exception("x"), "ctx");

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── ExceptionAndThrowIf ─────────────────────────────────────────────

    [Test]
    public void ExceptionAndThrowIf_WhenTrue_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() =>
                                                                     log.Log.ExceptionAndThrowIf(true, ex, "ctx"));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void ExceptionAndThrowIf_WhenFalse_DoesNothing()
    {
        log.Log.ExceptionAndThrowIf(false, new Exception("x"));

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── Return value semantics ──────────────────────────────────────────

    [Test]
    public void If_ReturnsTrueWhenConditionTrue()
    {
        NUnit.Framework.Assert.That(log.Log.If(true), Is.True);
    }

    [Test]
    public void If_ReturnsFalseWhenConditionFalse()
    {
        NUnit.Framework.Assert.That(log.Log.If(false), Is.False);
    }

    // ── ILoggable / ToLoggableState integration ─────────────────────────

    [Test]
    public void If_UsesToLoggableState()
    {
        var loggableValue = new HasToLoggableState();
        log.Log.If(true, loggableValue);

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("custom-loggable"));
    }

    class HasToLoggableState
    {
        public object ToLoggableState() => "custom-loggable";
    }
}
