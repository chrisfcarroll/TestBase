using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestBase
{
    public static class StringExtensions
    {
        public static bool HasTheSameWordsAs(this string left,
                                             string right,
                                             StringComparison stringComparison=StringComparison.CurrentCultureIgnoreCase)
        {
            var l = left?.RegexReplaceWhitespace().ReplaceRegex("[\"']","");
            var r = right?.RegexReplaceWhitespace().ReplaceRegex("[\"']","");
            if (l == null && r == null) return true;
            if (l == null & r != null) return false;
            return l.Equals(r, stringComparison);
        }
        public static string RegexReplaceWhitespaceAndBlankOutGuids(this string value) 
            => RegexReplaceWhitespace(value)
                ?.ReplaceRegex("[a-f0-9A-F\\-]{36}",Guid.Empty.ToString());

        public static string RegexReplaceWhitespace(this string value)
            => value?
                .ReplaceRegex("\\s+"," ")
                .ReplaceRegex("^\\s+","")
                .ReplaceRegex("\\s+$","");

        public static string ReplaceRegex(this string input,
                                          string pattern,
                                          string replacement,
                                          RegexOptions options = RegexOptions.None)
            => input is null
                ? null
                : Regex.Replace(input, pattern, replacement, options);
        public static string TruncateTo(this string @this, int maxLength = 99)
        {
            return @this?.Length <= maxLength ? @this : @this?.Substring(0, maxLength);
        }

        public static string WithWhiteSpaceRemoved(this string @this)
        {
            return @this?.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
        }

        public static string ReplaceWith(this string @this, string newValue, params string[] oldValues)
        {
            return oldValues.Aggregate(@this, (s, oldValue) => s.Replace(oldValue, newValue));
        }

        public static bool Matches(this string @this, string pattern, RegexOptions options = RegexOptions.None)
        {
            return Regex.IsMatch(@this, pattern, options);
        }

        public static string ToBase64(this string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string FromBase64(this string input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }
    }

    public static class HenriFormatter
    {
        static string OutExpression(object source, string expression)
        {
            var format = "";

            var colonIndex = expression.IndexOf(':');
            if (colonIndex > 0)
            {
                format     = expression.Substring(colonIndex + 1);
                expression = expression.Substring(0, colonIndex);
            }

            if (string.IsNullOrEmpty(format))
                return source.ToJQueryable().SelectToken(expression).ToString();
            else if (format.Length > 1 && format[1] == '%')
                switch (format[0])
                {
                    case 'i':
                        return source.ToJQueryable()
                                     .SelectToken(expression)
                                     .ToObject<int>()
                                     .ToString(format.Substring(2));
                    case 'd':
                        return source.ToJQueryable()
                                     .SelectToken(expression)
                                     .ToObject<decimal>()
                                     .ToString(format.Substring(2));
                    case 'n':
                        return source.ToJQueryable()
                                     .SelectToken(expression)
                                     .ToObject<double>()
                                     .ToString(format.Substring(2));
                    case 't':
                        return source.ToJQueryable()
                                     .SelectToken(expression)
                                     .ToObject<DateTime>()
                                     .ToString(format.Substring(2));
                }
            return source.ToJQueryable().SelectToken(expression).ToString();
            //return DataBinder.Eval(source, expression, "{0:" + format + "}") ?? "";
        }

        /// <summary>
        ///     String formatting based on names not numbers.
        ///     Example: "{name} {phone}".Formatz(new{name,phone});
        /// </summary>
        /// <param name="format">The string containing format names</param>
        /// <param name="source">The object with named properties</param>
        /// <returns></returns>
        public static string Formatz(this string format, object source)
        {
            if (format == null) throw new ArgumentNullException("format");

            var result = new StringBuilder(format.Length * 2);

            using (var reader = new StringReader(format))
            {
                var expression = new StringBuilder();
                var @char      = -1;

                var state = State.OutsideExpression;
                do
                {
                    switch (state)
                    {
                        case State.OutsideExpression:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case -1:
                                    state = State.End;
                                    break;
                                case '{':
                                    state = State.OnOpenBracket;
                                    break;
                                case '}':
                                    state = State.OnCloseBracket;
                                    break;
                                default:
                                    result.Append((char) @char);
                                    break;
                            }

                            break;
                        case State.OnOpenBracket:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case -1:
                                    throw new FormatException();
                                case '{':
                                    result.Append('{');
                                    state = State.OutsideExpression;
                                    break;
                                default:
                                    expression.Append((char) @char);
                                    state = State.InsideExpression;
                                    break;
                            }

                            break;
                        case State.InsideExpression:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case -1:
                                    throw new FormatException();
                                case '}':
                                    result.Append(OutExpression(source, expression.ToString()));
                                    expression.Length = 0;
                                    state             = State.OutsideExpression;
                                    break;
                                default:
                                    expression.Append((char) @char);
                                    break;
                            }

                            break;
                        case State.OnCloseBracket:
                            @char = reader.Read();
                            switch (@char)
                            {
                                case '}':
                                    result.Append('}');
                                    state = State.OutsideExpression;
                                    break;
                                default:
                                    throw new FormatException();
                            }

                            break;
                        default:
                            throw new InvalidOperationException("Invalid state.");
                    }
                } while (state != State.End);
            }

            return result.ToString();
        }

        enum State
        {
            OutsideExpression,
            OnOpenBracket,
            InsideExpression,
            OnCloseBracket,
            End
        }
    }
}
