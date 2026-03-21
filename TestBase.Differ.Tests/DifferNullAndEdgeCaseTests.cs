using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferNullAndEdgeCaseTests
{
    [Test] public void Both_null() => Assert.That(Differ.Diff(null, null).AreEqual, Is.True);
    [Test] public void Left_null_right_value() => Assert.That(Differ.Diff(null, 42).AreEqual, Is.False);
    [Test] public void Left_value_right_null() => Assert.That(Differ.Diff(42, null).AreEqual, Is.False);
    [Test] public void DBNull_and_null() => Assert.That(Differ.Diff(DBNull.Value, null).AreEqual, Is.True);
    [Test] public void Null_and_DBNull() => Assert.That(Differ.Diff(null, DBNull.Value).AreEqual, Is.True);

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
        Assert.That(notEqualTyped.AreEqual, Is.False);
    }

    class CyclicNode
    {
        public int Value { get; set; }
        public CyclicNode? Next { get; set; }
    }

    class TypeA { public int X { get; set; } }
    class TypeB { public int X { get; set; } }
}
