using System;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting.ShouldCallTheRightNUnitAssertThatOverload__GivenAssertionFail
{
    public static class AssertionFailureMessageVerifier
    {
        public static void FailureShouldResultInAssertionWithErrorMessage(this Action assertion, string name, string expectedErrorMessage)
        {
            try
            {
                assertion();
                throw new Assertion(string.Format("{0} Should have thrown an exception before reaching this line: {1} {2}", name, assertion, expectedErrorMessage));
            }
            catch (Assertion e)
            {
                e.Message.ShouldContain(expectedErrorMessage,
                    "Expected {0} to fail assertion with error message containing:\r\n---------------\r\n{1}\r\n-----------\r\n but got\r\n------------\r\n{2}\r\n----------------------", 
                    name, expectedErrorMessage, e.Message);
            }
        }
    }
}