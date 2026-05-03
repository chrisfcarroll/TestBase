using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FastExpressionCompiler;

namespace TestBase
{
    /// <summary>fluent-style assertions for when the subject of the assertion is an Action</summary>
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
        ///     Asserts that <code><paramref name="predicate" />( <paramref name="actual" /> )</code> throws a
        ///     <typeparamref name="TE" />, catching the exception and returning it.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="predicate" /> does not throw.</exception>
        public static T Throw<T, TE>(T                         actual,
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

        /// <summary>
        /// <p>⚠️ Unless this method is awaited, it cannot fail. ⚠️ </p>
        /// Assert that <paramref name="action" />() throws, catching the exception and returning it.
        /// </summary>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">
        /// is thrown if <paramref name="action" /> does not throw.
        /// </exception>
        /// <exception cref="Assertion">thrown if the wrong Exception is thrown.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task<TE> ThrowAsync<TE>(Task<Action> action,
                                               string comment = null,
                                               params object[] commentArgs) where TE : Exception
        {
            try { (await action)(); } catch (TE ex) { return ex; } catch (Exception ex)
            {
                if (ex.InnerException is TE inner) return inner;
                throw Assert.That(ex, e => e is TE, $"Expected to throw a {typeof(TE)} but threw {ex}");
            }

            throw new ShouldHaveThrownException(action.ToString()).ForActiveTestRunner();
        }

        /// <summary>
        /// <p>⚠️ Unless this method is awaited, it cannot fail. ⚠️ </p>
        ///
        /// Assert that evaluating <paramref name="predicate" />( <paramref name="actual" /> )
        /// throws an Exception of type <typeparamref name="TE"/>.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="predicate"></param>
        /// <param name="comment"></param>
        /// <param name="commentArgs"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <returns>
        /// <paramref name="actual"/>, if the expected assertion is thrown
        /// during evaluation. Otherwise throws.
        /// </returns>
        /// <exception cref="Assertion">thrown if the wrong Exception is thrown.</exception>
        /// <exception cref="ShouldHaveThrownException">thrown if no assertion is thrown</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task<T> ThrowAsync<T,TE>(T actual,
                                                Task<Expression<Func<T,bool>>> predicate,
                                                string comment,
                                                object[] commentArgs) where TE : Exception
        {
            Assertion<T> a = new Assertion<T>(actual, (await predicate), comment, commentArgs);
            if (a.ExceptionThrownByEvaluation is TE) return actual;
            if (a.ExceptionThrownByEvaluation != null)
                throw Assert.That(a.ExceptionThrownByEvaluation,
                                  e => e is TE,
                                  $"Expected to throw a {typeof(TE)} but threw {a.ExceptionThrownByEvaluation}");

            throw new ShouldHaveThrownException(a.Message).ForActiveTestRunner();
        }

        /// <summary>
        /// <p>⚠️ Unless this method is awaited, it cannot fail. ⚠️ </p>
        /// Awaits <paramref name="task" />().
        /// If <paramref name="task"/> throws, the thrown exception is wrapped in a
        /// <see cref="ShouldNotThrowException" /> and thrown.
        /// </summary>
        /// <returns>A completed Task.</returns>
        /// <exception cref="ShouldNotThrowException">is thrown if <paramref name="task" /> throws.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task NotThrowAsync(Func<Task> task,
                                               string comment = null,
                                               params object[] commentArgs)
        {
            try { await task(); }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment?? $"Expected not to throw but did throw: {ex}").Formatz(commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// <p>⚠️ Unless this method is awaited, it cannot fail. ⚠️ </p>
        /// Awaits <paramref name="task"/>. If the task throws, the thrown exception is wrapped
        /// in a <see cref="ShouldNotThrowException" /> and thrown.
        /// </summary>
        /// <returns>A completed task.</returns>
        /// <exception cref="ShouldNotThrowException">is thrown if <paramref name="task" /> throws.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task NotThrowAsync(Task task, string comment = null, params object[] commentArgs)
        {
            try { await task; }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment?? $"Expected not to throw but did throw: {ex}").Formatz(commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// <p>⚠️ Unless this method is awaited, it cannot fail. ⚠️ </p>
        ///     Awaits <paramref name="task"/>. If the task throws,
        ///     the thrown exception is wrapped in a <see cref="ShouldNotThrowException" /> and thrown.
        /// </summary>
        /// <returns>
        /// The result of the task.
        /// </returns>
        /// <exception cref="ShouldNotThrowException">is thrown if <paramref name="task" /> throws.</exception>
        public static async Task<T> NotThrowAsync<T>(Task<T> task, string comment = null, params object[] commentArgs)
        {
            try { return await task; }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment?? $"Expected not to throw but did throw: {ex}").Formatz(commentArgs))
                    .ForActiveTestRunner();
            }
        }
    }
}
