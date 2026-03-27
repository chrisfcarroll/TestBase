using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests;

[TestFixture]
public class WhenComparingAnonymousClassesByValue
{
    object objectL = new {Id = 1, Name = "1"};
    [Test]
    public void Should_return_false_when_not_the_same()
    {
            var objectR = new {Id = 1, Name = "2"};
            objectL.EqualsByValue(objectR).ShouldBeFalse();
    }

    [Test]
    public void Should_return_true_when_the_same()
    {
            var objectR = new {Id = 1, Name = "1"};
            objectL.EqualsByValue(objectR).ShouldBeTrue();
    }
}