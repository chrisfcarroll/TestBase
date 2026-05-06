using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TestBase
{
    /// <summary>
    ///     Static methods for marking test preconditions as inconclusive or failed.
    /// </summary>
    public static class Precondition
    {
        /// <summary>
        ///     Throws the active test runner's Inconclusive or Skip exception with the given message.
        /// </summary>
        [DoesNotReturn]
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void Inconclusive(string message, params object[] args)
        {
            TestBase.Inconclusive.Because(message, args);
        }

        /// <summary>
        ///     If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to false,
        ///     the test is inconclusive (or skipped) instead of failed.
        ///     Returns <paramref name="actual"/> if the precondition holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static T InconclusiveIf<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           commentArgs)
        {
            return TestBase.Inconclusive.IfPreconditionFails(actual, predicate, comment, commentArgs);
        }

        /// <summary>
        ///     If <paramref name="actual"/> evaluates to false, the test is inconclusive (or skipped).
        ///     Returns the <see cref="Precondition{T}"/> if it holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Precondition<BoolWithString> InconclusiveIf(
            BoolWithString  actual,
            string          comment = null,
            params object[] commentArgs)
        {
            return TestBase.Inconclusive.IfPreconditionFails(actual, comment, commentArgs);
        }

        /// <summary>
        ///     Throws the active test runner's assertion failure exception with the given message.
        /// </summary>
        [DoesNotReturn]
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void Fail(string message, params object[] args)
        {
            var formatted = args?.Length > 0 ? string.Format(message, args) : message;
            KnownTestRunnerAssertions.Throw(new Assertion(formatted));
        }

        /// <summary>
        ///     If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to false,
        ///     the test fails with an assertion failure (not inconclusive).
        ///     Returns <paramref name="actual"/> if the precondition holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static T FailIf<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           commentArgs)
        {
            var result = new Precondition<T>(actual, predicate, comment, commentArgs);
            if (result) return actual;
            KnownTestRunnerAssertions.Throw(result);
            return actual;
        }

        /// <summary>
        ///     If <paramref name="actual"/> evaluates to false,
        ///     the test fails with an assertion failure (not inconclusive).
        ///     Returns the <see cref="Precondition{T}"/> if it holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Precondition<BoolWithString> FailIf(
            BoolWithString  actual,
            string          comment = null,
            params object[] commentArgs)
        {
            var result = new Precondition<BoolWithString>(actual, a => a, comment, commentArgs);
            if (result) return result;
            KnownTestRunnerAssertions.Throw(result);
            return result;
        }
    }
}
