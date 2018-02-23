using System;
using System.Text.RegularExpressions;

namespace TestBase
{
    public static class StringShoulds
    {
        public static string ShouldMatch(this string @this, string expectedRegexPattern, RegexOptions regexOptions, string comment = null, params object[] args)
        {
            return Assert.That(@this, x=> Regex.IsMatch(x, expectedRegexPattern, regexOptions), comment??$"Should match Regex \"{expectedRegexPattern}\" with options {regexOptions}", args);
        }

        public static string ShouldMatch(this string @this, string pattern, string comment=null, params object[] args)
        {
            return ShouldMatch(@this, pattern, RegexOptions.None, comment ?? $"Should match Regex \"{pattern}\"", args);
        }

        public static string ShouldMatchIgnoringCase(this string @this, string pattern, string comment = null, params object[] args)
        {
            return ShouldMatch(@this, pattern, RegexOptions.IgnoreCase, comment?? $"Should match Regex \"{pattern}\" ignoring Case", args);
        }

        public static string ShouldEqualIgnoringCase(this string @this, string expected, string comment=null, params object[] args)
        {
            return Assert.That(@this, s => s.Equals(expected, StringComparison.CurrentCultureIgnoreCase), comment??$"Should Equal Ignoring Case \"{expected}\"", args );
        }

        public static string ShouldContain(this string @this, string expectedSubstring, string comment=null, params object[] args)
        {
            return Assert.That(@this, s => s.Contains(expectedSubstring), comment??$"Should contain substring \"{expectedSubstring}\"", args);
        }

        public static string ShouldBeContainedIn(this string @this, string expectedSuperstring, string comment=null, params object[] args)
        {
            return Assert.That(expectedSuperstring, s => s.Contains(@this), comment??$"Should be contained in superstring \"{expectedSuperstring}\"", args);
        }


        public static string ShouldNotContain(this string @this, string notExpected, string comment=null, params object[] args)
        {
            return Assert.That(@this, s => !s.Contains(notExpected), comment??$"Should not contain \"{notExpected}\"", args);
        }


        public static string ShouldStartWith(this string @this, string expected, string comment=null, params object[] args)
        {
            return Assert.That(@this, s => s.StartsWith(expected), comment??$"Should start with \"{expected}\"", args);
        }

        public static string ShouldEndWith(this string @this, string expected, string comment=null, params object[] args)
        {
            return Assert.That(@this, s => s.EndsWith(expected), comment??$"Should end with \"{expected}\"", args);
        }
    }
}