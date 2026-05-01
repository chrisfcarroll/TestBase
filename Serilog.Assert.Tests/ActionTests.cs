using Serilog.Events;

namespace SerilogAssert.Tests;

[TestFixture]
public class ActionTests
{
    TestLogger _tl = null!;
    ILogger _log = null!;

    [SetUp]
    public void SetUp()
    {
        _tl = new TestLogger();
        _log = _tl.Log;
    }

    [Test]
    public void Action_DefaultsToInformation()
    {
        _log.Action("state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Information));
        Assert.That(_tl.Last.Message, Does.Contain("state"));
    }

    [Test]
    public void Action_RespectsLogLevel()
    {
        _log.Action("s", LogEventLevel.Fatal);

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Fatal));
    }

    [Test]
    public void ActionInformation_LogsAtInformation()
    {
        _log.ActionInformation("state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Information));
    }

    [Test]
    public void ActionDebug_LogsAtDebug()
    {
        _log.ActionDebug("state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Debug));
    }

    [Test]
    public void ActionVerbose_LogsAtVerbose()
    {
        _log.ActionVerbose("state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Verbose));
    }

    [Test]
    public void ActionWarning_LogsAtWarning()
    {
        _log.ActionWarning("state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void ActionError_LogsAtError()
    {
        _log.ActionError("state");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void ActionException_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        _log.ActionException(ex, "ctx");

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(_tl.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void ActionExceptionThenThrow_LogsThenThrows()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.ActionExceptionThenThrow(ex, "ctx"));
        Assert.That(_tl.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void Action_NullState_LogsEmptyString()
    {
        _log.Action();

        Assert.That(_tl.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void Action_WithStateName_IncludesLabel()
    {
        _log.Action("val", stateName: "myParam");

        Assert.That(_tl.Last.Message, Does.Contain("myParam:"));
    }

    [Test]
    public void Action_UsesToLoggableState()
    {
        var obj = new CustomLoggable();
        _log.Action(obj);

        Assert.That(_tl.Last.Message, Does.Contain("custom-log-output"));
    }

    class CustomLoggable
    {
        public object ToLoggableState() => "custom-log-output";
    }
}
