using System.IO;
using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests;

[TestFixture]
public class WhenComparingFileSystemInfoInstances
{
    FileInfo[] list1 = new DirectoryInfo(".").GetFiles("*");
    FileInfo[] list1Again = new DirectoryInfo(".").GetFiles("*");
    FileInfo[] list2 = new DirectoryInfo("..").GetFiles("*");
        
    [Test]
    public void Should_return_false_when_not_the_same()
    {
            list1.ShouldNotBeEmpty("Aborted test : didn't expect an empty directory");
            list2.ShouldNotBeNull("Aboted test : didn't expect null return from DirectoryInfo.GetFiles()");
            list1.EqualsByValue(list2).ShouldBeFalse("Failed to distinguish object1 from object 2");
        }

    [Test]
    public void Should_return_true_and_not_throw_a_stackoverflow_when_the_same()
    {
            list1.EqualsByValue(list1Again).ShouldBeTrue();
        }
}