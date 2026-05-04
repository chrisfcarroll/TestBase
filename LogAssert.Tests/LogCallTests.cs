using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

[TestFixture]
public class LogCallTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void LogAction_DefaultsToInformation()
    {
        log.LogCall("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
        Assert.That(log.Last.Message, Does.Contain("state"));
    }

    [Test]
    public void LogAction_RespectsLogLevel()
    {
        log.LogCall("s", LogLevel.Critical);

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Critical));
    }

    [Test]
    public void LogActionInformation_LogsAtInformation()
    {
        log.LogCall("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    [Test]
    public void LogActionDebug_LogsAtDebug()
    {
        log.LogCallAsDebug("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    [Test]
    public void LogActionTrace_LogsAtTrace()
    {
        log.LogCallAsTrace("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Trace));
    }

    [Test]
    public void LogActionWarning_LogsAtWarning()
    {
        log.LogCallAsWarning("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Warning));
    }

    [Test]
    public void LogActionError_LogsAtError()
    {
        log.LogCallAsError("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogActionException_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        log.LogCallWithException(ex, "ctx");

        Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void LogActionExceptionThenThrow_LogsThenThrows()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            log.LogCallWithExceptionThenThrow(ex, "ctx"));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogAction_NullState_LogsEmptyString()
    {
        log.LogCall();

        Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void LogAction_WithStateName_IncludesLabel()
    {
        log.LogCall("val", helpfulLabel: "myParam");

        Assert.That(log.Last.Message, Does.Contain("myParam:"));
    }

    [Test]
    public void LogAction_UsesToLoggableState()
    {
        var obj = new CustomLoggable();
        log.LogCall(obj);

        Assert.That(log.Last.Message, Does.Contain("custom-log-output"));
    }

    class CustomLoggable
    {
        // ReSharper disable UnusedMember.Local
        public object ToLoggableState() => "custom-log-output";
        // ReSharper restore UnusedMember.Local
    }
}
