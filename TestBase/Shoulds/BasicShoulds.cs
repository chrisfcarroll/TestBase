using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace TestBase.Shoulds
{
    public static class BasicShoulds
    {
        public static T ShouldNotBeNull<T>(this T @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Not.Null, message, args);
            return @this;
        }

        public static void ShouldBeNull(this object @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Null, message, args);
        }

        public static void ShouldBeNullOrEmpty(this string @this, [Optional] string message, params object[] args)
        {
            Assert.That(String.IsNullOrEmpty(@this), message??"Should Be Null Or Empty", args);
        }

        public static void ShouldBeEmpty(this string @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this.Length == 0, message??"Should Be Empty", args);
        }

        public static void ShouldBeEmpty(this IEnumerable @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Empty, message, args);
        }

        public static void ShouldBeNullOrEmptyOrWhitespace(this object @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this == null || @this.ToString().Trim().Length == 0, message??"ShouldBeNullOrWhitespace", args);
        }

        public static string ShouldNotBeNullOrEmpty(this string @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Not.Null, message, args);
            Assert.That(@this, Is.Not.EqualTo(""), message, args);
            return @this;
        }

        public static T ShouldBeSuchThat<T>(this T @this, Func<T, bool> predicate, [Optional] string message, params object[] args)
        {
            ShouldSatisfy(@this, predicate, Is.True, message, args);
            return @this;
        }

        public static string ShouldEqualIgnoringCase(this string @this, string expected, [Optional] string message, params object[] args)
        {
            Assert.That(@this.ToLower(), Is.EqualTo(expected.ToLower()), message, args);
            return @this;
        }

        public static T ShouldEqual<T>(this T @this, object expectedValue, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.EqualTo(expectedValue), message, args);
            return @this;
        }


        public static void ShouldNotEqual<T>(this T @this, T obj, [Optional] string message, params object[] args)
        {
            Assert.AreNotEqual(obj, @this, message, args);
        }

        public static void ShouldBeTrue( this object @this, [Optional] string message, params object[] args )
        {
            Assert.That(@this, Is.True, message, args);
        }

        public static void ShouldBeFalse( this object @this, [Optional] string message, params object[] args )
        {
            Assert.That(@this, Is.False, message, args);
        }

        public static T ShouldBeGreaterThan<T>( this T @this, object expected, [Optional] string message, params object[] args)
        {
            Assert.That( @this, Is.GreaterThan( expected ), message, args );
            return @this;
        }

        public static T ShouldBeGreaterThanOrEqualTo<T>( this T @this, object expected, [Optional] string message, params object[] args )
        {
            Assert.That( @this, Is.GreaterThanOrEqualTo( expected ), message, args );
            return @this;
        }

        public static T ShouldBeOfType<T>(this object @this, [Optional] string message, params object[] args) 
        {
            Assert.That(@this, Is.InstanceOf<T>(), message, args);

            return (T)@this;
        }

        public static T ShouldBeOfTypeEvenIfNull<T>(this T @this, Type type, [Optional] string message, params object[] args) where T : class
        {
            typeof (T).ShouldEqual(type, message, args);
            return @this;
        }

        public static T ShouldBeAssignableTo<T>(this object @this, [Optional] string message, params object[] args) where T : class
        {
            Assert.That(@this, Is.AssignableTo<T>(), message, args);

            return @this as T;
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

        public static void ShouldContain<T>(this IEnumerable<T> @this, T item, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Contains.Item(item), message, args);
        }

        public static void ShouldBeEmpty<T>(this IEnumerable<T> @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Empty, message, args);
        }

        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Not.Empty, message, args);
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

        public static T ShouldSatisfy<T, TResult>(this T @this, Func<T, TResult> function, IResolveConstraint expression, [Optional] string message, params object[] args)
        {
            var result = function(@this);
            Assert.That(result , expression, message, args);
            return @this;
        }

        public static void ShouldContainInOrder<T>(this List<T> @this, T expectedFirst, T expectedAfter, [Optional] string message, params object[] args)
        {
            @this
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            @this.ShouldContain(expectedAfter, message, args);
        }
    }
}
