using NUnit.Framework;

namespace TestBase.DifferTests;

/// <summary>
/// Demo tests that demonstrate concise diff output for complex objects with many properties.
/// Shows the input objects and the concise diff output on TestContext.Out.
/// </summary>
[TestFixture]
public class DifferOutputConcisenessForComplexObjectsTests
{
    record Person(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        int Age,
        string Department,
        int Salary,
        bool IsActive,
        DateTime HireDate,
        int? ManagerId);

    record Address(
        string Street,
        string City,
        string State,
        string ZipCode,
        string Country);

    record Employee(
        int Id,
        string Name,
        Address HomeAddress,
        Address WorkAddress,
        List<string> Skills,
        Dictionary<string, int> Ratings);

    [Test]
    public void Demo_object_with_many_properties_few_differences()
    {
        var left = new Person(
            Id: 1,
            FirstName: "John",
            LastName: "Smith",
            Email: "john@example.com",
            Age: 30,
            Department: "Engineering",
            Salary: 75000,
            IsActive: true,
            HireDate: new DateTime(2020, 1, 15),
            ManagerId: 5);

        var right = new Person(
            Id: 1,
            FirstName: "John",
            LastName: "Smith",
            Email: "john.smith@example.com",  // changed
            Age: 31,                           // changed
            Department: "Engineering",
            Salary: 80000,                    // changed
            IsActive: true,
            HireDate: new DateTime(2020, 1, 15),
            ManagerId: 5);

        var result = Differ.Diff(left, right, new DiffOptions { MaxDifferences = 10 });
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Object with 10 properties, 3 differences ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Left:  " + left);
        TestContext.Out.WriteLine("Right: " + right);
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Assert conciseness: should show only the 3 differences, one per line
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(3), $"Expected 3 lines (one per diff):\n{output}");
        Assert.That(output, Does.Contain("Email"));
        Assert.That(output, Does.Contain("Age"));
        Assert.That(output, Does.Contain("Salary"));
    }

    [Test]
    public void Demo_object_with_nested_objects()
    {
        var left = new Employee(
            Id: 1,
            Name: "Alice",
            HomeAddress: new Address("123 Main St", "Boston", "MA", "02101", "USA"),
            WorkAddress: new Address("456 Office Blvd", "Boston", "MA", "02102", "USA"),
            Skills: new List<string> { "C#", "SQL", "Azure" },
            Ratings: new Dictionary<string, int> { ["2023"] = 4, ["2024"] = 5 });

        var right = new Employee(
            Id: 1,
            Name: "Alice",
            HomeAddress: new Address("789 New St", "Boston", "MA", "02103", "USA"),  // Street & Zip changed
            WorkAddress: new Address("456 Office Blvd", "Boston", "MA", "02102", "USA"),
            Skills: new List<string> { "C#", "SQL", "Azure" },
            Ratings: new Dictionary<string, int> { ["2023"] = 4, ["2024"] = 5 });

        var result = Differ.Diff(left, right, new DiffOptions { MaxDifferences = 10 });
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Object with nested Address, 2 differences in HomeAddress ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Left.HomeAddress:  " + left.HomeAddress);
        TestContext.Out.WriteLine("Right.HomeAddress: " + right.HomeAddress);
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Assert conciseness: nested diffs should show full path
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(2), $"Expected 2 lines:\n{output}");
        Assert.That(output, Does.Contain("HomeAddress.Street"));
        Assert.That(output, Does.Contain("HomeAddress.ZipCode"));
    }

    [Test]
    public void Demo_object_single_property_difference()
    {
        var left = new Person(1, "John", "Smith", "john@example.com", 30, "Engineering", 75000, true, new DateTime(2020, 1, 15), 5);
        var right = new Person(1, "John", "Smith", "john@example.com", 30, "Engineering", 75000, false, new DateTime(2020, 1, 15), 5);

        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Single property difference (IsActive) ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Assert conciseness: single line for single difference
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1), $"Expected 1 line:\n{output}");
        Assert.That(output, Does.Contain("IsActive"));
    }

    [Test]
    public void Demo_object_with_null_vs_value()
    {
        var left = new Person(1, "John", "Smith", "john@example.com", 30, "Engineering", 75000, true, new DateTime(2020, 1, 15), null);
        var right = new Person(1, "John", "Smith", "john@example.com", 30, "Engineering", 75000, true, new DateTime(2020, 1, 15), 5);

        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Null vs value (ManagerId: null vs 5) ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1), $"Expected 1 line:\n{output}");
        Assert.That(output, Does.Contain("ManagerId"));
    }

    [Test]
    public void Demo_objects_with_different_types_same_shape()
    {
        var left = new { Id = 1, Name = "Test", Value = 100 };
        var right = new { Id = 1, Name = "Test", Value = 200 };

        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Anonymous objects with one difference ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine($"Left:  {left}");
        TestContext.Out.WriteLine($"Right: {right}");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1), $"Expected 1 line:\n{output}");
        Assert.That(output, Does.Contain("Value"));
    }

    [Test]
    public void Demo_deeply_nested_object()
    {
        var left = new
        {
            Company = new
            {
                Name = "Acme Corp",
                Location = new
                {
                    Address = new { City = "Boston", State = "MA" }
                }
            }
        };

        var right = new
        {
            Company = new
            {
                Name = "Acme Corp",
                Location = new
                {
                    Address = new { City = "Seattle", State = "WA" }
                }
            }
        };

        var result = Differ.Diff(left, right, new DiffOptions { MaxDifferences = 10 });
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Deeply nested object (4 levels) ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        // Assert: full path shown on single lines
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(2), $"Expected 2 lines:\n{output}");
        Assert.That(output, Does.Contain("Company.Location.Address.City"));
        Assert.That(output, Does.Contain("Company.Location.Address.State"));
    }

    [Test]
    public void Demo_object_with_collection_property()
    {
        var left = new { Name = "Team A", Members = new[] { "Alice", "Bob", "Charlie" } };
        var right = new { Name = "Team A", Members = new[] { "Alice", "Bob", "David" } };

        var result = Differ.Diff(left, right);
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Object with collection property difference ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine($"Left.Members:  [{string.Join(", ", left.Members)}]");
        TestContext.Out.WriteLine($"Right.Members: [{string.Join(", ", right.Members)}]");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.LessThanOrEqualTo(2), $"Too many lines:\n{output}");
        Assert.That(output, Does.Contain("Members[2]"));
    }

    [Test]
    public void Demo_object_with_dictionary_property()
    {
        var left = new
        {
            Name = "Config",
            Settings = new Dictionary<string, string>
            {
                ["timeout"] = "30",
                ["retries"] = "3",
                ["debug"] = "false"
            }
        };

        var right = new
        {
            Name = "Config",
            Settings = new Dictionary<string, string>
            {
                ["timeout"] = "60",
                ["retries"] = "3",
                ["debug"] = "true"
            }
        };

        var result = Differ.Diff(left, right, new DiffOptions { MaxDifferences = 10 });
        var output = result.ToString();

        TestContext.Out.WriteLine("=== Object with dictionary property, 2 values differ ===");
        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine("Diff output:");
        TestContext.Out.WriteLine(output);
        TestContext.Out.WriteLine();

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(2), $"Expected 2 lines:\n{output}");
        Assert.That(output, Does.Contain("Settings[\"timeout\"]"));
        Assert.That(output, Does.Contain("Settings[\"debug\"]"));
    }
}
