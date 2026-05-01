using Serilog.Events;

namespace SerilogAssert.Tests;

/// <summary>
/// Tests for the traditional (message, params args / delayedArgs) style.
/// </summary>
[TestFixture]
public class IfClassicTests
{
    TestLogger _tl = null!;
    ILogger _log = null!;

    [SetUp]
    public void SetUp()
    {
        _tl = new TestLogger();
        _log = _tl.Log;
    }

    // ── Func<T> delayedArgs variants ────────────────────────────────────

    [Test]
    public void WarnIf_DelayedArgs_WhenTrue_EvaluatesAndLogs()
    {
        var evaluated = false;
        _log.WarnIf(true, "msg with {0}", () => { evaluated = true; return "value"; });

        Assert.That(evaluated, Is.True);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void WarnIf_DelayedArgs_WhenFalse_DoesNotEvaluate()
    {
        var evaluated = false;
        _log.WarnIf(false, "msg", () => { evaluated = true; return "value"; });

        Assert.That(evaluated, Is.False);
        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void ErrorIf_DelayedArgs_WhenTrue_Logs()
    {
        _log.ErrorIf(true, "err {0}", () => "detail");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void InformationIf_DelayedArgs_WhenTrue_Logs()
    {
        _log.InformationIf(true, "info {0}", () => "detail");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Information));
    }

    [Test]
    public void DebugIf_DelayedArgs_WhenTrue_Logs()
    {
        _log.DebugIf(true, "dbg {0}", () => "detail");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Debug));
    }

    // ── Func<T,bool> predicate variants ─────────────────────────────────

    [Test]
    public void WarnIf_Predicate_WhenTrue_Logs()
    {
        var result = _log.WarnIf(10, x => x > 5, "Value {0} is large", 10);

        Assert.That(result, Is.True);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void WarnIf_Predicate_WhenFalse_DoesNotLog()
    {
        var result = _log.WarnIf(2, x => x > 5, "Value {0} is large", 2);

        Assert.That(result, Is.False);
        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void WarnIf_Predicate_WhenEvaluationThrows_ReturnsNull()
    {
        var result = _log.WarnIf<int>(1, _ => throw new Exception("eval fail"), "msg");

        Assert.That(result, Is.Null);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void WarnIf_PredicateWithDelayedArgs_WhenTrue_EvaluatesArgs()
    {
        var evaluated = false;
        var result = _log.WarnIf(10, x => x > 5, "large", () => { evaluated = true; return "detail"; });

        Assert.That(result, Is.True);
        Assert.That(evaluated, Is.True);
    }

    // ── IfNot family ────────────────────────────────────────────────────

    [Test]
    public void WarnIfNot_WhenFalse_Logs()
    {
        var result = _log.WarnIfNot(false, "Expected true");

        Assert.That(result, Is.False);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void WarnIfNot_WhenTrue_DoesNotLog()
    {
        var result = _log.WarnIfNot(true, "Expected true");

        Assert.That(result, Is.True);
        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void ErrorIfNot_WhenFalse_LogsAtError()
    {
        _log.ErrorIfNot(false, "bad");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void InformationIfNot_WhenFalse_LogsAtInformation()
    {
        _log.InformationIfNot(false, "info");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Information));
    }

    [Test]
    public void WarnIfNot_Predicate_WhenFalse_Logs()
    {
        var result = _log.WarnIfNot(2, x => x > 5, "Expected > 5");

        Assert.That(result, Is.False);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void WarnIfNot_Predicate_WhenTrue_DoesNotLog()
    {
        var result = _log.WarnIfNot(10, x => x > 5, "Expected > 5");

        Assert.That(result, Is.True);
        Assert.That(_tl.Entries, Is.Empty);
    }

    // ── Exception safety ────────────────────────────────────────────────

    [Test]
    public void WarnIf_DelayedArgs_WhenEvaluationThrows_LogsError()
    {
        _log.WarnIf<string>(true, "msg", () => throw new Exception("boom"));

        // Should have logged the error from the catch block
        Assert.That(_tl.Entries, Has.Count.GreaterThanOrEqualTo(1));
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }
}
