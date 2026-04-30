using System;
using System.Diagnostics;
using System.Linq.Expressions;
using FastExpressionCompiler;

namespace TestBase
{
    /// <summary>Static convenience methods for fluent-style assertions.</summary>
    public static class Should
    {
        /// <summary>
        ///     Assert that <code><paramref name="action" />.Compile()()</code> throws, catching the exception and returning it.
        /// </summary>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="action" /> does not throw.</exception>
        public static Exception Throw(Expression<Action> action, string comment = null, params object[] commentArgs)
        {
            return Throw<Exception>(action.CompileFast(), comment, commentArgs);
        }

        /// <summary>
        ///     Asserts that <code><paramref name="action" />()</code> throws a <typeparamref name="TE" />, catching the exception
        ///     and returning it.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="action" /> does not throw.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static TE Throw1<TE>(Action action, string comment = null, params object[] commentArgs)
        where TE : Exception
        {
            return Throw<TE>(action, comment, commentArgs);
        }

        /// <summary>
        ///     Asserts that <code><paramref name="predicate" />( <paramref name="actual" /> )</code> throws a
        ///     <typeparamref name="TE" />, catching the exception and returning it.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="predicate" /> does not throw.</exception>
        public static T Throw<T, TE>(
            T                         actual,
            Expression<Func<T, bool>> predicate,
            TE                        dummyForTypeInference = null,
            string                    comment               = null,
            params object[]           commentArgs) where TE : Exception
        {
            return Throw<T, TE>(actual, predicate, comment, commentArgs);
        }

        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static TE Throw<TE>(Action action, string comment = null, params object[] commentArgs)
            where TE : Exception
        {
            try { action(); } catch (TE ex) { return ex; } catch (Exception ex)
            {
                if (ex.InnerException is TE inner) return inner;
                throw Assert.That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}");
            }

            throw new ShouldHaveThrownException(action.ToString()).ForActiveTestRunner();
        }

        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else

        [DebuggerHidden]
        #endif
        public static T Throw<T, TE>(T actual, Expression<Func<T, bool>> predicate, string comment, object[] commentArgs)
            where TE : Exception
        {
            Assertion<T> a;
            try { a = new Assertion<T>(actual, predicate, comment, commentArgs); } catch (TE) { return actual; } catch (
                Exception ex) when (ex.InnerException is TE) { return actual; } catch (
                Exception ex) { throw Assert.That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}"); }

            throw new ShouldHaveThrownException(a.Message).ForActiveTestRunner();
        }

        /// <summary>
        ///     Executes <code><paramref name="action" />.Compile()()</code>. If the execution throws,
        ///     the thrown exception is wrapped in a <see cref="ShouldNotThrowException" /> and thrown.
        /// </summary>
        /// <returns><paramref name="action" /></returns>
        /// <exception cref="ShouldNotThrowException">is thrown if <paramref name="action" /> throws.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Expression<Action> NotThrow(
            Expression<Action> action,
            string             comment = null,
            params object[]    commentArgs)
        {
            try { action.Compile(); } catch (Exception ex)
            {
                throw ShouldNotThrowException.For(action, $"Threw {ex} but expected not to throw.", commentArgs).ForActiveTestRunner();
            }

            return action;
        }
    }
}
