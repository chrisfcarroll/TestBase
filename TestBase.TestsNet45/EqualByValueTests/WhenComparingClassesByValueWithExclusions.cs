using System.Collections.Generic;
using NUnit.Framework;

namespace TestBase.TestsNet45.EqualByValueTests
{
    [TestFixture]
    public class WhenComparingClassesByValueWithExclusions
    {
        internal class AClass
        {
            public int    Id   { get; set; }
            public string Name { get; set; }
            public BClass More { get; set; }
        }

        internal class BClass
        {
            public string EvenMore { get; set; }
        }

        readonly AClass A11Evenmore1 = new AClass
                                       {
                                       Id   = 1,
                                       Name = "1",
                                       More = new BClass {EvenMore = "Evenmore1"}
                                       };

        readonly AClass A11Evenmore2 = new AClass
                                       {
                                       Id   = 1,
                                       Name = "1",
                                       More = new BClass {EvenMore = "Evenmore2"}
                                       };

        [Test]
        public void MemberCompare_Should_return_false_when_not_the_same()
        {
            var result = Comparer.MemberCompare(A11Evenmore1, A11Evenmore2, new List<string> {"IrrelevantExclusion"});
            ((bool) result).ShouldEqual(false);
            Assert.Throws<Assertion>(() => A11Evenmore1.ShouldEqualByValueExceptFor(A11Evenmore2,
                                                                                    new List<string>
                                                                                    {"IrrelevantExclusion"}));
        }

        [Test]
        public void Should_return_true_when_the_same_apart_from_exclusions()
        {
            Comparer.MemberCompare(A11Evenmore1, A11Evenmore2, new List<string> {"More"})
                    .ShouldEqual((BoolWithString) true, "Failed to exclude More");
            Comparer.MemberCompare(A11Evenmore1, A11Evenmore2, new List<string> {"More.EvenMore"})
                    .ShouldEqual((BoolWithString) true, "Failed to exclude More.EvenMore");
            A11Evenmore1.ShouldEqualByValueExceptFor(A11Evenmore2, new List<string> {"More"});
        }

        [Test]
        public void ShouldEqual_Should_Throw_Assertion_when_not_the_same()
        {
            var result = Comparer.MemberCompare(A11Evenmore1, A11Evenmore2, new List<string> {"IrrelevantExclusion"});
            ((bool) result).ShouldEqual(false);
            Assert.Throws<Assertion>(() => A11Evenmore1.ShouldEqualByValueExceptFor(A11Evenmore2,
                                                                                    new List<string>
                                                                                    {"IrrelevantExclusion"}));
        }
    }
}
