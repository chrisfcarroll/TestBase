using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferIndexerTests
{
    class ClassWithIndexer
    {
        public string this[string key] => key;
        public int    Id   { get; set; }
        public string Name { get; set; }
    }

    [Test]
    public void Equal_objects_with_indexer()
    {
        var left = new ClassWithIndexer { Id = 1, Name = "1" };
        var right = new ClassWithIndexer { Id = 1, Name = "1" };
        Assert.That(Differ.Diff(left, right).AreEqual, Is.True);
    }

    [Test]
    public void Different_objects_with_indexer()
    {
        var left = new ClassWithIndexer { Id = 1, Name = "1" };
        var right = new ClassWithIndexer { Id = 1, Name = "2" };
        var result = Differ.Diff(left, right);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }
}
