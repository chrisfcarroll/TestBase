using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsFeedbackWhenAssertingFailure.ShouldThrowWithUseableErrorMessage__GivenAssertionFail
{
    [TestFixture]
    public partial class GivenAssertionFail
    {
        [TestCase]
        public void And_Given_no_custom_failure_message_but_default_message()
        {
            var failures = new List<Exception>();
            foreach (var assertionWithMessage in TestCasesForAssertionsWithDefaultMessages.Cases)
            {
                try
                {
                    var assertion = assertionWithMessage.Value.Key;
                    var expectedExceptionMessage =
                        assertionWithMessage.Value.Value?.Replace("\r\n", Environment.NewLine);

                    assertion.FailureShouldResultInAssertionWithErrorMessage(assertionWithMessage.Key,
                                                                             expectedExceptionMessage
                                                                          ?? assertionWithMessage.Key.Split('(')[0]);
                }
                catch (Exception e)
                {
                    failures.Add(e);
                }
            }

            if (failures.Any())
            {
                throw new AggregateException(failures.ToList());
            }
        }
    }

    public static class TestCasesForAssertionsWithDefaultMessages
    {
        public static readonly Dictionary<string,KeyValuePair<Action,string>> Cases 
            = new Dictionary<string,KeyValuePair<Action, string>>()
        {
            { "ShouldBeOfType",         
                new KeyValuePair<Action,string>(()=> "13".ShouldBeOfType<bool>(),
                $"actual of type {13.GetType()} ShouldBeOfType {typeof(bool)} but isn't.")}, 

            { "ShouldBeOfTypeEvenIfNull", 
                new KeyValuePair<Action,string>(()=> "13".ShouldBeOfTypeEvenIfNull(typeof(bool)),
                $"ShouldBeOfTypeEvenIfNull {typeof(bool)}")}, 

            { "ShouldBeAssignableTo",
                new KeyValuePair<Action,string>(()=> 14.ShouldBeAssignableTo<string>(),
                $"{14.GetType()} ShouldBeAssignableTo {typeof(bool)} but isn't.")}, 

            { "ShouldBeCastableTo",
                new KeyValuePair<Action,string>(()=> 14.ShouldBeCastableTo<string>(),
                    $"actual of type {14.GetType()} ShouldBeCastableTo {typeof(bool)} but isn't.")}, 
        };
    };

}
