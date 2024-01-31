using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TestBase.Tests.AssertionFailureDisplay.
    ShouldThrowWithUseableErrorMessage__GivenAssertionFail;

public partial class GivenAssertionFail
{
    [Test]
    public void And_Given_custom_failure_message_with_no_args()
    {
        foreach (var assertion in TestCasesForCustomFailureMessageWithNoArgs
                     .AssertionsWithCustomMessage)
            assertion.Value.FailureShouldResultInAssertionWithErrorMessage(assertion.Key,
                TestCasesForCustomFailureMessageWithArgs
                    .FailureMessage);
    }
}

/// <summary>
///     Why a test case dictionary instead of a test for each method under test? I hear you say. Why 3 identical lines of
///     code per test case instead of 1?, I reply.
/// </summary>
public static class TestCasesForCustomFailureMessageWithNoArgs
{
    public static readonly Dictionary<string, Action> AssertionsWithCustomMessage =
        new Dictionary<string, Action>
        {
            {
                "ShouldNotBeNull",
                () => (null as string)
                    .ShouldNotBeNull(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeNull",
                () => 1
                    .ShouldBeNull(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeNullOrEmpty",
                () => "2"
                    .ShouldBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeEmpty",
                () => "3"
                    .ShouldBeEmpty(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeNullOrEmptyOrWhitespace",
                () => "4"
                    .ShouldBeNullOrEmptyOrWhitespace(
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldNotBeNullOrEmpty(null)",
                () => (null as string)
                    .ShouldNotBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldNotBeNullOrEmpty(emptystring)",
                () => ""
                    .ShouldNotBeNullOrEmpty(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeSuchThat",
                () => "5".ShouldBe(x => string
                        .IsNullOrEmpty(x),
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldEqualIgnoringCase",
                () => "6"
                    .ShouldEqualIgnoringCase("66",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldEqual",
                () => 7.ShouldEqual(77,
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldEqualByValue",
                () => "8".ShouldEqualByValue(88,
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldContain<string>",
                () => "9".ShouldContain("99",
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldNotEqual",
                () => "10".ShouldNotEqual("10",
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeTrue",
                () => false
                    .ShouldBeTrue(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeFalse",
                () => true
                    .ShouldBeFalse(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeGreaterThan",
                () => 11
                    .ShouldBeGreaterThan(111,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldBeGreaterThan.2",
                () => 11.ShouldBeGreaterThan(11,
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeGreaterThanOrEqualTo",
                () => 12
                    .ShouldBeGreaterThanOrEqualTo(122,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldBeLessThan",
                () => 11.ShouldBeLessThan(1,
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeLessThan.2",
                () => 11.ShouldBeLessThan(11,
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeLessThanOrEqualTo",
                () => 12
                    .ShouldBeLessThanOrEqualTo(2,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldBeOfType",
                () => "13"
                    .ShouldBeOfType<bool
                    >(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeOfTypeEvenIfNull",
                () => "14"
                    .ShouldBeOfTypeEvenIfNull(typeof(
                            bool
                        ),
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldBeAssignableTo",
                () => 15
                    .ShouldBeAssignableTo<string
                    >(TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeContainedIn",
                () => "16"
                    .ShouldBeContainedIn("sixteen",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldMatch.1",
                () => "16"
                    .ShouldMatch("sixteen",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldMatch.2",
                () => "16"
                    .ShouldMatch("sixteen",
                        RegexOptions
                            .Multiline,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldMatchIgnoringCase",
                () => "16"
                    .ShouldMatchIgnoringCase("sixteen",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldNotMatch.1",
                () => "16".ShouldNotMatch("16",
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldNotMatch.2",
                () => "16".ShouldNotMatch("16",
                    RegexOptions
                        .Multiline,
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldNotMatchIgnoringCase",
                () => "SIX"
                    .ShouldNotMatchIgnoringCase("six",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldNotContain(string)",
                () => "18"
                    .ShouldNotContain("18",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldContain(string)",
                () => "18".ShouldContain("boo",
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldContainEachOf(string)",
                () => "18"
                    .ShouldContainEachOf(new[]
                        {
                            "boo",
                            "booboo"
                        },
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldBeOneOf",
                () => "18".ShouldBeOneOf(new[]
                    {
                        "17",
                        "19"
                    },
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldBeIn",
                () => "18".ShouldBeIn(new[]
                    {
                        "17",
                        "19"
                    },
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldNotBeOneOf",
                () => "18"
                    .ShouldNotBeOneOf(new[]
                        {
                            "17",
                            "18"
                        },
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldNotBeInList",
                () => "18"
                    .ShouldNotBeInList(new[]
                        {
                            "17",
                            "18"
                        },
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldContain<IEnumerable>",
                () => new[] { "19" }
                    .ShouldContain("nineteen",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldStartWith",
                () => "20"
                    .ShouldStartWith("x20",
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldEndWith",
                () => "21".ShouldEndWith("21x",
                    TestCasesForCustomFailureMessageWithArgs
                        .FailureMessage)
            },
            {
                "ShouldSatisfy",
                ()
                    => 22.ShouldSatisfy(i => i
                            .ToString(),
                        x => x
                            .Equals("boo"),
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldContainInOrder",
                () => new List<int> { 23, 24 }
                    .ShouldContainInOrder(24,
                        23,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldContain(item)",
                () => new List<int> { 1, 2 }
                    .ShouldContain(3,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldContain(predicate)",
                () => new List<int> { 2, 1 }
                    .ShouldContain(x => x < 0,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldNotContain(item)",
                () => new List<int> { 1, 2 }
                    .ShouldNotContain(1,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            },
            {
                "ShouldNotContain(predicate)",
                () => new List<int> { 2, 1 }
                    .ShouldNotContain(x => x > 0,
                        TestCasesForCustomFailureMessageWithArgs
                            .FailureMessage)
            }
        };
}