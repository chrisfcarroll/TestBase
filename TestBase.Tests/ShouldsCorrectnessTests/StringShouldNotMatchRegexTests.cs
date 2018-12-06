using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests
{
    [TestFixture]
    public class StringShouldNotMatchRegexTests
    {
        [TestCase("input pattern", "boo")]
        [TestCase("input pattern", "p[A-Z]t")]
        [TestCase("input pattern", "PATT")]
        [TestCase("input pattern", "P[A-Z]T")]
        public void ShouldPass(string testInput, string testPattern) { testInput.ShouldNotMatch(testPattern); }

        [TestCase("input pattern", "patt")]
        [TestCase("input pattern", "p[a-z]t")]
        public void ShouldFail(string testInput, string testPattern)
        {
            try { testInput.ShouldNotMatch(testPattern); } catch (Assertion) { return; }

            throw new Assertion($"input {testInput} should not have matched {testPattern}");
        }

        [TestCase("input pattern", "PATT")]
        [TestCase("input pattern", "P[A-Z]T")]
        public void ShouldFailIgnoringCase(string testInput, string testPattern)
        {
            try { testInput.ShouldNotMatchIgnoringCase(testPattern); } catch (Assertion) { return; }

            throw new Assertion($"input {testInput} should have failed-to-not-match {testPattern} ignoring case");
        }
    }
}
