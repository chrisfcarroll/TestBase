using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestBase.Tests.AssertionFailureDisplay.
    ShouldThrowWithUseableErrorMessage__GivenAssertionFail;

public partial class GivenAssertionFail
{
    [Test]
    public void And_Given_custom_failure_message_with_args()
    {
        const string failureMessageWithArg = "Failure Message with " +
                                             TestCasesForCustomFailureMessageWithArgs
                                                 .FakeDetailArg;

        foreach (var assertion in TestCasesForCustomFailureMessageWithArgs
                     .AssertionsWithCustomMessageAndArg)
            assertion.Value.FailureShouldResultInAssertionWithErrorMessage(assertion.Key,
                failureMessageWithArg);
    }
}
    

public static class TestCasesForCustomFailureMessageWithArgs
{
    public const string FailureMessageWith = "Failure Message with {0}";
    public const string FakeDetailArg = "fakeDetailArg";
    public const string FailureMessage = "Failure Message";

    public static readonly Dictionary<string, Action> AssertionsWithCustomMessageAndArg =
        new Dictionary<string, Action>
        {
            {
                "ShouldNotBeNull",
                () => (null as string).ShouldNotBeNull(FailureMessageWith, FakeDetailArg)
            },
            { "ShouldBeNull", () => 1.ShouldBeNull(FailureMessageWith, FakeDetailArg) },
            {
                "ShouldBeNullOrEmpty",
                () => "2".ShouldBeNullOrEmpty(FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeEmpty",
                () => "3".ShouldBeEmpty(FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeNullOrEmptyOrWhitespace",
                () => "4".ShouldBeNullOrEmptyOrWhitespace(FailureMessageWith,
                    FakeDetailArg)
            },
            {
                "ShouldNotBeNullOrEmpty(null)",
                () => (null as string).ShouldNotBeNullOrEmpty(FailureMessageWith,
                    FakeDetailArg)
            },
            {
                "ShouldNotBeNullOrEmpty(emptystring)",
                () => "".ShouldNotBeNullOrEmpty(FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeSuchThat",
                () => "5".ShouldBe(x => string.IsNullOrEmpty(x), FailureMessageWith,
                    FakeDetailArg)
            },
            {
                "ShouldEqualIgnoringCase",
                () => "6".ShouldEqualIgnoringCase("66", FailureMessageWith, FakeDetailArg)
            },
            { "ShouldEqual", () => 7.ShouldEqual(77, FailureMessageWith, FakeDetailArg) },
            {
                "ShouldEqualByValue",
                () => "8".ShouldEqualByValue(88, FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldContain<string>",
                () => "9".ShouldContain("99", FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldNotEqual",
                () => "10".ShouldNotEqual("10", FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeTrue",
                () => false.ShouldBeTrue(FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeFalse",
                () => true.ShouldBeFalse(FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeGreaterThan",
                () => 11.ShouldBeGreaterThan(111, FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeGreaterThanOrEqualTo",
                () => 12.ShouldBeGreaterThanOrEqualTo(122, FailureMessageWith,
                    FakeDetailArg)
            },
            {
                "ShouldBeOfType",
                () => "13".ShouldBeOfType<bool>(FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeOfTypeEvenIfNull",
                () => "14".ShouldBeOfTypeEvenIfNull(typeof(bool), FailureMessageWith,
                    FakeDetailArg)
            },
            {
                "ShouldBeAssignableTo",
                () => 15.ShouldBeAssignableTo<string>(FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeContainedIn",
                () => "16".ShouldBeContainedIn("sixteen", FailureMessageWith,
                    FakeDetailArg)
            },
            {
                "ShouldMatch",
                () => "17".ShouldMatch("seventeen", FailureMessageWith, FakeDetailArg)
            },
            {
                "ShouldBeOneOf", () => "18".ShouldBeOneOf(new[]
                {
                    "17", "19"}, FailureMessageWith, FakeDetailArg)},
                    {"ShouldBeIn", () => "18".ShouldBeIn(new[]
                    {
                        "17", "19"}, FailureMessageWith, FakeDetailArg)},
                        {"ShouldNotBeOneOf", () => "18".ShouldNotBeOneOf(new[]
                        {
                            "17", "18"}, FailureMessageWith, FakeDetailArg)},
                            {"ShouldNotBeInList", () => "18".ShouldNotBeInList(new[]
                            {
                                "17", "18"}, FailureMessageWith, FakeDetailArg)},
                                {"ShouldNotContain(string)",
                                () => "18".ShouldNotContain("18", FailureMessageWith,
                                    FakeDetailArg)},
                                {
                                    "ShouldContain(string)",
                                    () => "18".ShouldContain("boo", FailureMessageWith,
                                        FakeDetailArg)
                                },
                                {
                                    "ShouldContainEachOf(string)",
                                    () => "18".ShouldContainEachOf(new[]
                                    {
                                        "boo",
                                        "booboo"}, FailureMessageWith, FakeDetailArg)
                                    }, 
                                    {"ShouldContain<IEnumerable>",
                                    () => new[] { "19" }.ShouldContain("nineteen",
                                        FailureMessageWith, FakeDetailArg)},
                                    {
                                        "ShouldStartWith",
                                        () => "20".ShouldStartWith("x20",
                                            FailureMessageWith, FakeDetailArg)
                                    },
                                    {
                                        "ShouldEndWith",
                                        () => "21".ShouldEndWith("21x",
                                            FailureMessageWith, FakeDetailArg)
                                    },
                                    {
                                        "ShouldSatisfy",
                                        () => 22.ShouldSatisfy(i => i.ToString(),
                                            x => x.Equals("boo"), FailureMessageWith,
                                            FakeDetailArg)
                                    },
                                    {
                                        "ShouldContainInOrder",
                                        () => new List<int> { 23, 24 }
                                            .ShouldContainInOrder(24, 23,
                                                FailureMessageWith, FakeDetailArg)
                                    },
                                    {
                                        "ShouldContain(item)",
                                        () => new List<int> { 1, 2 }.ShouldContain(3,
                                            FailureMessageWith, FakeDetailArg)
                                    },
                                    {
                                        "ShouldContain(predicate)",
                                        () => new List<int> { 2, 1 }.ShouldContain(
                                            x => x < 0, FailureMessageWith, FakeDetailArg)
                                    },
                                    {
                                        "ShouldNotContain(item)",
                                        () => new List<int> { 1, 2 }.ShouldNotContain(1,
                                            FailureMessageWith, FakeDetailArg)
                                    },
                                    {
                                        "ShouldNotContain(predicate)",
                                        () => new List<int> { 2, 1 }.ShouldNotContain(
                                            x => x > 0, FailureMessageWith, FakeDetailArg)
                                    }
                                };
                            }