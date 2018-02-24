using System.Text.RegularExpressions;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ShouldsCorrectnessAndVerbosityTests
{
    [TestFixture]
    public class StringShouldMatchRegexTests
    {
        [TestCase("input pattern", "patt")]
        [TestCase("input pattern", "p[a-z]t")]
        public void ShouldPass(string testInput, string testPattern)
        {
            testInput.ShouldMatch(testPattern);
        }

        [TestCase("input pattern", "boo")]
        [TestCase("input pattern", "p[A-Z]t")]
        public void ShouldFail(string testInput, string testPattern)
        {
            try
            {
                testInput.ShouldMatch(testPattern);
            }
            catch (Assertion)
            {
                return;
            }
            throw new Assertion( $"input {testInput} should not have matched {testPattern}");
        }

        [TestCase("input pattern", "PATT")]
        [TestCase("input pattern", "P[A-Z]T")]
        public void ShouldPassIgnoringCase(string testInput, string testPattern)
        {
            testInput.ShouldMatchIgnoringCase(testPattern);
        }

        [TestCase("input pattern", "PATT", RegexOptions.IgnoreCase)]
        [TestCase("line one\r\nline two", "^LINE two", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
        public void ShouldPassWithOptions(string testInput, string testPattern, RegexOptions options)
        {
            testInput.ShouldMatch(testPattern,options);
        }

    }
}
