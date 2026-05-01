using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

[TestFixture]
public class LogActionTests
{
    TestLogger _log = null!;

    [SetUp]
    public void SetUp() => _log = new TestLogger();

    [Test]
    public void LogAction_DefaultsToInformation()
    {
        _log.LogAction("state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Information));
        Assert.That(_log.Last.Message, Does.Contain("state"));
    }

    [Test]
    public void LogAction_RespectsLogLevel()
    {
        _log.LogAction("s", LogLevel.Critical);

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Critical));
    }

    [Test]
    public void LogActionInformation_LogsAtInformation()
    {
        _log.LogActionInformation("state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    [Test]
    public void LogActionDebug_LogsAtDebug()
    {
        _log.LogActionDebug("state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    [Test]
    public void LogActionTrace_LogsAtTrace()
    {
        _log.LogActionTrace("state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Trace));
    }

    [Test]
    public void LogActionWarning_LogsAtWarning()
    {
        _log.LogActionWarning("state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Warning));
    }

    [Test]
    public void LogActionError_LogsAtError()
    {
        _log.LogActionError("state");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogActionException_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        _log.LogActionException(ex, "ctx");

        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(_log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void LogActionExceptionThenThrow_LogsThenThrows()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.LogActionExceptionThenThrow(ex, "ctx"));
        Assert.That(_log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogAction_NullState_LogsEmptyString()
    {
        _log.LogAction();

        Assert.That(_log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void LogAction_WithStateName_IncludesLabel()
    {
        _log.LogAction("val", stateName: "myParam");

        Assert.That(_log.Last.Message, Does.Contain("myParam:"));
    }

    [Test]
    public void LogAction_UsesToLoggableState()
    {
        var obj = new CustomLoggable();
        _log.LogAction(obj);

        Assert.That(_log.Last.Message, Does.Contain("custom-log-output"));
    }

    class CustomLoggable
    {
        public object ToLoggableState() => "custom-log-output";
    }
}
