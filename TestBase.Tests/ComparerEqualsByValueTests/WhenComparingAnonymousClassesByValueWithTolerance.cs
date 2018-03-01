using NUnit.Framework;

namespace TestBase.Tests.ComparerEqualsByValueTests
{
    [TestFixture]
    public class WhenComparingAnonymousClassesByValueWithTolerance
    {
        [Test]
        public void Should_return_true_when_the_same_within_tolerance()
        {
            var objectL = new {field = new {Id = 1d, Name = "1"}};
            var objectR = new { field = new { Id = 1d + 5e-15d, Name = "1" }};

            objectL.EqualsByValue(objectR).ShouldBeTrue();
        }

        [Test]
        public void Should_return_false_when_not_the_same_within_tolerance()
        {
            var objectL = new { field = new { Id = 1d, Name = "1" } };
            var objectR = new { field = new { Id = 1d + 1.5e-14d, Name = "1" } };
            objectL.EqualsByValue(objectR).ShouldBeFalse();
        }
    }
}