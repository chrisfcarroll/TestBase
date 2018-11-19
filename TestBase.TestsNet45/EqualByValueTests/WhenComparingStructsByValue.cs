using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests
{
    [TestFixture]
    public class WhenComparingStructsByValue
    {
        public struct AStruct
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public BStruct More { get; set; }
        }
        public struct BStruct 
        {
            public int More        { get; set; }
            public string EvenMore { get; set; }
        }

        static readonly AStruct object1 = new AStruct
            {
                    Id=1,
                    Name = "1",
                    More = new BStruct{More=1, EvenMore = "Evenmore1"}
            };
        static readonly AStruct object1again = new AStruct
            {
                    Id=1,
                    Name = "1",
                    More = new BStruct{ More=1, EvenMore = "Evenmore1"}
            };
        static readonly AStruct object2 = new AStruct
        {
            Id = 1,
            Name = "1",
            More = new BStruct { EvenMore = "Evenmore2" }
        };
        static readonly AStruct object3 = new AStruct
        {
            Id = 1,
            Name = "1",
            More = new BStruct { More = 2, EvenMore = "Evenmore1" }
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