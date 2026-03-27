using System.IO;
using NUnit.Framework;

namespace TestBase.DifferTests;

[TestFixture]
public class DifferFileSystemInfoTests
{
    [Test]
    public void Equal_FileInfo_arrays()
    {
        var list1 = new DirectoryInfo(".").GetFiles("*");
        var list1Again = new DirectoryInfo(".").GetFiles("*");
        Assume.That(list1.Length, Is.GreaterThan(0), "Test needs at least one file in cwd");
        Assert.That(Differ.Diff(list1, list1Again).AreEqual, Is.True);
    }

    [Test]
    public void Different_FileInfo_arrays()
    {
        var list1 = new DirectoryInfo(".").GetFiles("*");
        var list2 = new DirectoryInfo("..").GetFiles("*");
        Assume.That(list1.Length, Is.GreaterThan(0));
        Assume.That(list2.Length, Is.GreaterThan(0));
        var result = Differ.Diff(list1, list2);
        //D
        TestContext.Progress.WriteLine(result.ToString());
        //A
        Assert.That(result.AreEqual, Is.False);
    }

    [Test]
    public void Same_FileInfo_is_equal()
    {
        var files = new DirectoryInfo(".").GetFiles("*");
        Assume.That(files.Length, Is.GreaterThan(0));
        Assert.That(Differ.Diff(files[0], files[0]).AreEqual, Is.True);
    }
}
