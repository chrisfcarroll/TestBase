using System.Text.RegularExpressions;

namespace TestBase
{
    public static class StringShoulds
    {
        public static string ShouldNotBeNullOrEmpty(this string @this, string message=null, params object[] args)
        {
            @this.ShouldNotBeNull(message, args);
            @this.ShouldNotBe(string.Empty, message ?? nameof(ShouldNotBeNullOrEmpty), args);
            return @this;
        }

        public static string ShouldNotBeNullOrEmptyOrWhiteSpace(this string @this, string message=null, params object[] args)
        {
            @this.ShouldNotBeNull(message, args);
            @this.Trim().ShouldNotBeNullOrEmpty(message?? nameof(ShouldNotBeNullOrEmptyOrWhiteSpace), args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this"/>.ToLower() equals <paramref name="expected"/>.ToLower()</summary>
        /// <returns>@this</returns>
        public static string ShouldEqualIgnoringCase(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this.ToLower(), Is.EqualTo(expected.ToLower()), message ??$"{nameof(ShouldEqualIgnoringCase)} {expected}", args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this"/> .Contains(<paramref name="expected"/>)</summary>
        /// <returns>@this</returns>
        public static string ShouldContain(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.Contains(expected), message??$"{nameof(ShouldContain)} {expected}", args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="expected"/>.Contains(<paramref name="@this"/>)</summary>
        /// <returns>@this</returns>
        public static string ShouldBeContainedIn(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this,  x=> expected.Contains(x), message ??$"{nameof(ShouldBeContainedIn)} {expected}", args);
            return @this;
        }

        /// <summary>Asserts that the <see cref="Regex"/> from <paramref name="expectedRegexPattern"/> matches <paramref name="@this"/></summary>
        /// <returns>@this</returns>
        public static string ShouldMatch(this string @this, string expectedRegexPattern, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.Matches(expectedRegexPattern,RegexOptions.None), message ?? $"{nameof(ShouldMatch)} /{expectedRegexPattern}/", args);
            return @this;
        }

        /// <summary>Asserts that the <see cref="Regex"/> from <paramref name="expectedRegexPattern"/> with <paramref name="regexOptions"/> matches <paramref name="@this"/></summary>
        /// <returns>@this</returns>
        public static string ShouldMatch(this string @this, string expectedRegexPattern, RegexOptions regexOptions, string message = null, params object[] args)
        {
            Assert.That(@this, x=>Regex.IsMatch(@this, expectedRegexPattern, regexOptions), message ?? $"{nameof(ShouldMatch)} /{expectedRegexPattern}/{regexOptions}", args);
            return @this;
        }

        /// <summary>Asserts that the <see cref="Regex"/> from <paramref name="expectedRegexPattern"/> matches <paramref name="@this"/> given <seealso cref="RegexOptions.IgnoreCase"/> </summary>
        /// <returns>@this</returns>
        public static string ShouldMatchIgnoringCase(this string @this, string expectedRegexPattern, string message = null, params object[] args)
        {
            Assert.That(@this, x=>x.Matches(expectedRegexPattern,RegexOptions.IgnoreCase), message ?? $"{nameof(ShouldMatchIgnoringCase)} /{expectedRegexPattern}/", args);
            return @this;
        }

        /// <summary>Asserts that the <see cref="Regex"/> from <paramref name="unexpectedRegexPattern"/> does not match <paramref name="@this"/></summary>
        /// <returns>@this</returns>
        public static string ShouldNotMatch(this string @this, string unexpectedRegexPattern, string message=null, params object[] args)
        {
            Assert.That(@this, x=>!x.Matches(unexpectedRegexPattern,RegexOptions.None), message ?? $"{nameof(ShouldNotMatch)} /{unexpectedRegexPattern}/", args);
            return @this;
        }

        /// <summary>Asserts that the <see cref="Regex"/> from <paramref name="unexpectedRegexPattern"/> with <paramref name="regexOptions"/> does not match <paramref name="@this"/></summary>
        /// <returns>@this</returns>
        public static string ShouldNotMatch(this string @this, string unexpectedRegexPattern, RegexOptions regexOptions, string message = null, params object[] args)
        {
            Assert.That(@this, x=>!Regex.IsMatch(@this, unexpectedRegexPattern, regexOptions), message ?? $"{nameof(ShouldNotMatch)} /{unexpectedRegexPattern}/{regexOptions}", args);
            return @this;
        }

        /// <summary>Asserts that the <see cref="Regex"/> from <paramref name="unexpectedRegexPattern"/> does not match <paramref name="@this"/> given <seealso cref="RegexOptions.IgnoreCase"/></summary>
        /// <returns>@this</returns>
        public static string ShouldNotMatchIgnoringCase(this string @this, string unexpectedRegexPattern, string message = null, params object[] args)
        {
            Assert.That(@this, x=>!x.Matches(unexpectedRegexPattern,RegexOptions.IgnoreCase), message ?? $"{nameof(ShouldNotMatchIgnoringCase)} /{unexpectedRegexPattern}/", args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this"/> .Contains(<paramref name="expected"/>) is false.</summary>
        /// <returns>@this</returns>
        public static string ShouldNotContain(this string @this, string notExpected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>!x.Contains(notExpected), message??$"{nameof(ShouldNotContain)} {notExpected}", args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this"/>.StartsWith(<paramref name="expected"/>).</summary>
        /// <returns>@this</returns>
        public static string ShouldStartWith(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.StartsWith(expected), message??$"{nameof(ShouldStartWith)} {expected}", args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this"/> .EndsWith(<paramref name="expected"/>).</summary>
        /// <returns>@this</returns>
        public static string ShouldEndWith(this string @this, string expected, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.EndsWith(expected), message, args);
            return @this;
        }
    }
}