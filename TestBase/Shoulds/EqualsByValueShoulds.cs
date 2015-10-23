using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace TestBase.Shoulds
{
    public static class EqualsByValueShoulds
    {
        public static T ShouldEqualByValue<T>(this T @this, T expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, new EqualsByValueConstraint(expectedValue), message, args);
            return @this;
        }
        /// <summary>
        /// Assert equality-by-value by recursively iterating over all elements (if the actual & expected are Enumerable) 
        /// and all properties. Recursion stops at value types and at types (including string) which override Equals()
        /// <see cref="TestBase.Comparer.MemberCompare"/>
        /// </summary>
        /// <returns><param name="actual"></param></returns>
        /// <exception cref="NUnit.Framework.AssertionException">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValue<T>(this T actual, object expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(actual, new EqualsByValueConstraint(expectedValue), message, args);
            return actual;
        }

        /// <summary>
        /// Assert equality-by-value by recursively iterating over all elements (if the actual & expected are Enumerable) 
        /// and all properties. Recursion stops at value types and at types (including string) which override Equals()
        /// <see cref="TestBase.Comparer.MemberCompare"/>
        /// </summary>
        /// <param name="exclusions">a possibly empty list of field names to exclude for the purposes of this 
        /// comparison. To exclude fields of fields, provide the full dotted 'breadcrumb' to the property 
        /// to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"} 
        /// </param>
        /// <returns><param name="actual"></param></returns>
        /// <exception cref="NUnit.Framework.AssertionException">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValueExceptFor<T>(this T actual, object expectedValue, List<string> exclusions, [Optional] string message, params object[] args)
        {
            Assert.That(actual, new EqualsByValueExceptForConstraint(expectedValue, exclusions), message, args);
            return actual;
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