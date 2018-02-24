using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TestBase.Shoulds
{
    public static class StringShoulds
    {
        public static string ShouldNotBeNullOrEmpty(this string @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.NotNull, message, args);
            @this.ShouldNotBe("", message ?? $"{nameof(ShouldNotBeNullOrEmpty)}", args);
            return @this;
        }

        public static string ShouldNotBeNullOrEmptyOrWhiteSpace(this string @this, string message=null, params object[] args)
        {
            @this.ShouldNotBeNull(message, args);
            @this.Trim().ShouldNotBeNullOrEmpty(message, args);
            return @this;
        }

        public static string ShouldEqualIgnoringCase(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this.ToLower(), Is.EqualTo(expected.ToLower()), message, args);
            return @this;
        }

        public static string ShouldContain(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.Contains(expected), message, args);
            return @this;
        }

        public static string ShouldBeContainedIn(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this,  x=> expected.Contains(x), message, args);
            return @this;
        }

        public static string ShouldMatch(this string @this, string pattern, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.Matches(pattern,RegexOptions.None), message, args);
            return @this;
        }

        public static string ShouldMatchIgnoringCase(this string @this, string expectedRegexPattern, string message = null, params object[] args)
        {
            Assert.That(@this, x=>x.Matches(expectedRegexPattern,RegexOptions.IgnoreCase), message, args);
            return @this;
        }

        public static string ShouldMatch(this string @this, string expectedRegexPattern, RegexOptions regexOptions, string message = null, params object[] args)
        {
            Assert.That(@this, x=>Regex.IsMatch(@this, expectedRegexPattern, regexOptions), message ?? string.Format("{0} didn't match Regex {1} with Options {2}", @this, expectedRegexPattern, regexOptions), args);
            return @this;
        }

        public static string ShouldNotContain(this string @this, string notExpected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>!x.Contains(notExpected), message, args);
            return @this;
        }


        public static string ShouldStartWith(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.StartsWith(expected), message, args);
            return @this;
        }

        public static string ShouldEndWith(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.EndsWith(expected), message, args);
            return @this;
        }
    }
}