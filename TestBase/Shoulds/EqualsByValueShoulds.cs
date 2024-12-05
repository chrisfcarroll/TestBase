using System.Collections.Generic;
using System.Linq;

namespace TestBase
{
    /// <summary>
    /// Extension methods, typically on <see cref="object"/>, <see cref="IEnumerable{T}"/>, and
    /// unconstrained type parameters, for asserting X.ShouldEqualByValue(Y) style fluent predicates.
    /// Typically, the underlying test is that <see cref="Comparer.MemberCompare"/> returns truthy.
    /// </summary>
    public static class EqualsByValueShoulds
    {
        /// <summary>
        ///     Assert equality-by-value by recursively iterating over all elements (if the actual &amp; expected are Enumerable)
        ///     and all properties. Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <returns>
        ///     <param name="@this"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValue<T>(
            this T          @this,
            T               expectedValue,
            string          message = null,
            params object[] args)
        {
            Assert.That(@this, x => x.EqualsByValue(expectedValue), message, args);
            return @this;
        }

        /// <summary>
        ///     Assert equality-by-value by recursively iterating over all elements (if the actual &amp; expected are Enumerable)
        ///     and all properties. Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <returns>
        ///     <param name="actual"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValue<T>(
            this T          actual,
            object          expectedValue,
            string          message = null,
            params object[] args)
        {
            Assert.That(actual, x => x.EqualsByValue(expectedValue), message, args);
            return actual;
        }

        /// <summary>
        ///     Assert equality-by-value by recursively iterating over all elements (if the actual &amp; expected are Enumerable)
        ///     and then over all properties specified by <paramref name="propertiesToCompare" />.
        ///     Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <param name="expectedValue"></param>
        /// <param name="propertiesToCompare">a list of property names to restrict the comparison to just those properties.</param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <returns>
        ///     <param name="actual"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValueOnMembers<T>(
            this T              actual,
            object              expectedValue,
            IEnumerable<string> propertiesToCompare,
            string              message = null,
            params object[]     args)
        {
            Assert.That(actual,
                        x => x.EqualsByValuesJustOnMembersNamed(expectedValue, propertiesToCompare),
                        message,
                        args);
            return actual;
        }

        /// <summary>
        ///     Assert equality-by-value by recursively iterating over all elements (if the actual &amp; expected are Enumerable)
        ///     and then over all properties specified by <paramref name="propertiesToCompare" />.
        ///     Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <param name="expectedValue"></param>
        /// <param name="propertiesToCompare">a list of property names to restrict the comparison.</param>
        /// <param name="actual"></param>
        /// <returns>
        ///     <param name="actual"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValueOnProperties<T>(
            this T          actual,
            object          expectedValue,
            params string[] propertiesToCompare)
        {
            Assert.That(actual, x => x.EqualsByValuesJustOnMembersNamed(expectedValue, propertiesToCompare));
            return actual;
        }

        /// <summary>
        ///     Assert equality-by-value by recursively iterating over all elements (if the actual &amp; expected are Enumerable)
        ///     and all properties. Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <param name="expectedValue"></param>
        /// <param name="exclusions">
        ///     a possibly empty list of field names to exclude for the purposes of this
        ///     comparison. To exclude fields of fields, provide the full dotted 'breadcrumb' to the property
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <returns>
        ///     <param name="actual"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValueExceptFor<T>(
            this T              actual,
            object              expectedValue,
            IEnumerable<string> exclusions,
            string              message = null,
            params object[]     args)
        {
            Assert.That(actual, x => x.EqualsByValueExceptFor(expectedValue, exclusions), message, args);
            return actual;
        }

        /// <summary>
        ///     Synonym for
        ///     <see cref="ShouldEqualByValueExceptFor{T}(T,object,System.Collections.Generic.IEnumerable{string},string,object[])" />
        ///     .
        ///     Assert equality-by-value by recursively iterating over all elements (if the actual &amp; expected are Enumerable)
        ///     and all properties. Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <param name="expectedValue"></param>
        /// <param name="exclusions">
        ///     a possibly empty list of field names to exclude for the purposes of this
        ///     comparison. To exclude fields of fields, provide the full dotted 'breadcrumb' to the property
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="actual"></param>
        /// <returns>
        ///     <param name="actual"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static T ShouldEqualByValueExceptFor<T>(this T actual, object expectedValue, params string[] exclusions)
        {
            Assert.That(actual, x => x.EqualsByValueExceptFor(expectedValue, exclusions));
            return actual;
        }

        /// <summary>
        ///     Assert equality-by-value by recursively iterating over all elements (if the actual &amp; expected are Enumerable)
        ///     and all properties. Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <param name="exclusions">
        ///     a possibly empty list of field names to exclude for the purposes of this
        ///     comparison. To exclude fields of fields, provide the full dotted 'breadcrumb' to the property
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="this"></param>
        /// <param name="expected"></param>
        /// <param name="exceptions"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns>
        ///     <param name="@this"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static IEnumerable<T>
        ShouldEqualByValueExceptForValues<T>(
            this IEnumerable<T> @this,
            IEnumerable<T>      expected,
            IEnumerable<T>      exceptions,
            string              message = null,
            params object[]     args)
        {
            if (expected == null && @this == null) return @this;
            expected   = expected   ?? new T[0];
            exceptions = exceptions ?? new T[0];
            ShouldEqualByValue(@this.Where(exceptions.DoesNotContain),
                               expected.Where(exceptions.DoesNotContain),
                               message,
                               args);
            return @this;
        }

        /// <summary>
        ///     Assert equality-by-value by recursively iterating over all elements
        ///     and all properties. Recursion stops at value types and at types (including string) which override Equals()
        ///     <see cref="TestBase.Comparer.MemberCompare" />
        /// </summary>
        /// <param name="exclusions">
        ///     a possibly empty list of field names to exclude for the purposes of this
        ///     comparison. To exclude fields of fields, provide the full dotted 'breadcrumb' to the property
        ///     to exclude, e.g. new List&lt;string&gt;{"Id","SomeProperty.SomePropertyOfThat.FieldName"}
        /// </param>
        /// <param name="this"></param>
        /// <param name="expected"></param>
        /// <param name="exceptions"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns>
        ///     <param name="@this"></param>
        /// </returns>
        /// <exception cref="Assertion">Returns a message indicating where the comparision failed</exception>
        public static IEnumerable<T>
        ShouldEqualByValueExceptForValuesIgnoringOrder<T>(
            this IEnumerable<T> @this,
            IEnumerable<T>      expected,
            IEnumerable<T>      exceptions,
            string              message = null,
            params object[]     args)
        {
            if (expected == null && @this == null) return @this;
            expected   = expected   ?? new T[0];
            exceptions = exceptions ?? new T[0];
            ShouldEqualByValue(@this.Where(exceptions.DoesNotContain).OrderBy(x => x),
                               expected.Where(exceptions.DoesNotContain).OrderBy(x => x),
                               message,
                               args);
            return @this;
        }
    }
}
