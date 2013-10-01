using System.Runtime.InteropServices;
using NUnit.Framework;

namespace XSell.Tests.TestBase.Shoulds
{
    public static class EqualsByValueShoulds
    {
        public static T ShouldEqualByValue<T>(this T @this, T expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new EqualsByValueConstraint(expectedValue), message, args);
            return @this;
        }

        public static T ShouldEqualByValue<T>(this T @this, object expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new EqualsByValueConstraint(expectedValue), message, args);
            return @this;
        }
    }
}