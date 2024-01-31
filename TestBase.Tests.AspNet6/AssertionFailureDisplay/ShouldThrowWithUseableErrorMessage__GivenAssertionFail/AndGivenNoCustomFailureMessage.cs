using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestBase.Tests.AspNet6.AssertionFailureDisplay.ShouldThrowWithUseableErrorMessage__GivenAssertionFail
{
    [TestFixture]
    public partial class GivenAssertionFail
    {
        [TestCase]
        public void And_Given_no_custom_failure_message()
        {
            var failures = new List<Exception>();
            foreach (var assertionWithMessage in TestCasesForNoCustomFailureMessage.AssertionsWithNoCustomFailureMessage
            )
                try
                {
                    var assertion = assertionWithMessage.Value.Key;
                    var expectedExceptionMessage =
                    assertionWithMessage.Value.Value?.Replace("\r\n", Environment.NewLine);

                    assertion.FailureShouldResultInAssertionWithErrorMessage(assertionWithMessage.Key,
                                                                             expectedExceptionMessage
                                                                          ?? assertionWithMessage.Key.Split('(')[0]);
                } catch (Exception e) { failures.Add(e); }

            if (failures.Any()) throw new AggregateException(failures.ToList());
        }
    }

    public static class TestCasesForNoCustomFailureMessage
    {
        public static readonly Dictionary<string, KeyValuePair<Action, string>> AssertionsWithNoCustomFailureMessage
        = new Dictionary<string, KeyValuePair<Action, string>>
          {
          {
          "ShouldBeFileResult", new KeyValuePair<Action, string>(
                                                                 () => new RedirectResult("/")
                                                                .ShouldBeFileResult(TestCasesForCustomFailureMessageWithArgs
                                                                                   .FailureMessage),
                                                                 null)
          },
          {
          "ShouldBeFileContentResult", new KeyValuePair<Action, string>(
                                                                        () => new RedirectResult("/")
                                                                       .ShouldBeFileContentResult(TestCasesForCustomFailureMessageWithArgs
                                                                                                 .FailureMessage),
                                                                        null)
          },
          {
          "ShouldBeFileStreamResult", new KeyValuePair<Action, string>(
                                                                       () => new RedirectResult("/")
                                                                      .ShouldBeFileStreamResult(TestCasesForCustomFailureMessageWithArgs
                                                                                               .FailureMessage),
                                                                       null)
          }
          };
    }
}
