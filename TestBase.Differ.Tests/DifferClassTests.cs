using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferClassTests
{
    class AClass
    {
        public int    Id   { get; set; }
        public string Name { get; set; }
        public BClass More { get; set; }
    }

    class BClass
    {
        public int    More     { get; set; }
        public string EvenMore { get; set; }
    }

    static readonly AClass object1 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BClass { More = 1, EvenMore = "Evenmore1" }
    };

    static readonly AClass object1again = new()
    {
        Id   = 1,
        Name = "1",
        More = new BClass { More = 1, EvenMore = "Evenmore1" }
    };

    static readonly AClass object2 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BClass { EvenMore = "Evenmore2" }
    };

    static readonly AClass object3 = new()
    {
        Id   = 1,
        Name = "1",
        More = new BClass { More = 2, EvenMore = "Evenmore1" }
    };

    [Test]
    public void Equal_classes() =>
        Assert.That(Differ.Diff(object1, object1again).AreEqual, Is.True);

    [Test]
    public void Different_classes_nested_EvenMore()
    {
        var result = Differ.Diff(object1, object2);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("EvenMore"));
    }

    [Test]
    public void Different_classes_nested_More_int()
    {
        var result = Differ.Diff(object1, object3);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
        var text = result.ToString();
        Assert.That(text, Does.Contain("More"));
    }

    [Test]
    public void Left_null_right_not_null()
    {
        var result = Differ.Diff(null, object1);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Left_not_null_right_null()
    {
        var result = Differ.Diff(object1, null);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Nested_null_vs_non_null()
    {
        var left = new AClass { Id = 1, Name = "1", More = null };
        var right = new AClass { Id = 1, Name = "1", More = new BClass { More = 1 } };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Both_nested_null_are_equal()
    {
        var left = new AClass { Id = 1, Name = "1", More = null };
        var right = new AClass { Id = 1, Name = "1", More = null };
        var result = Differ.Diff(left, right);
        Assert.That(result.AreEqual, Is.True);
    }
}
