using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests
{
    [TestFixture]
    public class WhenComparingClassesByValue
    {
        static readonly AClass object1 = new AClass
        {
            Id = 1,
            Name = "1",
            More = new BClass { More = 1, EvenMore = "Evenmore1" }
        };
        static readonly AClass object1again = new AClass
        {
            Id = 1,
            Name = "1",
            More = new BClass { More = 1, EvenMore = "Evenmore1" }
        };
        static readonly AClass object2 = new AClass
        {
            Id = 1,
            Name = "1",
            More = new BClass { EvenMore = "Evenmore2" }
        };
        static readonly AClass object3 = new AClass
        {
            Id = 1,
            Name = "1",
            More = new BClass { More = 2, EvenMore = "Evenmore1" }
        };

        [Test]
        public void Should_return_true_when_the_same()
        {
            object1.EqualsByValue(object1again).ShouldBeTrue();
        }

        [Test]
        public void Should_return_false_when_not_the_same()
        {
            object1.EqualsByValue(object2).ShouldBeFalse("Failed to distinguish object1 from object 2");
            object1.EqualsByValue(object3).ShouldBeFalse("Failed to distinguish object1 from object 3");
        }
    }
}