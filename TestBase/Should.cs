using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FastExpressionCompiler;

namespace TestBase
{
    /// <summary>
    /// fluent-style assertions for when the subject of the assertion is an Action
    /// </summary>
    public static class Should
    {
        /// <summary>
        /// Assert that <code><paramref name="action" />.Compile()()</code> throws, catching the
        /// exception and returning it.
        /// </summary>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="action" />
        /// does not throw.</exception>
        public static Exception Throw(Expression<Action> action,
                                      string comment = null,
                                      params object[] commentArgs) =>
            Throw<Exception>(action.CompileFast(),comment,commentArgs);

        /// <summary>
        /// Asserts that <code><paramref name="predicate" />( <paramref name="actual" /> )</code>
        /// throws a <typeparamref name="TE" />, catching the exception and returning it.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns>The caught exception</returns>
        /// <exception cref="ShouldHaveThrownException">is thrown if <paramref name="predicate" />
        /// does not throw.</exception>
        public static T Throw<T,TE>(T actual,
                                    Expression<Func<T,bool>> predicate,
                                    TE dummyForTypeInference = null,
                                    string comment = null,
                                    params object[] commentArgs) where TE : Exception
        {
            return Throw<T,TE>(actual,predicate,comment,commentArgs);
        }

#if NET6_0_OR_GREATER
        [StackTraceHidden]
#else
        [DebuggerHidden]
#endif
        public static TE Throw<TE>(Action action,string comment = null,params object[] commentArgs)
            where TE : Exception
        {
            try
            {
                action();
            }
            catch (TE ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is TE inner) return inner;
                throw Assert.That(ex,
                                  e => e is TE,
                                  $"Expected to throw a {typeof(TE)} but threw {ex}");
            }

            throw new ShouldHaveThrownException(action.ToString()).ForActiveTestRunner();
        }

        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static T Throw<T,TE>(T actual,
                                    Expression<Func<T,bool>> predicate,
                                    string comment,
                                    object[] commentArgs)
            where TE : Exception
        {
            Assertion<T> a;
            try
            {
                a = new Assertion<T>(actual,predicate,comment,commentArgs);
            }
            catch (TE) { return actual; }
            catch ( Exception ex) when (ex.InnerException is TE)
            {
                return actual;
            }
            catch (Exception ex)
            {
                throw Assert.That(ex,
                                  e => e is TE,
                                  $"Expected to throw a {typeof(TE)} but threw {ex}");
            }

            throw new ShouldHaveThrownException(a.Message).ForActiveTestRunner();
        }

