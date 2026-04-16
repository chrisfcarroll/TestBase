using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace TestBase
{
    public static class StringShoulds
    {
        static void ThrowStringAssertion(string actual, string assertionName, string assertedDetail, string message, object[] args,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
            throw new Assertion<string>(
                actual ?? "null",
                actualExpression,
                assertionName,
                assertedDetail,
                comment,
                false);
        }

        public static string ShouldNotBeNullOrEmpty(this string @this, string message = null, params object[] args)
        {
            if (@this == null)
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmpty), "Expected: not null or empty, Actual: null", message, args);
            if (@this.Length == 0)
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmpty), "Expected: not null or empty, Actual: \"\"", message, args);
            return @this;
        }

#if NET6_0_OR_GREATER
        public static string ShouldNotBeNullOrEmpty(this string @this, [CallerArgumentExpression("@this")] string actualExpression = null)
        {
            if (@this == null)
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmpty), "Expected: not null or empty, Actual: null", null, null, actualExpression);
            if (@this.Length == 0)
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmpty), "Expected: not null or empty, Actual: \"\"", null, null, actualExpression);
            return @this;
        }
#endif

        public static string ShouldNotBeNullOrEmptyOrWhiteSpace(
            this string     @this,
            string          message = null,
            params object[] args)
        {
            if (@this == null)
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmptyOrWhiteSpace), "Expected: not null/empty/whitespace, Actual: null", message, args);
            if (string.IsNullOrWhiteSpace(@this))
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmptyOrWhiteSpace), $"Expected: not null/empty/whitespace, Actual: \"{@this}\"", message, args);
            return @this;
        }

#if NET6_0_OR_GREATER
        public static string ShouldNotBeNullOrEmptyOrWhiteSpace(this string @this, [CallerArgumentExpression("@this")] string actualExpression = null)
        {
            if (@this == null)
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmptyOrWhiteSpace), "Expected: not null/empty/whitespace, Actual: null", null, null, actualExpression);
            if (string.IsNullOrWhiteSpace(@this))
                ThrowStringAssertion(@this, nameof(ShouldNotBeNullOrEmptyOrWhiteSpace), $"Expected: not null/empty/whitespace, Actual: \"{@this}\"", null, null, actualExpression);
            return @this;
        }
