using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DiffFormatterTests
{
    [TearDown]
    public void TearDown() => DiffFormatter.UseColour = false;

    [Test]
    public void Format_equal_result_without_colour()
    {
        var result = Differ.Diff(1, 1);
        Assert.That(DiffFormatter.Format(result), Is.EqualTo("Equal"));
    }

    [Test]
    public void Format_equal_result_with_colour()
    {
        DiffFormatter.UseColour = true;
        var result = Differ.Diff(1, 1);
        var text = DiffFormatter.Format(result);
        Assert.That(text, Does.Contain("Equal"));
        Assert.That(text, Does.Contain("\x1b["));
    }

    [Test]
    public void Format_diff_without_colour()
    {
        var result = Differ.Diff(1, 2);
        var text = DiffFormatter.Format(result);
        Assert.That(text, Does.Contain("Expected"));
        Assert.That(text, Does.Contain("Actual"));
        Assert.That(text, Does.Not.Contain("\x1b["));
    }

    [Test]
    public void Format_diff_with_colour_contains_ansi_codes()
    {
        DiffFormatter.UseColour = true;
        var result = Differ.Diff(1, 2);
        var text = DiffFormatter.Format(result);
        Assert.That(text, Does.Contain("\x1b[31m")); // Red
        Assert.That(text, Does.Contain("\x1b[32m")); // Green
        Assert.That(text, Does.Contain("\x1b[0m"));  // Reset
    }

    [Test]
    public void Format_collection_diff_with_colour()
    {
        DiffFormatter.UseColour = true;
        var result = Differ.Diff(new[] { 1, 2 }, new[] { 1, 3 });
        var text = DiffFormatter.Format(result);
        Assert.That(text, Does.Contain("[1]"));
        Assert.That(text, Does.Contain("\x1b["));
    }

    [Test]
    public void Format_object_diff_with_colour()
    {
        DiffFormatter.UseColour = true;
        var result = Differ.Diff(new { Name = "A" }, new { Name = "B" });
        var text = DiffFormatter.Format(result);
        Assert.That(text, Does.Contain("Name"));
        Assert.That(text, Does.Contain("\x1b[1m")); // Bold
    }
}
