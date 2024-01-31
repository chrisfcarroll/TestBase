using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestBase.Tests.AspNet6.AssertionFailureDisplay.
    ShouldThrowWithUseableErrorMessage__GivenAssertionFail
{
    public partial class GivenAssertionFail
    {
        [Test]
        public void And_Given_custom_failure_message_with_no_args()
        {
            foreach (var assertion in TestCasesForCustomFailureMessageWithNoArgs
                         .AssertionsWithCustomMessage)
                assertion.Value.FailureShouldResultInAssertionWithErrorMessage(
                    assertion.Key,
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
                    "ShouldBeFileResult",
                    () => new RedirectResult("/")
                        .ShouldBeFileResult(null,
                            TestCasesForCustomFailureMessageWithArgs
                                .FailureMessage)
                },
                {
                    "ShouldBeFileContentResult",
                    () => new RedirectResult("/")
                        .ShouldBeFileContentResult(null,
                            TestCasesForCustomFailureMessageWithArgs
                                .FailureMessage)
                },
                {
                    "ShouldBeFileStreamResult",
                    () => new RedirectResult("/")
                        .ShouldBeFileStreamResult(null,
                            TestCasesForCustomFailureMessageWithArgs
                                .FailureMessage)
                }
            };
    }
}