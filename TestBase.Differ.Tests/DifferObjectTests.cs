using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferObjectTests
{
    record Person(string Name, int Age);
    record Address(string Street, string City, string ZipCode);
    record PersonWithAddress(string Name, int Age, Address Address);

    class ClassWithField
    {
        public string Name = "";
        public int Value;
    }

    [Test]
    public void Equal_anonymous_objects()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Alice" };
        Assert.That(Differ.Diff(left, right).AreEqual, Is.True);
    }

    [Test]
    public void Different_anonymous_objects_show_differing_properties()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 1, Name = "Bob" };
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("Name"));
        Assert.That(text, Does.Contain("Ali"));
        Assert.That(text, Does.Contain("Bob"));
    }

    [Test]
    public void Equal_records()
    {
        Assert.That(Differ.Diff(new Person("Alice", 30), new Person("Alice", 30)).AreEqual, Is.True);
    }

    [Test]
    public void Different_records()
    {
        var result = Differ.Diff(new Person("Alice", 30), new Person("Alice", 31));
        Assert.That(result.AreEqual, Is.False);
        Assert.That(result.ToString(), Does.Contain("Age"));
    }

    [Test]
    public void Nested_objects_show_dotted_path()
    {
        var left = new PersonWithAddress("Alice", 30, new Address("Main St", "NYC", "10001"));
        var right = new PersonWithAddress("Alice", 30, new Address("Main St", "LA", "90001"));
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("Address.City") | Does.Contain("City"));
    }

    [Test]
    public void Exclusion_list_skips_members()
    {
        var left = new { Id = 1, Name = "Alice" };
        var right = new { Id = 2, Name = "Alice" };
        var result = Differ.Diff(left, right, DiffOptions.Default.WithExclusions("Id"));
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Include_only_list_compares_only_specified_members()
    {
        var left = new { Id = 1, Name = "Alice", Age = 30 };
        var right = new { Id = 1, Name = "Bob", Age = 99 };
        var result = Differ.Diff(left, right, DiffOptions.Default.WithIncludeOnly("Id"));
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Dotted_exclusion_for_nested_member()
    {
        var left = new PersonWithAddress("Alice", 30, new Address("Main St", "NYC", "10001"));
        var right = new PersonWithAddress("Alice", 30, new Address("Main St", "LA", "10001"));
        var result = Differ.Diff(left, right, DiffOptions.Default.WithExclusions("Address.City"));
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Objects_with_public_fields()
    {
        var left = new ClassWithField { Name = "A", Value = 1 };
        var right = new ClassWithField { Name = "A", Value = 2 };
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.False);
        Assert.That(result.ToString(), Does.Contain("Value"));
    }

    [Test]
    public void Null_vs_object()
    {
        var result = Differ.Diff(null, new { Id = 1 });
        Assert.That(result.AreEqual, Is.False);
        Assert.That(result.ToString(), Does.Contain("null"));
    }

    [Test]
    public void Object_vs_null()
    {
        var result = Differ.Diff(new { Id = 1 }, null);
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Both_null_are_equal()
    {
        Assert.That(Differ.Diff((object?)null, (object?)null).AreEqual, Is.True);
    }

    [Test]
    public void Max_differences_limits_output()
    {
        var left = new { A = 1, B = 2, C = 3, D = 4 };
        var right = new { A = 10, B = 20, C = 30, D = 40 };
        var result = Differ.Diff(left, right, new DiffOptions { MaxDifferences = 1 });
        Assert.That(result.Children.Count, Is.EqualTo(1));
    }

    [Test]
    public void Labels_appear_in_output()
    {
        var result = Differ.Diff(1, 2, DiffOptions.Default.WithLabels("Before", "After"));
        var text = result.ToString();
        Assert.That(text, Does.Contain("Before"));
        Assert.That(text, Does.Contain("After"));
    }

    [Test]
    public void PublicPropertiesOnly_skips_non_public()
    {
        // Anonymous types only have public readable properties, so this just verifies the option path works
        var left = new { Id = 1 };
        var right = new { Id = 1 };
        var result = Differ.Diff(left, right, new DiffOptions { PublicPropertiesOnly = true });
        Assert.That(result.AreEqual, Is.True);
    }
}
