using Serilog.Events;

namespace Serilog.Assert.Tests;

[TestFixture]
public class SerilogLogMemberTests
{
    TestLogger logBuilder = null!;
    ILogger log;

    [SetUp]
    public void SetUp()
    {
        logBuilder = new TestLogger();
        log = logBuilder.Log;
    }

    [Test]
    public void Call_DefaultsToInformation()
    {
        log.Member("state");

        TestContext.Out.WriteLine(logBuilder.Last.Message);

        NUnit.Framework.Assert.That(logBuilder.Last.Level, Is.EqualTo(LogEventLevel.Information));
        NUnit.Framework.Assert.That(logBuilder.Last.Message, Does.Contain("state"));
    }

    [Test]
    public void Call_RespectsLogLevel()
    {
        log.Member("s", LogEventLevel.Fatal);

        NUnit.Framework.Assert.That(logBuilder.Last.Level, Is.EqualTo(LogEventLevel.Fatal));
    }

    [Test]
    public void CallAsDebug_LogsAtDebug()
    {
        log.MemberDebug("state");

        NUnit.Framework.Assert.That(logBuilder.Last.Level, Is.EqualTo(LogEventLevel.Debug));
    }

    [Test]
    public void CallAsVerbose_LogsAtVerbose()
    {
        log.MemberVerbose("state");

        NUnit.Framework.Assert.That(logBuilder.Last.Level, Is.EqualTo(LogEventLevel.Verbose));
    }

    [Test]
    public void CallAsWarning_LogsAtWarning()
    {
        log.MemberWarning("state");

        NUnit.Framework.Assert.That(logBuilder.Last.Level, Is.EqualTo(LogEventLevel.Warning));
    }

    [Test]
    public void CallAsError_LogsAtError()
    {
        log.MemberError("state");

        NUnit.Framework.Assert.That(logBuilder.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void CallWithException_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        log.MemberException(ex, "ctx");

        NUnit.Framework.Assert.That(logBuilder.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(logBuilder.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void CallWithExceptionThenThrow_LogsThenThrows()
    {
        var ex = new InvalidOperationException("boom");
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() =>
                                                                     log.MemberExceptionThenThrow(ex, "ctx"));
        NUnit.Framework.Assert.That(logBuilder.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void Call_NullState_LogsEmptyString()
    {
        log.Member();

        NUnit.Framework.Assert.That(logBuilder.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void Call_WithStateName_IncludesLabel()
    {
        log.Member("val", label: "myParam");

        NUnit.Framework.Assert.That(logBuilder.Last.Message, Does.Contain("myParam="));
    }

    [Test]
    public void Call_UsesToLoggableState()
    {
        var obj = new CustomLoggable();
        log.Member(obj);

        NUnit.Framework.Assert.That(logBuilder.Last.Message, Does.Contain("custom-log-output"));
    }

    class CustomLoggable
    {
        public object ToLoggableState() => "custom-log-output";
    }
}
