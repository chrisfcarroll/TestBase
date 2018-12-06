using NUnit.Framework;

namespace TestBase.Tests.EqualByValueTests
{
    [TestFixture]
    class WhenComparingPrimitivesByValue
    {
        [TestCase(1,      1)]
        [TestCase(1.0d,   1.0d)]
        [TestCase("Left", "Left")]
        public void Should_return_true_when_the_same(object left, object right)
        {
            left.EqualsByValue(right).ShouldBeTrue("Error in Comparer");
        }

        [TestCase(1,      2)]
        [TestCase(1.0d,   1.1d)]
        [TestCase("Left", "Right")]
        public void Should_return_false_when_not_the_same(object left, object right)
        {
            left.EqualsByValue(right).ShouldBeFalse("Error in Comparer");
        }
    }
}
