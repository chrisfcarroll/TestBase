using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase
{
    static public class StringExtensions
    {
        public static string WithWhiteSpaceRemoved(this string @this)
        {
            return @this.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
        }

        public static bool Matches(this string @this, string pattern, RegexOptions options = RegexOptions.None)
        {
            return Regex.IsMatch(@this, pattern, options);
        }
    }
}