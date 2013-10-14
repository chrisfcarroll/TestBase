using System;
using System.Linq;
using TestBase.Shoulds;

namespace TestBase.FakeDb
{
    public static class DbConnectionVerifyExtensions
    {
        public static FakeDbConnection Verify(this FakeDbConnection @this,
                                              Func<FakeDbCommand, bool> commandInvocationPredicate,
                                              int expectedInvocationsCount = 1,
                                              bool exactly = false)
        {
            if (exactly)
            {
                @this.Invocations
                     .Where(commandInvocationPredicate)
                     .Count()
                     .ShouldBe(expectedInvocationsCount,
                               "Expected to be called exactly {0} times",
                               expectedInvocationsCount);
            }
            else
            {
                @this.Invocations
                     .Where(commandInvocationPredicate)
                     .Count()
                     .ShouldBeGreaterThanOrEqualTo(expectedInvocationsCount,
                                                   "Expected to be called at least {0} times",
                                                   expectedInvocationsCount);
            }
            return @this;
        }
    }
}
