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

        [TestCase("CampaignName=AString", "AString")]
        [TestCase("Campaign.Name=AString", "AString")]
        public void Should_parse_string_with_CampaignName(string input, string expectedCampaignName)
        {
            ((SearchBox)input).ShouldEqualByValue(new SearchBox { CampaignName = expectedCampaignName });
        }

        [TestCase("ClientStatus=NotProceeding", ClientStatus.NotProceeding)]
        public void Should_parse_string_with_ClientState(string input, ClientStatus expected)
        {
            ((SearchBox)input).ShouldEqualByValue(new SearchBox { ClientStatus = expected });
        }

        [TestCase("CaseStatus=Pending", CaseStatus.Pending)]
        public void Should_parse_string_with_CaseState(string input, CaseStatus expected)
        {
            ((SearchBox)input).ShouldEqualByValue(new SearchBox { CaseStatus = expected });
        }

    }
}
