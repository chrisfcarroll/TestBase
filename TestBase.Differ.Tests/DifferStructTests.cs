using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferStructTests
{
    public struct AStruct
    {
        public int     Id   { get; set; }
        public string  Name { get; set; }
        public BStruct More { get; set; }
    }

    public struct BStruct
    {
        public int    More     { get; set; }
        public string EvenMore { get; set; }
    }

    static readonly AStruct object1 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BStruct { More = 1, EvenMore = "Evenmore1" }
    };

    static readonly AStruct object1again = new()
    {
        Id   = 1,
        Name = "1",
        More = new BStruct { More = 1, EvenMore = "Evenmore1" }
    };

    static readonly AStruct object2 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BStruct { EvenMore = "Evenmore2" }
    };

    static readonly AStruct object3 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BStruct { More = 2, EvenMore = "Evenmore1" }
    };

    [Test]
    public void Equal_structs() =>
        Assert.That(Differ.Diff(object1, object1again).AreEqual, Is.True);

    [Test]
    public void Different_structs_nested_EvenMore()
    {
        var result = Differ.Diff(object1, object2);
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Different_structs_nested_More()
    {
        var result = Differ.Diff(object1, object3);
        Assert.That(result.AreEqual, Is.False);
    }
}
