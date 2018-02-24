using System;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting.ShouldCallTheRightNUnitAssertThatOverload__GivenAssertionFail
{
    public static class AssertionFailureMessageVerifier
    {
        public static void FailureShouldResultInAssertionExceptionWithErrorMessage(this Action assertion, string name, string expectedErrorMessage)
        {
            try
            {
                assertion();
                throw new AssertionException(string.Format("{0} Should have thrown an exception before reaching this line: {1} {2}", name, assertion, expectedErrorMessage));
            }
            catch (AssertionException e)
            {
                e.Message.ShouldStartWith(expectedErrorMessage,"Expected {0} to fail assertion with error message starting with {1}\r\n but got\r\n{2}", name, expectedErrorMessage, e.Message);
            }
        }
    }
}