using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#if NET5_0_OR_GREATER

namespace TestBase
{
    public static class BasicShouldsWithArgExpression
    {
        /// <summary>Asserts that <code>actual!=null</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldNotBeNull<T>(this T actual,
                                           [CallerArgumentExpression("actual")]
                                           string argumentExpression = null,
                                           IEnumerable<(string, object)> comments = default) 
            => (T)Assert.That(actual, Is.NotNull, comments, argumentExpression);


        /// <summary>Asserts that <code>actual==null</code></summary>
        public static T ShouldBeNull<T>(this T actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default) 
            => (T)Assert.That(actual, Is.Null, comments, argumentExpression);

        /// <summary>Asserts that <code>string.IsNullOrEmpty(actual)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static string ShouldBeNullOrEmpty(
                                 this string actual,
                                 [CallerArgumentExpression("actual")]
                                 string argumentExpression = null,
                                 IEnumerable<(string, object)> comments = default) 
            => Assert.That(actual, Is.NotNullOrEmpty, comments, argumentExpression);

        /// <summary>Asserts that <code>actual.Length==0</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static void ShouldBeEmpty(this string actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
            // ReSharper disable once VariableHidesOuterVariable
            => Assert.That(actual, actual=>actual.Length == 0, comments, argumentExpression);

        /// <summary>Asserts that <paramref name="actual" /> has no elements. This will fail if actual is null.</summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static void ShouldBeEmpty(this IEnumerable actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
            => Assert.That(actual, Is.Empty, comments, argumentExpression);

        /// <summary>Asserts that <code>string.IsNullOrWhitespace(actual.ToString())</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static object ShouldBeNullOrEmptyOrWhitespace(this object     actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            
            return Assert.That(actual, Is.NullOrEmptyOrWhitespace, comments, argumentExpression);
        }

        /// <summary>Asserts that <code>string.IsNullOrWhitespace(actual.ToString())</code> would fail.</summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static string ShouldNotBeNullOrEmptyOrWhitespace(
            this string actual,
            [CallerArgumentExpression("actual")] string argumentExpression = null,
            IEnumerable<(string, object)> comments = default)
            // ReSharper disable once VariableHidesOuterVariable
            => (string)Assert.That(actual, Is.NullOrEmptyOrWhitespace, comments, argumentExpression);

        /// <summary>
        ///     Asserts that <paramref name="actual" />.Equals(<paramref name="expected" />) or else that
        ///     <paramref name="actual" /> and <paramref name="expected" /> are both null.
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBe<T>(this T actual, T expected, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            if (actual == null && expected == null) { return default(T); }
            return (T)Assert.That(actual, Is.EqualTo(expected), comments,argumentExpression);
        }

        /// <summary>
        ///     Asserts that <code>actual.Equals(expected)</code> would fail (or else that <paramref name="actual" /> is null
        ///     and <paramref name="notExpected" /> is not).
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldNotBe<T>(this T actual, T notExpected, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, Is.NotEqualTo(notExpected), comments, argumentExpression);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual" />.Equals(<paramref name="expected" />)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldEqual<T>(this T actual, object expected, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            return Assert.That(actual, a => a.Equals(expected), comments,argumentExpression);
        }

        /// <summary>Asserts that <code>!<paramref name="actual" />.Equals(<paramref name="notExpected" />)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldNotEqual<T>(this T actual, T notExpected, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            return Assert.That(actual,
                               a => !a.Equals(notExpected),
                               comments,argumentExpression);
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is between <paramref name="left" /> and <paramref name="right" />
        ///     inclusively.
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeBetween<T>(this T actual, T left, T right, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        where T : IComparable<T>
        {
            Assert.That(actual, Is.InRange(left, right), comments, argumentExpression);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual" />.Equals(true)</code></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeTrue<T>(this T actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
            => Assert.That(actual, x => x.Equals(true), comments, argumentExpression);

        /// <summary>Asserts that <code><paramref name="actual" />.Equals(false)</code></summary>
        /// ///
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeFalse<T>(this T actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
            => Assert.That(actual, x => x.Equals(false), comments, argumentExpression);

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is GreaterThan <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        ///     https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeGreaterThan<T>(
            this T          actual,
            object          expected,
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            return (T)Assert.That(actual, Is.GreaterThan(expected), comments, argumentExpression);
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is GreaterThanOrEqualTo <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        ///     https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeGreaterThanOrEqualTo<T>(
            this T          actual,
            object          expected,
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual,
                        Is.GreaterThanOrEqualTo(expected),
                        comments,argumentExpression);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is LessThan <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        ///     https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeLessThan<T>(this T actual, object expected, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, Is.LessThan(expected), comments, argumentExpression);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> is LessThanOrEqualTo <paramref name="expected" />
        ///     The comparer used is the NUnitComparer
        ///     https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldBeLessThanOrEqualTo<T>(
            this T          actual,
            object          expected,
            [CallerArgumentExpression("actual")]
            string argumentExpression = null,
            IEnumerable<(string, object)> comments = default)
            => (T)Assert.That(actual, Is.LessThanOrEqualTo(expected), comments, argumentExpression);

        /// <summary>Asserts that <code><paramref name="actual" /> is <typeparamref name="T" /></code></summary>
        /// <returns>
        ///     <code>((<typeparamref name="T" />)<paramref name="actual" />)</code>
        /// </returns>
        public static T ShouldBeOfType<T>(this object actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
            => (T)Assert.That(actual, x => x is T, comments,argumentExpression);

        /// <summary>Asserts that <code>typeof(<typeparamref name="T" />) == <paramref name="type" /></code></summary>
        /// <returns>
        ///     <code><paramref name="actual" /></code>
        /// </returns>
        public static T ShouldBeOfTypeEvenIfNull<T>(this T actual,
                                                    Type type,
                                                    [CallerArgumentExpression("actual")]
                                                    string argumentExpression = null,
                                                    IEnumerable<(string, object)>
                                                        comments = default)
            where T : class
            => Assert.That(actual, a=> typeof(T)==type,comments, argumentExpression);

        /// <summary>Asserts that <code><paramref name="actual" /> is <typeparamref name="T" /></code></summary>
        /// <returns>
        ///     <code>((<typeparamref name="T" />)<paramref name="actual" />)</code>
        /// </returns>
        public static T ShouldBeAssignableTo<T>(this object actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        where T : class
            => (T)Assert.That(actual, x => x is T, comments,argumentExpression);

        /// <summary>Asserts that <code>((<typeparamref name="T" />)<paramref name="actual" />)</code> is not null.</summary>
        /// <returns>
        ///     <code>((<typeparamref name="T" />)<paramref name="actual" />)</code>
        /// </returns>
        public static T ShouldBeCastableTo<T>(this object actual, [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual,
                        x => (T) x != null,
                        comments,argumentExpression);

            return (T) actual;
        }

        /// <summary>
        /// A synonym of <seealso cref="Should{T}(T,Action{T})" />
        /// Asserts that <paramref name="actual" /> satisfies <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T ShouldBe<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            [CallerArgumentExpression("actual")]string argumentExpression = null, 
            IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, predicate, comments, argumentExpression);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> does not satisfy <paramref name="predicate" /></summary>
        /// <returns>actual</returns>
        public static T ShouldNotBe<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Expression<Func<bool, bool>> expression   = p => !p;
            var                          notPredicate = predicate.Chain(expression);

            Assert.That(actual, notPredicate, comments, argumentExpression);
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
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, transform.Chain(predicate), comments, argumentExpression);
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
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, predicate, comments, argumentExpression);
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
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, transform.Chain(predicate), comments, argumentExpression);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> satisfies <paramref name="predicate" /></summary>
        /// <returns>actual</returns>
        public static T Should<T>(
            this T actual,
            Expression<Func<T, bool>> predicate,
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, predicate, comments, argumentExpression);
            return actual;
        }

        /// <summary>
        ///     A synonym for <seealso cref="Should{T}" />
        ///     Asserts that <paramref name="actual" /> satisfies <paramref name="predicate" />
        /// </summary>
        /// <returns>actual</returns>
        public static T ShouldHave<T>(
            this T                    actual,
            Expression<Func<T, bool>> predicate,
            [CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            Assert.That(actual, predicate, comments, argumentExpression);
            return actual;
        }

        /// <summary>
        ///     Applies <paramref name="assertions" /> to <paramref name="actual" /> and returns <paramref name="actual" />.
        ///     There is no check that <paramref name="assertions" /> asserts anything. If it doesn't, <paramref name="actual" /> is
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
        /// <param name="assertions">An action. It is assumed to be an assertion</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T Should<T>(this T actual, Action<T> assertions,[CallerArgumentExpression("actual")]string argumentExpression = null, IEnumerable<(string,object)> comments = default)
        {
            var result = new Assertion<T>(actual,assertions,argumentExpression,comments);
            return result.DidPass ? actual : throw result;
        }
    }
}

#endif