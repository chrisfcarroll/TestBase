using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferCollectionTests
{
    [Test]
    public void Equal_lists()
    {
        var result = Differ.Diff(new[] { 1, 2, 3 }, new[] { 1, 2, 3 });
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Different_element_shows_index()
    {
        var result = Differ.Diff(new[] { 1, 2, 3 }, new[] { 1, 9, 3 });
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("[1]"));
    }

    [Test]
    public void Left_longer_shows_extra_elements()
    {
        var result = Differ.Diff(new[] { 1, 2, 3 }, new[] { 1, 2 });
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("lengths"));
    }

    [Test]
    public void Right_longer_shows_extra_elements()
    {
        var result = Differ.Diff(new[] { 1, 2 }, new[] { 1, 2, 3 });
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("lengths") | Does.Contain("extra"));
    }

    [Test]
    public void Empty_collections_are_equal()
    {
        var result = Differ.Diff(Array.Empty<int>(), Array.Empty<int>());
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Null_vs_empty_collection_are_not_equal_by_default()
    {
        var result = Differ.Diff(null, Array.Empty<int>());
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Null_vs_empty_collection_are_equal_when_option_set()
    {
        var result = Differ.Diff(null, Array.Empty<int>(), new DiffOptions { NullEqualsEmptyCollection = true });
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Empty_collection_vs_null_are_not_equal_by_default()
    {
        var result = Differ.Diff(Array.Empty<string>(), null);
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Empty_collection_vs_null_are_equal_when_option_set()
    {
        var result = Differ.Diff(Array.Empty<string>(), null, new DiffOptions { NullEqualsEmptyCollection = true });
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Null_vs_non_empty_collection()
    {
        var result = Differ.Diff(null, new[] { 1 });
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Collections_of_objects()
    {
        var left = new[] { new { Id = 1, Name = "A" }, new { Id = 2, Name = "B" } };
        var right = new[] { new { Id = 1, Name = "A" }, new { Id = 2, Name = "X" } };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("[1]"));
        Assert.That(text, Does.Contain("Name"));
    }

    [Test]
    public void List_vs_array_same_values()
    {
        var result = Differ.Diff(new List<int> { 1, 2, 3 }, new[] { 1, 2, 3 });
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Nested_collections()
    {
        var left = new[] { new[] { 1, 2 }, new[] { 3, 4 } };
        var right = new[] { new[] { 1, 2 }, new[] { 3, 5 } };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Max_differences_stops_early()
    {
        var left = new[] { 1, 2, 3, 4, 5 };
        var right = new[] { 10, 20, 30, 40, 50 };
        var result = Differ.Diff(left, right, new DiffOptions { MaxDifferences = 2 });
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
        Assert.That(result.Children.Count, Is.EqualTo(2));
    }

    [Test]
    public void Dictionary_comparison_works()
    {
        var left = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var right = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Dictionary_with_different_value()
    {
        var left = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var right = new Dictionary<string, int> { ["a"] = 1, ["b"] = 9 };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Reversed_int_array_is_different()
    {
        var left = new[] { 1, 2, 3 };
        var right = new[] { 3, 2, 1 };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Reversed_string_array_is_different()
    {
        var left = new[] { "1", "2", "3" };
        var right = new[] { "3", "2", "1" };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Array_vs_list_with_same_anonymous_objects()
    {
        var item = new { Id = 1, Name = "1" };
        var left = new[] { item };
        var right = new List<object> { new { Id = 1, Name = "1" } };
        Assert.That(Differ.Diff(left, right).AreEqual, Is.True);
    }

    [Test]
    public void Array_vs_list_with_different_anonymous_objects()
    {
        var left = new[] { new { Id = 1, Name = "1" } };
        var right = new[] { new { Id = 1, Name = "2" } };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Left_shorter_than_right()
    {
        var left = Array.Empty<int>();
        var right = new[] { 1 };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }
}
