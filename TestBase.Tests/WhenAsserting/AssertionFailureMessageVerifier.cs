using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting
{
    public static class AssertionFailureMessageVerifier
    {
        public static void FailureShouldResultInAssertionExceptionWithErrorMessage(this Action assertion, string expectedErrorMessage)
        {
            try
            {
                assertion();
                Assert.Fail("Should have thrown an exception before reaching this line: {0} {1}", assertion, expectedErrorMessage);
            }
            catch (NUnit.Framework.AssertionException e)
            {
                e.Message.ShouldStartWith(expectedErrorMessage,"Expected to catch an error message starting with {0}\r\n but got\r\n{1}", expectedErrorMessage, e.Message);
            }
        }
    }
}