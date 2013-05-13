using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting.UsingAnNUnitWrapperAssertion
{
    public partial class Should_have_called_the_right_overload_of_NUnit_AssertThat__Given_AssertionFail
    {
        [TestMethod]
        public void Given_message_parameter_and_no_args()
        {
            foreach (var assertion in TestCasesForCustomFailureMessageWithNoArgs.AssertionsWithCustomMessage)
            {
                assertion.FailureShouldResultInAssertionExceptionWithErrorMessage(NUnitFailureMessageIndent + TestCasesForCustomFailureMessageWithArgs.FailureMessage);
            }
        }
    }

    public static class TestCasesForCustomFailureMessageWithNoArgs
    {
        public static readonly List<Action> AssertionsWithCustomMessage = new List<Action>
            {
                () => (null as string).ShouldNotBeNull(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => 1.ShouldBeNull(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "2".ShouldBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "3".ShouldBeEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "4".ShouldBeNullOrEmptyOrWhitespace(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => (null as string).ShouldNotBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "".ShouldNotBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "5".ShouldBeSuchThat(String.IsNullOrEmpty, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "6".ShouldEqualIgnoringCase("66", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => 7.ShouldEqual(77, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "8".ShouldEqualByValue(88, TestCasesForCustomFailureMessageWithArgs.FailureMessage) , 
                () => "9".ShouldContain("99", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "10".ShouldNotEqual("10", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => false.ShouldBeTrue(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => true.ShouldBeFalse(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => 11.ShouldBeGreaterThan(111, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => 12.ShouldBeGreaterThanOrEqualTo(122, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "13".ShouldBeOfType<bool>(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "14".ShouldBeOfTypeEvenIfNull(typeof(bool), TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => 15.ShouldBeAssignableTo<string>(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "16".ShouldBeContainedIn("sixteen", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "17".ShouldMatch("seventeen", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "18".ShouldNotContain("18", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => (new[]{"19"}).ShouldContain("nineteen", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "20".ShouldStartWith("x20", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => "21".ShouldEndWith("21x", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => 22.ShouldSatisfy(i => i.ToString(), Is.True, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
                () => (new List<int>{23,24}).ShouldContainInOrder(24,23, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , 
            };
    }
}