using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting.ShouldCallTheRightNUnitAssertThatOverload__GivenAssertionFail
{
    public partial class Should_call_the_right_NUnitAssertThat_overload__Given_AssertionFail
    {
        [Test]
        public void Given_custom_fail_message_with_args()
        {
            const string failureMessageWithArg = "Failure Message with " + TestCasesForCustomFailureMessageWithArgs.FakeDetailArg;
            foreach (var assertion in TestCasesForCustomFailureMessageWithArgs.AssertionsWithCustomMessageAndArg)
            {
                assertion.Value.FailureShouldResultInAssertionExceptionWithErrorMessage(assertion.Key, nunitFailureMessageIndent + failureMessageWithArg);
            }
        }
    }

    public static class TestCasesForCustomFailureMessageWithArgs
    {
        public static readonly Dictionary<string,Action> AssertionsWithCustomMessageAndArg = new Dictionary<string, Action>
            {
                { "ShouldNotBeNull", () => (null as string).ShouldNotBeNull(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeNull", () => 1.ShouldBeNull(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeNullOrEmpty", () => "2".ShouldBeNullOrEmpty(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeEmpty", () => "3".ShouldBeEmpty(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeNullOrEmptyOrWhitespace", () => "4".ShouldBeNullOrEmptyOrWhitespace(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldNotBeNullOrEmpty(null)", () => (null as string).ShouldNotBeNullOrEmpty(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldNotBeNullOrEmpty(emptystring)", () => "".ShouldNotBeNullOrEmpty(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeSuchThat", () => "5".ShouldBeSuchThat(string.IsNullOrEmpty, FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldEqualIgnoringCase", () => "6".ShouldEqualIgnoringCase("66", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldEqual", () => 7.ShouldEqual(77, FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldEqualByValue", () => "8".ShouldEqualByValue(88, FailureMessageWith, FakeDetailArg) } ,
                { "ShouldContain<string>", () => "9".ShouldContain("99", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldNotEqual", () => "10".ShouldNotEqual("10", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeTrue", () => false.ShouldBeTrue(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeFalse", () => true.ShouldBeFalse(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeGreaterThan", () => 11.ShouldBeGreaterThan(111, FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeGreaterThanOrEqualTo", () => 12.ShouldBeGreaterThanOrEqualTo(122, FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeOfType", () => "13".ShouldBeOfType<bool>(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeOfTypeEvenIfNull", () => "14".ShouldBeOfTypeEvenIfNull(typeof(bool), FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeAssignableTo", () => 15.ShouldBeAssignableTo<string>(FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldBeContainedIn", () => "16".ShouldBeContainedIn("sixteen", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldMatch", () => "17".ShouldMatch("seventeen", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldNotContain", () => "18".ShouldNotContain("18", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldContain<IEnumerable>", () => (new[]{"19"}).ShouldContain("nineteen", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldStartWith", () => "20".ShouldStartWith("x20", FailureMessageWith, FakeDetailArg) },
                { "ShouldEndWith", () => "21".ShouldEndWith("21x", FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldSatisfy", () => 22.ShouldSatisfy(i => i.ToString(), Is.True, FailureMessageWith, FakeDetailArg ) } ,
                { "ShouldContainInOrder", () => (new List<int>{23,24}).ShouldContainInOrder(24,23, FailureMessageWith, FakeDetailArg ) } ,
 
                { "ShouldBeFileResult",   () => (new RedirectResult("/")).ShouldBeFileResult(null, FailureMessageWith, FakeDetailArg ) },
             };

        public const string FailureMessageWith = "Failure Message with {0}";
        public const string FakeDetailArg = "fakeDetailArg";
        public const string FailureMessage = "Failure Message";
    }

}