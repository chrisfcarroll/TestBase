using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests
{
    [TestFixture]
    class WhenComparingNumbersWithTolerance
    {
        [TestCase(1d,      1e-15d)]
        [TestCase(1e308d,  1e294d)]
        [TestCase(5e-324d, 5e-324d)]
        public void ShouldEqualWithTolerance__Should_Pass_when_the_same_within_tolerance(double left, double tolerance)
        {
            var right1 = left + tolerance * 0.999999999999d;
            var right2 = left - tolerance * 0.999999999999d;
            left.ShouldEqualWithTolerance(right1, tolerance);
            left.ShouldEqualWithTolerance(right2, tolerance);
        }

        [TestCase(1d,      1e-13d)]
        [TestCase(1e304d,  1e296d)]
        [TestCase(5e-318d, 5e-321d)]
        public void ShouldEqualWithTolerance__Should_Fail_when_outside_tolerance(double left, double tolerance)
        {
            var right1 = left + tolerance * 1.001d;
            var right2 = left - tolerance * 1.001d;
            Assert.Throws<Assertion>(() => { left.ShouldEqualWithTolerance(right1, tolerance); });
            Assert.Throws<Assertion>(() => { left.ShouldEqualWithTolerance(right2, tolerance); });
        }

        [TestCase(1d,      1e-13d)]
        [TestCase(1e304d,  1e296d)]
        [TestCase(5e-318d, 5e-321d)]
        public void ShouldNotEqualWithMargin__Should_pass_when_different_by_margin(double left, double tolerance)
        {
            var right1 = left + tolerance * 1.001d;
            var right2 = left - tolerance * 1.001d;
            left.ShouldNotEqualWithMargin(right1, tolerance);
            left.ShouldNotEqualWithMargin(right2, tolerance);
        }

        [TestCase(1d,      1e-14d)]
        [TestCase(1e305d,  1e296d)]
        [TestCase(5e-322d, 5e-324d)]
        public void ShouldNotEqualWithMargin__Should_fail_when_different_by_less_than_margin(
            double left,
            double tolerance)
        {
            var right1 = left + tolerance * 0.999999999999d;
            var right2 = left - tolerance * 0.999999999999d;
            Assert.Throws<Assertion>(() => { left.ShouldNotEqualWithMargin(right1, tolerance); });
            Assert.Throws<Assertion>(() => { left.ShouldNotEqualWithMargin(right2, tolerance); });
        }

        [TestCase(1d,      1e-14d)]
        [TestCase(1e308d,  1e294d)]
        [TestCase(5e-324d, 5e-324d)]
        public void ShouldBeGreaterThanOrEqualWithTolerance__Should_Pass_when_within_tolerance(
            double actual,
            double tolerance)
        {
            var right = actual - tolerance * 0.99999999999d;
            var left  = actual + tolerance * 0.999999999999d;
            (1 + actual).ShouldBeGreaterThanOrEqualToWithTolerance(actual);
            right.ShouldBeGreaterThanOrEqualToWithTolerance(actual, tolerance);
            actual.ShouldBeGreaterThanOrEqualToWithTolerance(left, tolerance);
        }

        [TestCase(1d,      1e-13d)]
        [TestCase(1e304d,  1e294d)]
        [TestCase(5e-318d, 5e-321d)]
        public void ShouldBeGreaterThanOrEqualWithTolerance__Should_Fail_when_outside_tolerance(
            double actual,
            double tolerance)
        {
            var right = actual - tolerance * 1.001d;
            var left  = actual + tolerance * 1.001d;

            Assert.Throws<Assertion>(() => right.ShouldBeGreaterThanOrEqualToWithTolerance(actual, tolerance));
            Assert.Throws<Assertion>(() => actual.ShouldBeGreaterThanOrEqualToWithTolerance(left, tolerance));
        }

        [TestCase(1d,      1e-14d)]
        [TestCase(1e308d,  1e294d)]
        [TestCase(5e-324d, 5e-324d)]
        public void ShouldBeLessThanOrEqualWithTolerance__Should_Pass_when_within_tolerance(
            double actual,
            double tolerance)
        {
            var right = actual - tolerance * 0.99999999999d;
            var left  = actual + tolerance * 0.999999999999d;
            (actual - 1).ShouldBeLessThanOrEqualToWithTolerance(actual);
            actual.ShouldBeLessThanOrEqualToWithTolerance(right, tolerance);
            left.ShouldBeLessThanOrEqualToWithTolerance(actual, tolerance);
        }

        [TestCase(1d,      1e-13d)]
        [TestCase(1e304d,  1e294d)]
        [TestCase(5e-318d, 5e-321d)]
        public void ShouldBeLessThanOrEqualWithTolerance__Should_Fail_when_outside_tolerance(
            double actual,
            double tolerance)
        {
            var right = actual - tolerance * 1.001d;
            var left  = actual + tolerance * 1.001d;

            Assert.Throws<Assertion>(() => actual.ShouldBeLessThanOrEqualToWithTolerance(right, tolerance));
            Assert.Throws<Assertion>(() => left.ShouldBeLessThanOrEqualToWithTolerance(actual, tolerance));
        }
    }
}
