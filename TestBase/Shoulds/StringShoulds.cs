using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class StringShoulds
    {
        public static string ShouldNotBeNullOrEmpty(this string @this, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.Not.Null, message, args);
            NUnit.Framework.Assert.That(@this, Is.Not.EqualTo(""), message, args);
            return @this;
        }

        public static string ShouldNotBeNullOrWhiteSpace(this string @this, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this,        Is.Not.Null, message, args);
            NUnit.Framework.Assert.That(@this.Trim(), Is.Not.Empty, message, args);
            return @this;
        }

        public static string ShouldEqualIgnoringCase(this string @this, string expected, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this.ToLower(), Is.EqualTo(expected.ToLower()), message, args);
            return @this;
        }

        public static string ShouldContain(this string @this, string expected, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.StringContaining(expected), message, args);
            return @this;
        }

        public static string ShouldBeContainedIn(this string @this, string expected, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(expected, Is.StringContaining(@this), message, args);
            return @this;
        }

        public static string ShouldMatch(this string @this, string pattern, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.StringMatching(pattern), message, args);
            return @this;
        }

        public static string ShouldMatchIgnoringCase(this string @this, string expectedRegexPattern, string message = null, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.StringMatching(expectedRegexPattern).IgnoreCase, message, args);
            return @this;
        }

        public static string ShouldMatch(this string @this, string expectedRegexPattern, RegexOptions regexOptions, string message = null, params object[] args)
        {
            var result = Regex.IsMatch(@this, expectedRegexPattern, regexOptions);
            NUnit.Framework.Assert.That(result, Is.True, message ?? string.Format("{0} didn't match Regex {1} with Options {2}", @this, expectedRegexPattern, regexOptions), args);
            return @this;
        }

        public static string ShouldNotContain(this string @this, string notExpected, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.Not.StringContaining(notExpected), message, args);
            return @this;
        }


        public static string ShouldStartWith(this string @this, string expected, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.StringStarting(expected), message, args);
            return @this;
        }

        public static string ShouldEndWith(this string @this, string expected, [Optional] string message, params object[] args)
        {
            NUnit.Framework.Assert.That(@this, Is.StringEnding(expected), message, args);
            return @this;
        }
    }
}