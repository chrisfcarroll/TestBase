using System;
using System.Collections;
using System.Linq.Expressions;

namespace TestBase
{
    public static class BasicShoulds
    {
        /// <summary>Asserts that <code>actual!=null</code></summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldNotBeNull<T>(this T actual, string message=null, params object[] args)
        {
            Assert.That(actual, Is.NotNull, message ?? nameof(ShouldNotBeNull), args);
            return actual;
        }

        /// <summary>Asserts that <code>actual==null</code></summary>
        public static void ShouldBeNull(this object actual, string message=null, params object[] args)
        {
            Assert.That(actual, Is.Null, message ?? nameof(ShouldBeNull), args);
        }

        /// <summary>Asserts that <code>string.IsNullOrEmpty(actual)</code></summary>
        /// <returns><paramref name="actual"/></returns>
        public static void ShouldBeNullOrEmpty(this string actual, string message=null, params object[] args)
        {
            Assert.That(string.IsNullOrEmpty(actual), message?? nameof(ShouldBeNullOrEmpty), args);
        }

        /// <summary>Asserts that <code>actual.Length==0</code></summary>
        /// <returns><paramref name="actual"/></returns>
        public static void ShouldBeEmpty(this string actual, string message=null, params object[] args)
        {
            Assert.That(actual.Length == 0, message??nameof(ShouldBeEmpty), args);
        }

        /// <summary>Asserts that <paramref name="actual"/> has no elements. This will fail if actual is null.</summary>
        /// <returns><paramref name="actual"/></returns>
        public static void ShouldBeEmpty(this IEnumerable actual, string message=null, params object[] args)
        {
            Assert.That(actual, Is.Empty, message??nameof(ShouldBeEmpty), args);
        }

        /// <summary>Asserts that <code>string.IsNullOrWhitespace(actual.ToString())</code></summary>
        /// <returns><paramref name="actual"/></returns>
        public static object ShouldBeNullOrEmptyOrWhitespace(this object actual, string message=null, params object[] args)
        {
            if (actual == null)
                return null;
            else
            {
                var trimmed = actual.ToString().Trim();
                return Assert.That(actual, x => trimmed.Length == 0, message ?? nameof(ShouldBeNullOrEmptyOrWhitespace), args);
            }
        }

        /// <summary>Asserts that <code>string.IsNullOrWhitespace(actual.ToString())</code> would fail.</summary>
        /// <returns><paramref name="actual"/></returns>
        public static object ShouldNotBeNullOrEmptyOrWhitespace(this object actual, string message=null, params object[] args)
        {
            Assert.That(actual, x=> x!= null, message ?? nameof(ShouldNotBeNullOrEmptyOrWhitespace), args);
            var trimmed = actual.ToString().Trim();
            return Assert.That(actual, x=> trimmed.Length != 0, message ?? nameof(ShouldNotBeNullOrEmptyOrWhitespace), args);
        }

        /// <summary>Asserts that <paramref name="actual"/>.Equals(<paramref name="expected"/>) or else that <paramref name="actual"/> and <paramref name="expected"/> are both null.</summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldBe<T>(this T actual, T expected, string message=null, params object[] args)
        {
            if (actual == null && expected == null)
                return actual;
            else if (expected == null)
                return (T) Assert.That(actual, Is.Null, message ?? $"{nameof(ShouldBe)} null", args);
            else if(actual==null)
                return Assert.That(actual, x=> false && x.Equals(expected) , message ?? $"{nameof(ShouldBe)} {expected}", args);
            else {
                Assert.That(actual, x => x.Equals(expected), message ?? $"{nameof(ShouldBe)} {expected}", args);
                return actual;
            }
        }

        /// <summary>Asserts that <code>actual.Equals(expected)</code> would fail (or else that <paramref name="actual"/> is null and <paramref name="notExpected"/> is not).</summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldNotBe<T>(this T actual, T notExpected, string message=null, params object[] args)
        {
            Assert.That(actual, Is.NotEqualTo(notExpected),  message ?? $"{nameof(ShouldNotBe)} {notExpected}", args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual"/>.Equals(<paramref name="expected"/>)</code></summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldEqual<T>(this T actual, object expected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a=>a.Equals(expected), comment ?? $"{nameof(ShouldEqual)} {expected}", args);
        }

        /// <summary>Asserts that <code>!<paramref name="actual"/>.Equals(<paramref name="notExpected"/>)</code></summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldNotEqual<T>(this T actual, T notExpected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => !a.Equals(notExpected), comment ?? $"{nameof(ShouldNotEqual)} {notExpected}", args);
        }

