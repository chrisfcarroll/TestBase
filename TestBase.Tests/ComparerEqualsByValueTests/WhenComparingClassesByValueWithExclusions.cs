using System.Collections.Generic;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ComparerEqualsByValueTests
{
    [TestFixture]
    public class WhenComparingClassesByValueWithExclusions
    {
        internal class AClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public BClass More { get; set; }
        }
        internal class BClass 
        {
            public string EvenMore { get; set; }
        }

        readonly AClass object1 = new AClass
            {
                    Id=1,
                    Name = "1",
                    More = new BClass{EvenMore = "Evenmore1"}
            };
        readonly AClass object2 = new AClass
        {
            Id = 1,
            Name = "1",
            More = new BClass { EvenMore = "Evenmore2" }
        };

        [Test]
        public void Should_return_true_when_the_same_apart_from_exclusions()
        {
            Comparer.MemberCompare(object1, object2, null, new List<string>{"More"}).ShouldEqual( (BoolWithString)true, "Failed to exclude More");
            Comparer.MemberCompare(object1, object2, null, new List<string>{ "More.EvenMore" }).ShouldEqual((BoolWithString)true, "Failed to exclude More.EvenMore");
            object1.ShouldEqualByValueExceptFor(object2, new List<string> {"More"});
        }

        [Test]
        public void Should_return_false_when_not_the_same()
        {
            var result = Comparer.MemberCompare(object1, object2, null, new List<string> { "IrrelevantExclusion" });
            ((bool)result).ShouldEqual(false);
            Assert.Throws<AssertionException>(
                () => object1.ShouldEqualByValueExceptFor(object2, new List<string> { "IrrelevantExclusion" }));
        }
    }
}