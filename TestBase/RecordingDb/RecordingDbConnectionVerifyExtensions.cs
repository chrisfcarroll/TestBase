using System;
using System.Data.Common;
using System.Linq;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace TestBase.RecordingDb
{
    public static class RecordingDbConnectionVerifyExtensions
    {
        /// <summary>
        /// Verifies that the <see cref="RecordingDbConnection"/> was asked to execute a command matching the given predicate
        /// </summary>
        /// <param name="commandInvocationPredicate">A predicate, which may examine the command (and its parameters) sent</param>
        /// <param name="expectedInvocationsCount">Optional, defaults to 1</param>
        /// <param name="exactly">Should the <see cref="expectedInvocationsCount"/>be treated as requiring an exact match or an 'at least' match?</param>
        /// <param name="message">Optional custom failure message</param>
        /// <param name="args">Optional customer failure message format arguments</param>
        /// <returns></returns>
        public static RecordingDbConnection Verify(this RecordingDbConnection @this,
                                              Func<DbCommand, bool> commandInvocationPredicate,
                                              int expectedInvocationsCount = 1,
                                              bool exactly = false,
                                              string message = null,
                                              params object[] args)
        {
            if (exactly)
            {
                @this.Invocations
                     .Count(commandInvocationPredicate)
                     .ShouldBe(expectedInvocationsCount,
                               message ?? "Expected to be called exactly {0} times",
                               args.Length == 0 ? new object[] {expectedInvocationsCount} : args);
            }
            else
            {
                @this.Invocations
                     .Count(commandInvocationPredicate)
                     .ShouldBeGreaterThanOrEqualTo(expectedInvocationsCount,
                                                   message ?? "Expected to be called at least {0} times",
                                                   args.Length == 0 ? new object[] {expectedInvocationsCount} : args);
            }
            return @this;
        }
    }
}
