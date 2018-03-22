using System;
using System.Collections;
using System.Linq.Expressions;

namespace TestBase
{
    public static class BasicShoulds
    {
        public static T ShouldNotBeNull<T>(this T @this, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x!=null, message ?? nameof(ShouldNotBeNull), args);
            return @this;
        }

        public static void ShouldBeNull(this object @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.Null, message ?? nameof(ShouldBeNull), args);
        }

        public static void ShouldBeNullOrEmpty(this string @this, string message=null, params object[] args)
        {
            Assert.That(String.IsNullOrEmpty(@this), message?? nameof(ShouldBeNullOrEmpty), args);
        }

        public static void ShouldBeEmpty(this string @this, string message=null, params object[] args)
        {
            Assert.That(@this.Length == 0, message??nameof(ShouldBeEmpty), args);
        }

        public static void ShouldBeEmpty(this IEnumerable @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.Empty, message??nameof(ShouldBeEmpty), args);
        }

        public static void ShouldBeNullOrEmptyOrWhitespace(this object @this, string message=null, params object[] args)
        {
            Assert.That(@this == null || @this.ToString().Trim().Length == 0, message??nameof(ShouldBeNullOrEmptyOrWhitespace), args);
        }

        public static void ShouldNotBeNullOrEmptyOrWhitespace(this object @this, string message=null, params object[] args)
        {
            Assert.That(@this != null && @this.ToString().Trim().Length != 0, message ?? nameof(ShouldNotBeNullOrEmptyOrWhitespace), args);
        }

        public static T ShouldBe<T>(this T @this, T expected, string message=null, params object[] args)
        {
            Assert.That(@this,  
                        x=> (x==null && expected==null) || x.Equals(expected), 
                        message ?? $"{nameof(ShouldBe)}\n\n{expected}", args);
            return @this;
        }

        public static T ShouldNotBe<T>(this T @this, T notExpected, string message=null, params object[] args)
        {
            Assert.That(@this, Is.NotEqualTo(notExpected),  message ?? $"{nameof(ShouldNotBe)}\n\n{notExpected}", args);
            return @this;
        }

        /// <summary>Tests whether @this.Equals(expected)</summary>
        public static T ShouldEqual<T>(this T actual, object expected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a=>a.Equals(expected), comment ?? $"{nameof(ShouldEqual)}\n\n{expected}", args);
        }

        /// <summary>Tests whether !@this.Equals(expected)</summary>
        public static T ShouldNotEqual<T>(this T actual, T notExpected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => !a.Equals(notExpected), comment ?? $"{nameof(ShouldNotEqual)}\n\n{notExpected}", args);
        }

        public static T ShouldBeBetween<T>(this T @this, T left, T right, string message=null, params object[] args)
                        where T : IComparable<T>
        {
            Assert.That(@this, Is.InRange(left, right), message ??nameof(ShouldBeBetween), args);
            return @this;
        }

        public static T ShouldBeTrue<T>(this T @this, string message=null, params object[] args)
        {
            Assert.That( @this, 
                         x=>x!=null&&x.Equals(true),
                         message ??nameof(ShouldBeTrue), args);
            return @this;
        }

        public static T ShouldBeFalse<T>( this T @this, string message=null, params object[] args )
        {
            Assert.That(@this, x=> x!=null&&x.Equals(false), message??nameof(ShouldBeFalse), args);
            return @this;
        }

        public static T ShouldBeGreaterThan<T>( this T @this, object expected, string message=null, params object[] args)
        {
            Assert.That( @this, Is.GreaterThan( expected ), message??nameof(ShouldBeGreaterThan), args );
            return @this;
        }

        public static T ShouldBeGreaterThanOrEqualTo<T>( this T @this, object expected, string message=null, params object[] args )
        {
            Assert.That( @this, Is.GreaterThanOrEqualTo( expected ), message??nameof(ShouldBeGreaterThanOrEqualTo), args );
            return @this;
        }

        public static T ShouldBeLessThan<T>(this T @this, object expected, string message=null, params object[] args)
        {
            Assert.That(@this, Is.LessThanOrEqualTo(expected), message??nameof(ShouldBeLessThan), args);
            return @this;
        }

        public static T ShouldBeLessThanOrEqualTo<T>(this T @this, object expected, string message=null, params object[] args)
        {
            Assert.That(@this, Is.LessThanOrEqualTo(expected), message??nameof(ShouldBeLessThanOrEqualTo), args);
            return @this;
        }


        public static T ShouldBeOfType<T>(this object @this, string message=null, params object[] args) 
        {
            Assert.That(@this, x=> x is T, message ??nameof(ShouldBeOfType), args);
            return (T)@this;
        }

        public static T ShouldBeOfTypeEvenIfNull<T>(this T @this, Type type, string message=null, params object[] args) where T : class
        {
            typeof (T).ShouldEqual(type, message??nameof(ShouldBeOfTypeEvenIfNull), args);
            return @this;
        }

        public static T ShouldBeAssignableTo<T>(this object @this, string message=null, params object[] args) where T : class
        {
            Assert.That(@this, x=>x is T, message??nameof(ShouldBeAssignableTo), args);

            return @this as T;
        }
        
        public static T ShouldBeCastableTo<T>(this object @this, string message=null, params object[] args)
        {
            Assert.That(@this, x=> ((T)x)!=null, message ??nameof(ShouldBeAssignableTo), args);

            return (T)@this ;
        }

        public static T ShouldBe<T>(this T @this, Expression<Func<T,bool>> expression, string message=null, params object[] args)
        {
            Assert.That(@this,expression, message??nameof(ShouldBe), args);
            return @this;
        }
        
        public static T ShouldNotBe<T>(this T @this, Expression<Func<T,bool>> predicate, string message=null, params object[] args)
        {
            var notPredicate = Expression.IsFalse(predicate) as Expression<Func<T,bool>>;
            Assert.That(@this, notPredicate, message??nameof(ShouldNotBe), args);
            return @this;
        }

        public static T ShouldSatisfy<T, TResult>(this T @this, Func<T, TResult> function, Expression<Func<TResult,bool>> expression, string message=null, params object[] args)
        {
            var result = function(@this);
            Assert.That(result, expression, message??nameof(ShouldSatisfy), args);
            return @this;
        }
        public static T ShouldSatisfy<T>(this T @this, Expression<Func<T, bool>> function, string message=null, params object[] args)
        {
            Assert.That(@this, function, message??nameof(ShouldSatisfy), args);
            return @this;
        }
        public static T Should<T, TResult>(this T @this, Func<T, TResult> function, Expression<Func<TResult,bool>> expression, string message=null, params object[] args)
        {
            var result = function(@this);
            Assert.That(result, expression, message??nameof(ShouldSatisfy), args);
            return @this;
        }
        public static T Should<T>(this T @this, Expression<Func<T, bool>> function, string message=null, params object[] args)
        {
            Assert.That(@this, function, message??nameof(ShouldSatisfy), args);
            return @this;
        }
    }
}
