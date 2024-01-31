using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestBase.Tests.AspNet6.AssertionFailureDisplay.ShouldThrowWithUseableErrorMessage__GivenAssertionFail
{
    public partial class GivenAssertionFail
    {
        [Test]
        public void And_Given_custom_failure_message_with_args()
        {
            const string failureMessageWithArg =
            "Failure Message with " + TestCasesForCustomFailureMessageWithArgs.FakeDetailArg;
            foreach (var assertion in TestCasesForCustomFailureMessageWithArgs.AssertionsWithCustomMessageAndArg)
                assertion.Value.FailureShouldResultInAssertionWithErrorMessage(assertion.Key, failureMessageWithArg);
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
        "ShouldBeFileResult", () => new RedirectResult("/").ShouldBeFileResult(null, FailureMessageWith, FakeDetailArg)
        },
        {
        "ShouldBeFileContentResult",
        () => new RedirectResult("/").ShouldBeFileContentResult(null, FailureMessageWith, FakeDetailArg)
        },
        {
        "ShouldBeFileStreamResult",
        () => new RedirectResult("/").ShouldBeFileStreamResult(null, FailureMessageWith, FakeDetailArg)
        }
        };
    }
}
