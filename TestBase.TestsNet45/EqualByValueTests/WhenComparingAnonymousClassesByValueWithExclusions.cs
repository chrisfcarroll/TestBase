using System.Collections.Generic;
using NUnit.Framework;

namespace TestBase.TestsNet45.EqualByValueTests
{
    [TestFixture]
    public class WhenComparingAnonymousClassesByValueWithExclusions
    {
        [Test]
        public void Should_return_false_when_not_the_same()
        {
            var objectL       = new {Id = 1, Name = "1", Nested = new {NestedName = "N1", NestedMore = "M1"}};
            var objectR       = new {Id = 1, Name = "1", Nested = new {NestedName = "N2", NestedMore = "M2"}};
            var exclusionList = new List<string> {"Nested.NestedName"};

            //A&A
            objectL.EqualsByValueOrDiffersExceptFor(objectR, exclusionList).ShouldBeFalse();
            Assert.Throws<Assertion>(
                                     () => objectL.ShouldEqualByValueExceptFor(objectR, exclusionList)
                                    );
            objectL.EqualsByValueExceptFor(objectR, exclusionList).ShouldBeFalse();
        }

        [Test]
        public void Should_return_true_when_the_same()
        {
            //A
            var objectL       = new {Id = 1, Name = "1", Nested = new {NestedName = "N1"}};
            var objectR       = new {Id = 1, Name = "1", Nested = new {NestedName = "N2"}};
            var exclusionList = new List<string> {"IrrelevantExclusion", "Nested.NestedName"};

            //A & A
            objectL.EqualsByValueOrDiffersExceptFor(objectR, exclusionList).ShouldBeTrue();
            objectL.ShouldEqualByValueExceptFor(objectR, exclusionList);
            objectL.EqualsByValueExceptFor(objectR, exclusionList).ShouldBeTrue();
        }
    }
}
