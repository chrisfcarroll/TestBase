using Serilog.Events;

namespace SerilogAssert.Tests;

[TestFixture]
public class SerilogCallTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Call_DefaultsToInformation()
    {
        log.Log.Call("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Information));
        Assert.That(log.Last.Message, Does.Contain("state"));
    }

    [Test]
    public void Call_RespectsLogLevel()
    {
        log.Log.Call("s", LogEventLevel.Fatal);

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Fatal));
    }

    [Test]
    public void CallAsDebug_LogsAtDebug()
    {
        log.Log.CallAsDebug("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Debug));
    }

    [Test]
    public void CallAsVerbose_LogsAtVerbose()
    {
        log.Log.CallAsVerbose("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Verbose));
    }

    [Test]
    public void CallAsWarning_LogsAtWarning()
    {
        log.Log.CallAsWarning("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void CallAsError_LogsAtError()
    {
        log.Log.CallAsError("state");

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void CallWithException_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        log.Log.CallWithException(ex, "ctx");

        Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void CallWithExceptionThenThrow_LogsThenThrows()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            log.Log.CallWithExceptionThenThrow(ex, "ctx"));
        Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void Call_NullState_LogsEmptyString()
    {
        log.Log.Call();

        Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void Call_WithStateName_IncludesLabel()
    {
        log.Log.Call("val", helpfulLabel: "myParam");

        Assert.That(log.Last.Message, Does.Contain("myParam:"));
    }

    [Test]
    public void Call_UsesToLoggableState()
    {
        var obj = new CustomLoggable();
        log.Log.Call(obj);

        Assert.That(log.Last.Message, Does.Contain("custom-log-output"));
    }

    class CustomLoggable
    {
        public object ToLoggableState() => "custom-log-output";
    }
}
