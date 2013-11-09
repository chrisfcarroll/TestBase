using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ComparerEqualsByValueTests
{
    [TestFixture]
    public class WhenComparingClassesByValue
    {
        public class AClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public BClass More { get; set; }
        }
        public class BClass 
        {
            public string EvenMore { get; set; }
        }

        static readonly AClass object1 = new AClass
            {
                    Id=1,
                    Name = "1",
                    More = new BClass{EvenMore = "Evenmore1"}
            };
        static readonly AClass object1again = new AClass
            {
                    Id=1,
                    Name = "1",
                    More = new BClass{EvenMore = "Evenmore1"}
            };
        static readonly AClass object2 = new AClass
        {
            Id = 1,
            Name = "1",
            More = new BClass { EvenMore = "Evenmore2" }
        };

        [Test]
        public void Should_return_true_when_the_same()
        {
            object1.EqualsByValue(object1again).ShouldBeTrue();
        }

        [Test]
        public void Should_return_false_when_not_the_same()
        {
            object1.EqualsByValue(object2).ShouldBeFalse();
        }
    }
}