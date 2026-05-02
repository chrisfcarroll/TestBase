using Microsoft.Extensions.Logging;

namespace LogAssert.Tests;

[TestFixture]
public class ExceptionTests
{
    TestLogger _log = null!;

    [SetUp]
    public void SetUp() => _log = new TestLogger();

    [Test]
    public void Exception_LogsAndReturnsException()
    {
        var ex = new InvalidOperationException("boom");
        var result = _log.Exception(ex);

        Assert.That(result, Is.SameAs(ex));
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(_log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void Exception_NullException_LogsErrorAndReturnsNull()
    {
        var result = _log.Exception(null);

        Assert.That(result, Is.Null);
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
        Assert.That(_log.Last.Message, Does.Contain("null exception"));
    }

    [Test]
    public void LogExceptionThenThrow_ThrowsTheException()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.LogExceptionThenThrow(ex));
        Assert.That(_log.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void LogExceptionThenThrow_WithMessage_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.LogExceptionThenThrow(ex, "custom msg"));
        Assert.That(_log.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogException_CreatesAndReturnsApplicationException()
    {
        var result = _log.LogException("something went wrong");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ApplicationException>());
        Assert.That(result!.Message, Is.EqualTo("something went wrong"));
        Assert.That(_log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void LogException_EmptyMessage_ReturnsNull()
    {
        var result = _log.LogException("");

        Assert.That(result, Is.Null);
        Assert.That(_log.Last.Message, Does.Contain("empty exception"));
    }
}
