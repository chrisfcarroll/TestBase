using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting.UsingAnNUnitWrapperAssertion
{
    [TestFixture]
    public partial class Should_call_the_right_NUnitAssertThat_overload__Given_AssertionFail
    {
        [Test]
        public void Given_no_custom_fail_message()
        {
            foreach (var assertionWithMessage in TestCasesForNoCustomFailureMessage.AssertionsWithNoCustomFailureMessage)
            {
                var assertion = assertionWithMessage.Value.Key;
                var expectedExceptionMessage = nunitFailureMessageIndent + 
                                               assertionWithMessage.Value.Value.Replace("\r\n",Environment.NewLine);

                assertion.FailureShouldResultInAssertionExceptionWithErrorMessage(assertionWithMessage.Key, expectedExceptionMessage);
            }
        }

        private const string nunitFailureMessageIndent = "  ";
    }

    public static class TestCasesForNoCustomFailureMessage
    {
        public static readonly Dictionary<string,KeyValuePair<Action,string>> AssertionsWithNoCustomFailureMessage 
            = new Dictionary<string,KeyValuePair<Action, string>>()
        {
            { "ShouldNotBeNull",        new KeyValuePair<Action,string>(()=> (null as string).ShouldNotBeNull()       , "Expected: not null\r\n  But was:") }, 
            { "ShouldBeNull",           new KeyValuePair<Action,string>(()=> 1.ShouldBeNull()                         , "Expected: null\r\n  But was:") }, 
            { "ShouldBeNullOrEmpty",    new KeyValuePair<Action,string>(()=> "2".ShouldBeNullOrEmpty()                , "Should Be Null Or Empty") }, 
            { "ShouldBeEmpty",          new KeyValuePair<Action,string>(()=> "3".ShouldBeEmpty()                      , "Should Be Empty") }, 
            { "ShouldBeNullOrEmptyOrWhitespace", new KeyValuePair<Action,string>(()=> "4".ShouldBeNullOrEmptyOrWhitespace()           , "ShouldBeNullOrWhitespace") }, 
            { "ShouldNotBeNullOrEmpty(null)", new KeyValuePair<Action,string>(()=> (null as string).ShouldNotBeNullOrEmpty(), "Expected: not null") }, 
            { "ShouldNotBeNullOrEmpty(stringempty)", new KeyValuePair<Action,string>(()=> "".ShouldNotBeNullOrEmpty()              , "Expected: not <string.Empty>") }, 
            { "ShouldBeSuchThat",       new KeyValuePair<Action,string>(()=> "5".ShouldBeSuchThat(String.IsNullOrEmpty)       , "Expected: True") }, 
            { "ShouldEqualIgnoringCase",new KeyValuePair<Action,string>(()=> "6".ShouldEqualIgnoringCase("66")        , "Expected string length 2 but was 1.") }, 
            { "ShouldEqual",            new KeyValuePair<Action,string>(()=> 7.ShouldEqual(77)                        , "Expected: 77") }, 
            { "ShouldEqualByValue",     new KeyValuePair<Action,string>(()=> "8".ShouldEqualByValue(88)               , "Expected: 88") }, 
            { "ShouldContain<string>",          new KeyValuePair<Action,string>(()=> "9".ShouldContain("99")                  , "Expected: String containing \"99\"") }, 
            { "ShouldNotEqual",         new KeyValuePair<Action,string>(()=> "10".ShouldNotEqual("10")                , "Expected: not \"10\"") }, 
            { "ShouldBeTrue",           new KeyValuePair<Action,string>(()=> false.ShouldBeTrue()                     , "Expected: True") }, 
            { "ShouldBeFalse",          new KeyValuePair<Action,string>(()=> true.ShouldBeFalse()                     , "Expected: False") }, 
            { "ShouldBeGreaterThan",    new KeyValuePair<Action,string>(()=> 11.ShouldBeGreaterThan(111)              , "Expected: greater than 111") }, 
            { "ShouldBeGreaterThanOrEqualTo", new KeyValuePair<Action,string>(()=> 12.ShouldBeGreaterThanOrEqualTo(122)     , "Expected: ") }, 
            { "ShouldBeOfType",         new KeyValuePair<Action,string>(()=> "13".ShouldBeOfType<bool>()              , "Expected: ") }, 
            { "ShouldBeOfTypeEvenIfNull", new KeyValuePair<Action,string>(()=> "13".ShouldBeOfTypeEvenIfNull(typeof(bool)) , "Expected: ") }, 
            { "ShouldBeAssignableTo",   new KeyValuePair<Action,string>(()=> 14.ShouldBeAssignableTo<string>()        , "Expected: ") }, 
            { "ShouldBeContainedIn",    new KeyValuePair<Action,string>(()=> "15".ShouldBeContainedIn("x")            , "Expected: ") }, 
            { "ShouldMatch",            new KeyValuePair<Action,string>(()=> "16".ShouldMatch("sixteen")              , "Expected: ") }, 
            { "ShouldNotContain",       new KeyValuePair<Action,string>(()=> "17".ShouldNotContain("17")              , "Expected: ") }, 
            { "ShouldContain<IEnumerable>",          new KeyValuePair<Action,string>(()=> (new[]{"18"}).ShouldContain("188")       , "Expected: ") }, 
            { "ShouldStartWith",        new KeyValuePair<Action,string>(()=> "19".ShouldStartWith("x19")              , "Expected: ") }, 
            { "ShouldEndWith",          new KeyValuePair<Action,string>(()=> "20".ShouldEndWith("20x")                , "Expected: ") }, 
            { "ShouldSatisfy",          new KeyValuePair<Action,string>(()=> 21.ShouldSatisfy(i => i.ToString(), Is.True)         , "Expected: ") }, 
            { "ShouldContainInOrder",   new KeyValuePair<Action,string>(()=> (new List<int>{22,222}).ShouldContainInOrder(222,22) , "Expected: ") },

            { "ShouldBeFileResult",     new KeyValuePair<Action,string>(() => (new RedirectResult("/")).ShouldBeFileResult(TestCasesForCustomFailureMessageWithArgs.FailureMessage ) , "Expected")},
        };
    };

}
