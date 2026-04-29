using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestBase
{
    public static class NumericShouldsWithTolerance
    {
        static void ThrowToleranceAssertion<T>(T actual, string assertionName, string assertedDetail, string message, object[] args,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
            throw Assertion.CreateFrameworkException(new Assertion<T>(
                actual?.ToString() ?? "null",
                actualExpression,
                assertionName,
                assertedDetail,
                comment,
                false));
        }

        /// <summary>Accepts 2 <see cref="double" />s as equal if they are within <paramref name="tolerance" /> of each other.</summary>
        public static T ShouldEqualWithTolerance<T>(
            this T @this,
            double expectedValue,
            [Optional] [DefaultParameterValue(1e-14d)]
            double tolerance,
            string          message = null,
            params object[] args)
        {
            if (tolerance     < 0d) tolerance = -tolerance;
            if (expectedValue < 0d) tolerance = -tolerance;

            var absTolerance = Math.Abs(tolerance);
            var lo = expectedValue - absTolerance;
            var hi = expectedValue + absTolerance;
            var actualDouble = Convert.ToDouble(@this);
            if (actualDouble >= lo && actualDouble <= hi) return @this;

            var diff = Math.Abs(actualDouble - expectedValue);
            var detail = $"Expected: {expectedValue} +/- {absTolerance}, Actual: {@this}" +
                         Environment.NewLine + $"Off by:   {diff}";
            ThrowToleranceAssertion(@this, nameof(ShouldEqualWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Accepts 2 <see cref="double" />s as not equal only if they differ by at least <paramref name="minimumDifference" />.</summary>
        public static T ShouldNotEqualWithMargin<T>(
            this T          @this,
            double          expectedValue,
            double          minimumDifference = 1e-14d,
            string          message           = null,
            params object[] args)
        {
            var absDiff = Math.Abs(minimumDifference);
            var lo = expectedValue - absDiff;
            var hi = expectedValue + absDiff;
            var actualDouble = Convert.ToDouble(@this);
            if (actualDouble < lo || actualDouble > hi) return @this;

            var diff = Math.Abs(actualDouble - expectedValue);
            var detail = $"Expected: not within {absDiff} of {expectedValue}, Actual: {@this}" +
                         Environment.NewLine + $"Diff:     {diff}";
            ThrowToleranceAssertion(@this, nameof(ShouldNotEqualWithMargin), detail, message, args);
            return @this;
        }

        /// <summary>
        ///     Assert that <paramref name="this" /> is between <paramref name="left" />-<paramref name="tolerance" /> and
        ///     <paramref name="right" />+<paramref name="tolerance" />
        /// </summary>
        public static T ShouldBeBetweenWithTolerance<T>(
            this T          @this,
            double          left,
            double          right,
            double          tolerance = 1e-14d,
            string          message   = null,
            params object[] args)
        where T : IComparable<T>
        {
            var actualDouble = Convert.ToDouble(@this);
            var lo = left - Math.Abs(tolerance);
            var hi = right + Math.Abs(tolerance);
            if (actualDouble >= lo && actualDouble <= hi) return @this;

            var detail = $"Expected: between {left} and {right} +/- {Math.Abs(tolerance)}, Actual: {@this}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeBetweenWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>
        ///     Assert that <paramref name="this" /> is greater than <paramref name="expectedValue" />-
        ///     <paramref name="tolerance" />
        /// </summary>
        public static T ShouldBeGreaterThanWithTolerance<T>(
            this T          @this,
            double          expectedValue,
            double          tolerance = 1e-14d,
            string          message   = null,
            params object[] args)
        {
            var actualDouble = Convert.ToDouble(@this);
            var threshold = expectedValue - Math.Abs(tolerance);
            if (actualDouble > threshold) return @this;

            var detail = $"Expected: > {expectedValue} (tolerance {Math.Abs(tolerance)}), Actual: {@this}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeGreaterThanWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>
        ///     Assert that <paramref name="this" /> is greater than or equal to <paramref name="expectedValue" />-
        ///     <paramref name="tolerance" />
        /// </summary>
        public static T ShouldBeGreaterThanOrEqualToWithTolerance<T>(
            this T          @this,
            double          expectedValue,
            double          tolerance = 1e-14d,
            string          message   = null,
            params object[] args)
        {
            var actualDouble = Convert.ToDouble(@this);
            var threshold = expectedValue - Math.Abs(tolerance);
            if (actualDouble >= threshold) return @this;

            var detail = $"Expected: >= {expectedValue} (tolerance {Math.Abs(tolerance)}), Actual: {@this}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeGreaterThanOrEqualToWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>
        ///     Assert that <paramref name="this" /> is less than <paramref name="expectedValue" />+
        ///     <paramref name="tolerance" />
        /// </summary>
        public static T ShouldBeLessThanWithTolerance<T>(
            this T          @this,
            double          expectedValue,
            double          tolerance = 1e-14d,
            string          message   = null,
            params object[] args)
        {
            var actualDouble = Convert.ToDouble(@this);
            var threshold = expectedValue + Math.Abs(tolerance);
            if (actualDouble < threshold) return @this;

            var detail = $"Expected: < {expectedValue} (tolerance {Math.Abs(tolerance)}), Actual: {@this}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeLessThanWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>
        ///     Assert that <paramref name="this" /> is less than or equal to <paramref name="expectedValue" />+
        ///     <paramref name="tolerance" />
        /// </summary>
        public static T ShouldBeLessThanOrEqualToWithTolerance<T>(
            this T          @this,
            double          expectedValue,
            double          tolerance = 1e-14d,
            string          message   = null,
            params object[] args)
        {
            var actualDouble = Convert.ToDouble(@this);
            var threshold = expectedValue + Math.Abs(tolerance);
            if (actualDouble <= threshold) return @this;

            var detail = $"Expected: <= {expectedValue} (tolerance {Math.Abs(tolerance)}), Actual: {@this}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeLessThanOrEqualToWithTolerance), detail, message, args);
            return @this;
        }
    }
}
