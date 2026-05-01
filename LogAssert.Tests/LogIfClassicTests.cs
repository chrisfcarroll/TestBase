using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

/// <summary>
/// Tests for the traditional (message, params args / delayedArgs) style.
/// </summary>
[TestFixture]
public class LogIfClassicTests
{
    TestLogger _log = null!;

    [SetUp]
    public void SetUp() => _log = new TestLogger();

    // ── Func<T> delayedArgs variants ────────────────────────────────────

    [Test]
    public void LogWarnIf_DelayedArgs_WhenTrue_EvaluatesAndLogs()
    {
        var evaluated = false;
        _log.LogWarnIf(true, "msg with {0}", () => { evaluated = true; return "value"; });

        Assert.That(evaluated, Is.True);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Warning));
    }

    [Test]
    public void LogWarnIf_DelayedArgs_WhenFalse_DoesNotEvaluate()
    {
        var evaluated = false;
        _log.LogWarnIf(false, "msg", () => { evaluated = true; return "value"; });

        Assert.That(evaluated, Is.False);
        Assert.That(_log.Entries, Is.Empty);
    }

    [Test]
    public void LogErrorIf_DelayedArgs_WhenTrue_Logs()
    {
        _log.LogErrorIf(true, "err {0}", () => "detail");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogInformationIf_DelayedArgs_WhenTrue_Logs()
    {
        _log.LogInformationIf(true, "info {0}", () => "detail");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    [Test]
    public void LogDebugIf_DelayedArgs_WhenTrue_Logs()
    {
        _log.LogDebugIf(true, "dbg {0}", () => "detail");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    // ── Func<T,bool> predicate variants ─────────────────────────────────

    [Test]
    public void LogWarnIf_Predicate_WhenTrue_Logs()
    {
        var result = _log.LogWarnIf(10, x => x > 5, "Value {0} is large", 10);

        Assert.That(result, Is.True);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Warning));
    }

    [Test]
    public void LogWarnIf_Predicate_WhenFalse_DoesNotLog()
    {
        var result = _log.LogWarnIf(2, x => x > 5, "Value {0} is large", 2);

        Assert.That(result, Is.False);
        Assert.That(_log.Entries, Is.Empty);
    }

    [Test]
    public void LogWarnIf_Predicate_WhenEvaluationThrows_ReturnsNull()
    {
        var result = _log.LogWarnIf<int>(1, _ => throw new Exception("eval fail"), "msg");

        Assert.That(result, Is.Null);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogWarnIf_PredicateWithDelayedArgs_WhenTrue_EvaluatesArgs()
    {
        var evaluated = false;
        var result = _log.LogWarnIf(10, x => x > 5, "large", () => { evaluated = true; return "detail"; });

        Assert.That(result, Is.True);
        Assert.That(evaluated, Is.True);
    }

    // ── LogIfNot family ─────────────────────────────────────────────────

    [Test]
    public void LogWarnIfNot_WhenFalse_Logs()
    {
        var result = _log.LogWarnIfNot(false, "Expected true");

        Assert.That(result, Is.False);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Warning));
    }

    [Test]
    public void LogWarnIfNot_WhenTrue_DoesNotLog()
    {
        var result = _log.LogWarnIfNot(true, "Expected true");

        Assert.That(result, Is.True);
        Assert.That(_log.Entries, Is.Empty);
    }

    [Test]
    public void LogErrorIfNot_WhenFalse_LogsAtError()
    {
        _log.LogErrorIfNot(false, "bad");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogInformationIfNot_WhenFalse_LogsAtInformation()
    {
        _log.LogInformationIfNot(false, "info");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    [Test]
    public void LogWarnIfNot_Predicate_WhenFalse_Logs()
    {
        var result = _log.LogWarnIfNot(2, x => x > 5, "Expected > 5");

        Assert.That(result, Is.False);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Warning));
    }

    [Test]
    public void LogWarnIfNot_Predicate_WhenTrue_DoesNotLog()
    {
        var result = _log.LogWarnIfNot(10, x => x > 5, "Expected > 5");

        Assert.That(result, Is.True);
        Assert.That(_log.Entries, Is.Empty);
    }

    // ── Exception safety ────────────────────────────────────────────────

    [Test]
    public void LogWarnIf_DelayedArgs_WhenEvaluationThrows_LogsError()
    {
        _log.LogWarnIf<string>(true, "msg", () => throw new Exception("boom"));

        // Should have logged the error from the catch block
        Assert.That(_log.Entries, Has.Count.GreaterThanOrEqualTo(1));
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
    }
}
