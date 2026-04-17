using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace TestBase
{
    /// <summary>
    /// Extension methods, typically on <see cref="object"/> and unconstrained type
    /// parameters, for asserting X.ShouldY() style fluent predicates.
    /// </summary>
    public static class BasicShoulds
    {
        static void ThrowAssertion<T>(T actual, string assertionName, string assertedDetail, string message, object[] args,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
            throw new Assertion<T>(
                actual?.ToString() ?? "null",
                actualExpression,
                assertionName,
                assertedDetail,
                comment,
                false);
        }

        /// <summary>Asserts that <code>actual!=null</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        [return:NotNull]
        public static T ShouldNotBeNull<T>(
            [NotNull]this T actual,
            string message = null,
            params object[] args)
        {
            if (actual == null)
                ThrowAssertion(actual, nameof(ShouldNotBeNull), "Expected: not null, Actual: null", message, args);
            return actual;
        }

#if NET6_0_OR_GREATER
        [return:NotNull]
        public static T ShouldNotBeNull<T>(
            [NotNull]this T actual,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            if (actual == null)
                ThrowAssertion(actual, nameof(ShouldNotBeNull), "Expected: not null, Actual: null", null, null, actualExpression);
            return actual;
        }
#endif

        /// <summary>Asserts that <code>actual==null</code></summary>
        public static void ShouldBeNull([AllowNull] this object actual, string message = null, params object[] args)
        {
            if (actual != null)
                ThrowAssertion(actual, nameof(ShouldBeNull), $"Expected: null, Actual: {actual}", message, args);
        }

        /// <summary>Asserts that <code>string.IsNullOrEmpty(actual)</code></summary>
        public static void ShouldBeNullOrEmpty([AllowNull]this string actual, string message = null, params object[] args)
        {
            if (!string.IsNullOrEmpty(actual))
                ThrowAssertion(actual, nameof(ShouldBeNullOrEmpty), $"Expected: null or empty, Actual: \"{actual}\"", message, args);
        }

        /// <summary>Asserts that <code>actual.Length==0</code></summary>
        public static void ShouldBeEmpty(this string actual, string message = null, params object[] args)
        {
            if (actual.Length != 0)
                ThrowAssertion(actual, nameof(ShouldBeEmpty), $"Expected: empty string, Actual: \"{actual}\" (length {actual.Length})", message, args);
        }

        /// <summary>Asserts that <paramref name="actual" /> has no elements. This will fail if actual is null.</summary>
        public static void ShouldBeEmpty(this IEnumerable actual, string message = null, params object[] args)
        {
            if (actual.HasAnyElements())
                ThrowAssertion(actual, nameof(ShouldBeEmpty), "Expected: empty collection, Actual: collection has elements", message, args);
        }

        /// <summary>Asserts that <code>string.IsNullOrWhitespace(actual.ToString())</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static object ShouldBeNullOrEmptyOrWhitespace(
            this object     actual,
            string          message = null,
            params object[] args)
        {
            if (actual == null) { return null; }
            var trimmed = (actual.ToString() ?? "").Trim();
            if (trimmed.Length != 0)
                ThrowAssertion(actual, nameof(ShouldBeNullOrEmptyOrWhitespace),
                    $"Expected: null, empty, or whitespace, Actual: \"{actual}\"", message, args);
            return actual;
        }

        /// <summary>Asserts that <code>string.IsNullOrWhitespace(actual.ToString())</code> would fail.</summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        [return:NotNull]
        public static object ShouldNotBeNullOrEmptyOrWhitespace(
            [NotNull]this object     actual,
            string          message = null,
            params object[] args)
        {
            if (actual == null)
                ThrowAssertion(actual, nameof(ShouldNotBeNullOrEmptyOrWhitespace),
                    "Expected: not null/empty/whitespace, Actual: null", message, args);
            var trimmed = (actual.ToString() ?? "").Trim();
            if (trimmed.Length == 0)
                ThrowAssertion(actual, nameof(ShouldNotBeNullOrEmptyOrWhitespace),
                    $"Expected: not null/empty/whitespace, Actual: \"{actual}\"", message, args);
            return actual;
        }

#if NET6_0_OR_GREATER
        [return:NotNull]
        public static object ShouldNotBeNullOrEmptyOrWhitespace(
            [NotNull]this object     actual,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            if (actual == null)
                ThrowAssertion(actual, nameof(ShouldNotBeNullOrEmptyOrWhitespace),
                    "Expected: not null/empty/whitespace, Actual: null", null, null, actualExpression);
            var trimmed = (actual.ToString() ?? "").Trim();
            if (trimmed.Length == 0)
                ThrowAssertion(actual, nameof(ShouldNotBeNullOrEmptyOrWhitespace),
                    $"Expected: not null/empty/whitespace, Actual: \"{actual}\"", null, null, actualExpression);
            return actual;
        }
#endif

        /// <summary>
        ///     Asserts that <paramref name="actual" />.Equals(<paramref name="expected" />) or else that
        ///     <paramref name="actual" /> and <paramref name="expected" /> are both null.
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBe<T>(this T actual, T expected, string message = null, params object[] args)
        {
            if (actual == null && expected == null) { return default(T); }
            if (actual == null || !actual.Equals(expected))
                ThrowAssertion(actual, $"{nameof(ShouldBe)} {expected}",
                    $"Expected: {expected ?? (object)"null"}, Actual: {actual ?? (object)"null"}", message, args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <code>actual.Equals(expected)</code> would fail (or else that <paramref name="actual" /> is null
        ///     and <paramref name="notExpected" /> is not).
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldNotBe<T>(this T actual, T notExpected, string message = null, params object[] args)
        {
            bool areEqual = (actual == null && notExpected == null)
                || (actual != null && actual.Equals(notExpected));
            if (areEqual)
                ThrowAssertion(actual, $"{nameof(ShouldNotBe)} {notExpected}",
                    $"Expected: not {notExpected ?? (object)"null"}, Actual: {actual ?? (object)"null"}", message, args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual" />.Equals(<paramref name="expected" />)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldEqual<T>(this T actual, object expected, string comment = null, params object[] args)
        {
            bool areEqual = (actual == null && expected == null)
                || (actual != null && actual.Equals(expected));
            if (!areEqual)
                ThrowAssertion(actual, $"{nameof(ShouldEqual)} {expected}",
                    $"Expected: {expected ?? "null"}, Actual: {actual ?? (object)"null"}", comment, args);
            return actual;
        }

        /// <summary>Asserts that <code>!<paramref name="actual" />.Equals(<paramref name="notExpected" />)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldNotEqual<T>(this T actual, T notExpected, string comment = null, params object[] args)
        {
            bool areEqual = (actual == null && notExpected == null)
                || (actual != null && actual.Equals(notExpected));
            if (areEqual)
                ThrowAssertion(actual, $"{nameof(ShouldNotEqual)} {notExpected}",
                    $"Expected: not {notExpected ?? (object)"null"}, Actual: {actual ?? (object)"null"}", comment, args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is between <paramref name="left" /> and <paramref name="right" />
        ///     inclusively.
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeBetween<T>(this T actual, T left, T right, string message = null, params object[] args)
        where T : IComparable<T>
        {
            if (actual.CompareTo(left) < 0 || actual.CompareTo(right) > 0)
                ThrowAssertion(actual, nameof(ShouldBeBetween),
                    $"Expected: between {left} and {right}, Actual: {actual}", message, args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual" />.Equals(true)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeTrue<T>(this T actual, string message = null, params object[] args)
        {
            if (actual == null || !actual.Equals(true))
                ThrowAssertion(actual, nameof(ShouldBeTrue),
                    $"Expected: true, Actual: {actual ?? (object)"null"}", message, args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual" />.Equals(false)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeFalse<T>(this T actual, string message = null, params object[] args)
        {
            if (actual == null || !actual.Equals(false))
                ThrowAssertion(actual, nameof(ShouldBeFalse),
                    $"Expected: false, Actual: {actual ?? (object)"null"}", message, args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is GreaterThan <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeGreaterThan<T>(
            this T          actual,
            object          expected,
            string          message = null,
            params object[] args)
        {
            if (new NUnitComparer().Compare(actual, expected) <= 0)
                ThrowAssertion(actual, nameof(ShouldBeGreaterThan),
                    $"Expected: > {expected}, Actual: {actual}", message, args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is GreaterThanOrEqualTo <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeGreaterThanOrEqualTo<T>(
            this T          actual,
            object          expected,
            string          message = null,
            params object[] args)
        {
            if (new NUnitComparer().Compare(actual, expected) < 0)
                ThrowAssertion(actual, nameof(ShouldBeGreaterThanOrEqualTo),
                    $"Expected: >= {expected}, Actual: {actual}", message, args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is LessThan <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeLessThan<T>(this T actual, object expected, string message = null, params object[] args)
        {
            if (new NUnitComparer().Compare(actual, expected) >= 0)
                ThrowAssertion(actual, nameof(ShouldBeLessThan),
                    $"Expected: < {expected}, Actual: {actual}", message, args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is LessThanOrEqualTo <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeLessThanOrEqualTo<T>(
            this T          actual,
            object          expected,
            string          message = null,
            params object[] args)
        {
            if (new NUnitComparer().Compare(actual, expected) > 0)
                ThrowAssertion(actual, nameof(ShouldBeLessThanOrEqualTo),
                    $"Expected: <= {expected}, Actual: {actual}", message, args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual" /> is <typeparamref name="T" /></code></summary>
        /// <returns>
        ///     <code>((<typeparamref name="T" />)<paramref name="actual" />)</code>
        /// </returns>
        public static T ShouldBeOfType<T>(this object actual, string message = null, params object[] args)
        {
            if (actual is not T)
                ThrowAssertion(actual, nameof(ShouldBeOfType),
                    $"Expected: type {typeof(T)}, Actual: {(actual == null ? "null" : $"type {actual.GetType()}")}", message, args);
            return (T) actual;
        }

        /// <summary>Asserts that <code>typeof(<typeparamref name="T" />) == <paramref name="type" /></code></summary>
        /// <returns>
        ///     <code><paramref name="actual" /></code>
        /// </returns>
        public static T ShouldBeOfTypeEvenIfNull<T>(
            this T          actual,
            Type            type,
            string          message = null,
            params object[] args) where T : class
        {
            if (typeof(T) != type)
                ThrowAssertion(actual, nameof(ShouldBeOfTypeEvenIfNull),
                    $"Expected: type {type}, Actual: type {typeof(T)}", message, args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual" /> is <typeparamref name="T" /></code></summary>
        /// <returns>
        ///     <code>((<typeparamref name="T" />)<paramref name="actual" />)</code>
        /// </returns>
        public static T ShouldBeAssignableTo<T>(this object actual, string message = null, params object[] args)
        where T : class
        {
            if (actual is not T)
                ThrowAssertion(actual, nameof(ShouldBeAssignableTo),
                    $"Expected: assignable to {typeof(T)}, Actual: {(actual == null ? "null" : $"type {actual.GetType()}")}", message, args);
            return actual as T;
        }

        /// <summary>
        /// Asserts that <code>((<typeparamref name="T" />)<paramref name="actual" />)</code> is not null.
        /// </summary>
        /// <returns>
        ///     <code>((<typeparamref name="T" />)<paramref name="actual" />)</code>
        /// </returns>
        public static T ShouldBeCastableTo<T>(this object actual, string message = null, params object[] args)
        {
            try
            {
                var cast = (T) actual;
                if (cast == null)
                    ThrowAssertion(actual, nameof(ShouldBeCastableTo),
                        $"Expected: castable to {typeof(T)}, Actual: cast result is null", message, args);
                return cast;
            }
            catch (InvalidCastException)
            {
                ThrowAssertion(actual, nameof(ShouldBeCastableTo),
                    $"Expected: castable to {typeof(T)}, Actual: {(actual == null ? "null" : $"type {actual.GetType()}")}", message, args);
                return default;
            }
        }

        /// <summary>
        /// Synonym of <see cref="Should{T,TResult}" />
        /// Asserts that <paramref name="actual" /> satisfies <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T ShouldBe<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            string                    message = null,
            params object[]           args)
        {
            Assert.That(actual, predicate, message ?? nameof(ShouldBe), args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> does not satisfy <paramref name="predicate" /></summary>
        /// <returns>actual</returns>
        public static T ShouldNotBe<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            string                    message = null,
            params object[]           args)
        {
            Expression<Func<bool, bool>> expression   = p => !p;
            var                          notPredicate = predicate.Chain(expression);

            Assert.That(actual, notPredicate, message ?? nameof(ShouldNotBe), args);
            return actual;
        }

        /// <summary>
        ///     Synonym of <see cref="Should{T,TResult}" /> Asserts that <paramref name="transform" />(
        ///     <paramref name="actual" />) satisfies <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T ShouldSatisfy<T, TResult>(
            this T                          actual,
            Expression<Func<T, TResult>>    transform,
            Expression<Func<TResult, bool>> predicate,
            string                          message = null,
            params object[]                 args)
        {
            Assert.That(actual, transform.Chain(predicate), message ?? nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>
        ///     Synonym of <seealso cref="Should{T,TResult}" />. Asserts that <paramref name="actual" /> satisfies
        ///     <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T ShouldSatisfy<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            string                    message = null,
            params object[]           args)
        {
            Assert.That(actual, predicate, message ?? nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="transform" />(<paramref name="actual" />) satisfies <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T Should<T, TResult>(
            this T                          actual,
            Expression<Func<T, TResult>>    transform,
            Expression<Func<TResult, bool>> predicate,
            string                          message = null,
            params object[]                 args)
        {
            Assert.That(actual, transform.Chain(predicate), message ?? nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>
        /// Asserts that <paramref name="actual" /> satisfies <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T Should<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            string                    message = null,
            params object[]           args)
        {
            Assert.That(actual, predicate, message ?? nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>
        ///     A synonym for <seealso cref="Should{T,TResult}" />
        ///     Asserts that <paramref name="actual" /> satisfies <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T ShouldHave<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            string                    message = null,
            params object[]           args)
        {
            Assert.That(actual, predicate, message ?? nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>
        ///     Applies <paramref name="assertion" /> to <paramref name="actual" /> and returns <paramref name="actual" />.
        ///     There is no check that <paramref name="assertion" /> asserts anything. If it doesn't, <paramref name="actual" /> is
        ///     returned.
        ///     This is intended as a convenience overload for the case where e.g. you prefer
        ///     <code> actual
        ///    .Should(a=> a.SomeProperty.ShouldBeGreaterThan(1))
        ///    .Should(a=> a.SomeOtherProperty.ShouldBeGreaterThan(2))
        ///    .Should(a=> someOtherPredicate )
        ///  </code>
        ///     over
        ///     <code>
        ///    actual.SomeProperty.ShouldBeGreaterThan(1)
        ///    actual.SomeOtherProperty.ShouldBeGreaterThan(2);
        ///    actual.SomeOtherPredicate()
        ///  </code>
        /// </summary>
        /// <param name="actual">the value under test</param>
        /// <param name="assertion">An action. It is assumed to be an assertion</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T Should<T>(this T actual, Action<T> assertion)
        {
            assertion(actual);
            return actual;
        }
    }
}
