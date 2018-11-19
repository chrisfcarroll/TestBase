using System;

namespace TestBase.Tests.ShouldsFeedbackWhenAssertingFailure.ShouldThrowWithUseableErrorMessage__GivenAssertionFail
{
    public static class AssertionFailureMessageVerifier
    {
        public static void FailureShouldResultInAssertionWithErrorMessage(this Action assertion, string name, string expectedErrorMessage)
        {
            try
            {
                assertion();
            }
            catch (Assertion e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(@"
----------------");

                if (!e.Message.Contains(expectedErrorMessage))
                {
                    Console.WriteLine(@"{0}
Warning: wrong error message. Expected failure message containing:
----------------
{1}
----------------
",name, expectedErrorMessage);
                }

                return;
            }

            throw new Assertion(string.Format("{0} Should have thrown an exception before reaching this line: {1} {2}", name, assertion, expectedErrorMessage));
        }
    }
}