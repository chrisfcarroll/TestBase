using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ShouldsFeedbackWhenAsserting.ShouldThrowWithUseableErrorMessage__GivenAssertionFail
{
    [TestFixture]
    public partial class GivenAssertionFail
    {
        [TestCase]
        public void And_Given_no_custom_failure_message()
        {
            var failures = new List<Exception>();
            foreach (var assertionWithMessage in TestCasesForNoCustomFailureMessage.AssertionsWithNoCustomFailureMessage)
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

    public static class TestCasesForNoCustomFailureMessage
    {
        public static readonly Dictionary<string,KeyValuePair<Action,string>> AssertionsWithNoCustomFailureMessage 
            = new Dictionary<string,KeyValuePair<Action, string>>()
        {
            { "ShouldNotBeNull",        new KeyValuePair<Action,string>(()=> (null as string).ShouldNotBeNull()       , null)}, 
            { "ShouldBeNull",           new KeyValuePair<Action,string>(()=> 1.ShouldBeNull()                         , null)}, 
            { "ShouldBeNullOrEmpty",    new KeyValuePair<Action,string>(()=> "2".ShouldBeNullOrEmpty()                , "Should Be Null Or Empty")}, 
            { "ShouldBeEmpty",          new KeyValuePair<Action,string>(()=> "3".ShouldBeEmpty()                      , "Should Be Empty")}, 
            { "ShouldBeNullOrEmptyOrWhitespace", new KeyValuePair<Action,string>(()=> "4".ShouldBeNullOrEmptyOrWhitespace()           , null)}, 
            { "ShouldNotBeNullOrEmpty(null)", new KeyValuePair<Action,string>(()=> (null as string).ShouldNotBeNullOrEmpty(), null)}, 
            { "ShouldNotBeNullOrEmpty(stringempty)", new KeyValuePair<Action,string>(()=> "".ShouldNotBeNullOrEmpty()              , null)}, 
            { "ShouldNotBeNullOrEmptyOrWhitespace(null)", new KeyValuePair<Action,string>(()=> "".ShouldNotBeNullOrEmpty(), "ShouldNotBeNull")}, 
            { "ShouldBeSuchThat",       new KeyValuePair<Action,string>(()=> "5".ShouldBe(x=>String.IsNullOrEmpty(x))     , null)}, 
            { "ShouldEqualIgnoringCase",new KeyValuePair<Action,string>(()=> "6".ShouldEqualIgnoringCase("66")        , null)}, 
            { "ShouldEqual",            new KeyValuePair<Action,string>(()=> 7.ShouldEqual(77)                        , null)}, 
            { "ShouldEqualByValue",     new KeyValuePair<Action,string>(()=> "8".ShouldEqualByValue(88)               , null)}, 
            { "ShouldContain<string>",          new KeyValuePair<Action,string>(()=> "9".ShouldContain("99")          , "Expected: String containing \"99\"") }, 
            { "ShouldNotEqual",         new KeyValuePair<Action,string>(()=> "10".ShouldNotEqual("10")                , "Expected: not \"10\"") }, 
            { "ShouldBeTrue",           new KeyValuePair<Action,string>(()=> false.ShouldBeTrue()                     , null)}, 
            { "ShouldBeFalse",          new KeyValuePair<Action,string>(()=> true.ShouldBeFalse()                     , null)}, 
            { "ShouldBeGreaterThan",    new KeyValuePair<Action,string>(()=> 11.ShouldBeGreaterThan(111)              , null)}, 
            { "ShouldBeGreaterThanOrEqualTo", new KeyValuePair<Action,string>(()=> 12.ShouldBeGreaterThanOrEqualTo(122)     , null)}, 
            { "ShouldBeOfType",         new KeyValuePair<Action,string>(()=> "13".ShouldBeOfType<bool>()              , null)}, 
            { "ShouldBeOfTypeEvenIfNull", new KeyValuePair<Action,string>(()=> "13".ShouldBeOfTypeEvenIfNull(typeof(bool)) , null)}, 
            { "ShouldBeAssignableTo",   new KeyValuePair<Action,string>(()=> 14.ShouldBeAssignableTo<string>()        , null)}, 
            { "ShouldBeContainedIn",    new KeyValuePair<Action,string>(()=> "15".ShouldBeContainedIn("x")            , null)}, 
            { "ShouldMatch",            new KeyValuePair<Action,string>(()=> "16".ShouldMatch("sixteen")              , null)}, 
            { "ShouldNotContain",       new KeyValuePair<Action,string>(()=> "17".ShouldNotContain("17")              , null)}, 
            { "ShouldContain<IEnumerable>",          new KeyValuePair<Action,string>(()=> (new[]{"18"}).ShouldContain("188")       , null)}, 
            { "ShouldStartWith",        new KeyValuePair<Action,string>(()=> "19".ShouldStartWith("x19")              , null)}, 
            { "ShouldEndWith",          new KeyValuePair<Action,string>(()=> "20".ShouldEndWith("20x")                , null)}, 
            { "ShouldSatisfy",          new KeyValuePair<Action,string>(()=> 21.ShouldSatisfy(i => i.ToString(), x=>x.Equals("boo"))         , null)}, 
            { "ShouldContainInOrder",   new KeyValuePair<Action,string>(()=> (new List<int>{22,222}).ShouldContainInOrder(222,22) , null)},

            { "ShouldBeFileResult",     new KeyValuePair<Action,string>(
                    () => new RedirectResult("/").ShouldBeFileResult(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , null)},
            { "ShouldBeFileContentResult",     new KeyValuePair<Action,string>(
                    () => new RedirectResult("/").ShouldBeFileContentResult(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , null)},
            { "ShouldBeFileStreamResult",     new KeyValuePair<Action,string>(
                    () => new RedirectResult("/").ShouldBeFileStreamResult(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , null)},
        };
    };

}
