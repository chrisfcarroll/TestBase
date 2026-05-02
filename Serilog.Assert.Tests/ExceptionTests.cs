using Serilog.Events;

namespace SerilogAssert.Tests;

[TestFixture]
public class ExceptionTests
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
    public void Exception_LogsAndReturnsException()
    {
        var ex = new InvalidOperationException("boom");
        var result = _log.Exception(ex);

        Assert.That(result, Is.SameAs(ex));
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(_tl.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void Exception_NullException_LogsErrorAndReturnsNull()
    {
        var result = _log.Exception(null);

        Assert.That(result, Is.Null);
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(_tl.Last.Message, Does.Contain("null exception"));
    }

    [Test]
    public void ExceptionThenThrow_ThrowsTheException()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.ExceptionThenThrow(ex));
        Assert.That(_tl.Last.Exception, Is.SameAs(ex));
    }

    [Test]
    public void ExceptionThenThrow_WithMessage_ThrowsAndLogs()
    {
        var ex = new InvalidOperationException("boom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.ExceptionThenThrow(ex, "custom msg"));
        Assert.That(_tl.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void LogException_CreatesAndReturnsApplicationException()
    {
        var result = _log.LogException("something went wrong");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ApplicationException>());
        Assert.That(result!.Message, Is.EqualTo("something went wrong"));
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void LogException_EmptyMessage_ReturnsNull()
    {
        var result = _log.LogException("");

        Assert.That(result, Is.Null);
        Assert.That(_tl.Last.Message, Does.Contain("empty exception"));
    }
}
