using Serilog.Events;

namespace Serilog.Assert.Tests;

[TestFixture]
public class SerilogAssertNotNullTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    // ── AssertNotNullOrEmpty ────────────────────────────────────────────

    [Test]
    public void AssertNotNullOrEmpty_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.Log.AssertNotNullOrEmpty("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNullOrEmpty_WhenNull_LogsError()
    {
        string? value = null;
        log.Log.AssertNotNullOrEmpty(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Assertion Not Null Or Empty Failed"));
    }

    [Test]
    public void AssertNotNullOrEmpty_WhenEmpty_LogsError()
    {
        log.Log.AssertNotNullOrEmpty("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    // ── AssertNotNullOrWhitespace ───────────────────────────────────────

    [Test]
    public void AssertNotNullOrWhitespace_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.Log.AssertNotNullOrWhitespace("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNullOrWhitespace_WhenNull_LogsError()
    {
        string? value = null;
        log.Log.AssertNotNullOrWhitespace(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Assertion Not Null Or Whitespace Failed"));
    }

    [Test]
    public void AssertNotNullOrWhitespace_WhenWhitespace_LogsError()
    {
        log.Log.AssertNotNullOrWhitespace("   ");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    [Test]
    public void AssertNotNullOrWhitespace_WhenEmpty_LogsError()
    {
        log.Log.AssertNotNullOrWhitespace("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    // ── PreconditionNotNullOrEmpty ──────────────────────────────────────

    [Test]
    public void PreconditionNotNullOrEmpty_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.Log.PreconditionNotNullOrEmpty("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNullOrEmpty_WhenNull_LogsError()
    {
        string? value = null;
        log.Log.PreconditionNotNullOrEmpty(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Precondition Not Null Or Empty Failed"));
    }

    [Test]
    public void PreconditionNotNullOrEmpty_WhenEmpty_LogsError()
    {
        log.Log.PreconditionNotNullOrEmpty("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    // ── PreconditionNotNullOrWhitespace ─────────────────────────────────

    [Test]
    public void PreconditionNotNullOrWhitespace_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.Log.PreconditionNotNullOrWhitespace("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNullOrWhitespace_WhenNull_LogsError()
    {
        string? value = null;
        log.Log.PreconditionNotNullOrWhitespace(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Precondition Not Null Or Whitespace Failed"));
    }

    [Test]
    public void PreconditionNotNullOrWhitespace_WhenWhitespace_LogsError()
    {
        log.Log.PreconditionNotNullOrWhitespace("   ");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    // ── PostconditionNotNullOrEmpty ─────────────────────────────────────

    [Test]
    public void PostconditionNotNullOrEmpty_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.Log.PostconditionNotNullOrEmpty("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNullOrEmpty_WhenNull_LogsError()
    {
        string? value = null;
        log.Log.PostconditionNotNullOrEmpty(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Postcondition Not Null Or Empty Failed"));
    }

    [Test]
    public void PostconditionNotNullOrEmpty_WhenEmpty_LogsError()
    {
        log.Log.PostconditionNotNullOrEmpty("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }

    // ── PostconditionNotNullOrWhitespace ────────────────────────────────

    [Test]
    public void PostconditionNotNullOrWhitespace_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.Log.PostconditionNotNullOrWhitespace("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNullOrWhitespace_WhenNull_LogsError()
    {
        string? value = null;
        log.Log.PostconditionNotNullOrWhitespace(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Postcondition Not Null Or Whitespace Failed"));
    }

    [Test]
    public void PostconditionNotNullOrWhitespace_WhenWhitespace_LogsError()
    {
        log.Log.PostconditionNotNullOrWhitespace("   ");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogEventLevel.Error));
    }
}
