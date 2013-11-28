using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ComparerEqualsByValueTests.Example
{
    [TestFixture]
    public class WhenComparingExampleClass
    {
        [TestCase("AStringWithNoDelimiters")]
        [TestCase("123456789")]
        public void Should_parse_string_with_no_delimiters_as_nameorid(string input)
        {
            ((SearchBox)input).ShouldEqualByValue(new SearchBox { NameOrId = input });
        }

        [TestCase("Datum1=AString", "AString")]
        public void Should_parse_string_with_Datum1(string input, string expectedDatum1)
        {
            ((SearchBox)input).ShouldEqualByValue(new SearchBox { Datum1 = expectedDatum1 });
        }

        [TestCase("StatusEnum=NotProceeding", StatusEnum.NotProceeding)]
        public void Should_parse_string_with_ClientState(string input, StatusEnum expected)
        {
            ((SearchBox)input).ShouldEqualByValue(new SearchBox { Enum1 = expected });
        }

    }
}