        /// <summary>Asserts that <paramref name="actual"/> is between <paramref name="left"/> and <paramref name="right"/> inclusively.</summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldBeBetween<T>(this T actual, T left, T right, string message=null, params object[] args)
                        where T : IComparable<T>
        {
            Assert.That(actual, Is.InRange(left, right), message ??nameof(ShouldBeBetween), args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual"/>.Equals(true)</code></summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldBeTrue<T>(this T actual, string message=null, params object[] args)
        {
            if (actual == null) 
                return Assert.That(actual, x => x != null, message??nameof(ShouldBeTrue), args);
            else
            {
                Assert.That(actual, x => x.Equals(true), message ?? nameof(ShouldBeTrue), args);
                return actual;
            }
        }

        /// <summary>Asserts that <code><paramref name="actual"/>.Equals(false)</code></summary>
        /// /// <returns><paramref name="actual"/></returns>
        public static T ShouldBeFalse<T>( this T actual, string message=null, params object[] args )
        {
            if (actual == null) 
                return Assert.That(actual, x => x != null, message??nameof(ShouldBeFalse), args);
            else
            {
                Assert.That(actual, x => x.Equals(false), message ?? nameof(ShouldBeFalse), args);
                return actual;
            }
        }

        /// <summary>Asserts that <paramref name="actual"/> is GreaterThan <paramref name="expected"/>
        /// The comparer used is the NUnitComparer https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs 
        /// </summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldBeGreaterThan<T>( this T actual, object expected, string message=null, params object[] args)
        {
            Assert.That( actual, Is.GreaterThan( expected ), message??nameof(ShouldBeGreaterThan), args );
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual"/> is GreaterThanOrEqualTo <paramref name="expected"/>
        /// The comparer used is the NUnitComparer https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs 
        /// </summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldBeGreaterThanOrEqualTo<T>( this T actual, object expected, string message=null, params object[] args )
        {
            Assert.That( actual, Is.GreaterThanOrEqualTo( expected ), message??nameof(ShouldBeGreaterThanOrEqualTo), args );
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual"/> is LessThan <paramref name="expected"/>
        /// The comparer used is the NUnitComparer https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs 
        /// </summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldBeLessThan<T>(this T actual, object expected, string message=null, params object[] args)
        {
            Assert.That(actual, Is.LessThan(expected), message??nameof(ShouldBeLessThan), args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual"/> is LessThanOrEqualTo <paramref name="expected"/>
        /// The comparer used is the NUnitComparer https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Constraints/NUnitComparer.cs 
        /// </summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldBeLessThanOrEqualTo<T>(this T actual, object expected, string message=null, params object[] args)
        {
            Assert.That(actual, Is.LessThanOrEqualTo(expected), message??nameof(ShouldBeLessThanOrEqualTo), args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual"/> is <typeparamref name="T"/></code></summary>
        /// <returns><code>((<typeparamref name="T"/>)<paramref name="actual"/>)</code></returns>
        public static T ShouldBeOfType<T>(this object actual, string message=null, params object[] args) 
        {
            Assert.That(actual, x=> x is T, message ??$"actual of type {actual?.GetType()} {nameof(ShouldBeOfType)} {typeof(T)} but isn't.", args);
            return (T)actual;
        }

        /// <summary>Asserts that <code>typeof(<typeparamref name="T"/>) == <paramref name="type"/></code></summary>
        /// <returns><code><paramref name="actual"/></code></returns>
        public static T ShouldBeOfTypeEvenIfNull<T>(this T actual, Type type, string message=null, params object[] args) where T : class
        {
            typeof (T).ShouldEqual(type, message??$"{nameof(ShouldBeOfTypeEvenIfNull)} {typeof(T)}", args);
            return actual;
        }

        /// <summary>Asserts that <code><paramref name="actual"/> is <typeparamref name="T"/></code></summary>
        /// <returns><code>((<typeparamref name="T"/>)<paramref name="actual"/>)</code></returns>
        public static T ShouldBeAssignableTo<T>(this object actual, string message=null, params object[] args) where T : class
        {
            Assert.That(actual, x=>x is T, message??$"{actual?.GetType()} {nameof(ShouldBeAssignableTo)} {typeof(T)} but isn't.", args);

            return actual as T;
        }
        
        /// <summary>Asserts that <code>((<typeparamref name="T"/>)<paramref name="actual"/>)</code> is not null.</summary>
        /// <returns><code>((<typeparamref name="T"/>)<paramref name="actual"/>)</code></returns>
        public static T ShouldBeCastableTo<T>(this object actual, string message=null, params object[] args)
        {
            Assert.That(actual, x=> ((T)x)!=null, message??$"actual of type {actual?.GetType()} {nameof(ShouldBeCastableTo)} {typeof(T)} but isn't.", args);

            return (T)actual ;
        }

        /// <summary>Synonym of <seealso cref="Should{T}"/> Asserts that <paramref name="actual"/> satisfies <paramref name="predicate"/></summary>
        /// <returns>actual</returns>
        public static T ShouldBe<T>(this T actual, Expression<Func<T,bool>> predicate, string message=null, params object[] args)
        {
            Assert.That(actual,predicate, message??nameof(ShouldBe), args);
            return actual;
        }
        
        /// <summary>Asserts that <paramref name="actual"/> does not satisfy <paramref name="predicate"/></summary>
        /// <returns>actual</returns>
        public static T ShouldNotBe<T>(this T actual, Expression<Func<T,bool>> predicate, string message=null, params object[] args)
        {
            Expression<Func<bool, bool>> expression = p=>!p;
            var notPredicate = predicate.Chain(expression);

            Assert.That(actual, notPredicate, message??nameof(ShouldNotBe), args);
            return actual;
        }

        /// <summary>Synonym of <see cref="Should{T,TResult}"/> Asserts that <paramref name="transform"/>(<paramref name="actual"/>) satisfies <paramref name="predicate"/></summary>
        /// <returns>actual</returns>
        public static T ShouldSatisfy<T, TResult>(this T actual, Expression<Func<T, TResult>> transform, Expression<Func<TResult,bool>> predicate, string message=null, params object[] args)
        {
            Assert.That(actual, transform.Chain(predicate), message??nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>Synonym of <seealso cref="Should{T,TResult}"/>. Asserts that <paramref name="actual"/> satisfies <paramref name="predicate"/></summary>
        /// <returns>actual</returns>
        public static T ShouldSatisfy<T>(this T actual, Expression<Func<T, bool>> predicate, string message=null, params object[] args)
        {
            Assert.That(actual, predicate, message??nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="transform"/>(<paramref name="actual"/>) satisfies <paramref name="predicate"/></summary>
        /// <returns>actual</returns>
        public static T Should<T, TResult>(this T actual, Expression<Func<T, TResult>> transform, Expression<Func<TResult,bool>> predicate, string message=null, params object[] args)
        {
            Assert.That(actual, transform.Chain(predicate), message??nameof(ShouldSatisfy), args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual"/> satisfies <paramref name="predicate"/></summary>
        /// <returns>actual</returns>
        public static T Should<T>(this T actual, Expression<Func<T, bool>> predicate, string message=null, params object[] args)
        {
            Assert.That(actual, predicate, message??nameof(ShouldSatisfy), args);
            return actual;
        }
        /// <summary>A synonym for <seealso cref="Should{T}"/>
        /// Asserts that <paramref name="actual"/> satisfies <paramref name="predicate"/></summary>
        /// <returns>actual</returns>
        public static T ShouldHave<T>(this T actual, Expression<Func<T, bool>> predicate, string message=null, params object[] args)
        {
            Assert.That(actual, predicate, message??nameof(ShouldSatisfy), args);
            return actual;
        }

        ///<summary>
        /// Applies <paramref name="assertion"/> to <paramref name="actual"/> and returns <paramref name="actual"/>.
        /// There is no check that <paramref name="assertion"/> asserts anything. If it doesn't, <paramref name="actual"/> is returned.
        /// 
        /// This is intended as a convenience overload for the case where e.g. you prefer
        /// <code> actual
        ///   .Should(a=> a.SomeProperty.ShouldBeGreaterThan(1))
        ///   .Should(a=> a.SomeOtherProperty.ShouldBeGreaterThan(2))
        ///   .Should(a=> someOtherPredicate )
        /// </code>
        /// over
        /// <code>  
        ///   actual.SomeProperty.ShouldBeGreaterThan(1)
        ///   actual.SomeOtherProperty.ShouldBeGreaterThan(2);
        ///   actual.SomeOtherPredicate()
        /// </code>
        /// </summary>
        /// <param name="actual">the value under test</param>
        /// <param name="assertion">An action. It is assumed to be an assertion</param>
        /// <returns><paramref name="actual"/></returns>
        public static T Should<T>(this T actual, Action<T> assertion)
        {
            assertion(actual);
            return actual;
        }
    }
}
