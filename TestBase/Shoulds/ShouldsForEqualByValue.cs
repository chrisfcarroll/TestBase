using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class ShouldsForEqualByValue
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

        public static T ShouldEqualByValueExceptFor<T>(this T @this, object expectedValue, List<string> exclusions, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new EqualsByValueExceptForConstraint(expectedValue, exclusions), message, args);
            return @this;
        }

        public static IEnumerable<T>
            ShouldEqualByValueExceptForValues<T>(this IEnumerable<T> @this,
                                              IEnumerable<T> expected,
                                              IEnumerable<T> exceptions,
                                              string message = null,
                                              params object[] args)
        {
            @this.Where(exceptions.DoesNotContain).ShouldEqualByValue(expected.Where(exceptions.DoesNotContain), message, args);
            return @this;
        }
        public static IEnumerable<T>
            ShouldEqualByValueExpectForValuesIgnoringOrder<T>(this IEnumerable<T> @this,
                                              IEnumerable<T> expected,
                                              IEnumerable<T> exceptions,
                                              string message = null,
                                              params object[] args)
        {
            @this.Where(exceptions.DoesNotContain).OrderBy(x => x)
                .ShouldEqualByValue(
                    expected.Where(exceptions.DoesNotContain).OrderBy(x => x), message, args);
            return @this;
        }
    }
}