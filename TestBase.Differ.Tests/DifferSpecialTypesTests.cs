using System.Numerics;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace TestBase.DifferTests;

/// <summary>
/// Tests for special types that need specific handling:
/// - ValueTuples (ITuple)
/// - Type objects
/// - System.Numerics types (BigInteger, Complex)
/// </summary>
[TestFixture]
public class DifferSpecialTypesTests
{
    // ========== ValueTuple Tests ==========

    [Test]
    public void ValueTuple_equal_values()
    {
        var left = (Id: 1, Name: "John");
        var right = (Id: 1, Name: "John");
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void ValueTuple_different_values()
    {
        var left = (Id: 1, Name: "John");
        var right = (Id: 1, Name: "Jane");
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void ValueTuple_nested()
    {
        var left = (Id: 1, Inner: (A: "x", B: "y"));
        var right = (Id: 1, Inner: (A: "x", B: "z"));
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void ValueTuple_in_collection()
    {
        var left = new[] { (1, "A"), (2, "B") };
        var right = new[] { (1, "A"), (2, "C") };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void ValueTuple_is_ITuple()
    {
        var tuple = (1, "test");
        Assert.That(tuple, Is.InstanceOf<ITuple>());
    }

    // ========== Type Object Tests ==========

    [Test]
    public void Type_same_type_should_be_equal()
    {
        var left = typeof(string);
        var right = typeof(string);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Type_different_types_should_differ()
    {
        var left = typeof(string);
        var right = typeof(int);
        var result = Differ.Diff(left, right);
        var output = result.ToString();
        //D
        TestContext.Out.WriteLine(output);
        //A
        Assert.That(result.AreEqual, Is.False);
        // Output should be concise - just the type names, not all Type properties
        Assert.That(output, Does.Contain("System.String"));
        Assert.That(output, Does.Contain("System.Int32"));
        Assert.That(output.Length, Is.LessThan(200), "Type diff should be concise");
    }

    [Test]
    public void Type_in_object_property()
    {
        var left = new { Name = "Test", ValueType = typeof(int) };
        var right = new { Name = "Test", ValueType = typeof(string) };
        var result = Differ.Diff(left, right);
        var output = result.ToString();
        //D
        TestContext.Out.WriteLine(output);
        //A
        Assert.That(result.AreEqual, Is.False);
        Assert.That(output, Does.Contain("ValueType"));
        Assert.That(output, Does.Contain("System.Int32"));
        Assert.That(output, Does.Contain("System.String"));
        // Should be concise - one line, not a huge dump of Type properties
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1), $"Should be 1 line:\n{output}");
    }

    // ========== System.Numerics Tests ==========

    [Test]
    public void BigInteger_equal_values()
    {
        var left = new BigInteger(12345678901234567890);
        var right = new BigInteger(12345678901234567890);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void BigInteger_different_values()
    {
        var left = new BigInteger(12345678901234567890);
        var right = new BigInteger(12345678901234567891);
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Complex_equal_values()
    {
        var left = new Complex(1.0, 2.0);
        var right = new Complex(1.0, 2.0);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Complex_different_values()
    {
        var left = new Complex(1.0, 2.0);
        var right = new Complex(1.0, 3.0);
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    // ========== DateTime/DateOnly/TimeOnly Tests ==========

    [Test]
    public void DateTime_equal()
    {
        var left = new DateTime(2024, 1, 15, 10, 30, 0);
        var right = new DateTime(2024, 1, 15, 10, 30, 0);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void DateTime_different()
    {
        var left = new DateTime(2024, 1, 15, 10, 30, 0);
        var right = new DateTime(2024, 1, 15, 10, 31, 0);
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void DateOnly_equal()
    {
        var left = new DateOnly(2024, 1, 15);
        var right = new DateOnly(2024, 1, 15);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void DateOnly_different()
    {
        var left = new DateOnly(2024, 1, 15);
        var right = new DateOnly(2024, 1, 16);
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void TimeOnly_equal()
    {
        var left = new TimeOnly(10, 30, 0);
        var right = new TimeOnly(10, 30, 0);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void TimeOnly_different()
    {
        var left = new TimeOnly(10, 30, 0);
        var right = new TimeOnly(10, 31, 0);
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void TimeSpan_equal()
    {
        var left = TimeSpan.FromMinutes(90);
        var right = TimeSpan.FromMinutes(90);
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void TimeSpan_different()
    {
        var left = TimeSpan.FromMinutes(90);
        var right = TimeSpan.FromMinutes(91);
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Guid_equal()
    {
        var guid = Guid.NewGuid();
        var left = guid;
        var right = guid;
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void Guid_different()
    {
        var left = Guid.NewGuid();
        var right = Guid.NewGuid();
        var result = Differ.Diff(left, right);
        //D
        TestContext.Out.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }
}
