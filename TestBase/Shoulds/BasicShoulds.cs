using System;
using System.Collections;
using System.Runtime.InteropServices;
using TestBase;

namespace TestBase.Shoulds
{
    public static class BasicShoulds
    {
        public static T ShouldNotBeNull<T>(this T @this, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x!=null, message, args);
            return @this;
        }

        public static void ShouldBeNull(this object @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.Null, message, args);
        }

        public static void ShouldBeNullOrEmpty(this string @this, string message=null, params object[] args)
        {
            Assert.That(String.IsNullOrEmpty(@this), message??"Should Be Null Or Empty", args);
        }

        public static void ShouldBeEmpty(this string @this, string message=null, params object[] args)
        {
            Assert.That(@this.Length == 0, message??"Should Be Empty", args);
        }

        public static void ShouldBeEmpty(this IEnumerable @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.Empty, message, args);
        }

        public static void ShouldBeNullOrEmptyOrWhitespace(this object @this, string message=null, params object[] args)
        {
            Assert.That(@this == null || @this.ToString().Trim().Length == 0, message??"ShouldBeNullOrWhitespace", args);
        }

        public static void ShouldNotBeNullOrEmptyOrWhitespace(this object @this, string message=null, params object[] args)
        {
            Assert.That(@this != null && @this.ToString().Trim().Length != 0, message ?? "ShouldNotBeNullOrWhitespace", args);
        }

        public static T ShouldEqual<T>(this T @this, object expectedValue, string message=null, params object[] args)
        {
            Assert.That(@this, Is.EqualTo(expectedValue), message, args);
            return @this;
        }

        public static T ShouldBe<T>(this T @this, T expectedValue, string message=null, params object[] args)
        {
            Assert.AreEqual(expectedValue, @this, message, args);
            return @this;
        }

        public static T ShouldNotEqual<T>(this T @this, T obj, string message=null, params object[] args)
        {
            Assert.AreNotEqual(obj, @this, message, args);
            return @this;
        }

        public static T ShouldNotBe<T>(this T @this, T obj, string message=null, params object[] args)
        {
            Assert.AreNotEqual(obj, @this, message, args);
            return @this;
        }

        public static T ShouldBeBetween<T>(this T @this, T left, T right, string message=null, params object[] args)
                        where T : IComparable<T>
        {
            Assert.That(@this, Is.InRange(left, right), message, args);
            return @this;
        }

        public static T ShouldBeTrue<T>(this T @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.True, message, args);
            return @this;
        }

        public static T ShouldBeFalse<T>( this T @this, string message=null, params object[] args )
        {
            Assert.That(@this, Is.False, message, args);
            return @this;
        }

        public static T ShouldBeGreaterThan<T>( this T @this, object expected, string message=null, params object[] args)
        {
            Assert.That( @this, Is.GreaterThan( expected ), message, args );
            return @this;
        }

        public static T ShouldBeGreaterThanOrEqualTo<T>( this T @this, object expected, string message=null, params object[] args )
        {
            Assert.That( @this, Is.GreaterThanOrEqualTo( expected ), message, args );
            return @this;
        }

        public static T ShouldBeLessThan<T>(this T @this, object expected, string message=null, params object[] args)
        {
            Assert.That(@this, Is.LessThanOrEqualTo(expected), message, args);
            return @this;
        }

        public static T ShouldBeLessThanOrEqualTo<T>(this T @this, object expected, string message=null, params object[] args)
        {
            Assert.That(@this, Is.LessThanOrEqualTo(expected), message, args);
            return @this;
        }


        public static T ShouldBeOfType<T>(this object @this, string message=null, params object[] args) 
        {
            Assert.That(@this, Is.InstanceOf<T>(), message, args);

            return (T)@this;
        }

        public static T ShouldBeOfTypeEvenIfNull<T>(this T @this, Type type, string message=null, params object[] args) where T : class
        {
            typeof (T).ShouldEqual(type, message, args);
            return @this;
        }

        public static T ShouldBeAssignableTo<T>(this object @this, string message=null, params object[] args) where T : class
        {
            Assert.That(@this, Is.AssignableTo<T>(), message, args);

            return @this as T;
        }

        public static T ShouldHave<T>(this T @this, Func<T, bool> predicate, string message=null, params object[] args)
        {
            ShouldSatisfy(@this, predicate, Is.True, message, args);
            return @this;
        }

        public static T ShouldHave<T>(this T @this, Action<T> assertion)
        {
            assertion(@this);
            return @this;
        }

        public static T ShouldNotHave<T>(this T @this, Func<T, bool> predicate, string message=null, params object[] args)
        {
            ShouldSatisfy(@this, (t)=> !predicate(t), Is.True, message, args);
            return @this;
        }

        public static T ShouldHave<T, TResult>(this T @this, Func<T, TResult> function, Predicate<TResult> expression, string message=null, params object[] args)
        {
            return ShouldSatisfy(@this, function, expression, message, args);
        }

        public static T ShouldSatisfy<T, TResult>(this T @this, Func<T, TResult> function, Predicate<TResult> expression, string message=null, params object[] args)
        {
            var result = function(@this);
            Assert.That(result , expression(result), message, args);
            return @this;
        }
        public static T ShouldBeSuchThat<T>(this T @this, Func<T, bool> function, string message=null, params object[] args)
        {
            var result = function(@this);
            Assert.IsTrue(result, message, args);
            return @this;
        }
    }
}
