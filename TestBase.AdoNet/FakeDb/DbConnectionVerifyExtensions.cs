using System;
using System.Linq;
using System.Linq.Expressions;
#if NETSTANDARD
using FastExpressionCompiler;
#endif

namespace TestBase.AdoNet
{
    public static class DbConnectionVerifyExtensions
    {
        /// <summary>
        /// Verifies that the <see cref="FakeDbConnection"/> was asked to execute a command matching the given predicate
        /// </summary>
        /// <param name="this">The FakeDbConnection</param>
        /// <param name="commandInvocationPredicate">A predicate, which may examine the command (and its parameters) sent</param>
        /// <param name="expectedInvocationsCount">Optional, defaults to 1</param>
        /// <param name="exactly">Should the <see cref="expectedInvocationsCount"/>be treated as requiring an exact match or an 'at least' match?</param>
        /// <param name="message">Optional custom failure message</param>
        /// <param name="args">Optional customer failure message format arguments</param>
        /// <returns></returns>
        public static FakeDbConnection Verify(this FakeDbConnection @this,
                                              Func<FakeDbCommand, bool> commandInvocationPredicate,
                                              int expectedInvocationsCount = 1,
                                              bool exactly = false,
                                              string message = null,
                                              params object[] args)
        {
            if (exactly)
            {
                @this.Invocations
                     .Where(commandInvocationPredicate)
                     .Count()
                     .ShouldBe(expectedInvocationsCount,
                               string.Format(message ?? "Expected to be called exactly {0} times", args.Length == 0 ? new object[] {expectedInvocationsCount} : args)
                               );
            }
            else
            {
                @this.Invocations
                     .Where(commandInvocationPredicate)
                     .Count()
                     .ShouldBeGreaterThanOrEqualTo(expectedInvocationsCount,
                                                   string.Format(message ?? "Expected to be called at least {0} times",args.Length == 0 ? new object[] {expectedInvocationsCount} : args)
                                                   );
            }
            return @this;
        }
       /// <summary>
        /// Verifies that the <see cref="FakeDbConnection"/> was asked to execute a command matching the given predicate,
        /// and returns the first such command.
        /// </summary>
       /// <param name="this">The FakeDbConnection</param>
        /// <param name="commandInvocationPredicate">A predicate, which may examine the command (and its parameters) sent</param>
        /// <param name="message">Optional custom failure message</param>
        /// <param name="args">Optional customer failure message format arguments</param>
        /// <returns>The first executed command which satisfies <paramref name="commandInvocationPredicate"/></returns>
        /// <exception cref="Assertion">thrown if no matching command is found</exception>
        public static FakeDbCommand VerifyFirst(this FakeDbConnection @this,
                                              Expression<Func<FakeDbCommand, bool>> commandInvocationPredicate,
                                              string message = null,
                                              params object[] args)
        {
            return @this.Invocations
                .Where(commandInvocationPredicate.CompileFast())
                .FirstOrDefault()
                .ShouldNotBeNull(
                    message ?? $"Expected at least once command matching {commandInvocationPredicate.ToCodeString()}", 
                    args);
        }

        /// <summary>
        /// Verifies that the <see cref="FakeDbConnection"/> was asked to execute a command matching the given predicate,
        /// and returns the last such command.
        /// </summary>
        /// <param name="this">The FakeDbConnection</param>
        /// <param name="commandInvocationPredicate">A predicate, which may examine the command (and its parameters) sent</param>
        /// <param name="message">Optional custom failure message</param>
        /// <param name="args">Optional customer failure message format arguments</param>
        /// <returns>That lastmost executed command which satisfies <paramref name="commandInvocationPredicate"/></returns>
        /// <exception cref="Assertion">thrown if no matching command is found</exception>
        public static FakeDbCommand VerifyLast(this FakeDbConnection @this,
            Expression<Func<FakeDbCommand, bool>> commandInvocationPredicate,
            string message = null,
            params object[] args)
        {
            return @this.Invocations
                .Where(commandInvocationPredicate.CompileFast())
                .LastOrDefault()
                .ShouldNotBeNull(
                    message ?? $"Expected at least once command matching {commandInvocationPredicate.ToCodeString()}", 
                    args);
        }

        /// <summary>Verifies that the <see cref="FakeDbConnection"/> was asked exactly once to execute a command matching the given
        /// predicate and returns that single invocation. </summary>
        /// <param name="this">The FakeDbConnection</param>
        /// <param name="commandInvocationPredicate">A predicate, which may examine the command (and its parameters) sent</param>
        /// <param name="message">Optional custom failure message</param>
        /// <param name="args">Optional customer failure message format arguments</param>
        /// <returns>The single executed command which satisfies <paramref name="commandInvocationPredicate"/></returns>
        /// <exception cref="Assertion">thrown if no matching command is found, or if more than one matching command is found</exception>
        public static FakeDbCommand VerifySingle(this FakeDbConnection @this,
            Expression<Func<FakeDbCommand, bool>> commandInvocationPredicate,
            string message = null,
            params object[] args)
        {
            var predicate = commandInvocationPredicate.CompileFast();
            @this.Invocations.Count(predicate)
                .ShouldBe(1,
                        message ?? $"Expected exactly one command matching {commandInvocationPredicate.ToCodeString()}", args);

            return @this.Invocations.Where(commandInvocationPredicate.CompileFast()).Single();
        }    }
}
