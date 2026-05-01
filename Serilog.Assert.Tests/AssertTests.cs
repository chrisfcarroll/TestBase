using Serilog.Events;

namespace SerilogAssert.Tests;

[TestFixture]
public class AssertTests
{
    TestLogger _tl = null!;
    ILogger _log = null!;

    [SetUp]
    public void SetUp()
    {
        _tl = new TestLogger();
        _log = _tl.Log;
    }

    // ── Assert ──────────────────────────────────────────────────────────

    [Test]
    public void Assert_WhenTrue_DoesNotLog()
    {
        _log.Assert(true, "state");

        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void Assert_WhenFalse_LogsError()
    {
        _log.Assert(false, "state");

        Assert.That(_tl.Entries, Has.Count.EqualTo(1));
        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
        Assert.That(_tl.Last.Message, Does.Contain("AssertionFailed"));
    }

    [Test]
    public void Assert_WhenFalse_IncludesLoggableState()
    {
        _log.Assert(false, "context-value");

        Assert.That(_tl.Last.Message, Does.Contain("context-value"));
    }

    [Test]
    public void Assert_WhenFalse_NullState_LogsEmpty()
    {
        _log.Assert(false);

        Assert.That(_tl.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    // ── AssertNotNull ───────────────────────────────────────────────────

    [Test]
    public void AssertNotNull_WhenNotNull_ReturnsTrue()
    {
        var result = _log.AssertNotNull("hello");

        Assert.That(result, Is.True);
        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNull_WhenNull_ReturnsFalseAndLogs()
    {
        string? value = null;
        var result = _log.AssertNotNull(value);

        Assert.That(result, Is.False);
        Assert.That(_tl.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void AssertNotNull_WithLoggableState_UsesItInMessage()
    {
        string? value = null;
        _log.AssertNotNull(value, "custom message");

        Assert.That(_tl.Entries, Has.Count.GreaterThanOrEqualTo(1));
    }

    // ── AndThrowIfNot ───────────────────────────────────────────────────

    [Test]
    public void AndThrowIfNot_WhenTrue_Returns()
    {
        var result = _log.AndThrowIfNot(true, "should not throw");

        Assert.That(result, Is.True);
        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void AndThrowIfNot_WhenFalse_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _log.AndThrowIfNot(false, "condition failed"));
    }

    [Test]
    public void AndThrowIfNot_WithException_WhenFalse_ThrowsThatException()
    {
        var ex = new ArgumentException("bad arg");
        Assert.Throws<ArgumentException>(() =>
            _log.AndThrowIfNot(false, ex));
    }

    [Test]
    public void AndThrowIfNot_WithExceptionFactory_WhenFalse_ThrowsCreatedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            _log.AndThrowIfNot(false, () => new NotSupportedException("nope")));
    }

    [Test]
    public void AndThrowIfNot_WithDelayedArgs_WhenFalse_ThrowsAndLogs()
    {
        var argsEvaluated = false;
        Assert.Throws<NotSupportedException>(() =>
            _log.AndThrowIfNot(false,
                () => new NotSupportedException("nope"),
                () => { argsEvaluated = true; return new object[] { "detail" }; }));
        Assert.That(argsEvaluated, Is.True);
    }

    [Test]
    public void AndThrowIfNot_WithDelayedArgs_WhenTrue_DoesNotEvaluateArgs()
    {
        var argsEvaluated = false;
        _log.AndThrowIfNot(true,
            () => new Exception("x"),
            () => { argsEvaluated = true; return new object[] { }; });
        Assert.That(argsEvaluated, Is.False);
    }

    // ── AndThrowIfNull ──────────────────────────────────────────────────

    [Test]
    public void AndThrowIfNull_WhenNotNull_ReturnsValue()
    {
        var result = _log.AndThrowIfNull("hello", "should not throw");

        Assert.That(result, Is.EqualTo("hello"));
        Assert.That(_tl.Entries, Is.Empty);
    }

    [Test]
    public void AndThrowIfNull_WhenNull_ThrowsArgumentNullException()
    {
        string? value = null;
        Assert.Throws<ArgumentNullException>(() =>
            _log.AndThrowIfNull(value, "was null"));
    }

    [Test]
    public void AndThrowIfNull_WithException_WhenNull_ThrowsThatException()
    {
        string? value = null;
        var ex = new InvalidOperationException("custom");
        Assert.Throws<InvalidOperationException>(() =>
            _log.AndThrowIfNull(value, ex));
    }

    [Test]
    public void AndThrowIfNull_WithFactory_WhenNull_ThrowsCreatedException()
    {
        string? value = null;
        Assert.Throws<NotSupportedException>(() =>
            _log.AndThrowIfNull(value, () => new NotSupportedException("nope")));
    }

    [Test]
    public void AndThrowIfNull_WithDelayedArgs_WhenNull_EvaluatesArgs()
    {
        string? value = null;
        var argsEvaluated = false;
        Assert.Throws<NotSupportedException>(() =>
            _log.AndThrowIfNull(value,
                () => new NotSupportedException("nope"),
                () => { argsEvaluated = true; return new object[] { "detail" }; }));
        Assert.That(argsEvaluated, Is.True);
    }
}