        /// <summary>
        /// Executes <code><paramref name="action" />.Compile()()</code>. If the execution
        /// throws, the thrown exception is wrapped in a <see cref="ShouldNotThrowException" />
        /// and thrown.
        /// </summary>
        /// <returns><paramref name="action" /></returns>
        /// <exception cref="ShouldNotThrowException">is thrown if <paramref name="action" />
        /// throws.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Expression<Action> NotThrow(Expression<Action> action,
                                                  string comment = null,
                                                  params object[] commentArgs)
        {
            try { action.Compile(); }
            catch (Exception ex)
            {
                throw ShouldNotThrowException
                    .For(action,$"Threw {ex} but expected not to throw.",commentArgs)
                    .ForActiveTestRunner();
            }
            return action;
        }

        /// <summary>
        /// Invokes <paramref name="action"/>(), waits for the task, and asserts that it does
        /// not throw.
        /// Blocks the calling thread.
        /// </summary>
        /// <exception cref="ShouldNotThrowException">thrown if <paramref name="action" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void NotThrow(Func<Task> action,
                                    string comment = null,
                                    params object[] commentArgs)
        {
            try { action().GetAwaiter().GetResult(); }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// Waits for <paramref name="task"/> and asserts that it does not throw.
        /// Blocks the calling thread.
        /// </summary>
        /// <exception cref="ShouldNotThrowException">thrown if <paramref name="task" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static void NotThrow(Task task,
                                    string comment = null,
                                    params object[] commentArgs)
        {
            try { task.GetAwaiter().GetResult(); }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// Asserts that <paramref name="action"/> throws when awaited, catching and returning
        /// the exception.
        /// <p>
        /// ⚠️ Consider using <see cref="Throw(Func{Task}, string, object[])"/> instead. ⚠️
        /// If this method is not awaited, it will silently pass regardless of whether the task
        /// throws.
        /// </p>
        /// </summary>
        /// <remarks>
        /// ⚠️ Consider using <see cref="Throw(Func{Task}, string, object[])"/> instead. ⚠️
        /// If this method is not awaited, it will silently pass regardless of whether the
        /// task throws.
        /// </remarks>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">thrown if <paramref name="action" />
        /// does not throw.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task<Exception> ThrowAsync(Func<Task> action,
                                                       string comment = null,
                                                       params object[] commentArgs)
        {
            try { await action(); }
            catch (Exception ex) { return ex; }

            throw new ShouldHaveThrownException(
                comment ?? "Expected action to throw but it did not.").ForActiveTestRunner();
        }

        /// <summary>
        /// Asserts that <paramref name="task"/> throws when awaited, catching and returning
        /// the exception.
        /// <p>
        /// ⚠️ Consider using <see cref="Throw(Task, string, object[])"/> instead. ⚠️
        /// If this method is not awaited, it will silently pass regardless of whether the
        /// task throws.
        /// </p>
        /// </summary>
        /// <remarks>
        /// ⚠️ Consider using <see cref="Throw(Task, string, object[])"/> instead.
        /// If this method is not awaited, it will silently pass regardless of whether the task
        /// throws.
        /// </remarks>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">thrown if <paramref name="task" /> does
        /// not throw.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task<Exception> ThrowAsync(Task task,
                                                       string comment = null,
                                                       params object[] commentArgs)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                return ex;
            }

            throw new ShouldHaveThrownException(
                comment ?? "Expected task to throw but it did not.").ForActiveTestRunner();
        }

        /// <summary>
        /// Asserts that <paramref name="action"/> throws when invoked and awaited, catching and
        /// returning the exception.
        /// Blocks the calling thread until the task completes.
        /// </summary>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">thrown if <paramref name="action" />
        /// does not throw.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Exception Throw(Func<Task> action,
                                      string comment = null,
                                      params object[] commentArgs)
        {
            try { action().GetAwaiter().GetResult(); }
            catch (Exception ex) { return ex; }

            throw new ShouldHaveThrownException(
                comment ?? "Expected action to throw but it did not.").ForActiveTestRunner();
        }

        /// <summary>
        /// Asserts that <paramref name="task"/> throws when awaited, catching and returning the
        /// exception.
        /// Blocks the calling thread until the task completes.
        /// </summary>
        /// <returns>The caught exception.</returns>
        /// <exception cref="ShouldHaveThrownException">thrown if <paramref name="task" /> does
        /// not throw.</exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static Exception Throw(Task task,
                                      string comment = null,
                                      params object[] commentArgs)
        {
            try { task.GetAwaiter().GetResult(); }
            catch (Exception ex) { return ex; }

            throw new ShouldHaveThrownException(
                comment ?? "Expected task to throw but it did not.").ForActiveTestRunner();
        }

        /// <summary>
        /// Awaits <paramref name="action"/>() and asserts that it does not throw.
        /// <p>
        /// ⚠️ Consider using <see cref="NotThrow(Func{Task}, string, object[])"/> instead. ⚠️
        /// If this method is not awaited, it will silently pass regardless of whether the
        /// task throws.
        /// </p>
        /// </summary>
        /// <remarks>
        /// ⚠️ If this method is not awaited, it will silently pass regardless of whether the
        /// task throws. ⚠️
        /// </remarks>
        /// <exception cref="ShouldNotThrowException">thrown if <paramref name="action" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task NotThrowAsync(Func<Task> action,
                                               string comment = null,
                                               params object[] commentArgs)
        {
            try { await action(); }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// Awaits <paramref name="task"/> and asserts that it does not throw.
        /// <p>
        /// ⚠️ Consider using <see cref="NotThrow(Task, string, object[])"/> instead. ⚠️
        /// If this method is not awaited, it will silently pass regardless of whether the task
        /// throws.
        /// </p>
        /// </summary>
        /// <remarks>
        /// ⚠️ If this method is not awaited, it will silently pass regardless of whether the
        /// task throws. ⚠️
        /// </remarks>
        /// <exception cref="ShouldNotThrowException">thrown if <paramref name="task" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task NotThrowAsync(Task task,
                                               string comment = null,
                                               params object[] commentArgs)
        {
            try { await task; }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// Awaits <paramref name="action"/>() and asserts that it does not throw.
        /// Returns the result of the task.
        /// <p>
        /// ⚠️ Consider using <see cref="Throw(Func{Task}, string, object[])"/> instead. ⚠️
        /// If this method is not awaited, it will silently pass regardless of whether the
        /// task throws.
        /// </p>
        /// </summary>
        /// <remarks>
        /// ⚠️ If this method is not awaited, it will silently pass regardless of whether the
        /// task throws. ⚠️
        /// </remarks>
        /// <returns>The result of the task.</returns>
        /// <exception cref="ShouldNotThrowException">thrown if <paramref name="action" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task<T> NotThrowAsync<T>(Func<Task<T>> action,
                                                     string comment = null,
                                                     params object[] commentArgs)
        {
            try { return await action(); }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// Awaits <paramref name="task"/> and asserts that it does not throw.
        /// Returns the result of the task.
        /// <p>
        /// ⚠️ If this method is not awaited, it will silently pass regardless of whether the
        /// task throws. ⚠️
        /// </p>
        /// </summary>
        /// <remarks>
        /// ⚠️ If this method is not awaited, it will silently pass regardless of whether the
        /// task throws. ⚠️
        /// </remarks>
        /// <returns>The result of the task.</returns>
        /// <exception cref="ShouldNotThrowException">thrown if <paramref name="task" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static async Task<T> NotThrowAsync<T>(Task<T> task,
                                                     string comment = null,
                                                     params object[] commentArgs)
        {
            try { return await task; }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// Invokes <paramref name="action"/>(), waits for the task, and asserts that it does not
        /// throw.
        /// Returns the result. Blocks the calling thread.
        /// </summary>
        /// <returns>The result of the task.</returns>
        /// <exception cref="ShouldNotThrowException">
        /// thrown if <paramref name="action" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static T NotThrow<T>(Func<Task<T>> action,
                                    string comment = null,
                                    params object[] commentArgs)
        {
            try { return action().GetAwaiter().GetResult(); }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }

        /// <summary>
        /// Waits for <paramref name="task"/> and asserts that it does not throw.
        /// Returns the result. Blocks the calling thread.
        /// </summary>
        /// <returns>The result of the task.</returns>
        /// <exception cref="ShouldNotThrowException">thrown if <paramref name="task" /> throws.
        /// </exception>
        #if NET6_0_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
        public static T NotThrow<T>(Task<T> task,
                                    string comment = null,
                                    params object[] commentArgs)
        {
            try { return task.GetAwaiter().GetResult(); }
            catch (Exception ex)
            {
                throw new ShouldNotThrowException(
                        (comment ?? $"Expected not to throw but did throw: {ex}").Formatz(
                            commentArgs))
                    .ForActiveTestRunner();
            }
        }
    }
}