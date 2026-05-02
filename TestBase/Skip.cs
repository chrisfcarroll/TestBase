using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace TestBase
{
    /// <summary>
    ///     Static methods for skipping tests by throwing the active test runner's
    ///     inconclusive/skip exception (NUnit InconclusiveException, xUnit SkipException,
    ///     MSTest AssertInconclusiveException).
    /// </summary>
    public static class Skip
    {
        /// <summary>
        ///     Throws the active test runner's skip/inconclusive exception with the given message.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void Because(string message, params object[] args)
        {
            var formatted = args?.Length > 0 ? string.Format(message, args) : message;
            KnownTestRunnerAssertions.ThrowInconclusive(formatted);
        }

        /// <summary>
        ///     If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to false,
        ///     the test is skipped (inconclusive) instead of failed.
        ///     Returns <paramref name="actual"/> if the precondition holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static T IfPreconditionFails<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           commentArgs)
        {
            var result = new Precondition<T>(actual, predicate, comment, commentArgs);
            if (result) return actual;
            KnownTestRunnerAssertions.ThrowInconclusive(result.Message);
            return actual;
        }

        /// <summary>
        ///     If <paramref name="predicate"/>(<paramref name="actual"/>) evaluates to false,
        ///     the test is skipped (inconclusive) instead of failed.
        ///     Returns <paramref name="actual"/> if the precondition holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static T If<T>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           commentArgs)
        {
            return IfPreconditionFails(actual, predicate, comment, commentArgs);
        }

        /// <summary>
        ///     If <paramref name="actual"/> evaluates to false, the test is skipped (inconclusive).
        ///     Returns the <see cref="Precondition{T}"/> if it holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Precondition<BoolWithString> IfPreconditionFails(
            BoolWithString  actual,
            string          comment = null,
            params object[] commentArgs)
        {
            var result = new Precondition<BoolWithString>(actual, a => a, comment, commentArgs);
            if (result) return result;
            KnownTestRunnerAssertions.ThrowInconclusive(result.Message);
            return result;
        }

        /// <summary>
        ///     If <paramref name="actual"/> evaluates to false, the test is skipped (inconclusive).
        ///     Returns the <see cref="Precondition{T}"/> if it holds.
        /// </summary>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Precondition<BoolWithString> If(
            BoolWithString  actual,
            string          comment = null,
            params object[] commentArgs)
        {
            return IfPreconditionFails(actual, comment, commentArgs);
        }
    }
}
