using System;

namespace TestBase.Tests.ShouldsFeedbackWhenAsserting.ShouldThrowWithUseableErrorMessage__GivenAssertionFail
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
                if (!e.Message.Contains(expectedErrorMessage))
                {
                    Console.WriteLine(@"{0}
Warning: wrong error message. Expected failure message containing:
------------
{1}
------------
but got
------------
{2}
------------",name, expectedErrorMessage, e.Message);
                }

                return;
            }
            throw new Assertion(string.Format("{0} Should have thrown an exception before reaching this line: {1} {2}", name, assertion, expectedErrorMessage));
        }
    }
}