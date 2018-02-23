using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TestBase
{
    public static class EqualsByValueShoulds
    {
        public static T ShouldEqualByValue<T>(this T actual, T expectedValue, [Optional] string message, params object[] args)
        {
            var result = Comparer.EqualsByValueOrDiffers(actual, expectedValue);
            return Assert.That(actual, val => result, message??result.ToString() ,args);
        }
        /// <summary>
        /// Assert equality-by-value by recursively iterating over all elements (if the actual & expected are Enumerable) 
        /// and all properties. Recursion stops at value types and at types (including string) which override Equals()
        /// <see cref="TestBase.Comparer.MemberCompare"/>
        /// </summary>
        /// <returns><param name="this"></param></returns>
        /// <exception cref="NUnit.Framework.AssertionException">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValue<T>(this T actual, object expectedValue, [Optional] string message, params object[] args)
        {
            var result = Comparer.EqualsByValueOrDiffers(actual, expectedValue);
            return Assert.That(actual, val => result, message ?? result.ToString(), args);
        }

        /// <summary>
        /// Assert equality-by-value by recursively iterating over all elements (if the actual & expected are Enumerable) 
        /// and all properties. Recursion stops at value types and at types (including string) which override Equals()
        /// <see cref="TestBase.Comparer.MemberCompare"/>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expectedValue"></param>
        /// <param name="exclusions">a possibly empty list of field names to exclude for the purposes of this 
        /// comparison. To exclude fields of fields, provide the full dotted 'breadcrumb' to the property 
        /// to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"} 
        /// </param>
        /// <returns><param name="actual"></param></returns>
        public static T ShouldEqualByValueExceptFor<T>(this T actual, object expectedValue, IEnumerable<string> exclusions, [Optional] string message, params object[] args)
        {
            var result = Comparer.EqualsByValueOrDiffersExceptFor(actual, expectedValue, exclusions);
            return Assert.That(actual, val => result, message ?? result.ToString(), args);
        }

        public static IEnumerable<T>
            ShouldEqualByValueExceptForValues<T>(this IEnumerable<T> actual,
                                              IEnumerable<T> expected,
                                              IEnumerable<T> exceptions,
                                              string message = null,
                                              params object[] args)
        {
            actual.Where(exceptions.DoesNotContain).ShouldEqualByValue(expected.Where(exceptions.DoesNotContain), message, args);
            return actual;
        }
        public static IEnumerable<T> ShouldEqualByValueExceptForValuesIgnoringOrder<T>(this IEnumerable<T> actual,
                                              IEnumerable<T> expected,
                                              IEnumerable<T> exceptions,
                                              string message = null,
                                              params object[] args)
        {
            actual.Where(exceptions.DoesNotContain).OrderBy(x => x)
                .ShouldEqualByValue(
                    expected.Where(exceptions.DoesNotContain).OrderBy(x => x), message, args);
            return actual;
        }

        public static T PropertiesShouldMatch<T, Tother>(this T actual, Tother other, string comment = null, params object[] commentArgs)
        {
            Comparer.PropertiesMatch(actual, other).ShouldBeTrue("PropertiesShouldMatch");
            return actual;
        }

    }
}