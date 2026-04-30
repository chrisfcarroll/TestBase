using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace TestBase
{
    public static class StringShoulds
    {
        static readonly string nl = Environment.NewLine;

        #if NET6_OR_GREATER
        [StackTraceHidden]
        #else
        [DebuggerHidden]
        #endif
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
                false).ForActiveTestRunner();
        }

        /// <summary>Asserts string equality with detailed diff output showing the point of difference.</summary>
        public static string ShouldBe(this string @this, string expected, string message = null, params object[] args)
        {
            if (@this == null && expected == null) return null;
            if (@this != null && @this.Equals(expected)) return @this;
            var detail = FormatStringEqualityDiff(expected, @this);
            ThrowStringAssertion(@this, nameof(ShouldBe), detail, message, args);
            return @this;
        }

        static string FormatStringEqualityDiff(string expected, string actual)
        {
            if (expected == null)
                return $"Expected: null{nl}Actual:   \"{actual}\"";
            if (actual == null)
                return $"Expected: \"{expected}\"{nl}Actual:   null";

            int diffIndex = FindFirstDifference(expected, actual);

            bool isMultiLine = expected.IndexOf('\n') >= 0 || actual.IndexOf('\n') >= 0;
            if (isMultiLine)
                return FormatMultiLineDiff(expected, actual, diffIndex);

            return FormatSingleLineDiff(expected, actual, diffIndex);
        }

        static string FormatSingleLineDiff(string expected, string actual, int diffIndex)
        {
            var sb = new StringBuilder();

            if (expected.Length != actual.Length)
                sb.Append($"Expected string length {expected.Length} but was {actual.Length}. ");
            else
                sb.Append($"String lengths are both {expected.Length}. ");
            sb.Append($"Strings differ at index {diffIndex}.");
            sb.Append(nl);

            const int windowSize = 60;
            bool needsLeftTruncation = diffIndex > windowSize / 2;
            int showStart = needsLeftTruncation ? diffIndex - windowSize / 2 : 0;

            string expectedSnippet = Snippet(expected, showStart, windowSize);
            string actualSnippet = Snippet(actual, showStart, windowSize);

            string prefix = needsLeftTruncation ? "\"..." : "\"";
            string expectedSuffix = showStart + windowSize < expected.Length ? "...\"" : "\"";
            string actualSuffix = showStart + windowSize < actual.Length ? "...\"" : "\"";

            sb.Append($"Expected: {prefix}{Escape(expectedSnippet)}{expectedSuffix}");
            sb.Append(nl);
            sb.Append($"Actual:   {prefix}{Escape(actualSnippet)}{actualSuffix}");
            sb.Append(nl);

            int caretOffset = "Actual:   ".Length + prefix.Length + (diffIndex - showStart);
            sb.Append(new string('-', caretOffset));
            sb.Append('^');

            return sb.ToString();
        }

        static string FormatMultiLineDiff(string expected, string actual, int diffIndex)
        {
            var expectedLines = expected.Split('\n');
            var actualLines = actual.Split('\n');

            int line = 0;
            int charCount = 0;
            for (int i = 0; i < actualLines.Length; i++)
            {
                if (charCount + actualLines[i].Length + 1 > diffIndex)
                {
                    line = i;
                    break;
                }
                charCount += actualLines[i].Length + 1;
            }

            int col = diffIndex - charCount;
            int lineNumber = line + 1;

            string expectedLineText = line < expectedLines.Length ? expectedLines[line].TrimEnd('\r') : "(no line)";
            string actualLineText = line < actualLines.Length ? actualLines[line].TrimEnd('\r') : "(no line)";

            const int maxLineDisplay = 60;
            string expectedSnippet = expectedLineText.Length <= maxLineDisplay
                ? expectedLineText : SnippetAround(expectedLineText, col, maxLineDisplay / 2);
            string actualSnippet = actualLineText.Length <= maxLineDisplay
                ? actualLineText : SnippetAround(actualLineText, col, maxLineDisplay / 2);

            var sb = new StringBuilder();
            if (expectedLines.Length != actualLines.Length)
                sb.Append($"Expected {expectedLines.Length} lines but was {actualLines.Length} lines. ");
            sb.Append($"First difference at Line {lineNumber}, index {col}.");
            sb.Append(nl);
            sb.Append($"Expected: Line {lineNumber}, index {col}: \"{Escape(expectedSnippet)}\"");
            sb.Append(nl);
            sb.Append($"Actual:   Line {lineNumber}, index {col}: \"{Escape(actualSnippet)}\"");

            return sb.ToString();
        }

        static int FindFirstDifference(string expected, string actual)
        {
            int minLen = Math.Min(expected.Length, actual.Length);
            for (int i = 0; i < minLen; i++)
                if (expected[i] != actual[i]) return i;
            return minLen;
        }

        static string Snippet(string s, int start, int maxLen)
        {
            if (start >= s.Length) return "";
            int len = Math.Min(maxLen, s.Length - start);
            return s.Substring(start, len);
        }

        static string SnippetAround(string s, int center, int radius)
        {
            int start = Math.Max(0, center - radius);
            int end = Math.Min(s.Length, center + radius);
            return s.Substring(start, end - start);
        }

        static string Escape(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
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
