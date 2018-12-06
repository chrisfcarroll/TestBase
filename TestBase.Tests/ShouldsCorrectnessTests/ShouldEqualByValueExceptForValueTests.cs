using System.Linq;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests
{
    [TestFixture]
    public class ShouldEqualByValueExceptForValueTests
    {
        [TestCase(3, 3, "1", "3", "2", "1", "2", "3", "4")]
        [TestCase(3, 3, "2", "1", "4", "1", "5", "2", "4", "5")]
        public void Given_lists_that_are_the_same_after_ordering_except_for_exceptions_should_pass(
            params object[] testcase)
        {
            var leftCount  = (int) testcase[0];
            var rightCount = (int) testcase[1];
            var actual     = testcase.Skip(2).Take(leftCount);
            var expected   = testcase.Skip(2 + leftCount).Take(rightCount);
            var exceptions = testcase.Skip(2 + leftCount + rightCount);

            actual.ShouldEqualByValueExceptForValuesIgnoringOrder(expected, exceptions);
        }

        [TestCase(3, 3, "1", "2", "3", "1", "2", "3", "4")]
        [TestCase(3, 3, "1", "2", "4", "1", "5", "2", "4", "5")]
        public void Given_lists_that_are_the_same_except_for_exceptions_should_pass(params object[] testcase)
        {
            var leftCount  = (int) testcase[0];
            var rightCount = (int) testcase[1];
            var actual     = testcase.Skip(2).Take(leftCount);
            var expected   = testcase.Skip(2 + leftCount).Take(rightCount);
            var exceptions = testcase.Skip(2 + leftCount + rightCount);

            actual.ShouldEqualByValueExceptForValues(expected, exceptions);
        }

        [TestCase(3, 3, "1", "2", "3", "2", "2", "3", "4")]
        [TestCase(3, 3, "1", "2", "4", "1", "4", "5", "4", "5")]
        public void Given_lists_that_are_not_the_same_even_with_exceptions_should_fail(params object[] testcase)
        {
            var leftCount  = (int) testcase[0];
            var rightCount = (int) testcase[1];
            var actual     = testcase.Skip(2).Take(leftCount);
            var expected   = testcase.Skip(2 + leftCount).Take(rightCount);
            var exceptions = testcase.Skip(2 + leftCount + rightCount);

            Assert.Throws<Assertion>(
                                     () => actual.ShouldEqualByValueExceptForValues(expected, exceptions)
                                    );
        }
    }
}
