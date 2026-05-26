using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestBase
{
    public static class DateTimeShouldsWithTolerance
    {
        const long DefaultToleranceNanoseconds = 1_000_000; // 1 millisecond

        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        static void ThrowToleranceAssertion<T>(T actual, string assertionName, string assertedDetail, string message, object[] args,
                                               [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
            throw new Assertion<T>(
                actual?.ToString() ?? "null",
                actualExpression,
                assertionName,
                assertedDetail,
                comment,
                false).ForActiveTestRunner();
        }

        static TimeSpan ToTimeSpan(long nanoseconds) => new TimeSpan(Math.Abs(nanoseconds) / 100);

        // ──────────────────────────────────────────────────────────────
        //  DateTime
        // ──────────────────────────────────────────────────────────────

        /// <summary>Accepts 2 <see cref="DateTime"/>s as equal if they are within <paramref name="toleranceNanoseconds"/> of each other.</summary>
        public static DateTime ShouldEqualWithTolerance(
            this DateTime   @this,
            DateTime        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tol = ToTimeSpan(toleranceNanoseconds);
            var diff = (@this - expectedValue).Duration();
            if (diff <= tol) return @this;

            var detail = $"Expected: {expectedValue:O} +/- {tol}, Actual: {@this:O}" +
                         Environment.NewLine + $"Off by:   {diff}";
            ThrowToleranceAssertion(@this, nameof(ShouldEqualWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Accepts 2 <see cref="DateTime"/>s as not equal only if they differ by at least <paramref name="minimumDifferenceNanoseconds"/>.</summary>
        public static DateTime ShouldNotEqualWithMargin(
            this DateTime   @this,
            DateTime        expectedValue,
            long            minimumDifferenceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var margin = ToTimeSpan(minimumDifferenceNanoseconds);
            var diff = (@this - expectedValue).Duration();
            if (diff > margin) return @this;

            var detail = $"Expected: not within {margin} of {expectedValue:O}, Actual: {@this:O}" +
                         Environment.NewLine + $"Diff:     {diff}";
            ThrowToleranceAssertion(@this, nameof(ShouldNotEqualWithMargin), detail, message, args);
            return @this;
        }

        /// <summary>
        ///     Assert that <paramref name="this"/> is between <paramref name="left"/>-<paramref name="toleranceNanoseconds"/> and
        ///     <paramref name="right"/>+<paramref name="toleranceNanoseconds"/>
        /// </summary>
        public static DateTime ShouldBeBetweenWithTolerance(
            this DateTime   @this,
            DateTime        left,
            DateTime        right,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tol = ToTimeSpan(toleranceNanoseconds);
            var lo = left - tol;
            var hi = right + tol;
            if (@this >= lo && @this <= hi) return @this;

            var detail = $"Expected: between {left:O} and {right:O} +/- {tol}, Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeBetweenWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is greater than <paramref name="expectedValue"/>-<paramref name="toleranceNanoseconds"/></summary>
        public static DateTime ShouldBeGreaterThanWithTolerance(
            this DateTime   @this,
            DateTime        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tol = ToTimeSpan(toleranceNanoseconds);
            var threshold = expectedValue - tol;
            if (@this > threshold) return @this;

            var detail = $"Expected: > {expectedValue:O} (tolerance {tol}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeGreaterThanWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is greater than or equal to <paramref name="expectedValue"/>-<paramref name="toleranceNanoseconds"/></summary>
        public static DateTime ShouldBeGreaterThanOrEqualToWithTolerance(
            this DateTime   @this,
            DateTime        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tol = ToTimeSpan(toleranceNanoseconds);
            var threshold = expectedValue - tol;
            if (@this >= threshold) return @this;

            var detail = $"Expected: >= {expectedValue:O} (tolerance {tol}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeGreaterThanOrEqualToWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is less than <paramref name="expectedValue"/>+<paramref name="toleranceNanoseconds"/></summary>
        public static DateTime ShouldBeLessThanWithTolerance(
            this DateTime   @this,
            DateTime        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tol = ToTimeSpan(toleranceNanoseconds);
            var threshold = expectedValue + tol;
            if (@this < threshold) return @this;

            var detail = $"Expected: < {expectedValue:O} (tolerance {tol}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeLessThanWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is less than or equal to <paramref name="expectedValue"/>+<paramref name="toleranceNanoseconds"/></summary>
        public static DateTime ShouldBeLessThanOrEqualToWithTolerance(
            this DateTime   @this,
            DateTime        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tol = ToTimeSpan(toleranceNanoseconds);
            var threshold = expectedValue + tol;
            if (@this <= threshold) return @this;

            var detail = $"Expected: <= {expectedValue:O} (tolerance {tol}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeLessThanOrEqualToWithTolerance), detail, message, args);
            return @this;
        }

        // ──────────────────────────────────────────────────────────────
        //  TimeOnly  (net6.0+)
        // ──────────────────────────────────────────────────────────────
#if NET6_0_OR_GREATER

        /// <summary>Accepts 2 <see cref="TimeOnly"/>s as equal if they are within <paramref name="toleranceNanoseconds"/> of each other.</summary>
        public static TimeOnly ShouldEqualWithTolerance(
            this TimeOnly   @this,
            TimeOnly        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tolTicks = Math.Abs(toleranceNanoseconds) / 100;
            var diff = Math.Abs(@this.Ticks - expectedValue.Ticks);
            if (diff <= tolTicks) return @this;

            var detail = $"Expected: {expectedValue:O} +/- {TimeSpan.FromTicks(tolTicks)}, Actual: {@this:O}" +
                         Environment.NewLine + $"Off by:   {TimeSpan.FromTicks(diff)}";
            ThrowToleranceAssertion(@this, nameof(ShouldEqualWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Accepts 2 <see cref="TimeOnly"/>s as not equal only if they differ by at least <paramref name="minimumDifferenceNanoseconds"/>.</summary>
        public static TimeOnly ShouldNotEqualWithMargin(
            this TimeOnly   @this,
            TimeOnly        expectedValue,
            long            minimumDifferenceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var marginTicks = Math.Abs(minimumDifferenceNanoseconds) / 100;
            var diff = Math.Abs(@this.Ticks - expectedValue.Ticks);
            if (diff > marginTicks) return @this;

            var detail = $"Expected: not within {TimeSpan.FromTicks(marginTicks)} of {expectedValue:O}, Actual: {@this:O}" +
                         Environment.NewLine + $"Diff:     {TimeSpan.FromTicks(diff)}";
            ThrowToleranceAssertion(@this, nameof(ShouldNotEqualWithMargin), detail, message, args);
            return @this;
        }

        /// <summary>
        ///     Assert that <paramref name="this"/> is between <paramref name="left"/>-<paramref name="toleranceNanoseconds"/> and
        ///     <paramref name="right"/>+<paramref name="toleranceNanoseconds"/>
        /// </summary>
        public static TimeOnly ShouldBeBetweenWithTolerance(
            this TimeOnly   @this,
            TimeOnly        left,
            TimeOnly        right,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tolTicks = Math.Abs(toleranceNanoseconds) / 100;
            var loTicks = Math.Max(0, left.Ticks - tolTicks);
            var hiTicks = Math.Min(TimeOnly.MaxValue.Ticks, right.Ticks + tolTicks);
            if (@this.Ticks >= loTicks && @this.Ticks <= hiTicks) return @this;

            var detail = $"Expected: between {left:O} and {right:O} +/- {TimeSpan.FromTicks(tolTicks)}, Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeBetweenWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is greater than <paramref name="expectedValue"/>-<paramref name="toleranceNanoseconds"/></summary>
        public static TimeOnly ShouldBeGreaterThanWithTolerance(
            this TimeOnly   @this,
            TimeOnly        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tolTicks = Math.Abs(toleranceNanoseconds) / 100;
            var thresholdTicks = Math.Max(0, expectedValue.Ticks - tolTicks);
            if (@this.Ticks > thresholdTicks) return @this;

            var detail = $"Expected: > {expectedValue:O} (tolerance {TimeSpan.FromTicks(tolTicks)}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeGreaterThanWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is greater than or equal to <paramref name="expectedValue"/>-<paramref name="toleranceNanoseconds"/></summary>
        public static TimeOnly ShouldBeGreaterThanOrEqualToWithTolerance(
            this TimeOnly   @this,
            TimeOnly        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tolTicks = Math.Abs(toleranceNanoseconds) / 100;
            var thresholdTicks = Math.Max(0, expectedValue.Ticks - tolTicks);
            if (@this.Ticks >= thresholdTicks) return @this;

            var detail = $"Expected: >= {expectedValue:O} (tolerance {TimeSpan.FromTicks(tolTicks)}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeGreaterThanOrEqualToWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is less than <paramref name="expectedValue"/>+<paramref name="toleranceNanoseconds"/></summary>
        public static TimeOnly ShouldBeLessThanWithTolerance(
            this TimeOnly   @this,
            TimeOnly        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tolTicks = Math.Abs(toleranceNanoseconds) / 100;
            var thresholdTicks = Math.Min(TimeOnly.MaxValue.Ticks, expectedValue.Ticks + tolTicks);
            if (@this.Ticks < thresholdTicks) return @this;

            var detail = $"Expected: < {expectedValue:O} (tolerance {TimeSpan.FromTicks(tolTicks)}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeLessThanWithTolerance), detail, message, args);
            return @this;
        }

        /// <summary>Assert that <paramref name="this"/> is less than or equal to <paramref name="expectedValue"/>+<paramref name="toleranceNanoseconds"/></summary>
        public static TimeOnly ShouldBeLessThanOrEqualToWithTolerance(
            this TimeOnly   @this,
            TimeOnly        expectedValue,
            long            toleranceNanoseconds = DefaultToleranceNanoseconds,
            string          message = null,
            params object[] args)
        {
            var tolTicks = Math.Abs(toleranceNanoseconds) / 100;
            var thresholdTicks = Math.Min(TimeOnly.MaxValue.Ticks, expectedValue.Ticks + tolTicks);
            if (@this.Ticks <= thresholdTicks) return @this;

            var detail = $"Expected: <= {expectedValue:O} (tolerance {TimeSpan.FromTicks(tolTicks)}), Actual: {@this:O}";
            ThrowToleranceAssertion(@this, nameof(ShouldBeLessThanOrEqualToWithTolerance), detail, message, args);
            return @this;
        }
#endif
    }
}
