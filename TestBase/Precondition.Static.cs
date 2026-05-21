using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace TestBase
{
    /// <summary>
    /// Static methods for marking test preconditions as inconclusive or failed.
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
        public static void InconclusiveBecause(string message, params object[] args)
        {
            Inconclusive.Because(message, args);
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
        public static T InconclusiveIf<T>(T                         actual,
                                          Expression<Func<T, bool>> predicate,
                                          [CallerArgumentExpression("predicate")]
                                          string                    comment = null)
        {
            if (predicate.Compile()(actual))
            {
                KnownTestRunnerAssertions.ThrowInconclusive(comment);
            }
            return actual;
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
        public static bool InconclusiveIf(bool actual,
                                          [CallerArgumentExpression("actual")]
                                          string comment = null)
        {
            if (actual)
            {
                KnownTestRunnerAssertions.ThrowInconclusive(comment);
            }
            return false;
        }

        /// <summary>
        /// Throws the active test runner's assertion failure exception with the given message.
        /// </summary>
        [DoesNotReturn]
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void Failed(string message, params object[] args)
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
        public static T FailIf<T>(T                         actual,
                                  Expression<Func<T, bool>> predicate,
                                  [CallerArgumentExpression("predicate")]
                                  string                    comment = null)
        {
            var result = new Precondition<T>(actual, predicate, comment);
            if (result) KnownTestRunnerAssertions.Throw(result);
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
        public static bool FailIf(bool actual,
                                  [CallerArgumentExpression("actual")]
                                  string          comment = null)
        {
            if (actual) KnownTestRunnerAssertions.Throw(comment);
            return false;
        }
    }
}
