using Microsoft.Extensions.Logging;

namespace Log.Assert.Tests;

[TestFixture]
public class LogAssertNotNullTests
{
    TestLogger log = null!;

    [SetUp]
    public void SetUp() => log = new TestLogger();

    // ── AssertNotNullOrEmpty ────────────────────────────────────────────

    [Test]
    public void AssertNotNullOrEmpty_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.AssertNotNullOrEmpty("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNullOrEmpty_WhenNull_LogsError()
    {
        string? value = null;
        log.AssertNotNullOrEmpty(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Assertion Not Null Or Empty Failed"));
    }

    [Test]
    public void AssertNotNullOrEmpty_WhenEmpty_LogsError()
    {
        log.AssertNotNullOrEmpty("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    // ── AssertNotNullOrWhitespace ───────────────────────────────────────

    [Test]
    public void AssertNotNullOrWhitespace_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.AssertNotNullOrWhitespace("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void AssertNotNullOrWhitespace_WhenNull_LogsError()
    {
        string? value = null;
        log.AssertNotNullOrWhitespace(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Assertion Not Null Or Whitespace Failed"));
    }

    [Test]
    public void AssertNotNullOrWhitespace_WhenWhitespace_LogsError()
    {
        log.AssertNotNullOrWhitespace("   ");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    [Test]
    public void AssertNotNullOrWhitespace_WhenEmpty_LogsError()
    {
        log.AssertNotNullOrWhitespace("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
    }

    // ── PreconditionNotNullOrEmpty ──────────────────────────────────────

    [Test]
    public void PreconditionNotNullOrEmpty_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.PreconditionNotNullOrEmpty("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNullOrEmpty_WhenNull_LogsError()
    {
        string? value = null;
        log.PreconditionNotNullOrEmpty(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Precondition Not Null Or Empty Failed"));
    }

    [Test]
    public void PreconditionNotNullOrEmpty_WhenEmpty_LogsError()
    {
        log.PreconditionNotNullOrEmpty("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    // ── PreconditionNotNullOrWhitespace ─────────────────────────────────

    [Test]
    public void PreconditionNotNullOrWhitespace_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.PreconditionNotNullOrWhitespace("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PreconditionNotNullOrWhitespace_WhenNull_LogsError()
    {
        string? value = null;
        log.PreconditionNotNullOrWhitespace(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Precondition Not Null Or Whitespace Failed"));
    }

    [Test]
    public void PreconditionNotNullOrWhitespace_WhenWhitespace_LogsError()
    {
        log.PreconditionNotNullOrWhitespace("   ");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    // ── PostconditionNotNullOrEmpty ─────────────────────────────────────

    [Test]
    public void PostconditionNotNullOrEmpty_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.PostconditionNotNullOrEmpty("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNullOrEmpty_WhenNull_LogsError()
    {
        string? value = null;
        log.PostconditionNotNullOrEmpty(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Postcondition Not Null Or Empty Failed"));
    }

    [Test]
    public void PostconditionNotNullOrEmpty_WhenEmpty_LogsError()
    {
        log.PostconditionNotNullOrEmpty("");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }

    // ── PostconditionNotNullOrWhitespace ────────────────────────────────

    [Test]
    public void PostconditionNotNullOrWhitespace_WhenValid_ReturnsValueAndDoesNotLog()
    {
        var result = log.PostconditionNotNullOrWhitespace("hello");

        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
        NUnit.Framework.Assert.That(log.Entries, Is.Empty);
    }

    [Test]
    public void PostconditionNotNullOrWhitespace_WhenNull_LogsError()
    {
        string? value = null;
        log.PostconditionNotNullOrWhitespace(value);

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
        NUnit.Framework.Assert.That(log.Last.Message, Does.Contain("Postcondition Not Null Or Whitespace Failed"));
    }

    [Test]
    public void PostconditionNotNullOrWhitespace_WhenWhitespace_LogsError()
    {
        log.PostconditionNotNullOrWhitespace("   ");

        NUnit.Framework.Assert.That(log.Entries, Has.Count.EqualTo(1));
        NUnit.Framework.Assert.That(log.Last.Level, Is.EqualTo(LogLevel.Error));
    }
}
