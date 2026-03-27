using NUnit.Framework;

namespace TestBase.DifferTests;

/// <summary>
/// Demo tests that demonstrate how concise the diff output is.
/// These tests output to TestContext.Out to show what a user would see,
/// and assert that the output is as concise as we want it to be.
/// </summary>
[TestFixture]
public class DifferOutputConcisenessTests
{
    [Test]
    public void Demo_single_element_difference()
    {
        var left = new[] { 1, 2, 3 };
        var right = new[] { 1, 9, 3 };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Single element difference ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Assert conciseness: should show index and values on one line
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.LessThanOrEqualTo(2), $"Too many lines:\n{output}");
        Assert.That(output, Does.Contain("[1]"));
    }

    [Test]
    public void Demo_left_longer()
    {
        var left = new[] { 1, 2, 3 };
        var right = new[] { 1, 2 };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Left longer (extra element) ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Should be just one line showing the missing element
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1), $"Expected 1 line:\n{output}");
        Assert.That(output, Does.Contain("[2]") & Does.Contain("missing"));
    }

    [Test]
    public void Demo_right_longer()
    {
        var left = new[] { 1, 2 };
        var right = new[] { 1, 2, 3 };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Right longer (extra element) ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Should be just one line showing the missing element
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1), $"Expected 1 line:\n{output}");
        Assert.That(output, Does.Contain("[2]") & Does.Contain("missing"));
    }

    [Test]
    public void Demo_multiple_differences()
    {
        var left = new[] { 1, 2, 3 };
        var right = new[] { 9, 8, 7 };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Multiple differences ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Each difference should be one line
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.LessThanOrEqualTo(4), $"Too many lines:\n{output}");
    }

    [Test]
    public void Demo_nested_collections()
    {
        var left = new[] { new[] { 1, 2 }, new[] { 3, 4 } };
        var right = new[] { new[] { 1, 2 }, new[] { 3, 9 } };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Nested collections ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Should not repeat "collections differ" at each level
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.LessThanOrEqualTo(2), $"Too many lines:\n{output}");
    }

    [Test]
    public void Demo_object_collection()
    {
        var left = new[] { new { Id = 1, Name = "A" }, new { Id = 2, Name = "B" } };
        var right = new[] { new { Id = 1, Name = "A" }, new { Id = 2, Name = "X" } };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Collection of objects ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Should show just the property that differs
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.LessThanOrEqualTo(2), $"Too many lines:\n{output}");
        Assert.That(output, Does.Contain("Name"));
    }

    [Test]
    public void Demo_dictionary_difference()
    {
        var left = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var right = new Dictionary<string, int> { ["a"] = 1, ["b"] = 9 };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Dictionary difference ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Should show key-based diff: ["b"]: Expected = 2, Actual = 9
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1), $"Expected 1 line:\n{output}");
        Assert.That(output, Does.Contain("[\"b\"]"));
    }

    [Test]
    public void Demo_length_difference_only()
    {
        var left = new[] { 1, 2, 3, 4, 5 };
        var right = new[] { 1, 2, 3 };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Length difference (extra elements) ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Should show each missing element on its own line
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(2), $"Expected 2 lines (one per extra element):\n{output}");
        Assert.That(output, Does.Contain("[3]") & Does.Contain("[4]") & Does.Contain("missing"));
    }

    [Test]
    public void Demo_deeply_nested()
    {
        var left = new { Items = new[] { new { Sub = new[] { 1, 2 } } } };
        var right = new { Items = new[] { new { Sub = new[] { 1, 9 } } } };
        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Deeply nested ===");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Should show path to difference concisely
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.LessThanOrEqualTo(2), $"Too many lines:\n{output}");
    }
}
