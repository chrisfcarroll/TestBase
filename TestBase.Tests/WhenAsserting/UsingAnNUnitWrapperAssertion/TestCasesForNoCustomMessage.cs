using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting.UsingAnNUnitWrapperAssertion
{
    [TestClass]
    public partial class Should_have_called_the_right_overload_of_NUnit_AssertThat__Given_AssertionFail
    {
        [TestMethod]
        public void Given_no_message_parameter()
        {
            foreach (var assertion in TestCasesForNoCustomFailureMessage.AssertionsWithNoCustomFailureMessage)
            {
                assertion.Key.FailureShouldResultInAssertionExceptionWithErrorMessage(NUnitFailureMessageIndent + assertion.Value);
            }
        }

        private const string NUnitFailureMessageIndent = "  ";
    }

    public static class TestCasesForNoCustomFailureMessage
    {
        public static readonly Dictionary<Action, string> AssertionsWithNoCustomFailureMessage = new Dictionary<Action, string>()
        {
            {() => (null as string).ShouldNotBeNull()       , "Expected: not null\r\n  But was:"},
            {() => 1.ShouldBeNull()                         , "Expected: null\r\n  But was:"},
            {() => "2".ShouldBeNullOrEmpty()                , "Should Be Null Or Empty"},
            {() => "3".ShouldBeEmpty()                      , "Should Be Empty"},
            {() => "4".ShouldBeNullOrEmptyOrWhitespace()           , "ShouldBeNullOrWhitespace"},
            {() => (null as string).ShouldNotBeNullOrEmpty(), "Expected: not null"},
            {() => "".ShouldNotBeNullOrEmpty()              , "Expected: not <string.Empty>"},
            {() => "5".ShouldBeSuchThat(String.IsNullOrEmpty)       , "Expected: True"},
            {() => "6".ShouldEqualIgnoringCase("66")        , "Expected string length 2 but was 1."},
            {() => 7.ShouldEqual(77)                        , "Expected: 77"},
            {() => "8".ShouldEqualByValue(88)               , "Expected: 88"},
            {() => "9".ShouldContain("99")                  , "Expected: String containing \"99\""},
            {() => "10".ShouldNotEqual("10")                , "Expected: not \"10\""},
            {() => false.ShouldBeTrue()                     , "Expected: True"},
            {() => true.ShouldBeFalse()                     , "Expected: False"},
            {() => 11.ShouldBeGreaterThan(111)              , "Expected: greater than 111"},
            {() => 12.ShouldBeGreaterThanOrEqualTo(122)     , "Expected: "},
            {() => "13".ShouldBeOfType<bool>()              , "Expected: "},
            {() => "13".ShouldBeOfTypeEvenIfNull(typeof(bool)) , "Expected: "},
            {() => 14.ShouldBeAssignableTo<string>()        , "Expected: "},
            {() => "15".ShouldBeContainedIn("x")            , "Expected: "},
            {() => "16".ShouldMatch("sixteen")              , "Expected: "},
            {() => "17".ShouldNotContain("17")              , "Expected: "},
            {() => (new[]{"18"}).ShouldContain("188")       , "Expected: "},
            {() => "19".ShouldStartWith("x19")              , "Expected: "},
            {() => "20".ShouldEndWith("20x")                , "Expected: "},
            {() => 21.ShouldSatisfy(i => i.ToString(), Is.True)         , "Expected: "},
            {() => (new List<int>{22,222}).ShouldContainInOrder(222,22) , "Expected: "},
        };
    }

}
