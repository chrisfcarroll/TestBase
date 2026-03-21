using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferStringTests
{
    [Test]
    public void Equal_strings_return_equal()
    {
        var result = Differ.Diff("hello", "hello");
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Empty_strings_return_equal()
    {
        var result = Differ.Diff("", "");
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Different_strings_show_index_and_snippet()
    {
        var result = Differ.Diff("hello world", "hello World");
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("index 6"));
        Assert.That(text, Does.Contain("hello"));
    }

    [Test]
    public void Different_length_strings_show_lengths()
    {
        var result = Differ.Diff("short", "short and long");
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("5"));
        Assert.That(text, Does.Contain("14"));
    }

    [Test]
    public void Long_strings_show_snippet_around_diff_point()
    {
        var left = new string('a', 100) + "X" + new string('b', 100);
        var right = new string('a', 100) + "Y" + new string('b', 100);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("index 100"));
        Assert.That(text, Does.Contain("..."));
    }

    [Test]
    public void Null_vs_string_shows_null()
    {
        var result = Differ.Diff(null, "hello");
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("null"));
    }

    [Test]
    public void String_vs_null_shows_null()
    {
        var result = Differ.Diff("hello", null);
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("null"));
    }

    [Test]
    public void Both_null_strings_are_equal()
    {
        var result = Differ.Diff((string?)null, (string?)null);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Empty_vs_null_are_not_equal()
    {
        var result = Differ.Diff("", null);
        Assert.That(result.AreEqual, Is.False);
    }
}