#endif

        /// <summary>Asserts that <paramref name="@this" />.ToLower() equals <paramref name="expected" />.ToLower()</summary>
        /// <returns>@this</returns>
        public static string ShouldEqualIgnoringCase(
            this string     @this,
            string          expected,
            string          message = null,
            params object[] args)
        {
            if (!@this.ToLower().Equals(expected.ToLower()))
                ThrowStringAssertion(@this, nameof(ShouldEqualIgnoringCase),
                    $"Expected: \"{expected}\" (ignoring case), Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this" /> .Contains(<paramref name="expected" />)</summary>
        /// <returns>@this</returns>
        public static string ShouldContain(
            this string     @this,
            string          expected,
            string          message = null,
            params object[] args)
        {
            if (!@this.Contains(expected))
                ThrowStringAssertion(@this, nameof(ShouldContain),
                    $"Expected: String containing \"{expected}\", Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this" /> .Contains(<paramref name="substrings" />)</summary>
        /// <returns>@this</returns>
        public static string ShouldContainEachOf(
            this string         @this,
            IEnumerable<string> substrings,
            string              message = null,
            params object[]     args)
        {
            foreach (var substring in substrings)
                if (!@this.Contains(substring))
                    ThrowStringAssertion(@this, nameof(ShouldContainEachOf),
                        $"Expected: String containing \"{substring}\", Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this" /> .Contains(<paramref name="substrings" />)</summary>
        /// <returns>@this</returns>
        public static string ShouldContainEachOf(this string @this, params string[] substrings)
        {
            return ShouldContainEachOf(@this, (IEnumerable<string>) substrings);
        }

        /// <summary>Asserts that <paramref name="expected" />.Contains(<paramref name="@this" />)</summary>
        /// <returns>@this</returns>
        public static string ShouldBeContainedIn(
            this string     @this,
            string          expected,
            string          message = null,
            params object[] args)
        {
            if (!expected.Contains(@this))
                ThrowStringAssertion(@this, nameof(ShouldBeContainedIn),
                    $"Expected: \"{@this}\" contained in \"{expected}\", but was not", message, args);
            return @this;
        }

        /// <summary>
        ///     Asserts that the <see cref="Regex" /> from <paramref name="expectedRegexPattern" /> matches
        ///     <paramref name="@this" />
        /// </summary>
        /// <returns>@this</returns>
        public static string ShouldMatch(
            this string     @this,
            string          expectedRegexPattern,
            string          message = null,
            params object[] args)
        {
            if (!@this.Matches(expectedRegexPattern, RegexOptions.None))
                ThrowStringAssertion(@this, nameof(ShouldMatch),
                    $"Expected: match /{expectedRegexPattern}/, Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>
        ///     Asserts that the <see cref="Regex" /> from <paramref name="expectedRegexPattern" /> with
        ///     <paramref name="regexOptions" /> matches <paramref name="@this" />
        /// </summary>
        /// <returns>@this</returns>
        public static string ShouldMatch(
            this string     @this,
            string          expectedRegexPattern,
            RegexOptions    regexOptions,
            string          message = null,
            params object[] args)
        {
            if (!Regex.IsMatch(@this, expectedRegexPattern, regexOptions))
                ThrowStringAssertion(@this, nameof(ShouldMatch),
                    $"Expected: match /{expectedRegexPattern}/{regexOptions}, Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>
        ///     Asserts that the <see cref="Regex" /> from <paramref name="expectedRegexPattern" /> matches
        ///     <paramref name="@this" /> given <seealso cref="RegexOptions.IgnoreCase" />
        /// </summary>
        /// <returns>@this</returns>
        public static string ShouldMatchIgnoringCase(
            this string     @this,
            string          expectedRegexPattern,
            string          message = null,
            params object[] args)
        {
            if (!@this.Matches(expectedRegexPattern, RegexOptions.IgnoreCase))
                ThrowStringAssertion(@this, nameof(ShouldMatchIgnoringCase),
                    $"Expected: match /{expectedRegexPattern}/ (ignoring case), Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>
        ///     Asserts that the <see cref="Regex" /> from <paramref name="unexpectedRegexPattern" /> does not match
        ///     <paramref name="@this" />
        /// </summary>
        /// <returns>@this</returns>
        public static string ShouldNotMatch(
            this string     @this,
            string          unexpectedRegexPattern,
            string          message = null,
            params object[] args)
        {
            if (@this.Matches(unexpectedRegexPattern, RegexOptions.None))
                ThrowStringAssertion(@this, nameof(ShouldNotMatch),
                    $"Expected: not match /{unexpectedRegexPattern}/, Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>
        ///     Asserts that the <see cref="Regex" /> from <paramref name="unexpectedRegexPattern" /> with
        ///     <paramref name="regexOptions" /> does not match <paramref name="@this" />
        /// </summary>
        /// <returns>@this</returns>
        public static string ShouldNotMatch(
            this string     @this,
            string          unexpectedRegexPattern,
            RegexOptions    regexOptions,
            string          message = null,
            params object[] args)
        {
            if (Regex.IsMatch(@this, unexpectedRegexPattern, regexOptions))
                ThrowStringAssertion(@this, nameof(ShouldNotMatch),
                    $"Expected: not match /{unexpectedRegexPattern}/{regexOptions}, Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>
        ///     Asserts that the <see cref="Regex" /> from <paramref name="unexpectedRegexPattern" /> does not match
        ///     <paramref name="@this" /> given <seealso cref="RegexOptions.IgnoreCase" />
        /// </summary>
        /// <returns>@this</returns>
        public static string ShouldNotMatchIgnoringCase(
            this string     @this,
            string          unexpectedRegexPattern,
            string          message = null,
            params object[] args)
        {
            if (@this.Matches(unexpectedRegexPattern, RegexOptions.IgnoreCase))
                ThrowStringAssertion(@this, nameof(ShouldNotMatchIgnoringCase),
                    $"Expected: not match /{unexpectedRegexPattern}/ (ignoring case), Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this" /> .Contains(<paramref name="notExpected" />) is false.</summary>
        /// <returns>@this</returns>
        public static string ShouldNotContain(
            this string     @this,
            string          notExpected,
            string          message = null,
            params object[] args)
        {
            if (@this.Contains(notExpected))
                ThrowStringAssertion(@this, nameof(ShouldNotContain),
                    $"Expected: not containing \"{notExpected}\", Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this" />.StartsWith(<paramref name="expected" />).</summary>
        /// <returns>@this</returns>
        public static string ShouldStartWith(
            this string     @this,
            string          expected,
            string          message = null,
            params object[] args)
        {
            if (!@this.StartsWith(expected))
                ThrowStringAssertion(@this, nameof(ShouldStartWith),
                    $"Expected: starts with \"{expected}\", Actual: \"{@this}\"", message, args);
            return @this;
        }

        /// <summary>Asserts that <paramref name="@this" /> .EndsWith(<paramref name="expected" />).</summary>
        /// <returns>@this</returns>
        public static string ShouldEndWith(
            this string     @this,
            string          expected,
            string          message = null,
            params object[] args)
        {
            if (!@this.EndsWith(expected))
                ThrowStringAssertion(@this, nameof(ShouldEndWith),
                    $"Expected: ends with \"{expected}\", Actual: \"{@this}\"", message, args);
            return @this;
        }
    }
}
