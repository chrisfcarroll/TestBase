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
        public void Given_message_parameter_and_args()
        {
            const string failureMessageWithArg = "Failure Message with " + TestCasesForCustomFailureMessageWithArgs.FakeDetailArg;
            foreach (var assertion in TestCasesForCustomFailureMessageWithArgs.AssertionsWithCustomMessageAndArg)
            {
                assertion.FailureShouldResultInAssertionExceptionWithErrorMessage(NUnitFailureMessageIndent + failureMessageWithArg);
            }
        }
    }

    public static class TestCasesForCustomFailureMessageWithArgs
    {
        public static readonly List<Action> AssertionsWithCustomMessageAndArg = new List<Action>
            {
                () => (null as string).ShouldNotBeNull(FailureMessageWith, FakeDetailArg ) , 
                () => 1.ShouldBeNull(FailureMessageWith, FakeDetailArg ) , 
                () => "2".ShouldBeNullOrEmpty(FailureMessageWith, FakeDetailArg ) , 
                () => "3".ShouldBeEmpty(FailureMessageWith, FakeDetailArg ) , 
                () => "4".ShouldBeNullOrEmptyOrWhitespace(FailureMessageWith, FakeDetailArg ) , 
                () => (null as string).ShouldNotBeNullOrEmpty(FailureMessageWith, FakeDetailArg ) , 
                () => "".ShouldNotBeNullOrEmpty(FailureMessageWith, FakeDetailArg ) , 
                () => "5".ShouldBeSuchThat(string.IsNullOrEmpty, FailureMessageWith, FakeDetailArg ) , 
                () => "6".ShouldEqualIgnoringCase("66", FailureMessageWith, FakeDetailArg ) , 
                () => 7.ShouldEqual(77, FailureMessageWith, FakeDetailArg ) , 
                () => "8".ShouldEqualByValue(88, FailureMessageWith, FakeDetailArg) , 
                () => "9".ShouldContain("99", FailureMessageWith, FakeDetailArg ) , 
                () => "10".ShouldNotEqual("10", FailureMessageWith, FakeDetailArg ) , 
                () => false.ShouldBeTrue(FailureMessageWith, FakeDetailArg ) , 
                () => true.ShouldBeFalse(FailureMessageWith, FakeDetailArg ) , 
                () => 11.ShouldBeGreaterThan(111, FailureMessageWith, FakeDetailArg ) , 
                () => 12.ShouldBeGreaterThanOrEqualTo(122, FailureMessageWith, FakeDetailArg ) , 
                () => "13".ShouldBeOfType<bool>(FailureMessageWith, FakeDetailArg ) , 
                () => "14".ShouldBeOfTypeEvenIfNull(typeof(bool), FailureMessageWith, FakeDetailArg ) , 
                () => 15.ShouldBeAssignableTo<string>(FailureMessageWith, FakeDetailArg ) , 
                () => "16".ShouldBeContainedIn("sixteen", FailureMessageWith, FakeDetailArg ) , 
                () => "17".ShouldMatch("seventeen", FailureMessageWith, FakeDetailArg ) , 
                () => "18".ShouldNotContain("18", FailureMessageWith, FakeDetailArg ) , 
                () => (new[]{"19"}).ShouldContain("nineteen", FailureMessageWith, FakeDetailArg ) , 
                () => "20".ShouldStartWith("x20", FailureMessageWith, FakeDetailArg),
                () => "21".ShouldEndWith("21x", FailureMessageWith, FakeDetailArg ) , 
                () => 22.ShouldSatisfy(i => i.ToString(), Is.True, FailureMessageWith, FakeDetailArg ) , 
                () => (new List<int>{23,24}).ShouldContainInOrder(24,23, FailureMessageWith, FakeDetailArg ) , 
            };

        public const string FailureMessageWith = "Failure Message with {0}";
        public const string FakeDetailArg = "fakeDetailArg";
        public const string FailureMessage = "Failure Message";
    }

}