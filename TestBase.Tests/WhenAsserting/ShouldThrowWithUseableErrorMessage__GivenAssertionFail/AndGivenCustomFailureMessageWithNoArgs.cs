using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;
using TestBase.Tests.WhenAsserting.ShouldCallTheRightNUnitAssertThatOverload__GivenAssertionFail;

namespace TestBase.Tests.WhenAsserting.ShouldThrowWithUseableErrorMessage__GivenAssertionFail
{
    public partial class GivenAssertionFail
    {
        [Test]
        public void And_Given_custom_failure_message_with_no_args()
        {
            foreach (var assertion in TestCasesForCustomFailureMessageWithNoArgs.AssertionsWithCustomMessage)
            {
                assertion.Value.FailureShouldResultInAssertionWithErrorMessage(assertion.Key, TestCasesForCustomFailureMessageWithArgs.FailureMessage);
            }
        }
    }

    /// <summary>
    /// Why a test case dictionary instead of a test for each method under test? I hear you say. Why 3 identical lines of code per test case instead of 1?, I reply.
    /// </summary>
    public static class TestCasesForCustomFailureMessageWithNoArgs
    {
        public static readonly Dictionary<string, Action> AssertionsWithCustomMessage = new Dictionary<string, Action>
        {
            { "ShouldNotBeNull",        () => (null as string).ShouldNotBeNull(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeNull",           () => 1.ShouldBeNull(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeNullOrEmpty",    () => "2".ShouldBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeEmpty",          () => "3".ShouldBeEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeNullOrEmptyOrWhitespace", () => "4".ShouldBeNullOrEmptyOrWhitespace(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldNotBeNullOrEmpty(null)", () => (null as string).ShouldNotBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldNotBeNullOrEmpty(emptystring)", () => "".ShouldNotBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeSuchThat",       () => "5".ShouldBe(x=>String.IsNullOrEmpty(x), TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldEqualIgnoringCase",() => "6".ShouldEqualIgnoringCase("66", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldEqual",            () => 7.ShouldEqual(77, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldEqualByValue",     () => "8".ShouldEqualByValue(88, TestCasesForCustomFailureMessageWithArgs.FailureMessage) }, 
            { "ShouldContain<string>",          () => "9".ShouldContain("99", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldNotEqual",         () => "10".ShouldNotEqual("10", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeTrue",           () => false.ShouldBeTrue(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeFalse",          () => true.ShouldBeFalse(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeGreaterThan",    () => 11.ShouldBeGreaterThan(111, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeGreaterThanOrEqualTo", () => 12.ShouldBeGreaterThanOrEqualTo(122, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeOfType",         () => "13".ShouldBeOfType<bool>(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeOfTypeEvenIfNull", () => "14".ShouldBeOfTypeEvenIfNull(typeof(bool), TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeAssignableTo",   () => 15.ShouldBeAssignableTo<string>(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldBeContainedIn",    () => "16".ShouldBeContainedIn("sixteen", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldMatch",            () => "17".ShouldMatch("seventeen", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldNotContain",       () => "18".ShouldNotContain("18", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldContain<IEnumerable>",          () => (new[]{"19"}).ShouldContain("nineteen", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldStartWith",        () => "20".ShouldStartWith("x20", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldEndWith",          () => "21".ShouldEndWith("21x", TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldSatisfy",          () => 22.ShouldSatisfy(i => i.ToString(), x=>x.Equals("boo"), TestCasesForCustomFailureMessageWithArgs.FailureMessage ) }, 
            { "ShouldContainInOrder",   () => (new List<int>{23,24}).ShouldContainInOrder(24,23, TestCasesForCustomFailureMessageWithArgs.FailureMessage ) },

            { "ShouldBeFileResult",   () => (new RedirectResult("/")).ShouldBeFileResult(null,TestCasesForCustomFailureMessageWithArgs.FailureMessage ) },
            { "ShouldBeFileContentResult",   () => (new RedirectResult("/")).ShouldBeFileContentResult(null,TestCasesForCustomFailureMessageWithArgs.FailureMessage ) },
            { "ShouldBeFileStreamResult",   () => (new RedirectResult("/")).ShouldBeFileStreamResult(null,TestCasesForCustomFailureMessageWithArgs.FailureMessage ) },
        };
    }
}