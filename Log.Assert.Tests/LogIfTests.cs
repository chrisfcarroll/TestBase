using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

[TestFixture]
public class LogIfTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    // ── If base ─────────────────────────────────────────────────────────

    [Test]
    public void If_WhenTrue_Logs()
    {
        var result = log.If(true, helpfulInformation: "some state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("some state"));
    }

    [Test]
    public void If_WhenFalse_DoesNotLog()
    {
        var result = log.If(false, helpfulInformation: "some state");

        NUnit.Framework.Assert.That(result, Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void If_NullState_LogsEmpty()
    {
        log.If(true);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void If_RespectsLogLevel()
    {
        log.If(true, helpfulInformation: "state", logLevel: LogLevel.Critical);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Critical));
    }

    [Test]
    public void If_IncludesStateName_WhenDistinctFromAction()
    {
        log.If(true, helpfulInformation: "myValue", label: "myLabel");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myLabel:"));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myValue"));
    }

    // ── WarnIf ──────────────────────────────────────────────────────────

    [Test]
    public void WarnIf_WhenTrue_LogsAtWarning()
    {
        var result = log.WarnIf(true, helpfulInformation: "warning state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Warning));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("warning state"));
    }

    [Test]
    public void WarnIf_WhenFalse_DoesNotLog()
    {
        NUnit.Framework.Assert.That(log.WarnIf(false, helpfulInformation: "x"), Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── ErrorIf ─────────────────────────────────────────────────────────

    [Test]
    public void ErrorIf_WhenTrue_LogsAtError()
    {
        var result = log.ErrorIf(true, helpfulInformation: "err state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void ErrorIf_WhenFalse_DoesNotLog()
    {
        NUnit.Framework.Assert.That(log.ErrorIf(false, helpfulInformation: "x"), Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── InformationIf ─────────────────────────────────────────────────

    [Test]
    public void InformationIf_WhenTrue_LogsAtInformation()
    {
        var result = log.InformationIf(true, helpfulInformation: "info state");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("info state"));
    }

    [Test]
    public void InformationIf_WhenFalse_DoesNotLog()
    {
        NUnit.Framework.Assert.That(log.InformationIf(false, helpfulInformation: "x"), Is.False);
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── DebugIf ─────────────────────────────────────────────────────────

    [Test]
    public void DebugIf_WhenTrue_LogsAtDebug()
    {
        log.DebugIf(true, helpfulInformation: "debug state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    // ── TraceIf ─────────────────────────────────────────────────────────

    [Test]
    public void TraceIf_WhenTrue_LogsAtTrace()
    {
        log.TraceIf(true, helpfulInformation: "trace state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Trace));
    }

    // ── ExceptionIf ─────────────────────────────────────────────────────

    [Test]
    public void ExceptionIf_WhenTrue_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        var result = log.ExceptionIf(true, ex, helpfulInformation: "ctx");

        NUnit.Framework.Assert.That(result, Is.True);
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void ExceptionIf_WhenFalse_DoesNotLog()
    {
        log.ExceptionIf(false, new Exception("x"), helpfulInformation: "ctx");

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── ExceptionAndThrowIf ─────────────────────────────────────────────

    [Test]
    public void ExceptionAndThrowIf_WhenTrue_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() =>
                                                                     log.ExceptionAndThrowIf(true, ex, helpfulInformation: "ctx"));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void ExceptionAndThrowIf_WhenFalse_DoesNothing()
    {
        log.ExceptionAndThrowIf(false, new Exception("x"));

        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    // ── Return value semantics ──────────────────────────────────────────

    [Test]
    public void If_ReturnsTrueWhenConditionTrue()
    {
        NUnit.Framework.Assert.That(log.If(true), Is.True);
    }

    [Test]
    public void If_ReturnsFalseWhenConditionFalse()
    {
        NUnit.Framework.Assert.That(log.If(false), Is.False);
    }

    // ── ILoggable / ToLoggableState integration ─────────────────────────

    [Test]
    public void If_UsesToLoggableState()
    {
        var loggableValue = new HasToLoggableState();
        log.If(true, helpfulInformation: loggableValue);

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("custom-loggable"));
    }

    class HasToLoggableState
    {
        // ReSharper disable UnusedMember.Local
        public object ToLoggableState() => "custom-loggable";
        // ReSharper restore UnusedMember.Local
    }
}
