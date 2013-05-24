using System.Runtime.InteropServices;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class StringShoulds
    {
        public static string ShouldNotBeNullOrEmpty(this string @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Not.Null, message, args);
            Assert.That(@this, Is.Not.EqualTo(""), message, args);
            return @this;
        }

        public static string ShouldEqualIgnoringCase(this string @this, string expected, [Optional] string message, params object[] args)
        {
            Assert.That(@this.ToLower(), Is.EqualTo(expected.ToLower()), message, args);
            return @this;
        }

        public static string ShouldContain(this string @this, string expected, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.StringContaining(expected), message, args);
            return @this;
        }

        public static string ShouldBeContainedIn(this string @this, string expected, [Optional] string message, params object[] args)
        {
            Assert.That(expected, Is.StringContaining(@this), message, args);
            return @this;
        }

        public static string ShouldMatch(this string @this, string pattern, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.StringMatching(pattern), message, args);
            return @this;
        }

        public static string ShouldNotContain(this string @this, string expected, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Not.StringContaining(expected), message, args);
            return @this;
        }


        public static string ShouldStartWith(this string @this, string expected, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.StringStarting(expected), message, args);
            return @this;
        }

        public static string ShouldEndWith(this string @this, string expected, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.StringEnding(expected), message, args);
            return @this;
        }
    }
}