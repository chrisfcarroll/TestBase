using Serilog.Events;

namespace Serilog.Assert.Tests;

[TestFixture]
public class SerilogCallTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void Call_DefaultsToInformation()
    {
        log.Log.Member("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Information));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("state"));
    }

    [Test]
    public void Call_RespectsLogLevel()
    {
        log.Log.Member("s", LogEventLevel.Fatal);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Fatal));
    }

    [Test]
    public void CallAsDebug_LogsAtDebug()
    {
        log.Log.MemberDebug("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Debug));
    }

    [Test]
    public void CallAsVerbose_LogsAtVerbose()
    {
        log.Log.MemberVerbose("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Verbose));
    }

    [Test]
    public void CallAsWarning_LogsAtWarning()
    {
        log.Log.MemberWarning("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void CallAsError_LogsAtError()
    {
        log.Log.MemberError("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void CallWithException_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        log.Log.MemberException(ex, "ctx");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void CallWithExceptionThenThrow_LogsThenThrows()
    {
        var ex = new InvalidOperationException("boom");
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() =>
                                                                     log.Log.MemberExceptionThenThrow(ex, "ctx"));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void Call_NullState_LogsEmptyString()
    {
        log.Log.Member();

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void Call_WithStateName_IncludesLabel()
    {
        log.Log.Member("val", label: "myParam");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myParam="));
    }

    [Test]
    public void Call_UsesToLoggableState()
    {
        var obj = new CustomLoggable();
        log.Log.Member(obj);

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("custom-log-output"));
    }

    class CustomLoggable
    {
        public object ToLoggableState() => "custom-log-output";
    }
}
