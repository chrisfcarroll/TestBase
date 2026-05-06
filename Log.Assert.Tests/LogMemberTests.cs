using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

[TestFixture]
public class LogMemberTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    [Test]
    public void LogMember_DefaultsToInformation()
    {
        log.Member("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("state"));
    }

    [Test]
    public void LogMember_RespectsLogLevel()
    {
        log.Member("s", logLevel: LogLevel.Critical);

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Critical));
    }

    [Test]
    public void LogActionInformation_LogsAtInformation()
    {
        log.Member("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Information));
    }

    [Test]
    public void LogActionDebug_LogsAtDebug()
    {
        log.MemberDebug("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Debug));
    }

    [Test]
    public void LogActionTrace_LogsAtTrace()
    {
        log.MemberTrace("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Trace));
    }

    [Test]
    public void LogActionWarning_LogsAtWarning()
    {
        log.MemberWarning("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Warning));
    }

    [Test]
    public void LogActionError_LogsAtError()
    {
        log.MemberError("state");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogActionException_LogsExceptionAtError()
    {
        var ex = new InvalidOperationException("boom");
        log.MemberException(ex, "ctx");

        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void LogActionExceptionThenThrow_LogsThenThrows()
    {
        var ex = new InvalidOperationException("boom");
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() =>
                                                                     log.MemberExceptionThenThrow(ex, helpfulInformation: "ctx"));
        NUnit.Framework.Assert.That(log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogMember_NullState_LogsEmptyString()
    {
        log.Member();

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    [Test]
    public void LogMember_WithStateName_IncludesLabel()
    {
        log.Member("val", label:"myParam");

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("myParam="));
    }

    [Test]
    public void LogMember_UsesToLoggableState()
    {
        var obj = new CustomLoggable();
        log.Member(helpfulInformation: obj);

        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("custom-log-output"));
    }

    class CustomLoggable
    {
        // ReSharper disable UnusedMember.Local
        public object ToLoggableState() => "custom-log-output";
        // ReSharper restore UnusedMember.Local
    }
}
