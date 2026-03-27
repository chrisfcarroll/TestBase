using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferNullAndEdgeCaseTests
{
    [Test] public void Both_null() => Assert.That(Differ.Diff(null, null).AreEqual, Is.True);
    [Test]
    public void Left_null_right_value()
    {
        var result = Differ.Diff(null, 42);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }
    [Test]
    public void Left_value_right_null()
    {
        var result = Differ.Diff(42, null);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }
    [Test] public void DBNull_and_null() => Assert.That(Differ.Diff(DBNull.Value, null).AreEqual, Is.True);
    [Test] public void Null_and_DBNull() => Assert.That(Differ.Diff(null, DBNull.Value).AreEqual, Is.True);

    [Test]
    public void DBNull_and_null_when_NullEqualsDbNull_is_false()
    {
        var opts = new DiffOptions { NullEqualsDbNull = false };
        Assert.That(Differ.Diff(DBNull.Value, null, opts).AreEqual, Is.False);
        Assert.That(Differ.Diff(null, DBNull.Value, opts).AreEqual, Is.False);
    }

    [Test]
    public void Same_reference_is_equal()
    {
        var obj = new { Id = 1 };
        Assert.That(Differ.Diff(obj, obj).AreEqual, Is.True);
    }

    [Test]
    public void Cyclic_reference_does_not_stack_overflow()
    {
        var left = new CyclicNode { Value = 1 };
        left.Next = left;
        var right = new CyclicNode { Value = 1 };
        right.Next = right;
        // Should not throw
        var result = Differ.Diff(left, right);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void ToString_on_equal_result()
    {
        var result = Differ.Diff(1, 1);
        Assert.That(result.ToString(), Is.EqualTo("Equal"));
    }

    [Test]
    public void DiffResult_implicit_bool_conversion()
    {
        DiffResult equal = Differ.Diff(1, 1);
        DiffResult notEqual = Differ.Diff(1, 2);
        Assert.That((bool)equal, Is.True);
        //D
        TestContext.Progress.WriteLine(notEqual.ToString());
        //A
        Assert.That((bool)notEqual, Is.False);
    }

    [Test]
    public void RequireSameType_catches_structural_match_different_type()
    {
        var left = new TypeA { X = 1 };
        var right = new TypeB { X = 1 };
        var equalStructural = Differ.Diff(left, right);
        Assert.That(equalStructural.AreEqual, Is.True);

        var notEqualTyped = Differ.Diff(left, right, new DiffOptions { RequireSameType = true });
        //D
        TestContext.Progress.WriteLine(notEqualTyped.ToString());
        //A
        Assert.That(notEqualTyped.AreEqual, Is.False);
    }

    [Test]
    public void NullEqualsMissingProperty_default_is_true()
    {
        var left = new { A = 1, B = (string?)null };
        var right = new { A = 1 };
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }

    [Test]
    public void NullEqualsMissingProperty_set_to_false()
    {
        var left = new { A = 1, B = (string?)null };
        var right = new { A = 1 };
        var opts = new DiffOptions { NullEqualsMissingProperty = false };
        var result = Differ.Diff(left, right, opts);
        Assert.That(result.AreEqual, Is.False);
        Assert.That(result.ToString(), Does.Contain("property missing on right"));
    }

    [Test]
    public void NullEqualsMissingProperty_missing_on_left()
    {
        var left = new { A = 1 };
        var right = new { A = 1, B = (string?)null };
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);

        var opts = new DiffOptions { NullEqualsMissingProperty = false };
        var result2 = Differ.Diff(left, right, opts);
        Assert.That(result2.AreEqual, Is.False);
        Assert.That(result2.ToString(), Does.Contain("property missing on left"));
    }

    [Test]
    public void NullEqualsMissingProperty_applies_to_fields()
    {
        var left = new TypeWithField { X = null };
        var right = new TypeWithoutField { };
        
        // Default (true)
        Assert.That(Differ.Diff(left, right).AreEqual, Is.True);
        Assert.That(Differ.Diff(right, left).AreEqual, Is.True);

        // False
        var opts = new DiffOptions { NullEqualsMissingProperty = false };
        Assert.That(Differ.Diff(left, right, opts).AreEqual, Is.False);
        Assert.That(Differ.Diff(right, left, opts).AreEqual, Is.False);
    }

    class TypeWithField { public string? X; }
    class TypeWithoutField { }

    class CyclicNode
    {
        public int Value { get; set; }
        public CyclicNode? Next { get; set; }
    }

    class TypeA { public int X { get; set; } }
    class TypeB { public int X { get; set; } }
}
