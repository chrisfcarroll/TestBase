using System;
using System.Collections.Generic;
using System.Linq;

namespace TestBase
{
    public static class BasicShoulds
    {
        public static T ShouldNotBeNull<T>(this T actual, string comment=null, params object[] args)
        {
            return Assert.That(actual, x=>x!=null, comment ?? nameof(ShouldNotBeNull), args);
        }

        public static T ShouldBeNull<T>(this T actual, string comment=null, params object[] args)
        {
            return Assert.That(actual, x=>x==null, comment ?? nameof(ShouldBeNull), args);
        }

        public static string ShouldBeNullOrEmpty(this string actual, string comment=null, params object[] args)
        {
            return Assert.That(actual, a=>string.IsNullOrEmpty(a), comment ?? "Should Be Null Or Empty", args);
        }

        public static string ShouldBeEmpty(this string actual, string comment=null, params object[] args)
        {
            return Assert.That(actual, a=>a.Length == 0, comment ?? "Should Be Empty", args);
        }

        public static T ShouldBeNullOrEmptyOrWhitespace<T>(this T actual, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => a== null || a.ToString().Trim().Length == 0, comment ?? nameof(ShouldBeNullOrEmptyOrWhitespace), args);
        }

        public static T ShouldNotBeNullOrEmptyOrWhitespace<T>(this T actual, string comment=null, params object[] args)
        {
            return Assert.That(actual, a=> a != null && a.ToString().Trim().Length != 0, comment ?? nameof(ShouldNotBeNullOrEmptyOrWhitespace), args);
        }

        public static T ShouldEqual<T>(this T actual, object expected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a=>a.Equals(expected), comment ?? $"{nameof(ShouldEqual)}\n\n{expected}", args);
        }

        public static T ShouldBe<T>(this T actual, T expected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a=>a.Equals(expected), comment ?? $"{nameof(ShouldBe)}\n\n{expected}", args);
        }

        public static T ShouldNotEqual<T>(this T actual, T notExpected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => !a.Equals(notExpected), comment ?? $"{nameof(ShouldNotEqual)}\n\n{notExpected}", args);
        }

        public static T ShouldNotBe<T>(this T actual, T notExpected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => !a.Equals(notExpected), comment?? $"{nameof(ShouldNotEqual)}\n\n{notExpected}", args);
        }

        public static T ShouldBeBetween<T>(this T actual, T left, T right, string comment=null, params object[] args) where T : IComparable<T>
        {
            return Assert.That(
                actual,  
                a=> left.CompareTo(a)<=0 && a.CompareTo(right)<=0, 
                comment?? $"ShouldBeBetween\n\n{left}\n\n{right}", args);
        }

        public static bool ShouldBeTrue(this bool actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a, comment ?? nameof(ShouldBeTrue), args);
        }

        public static BoolWithString ShouldBeTrue(this BoolWithString actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Equals(true), comment ?? nameof(ShouldBeTrue), args);
        }

        public static T ShouldBeTrue<T>(this T actual, string comment = null, params object[] args)
        {
            try
            {
                var actualAsBool = Convert.ToBoolean(actual);
                return Assert.That(actual, a => actualAsBool, comment ?? nameof(ShouldBeTrue), args); ;
            }
            catch (Exception e)
            {
                return Assert.That(actual, a => BoolWithString.False(e.Message), comment ?? nameof(ShouldBeTrue), args);
            }
        }

        public static bool ShouldBeFalse(this bool actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a, comment ?? nameof(ShouldBeFalse), args);
        }

        public static BoolWithString ShouldBeFalse(this BoolWithString actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a, comment ?? nameof(ShouldBeFalse), args);
        }

        public static T ShouldBeFalse<T>(this T actual, string comment=null, params object[] args)
        {
            try
            {
                var actualAsBool = Convert.ToBoolean(actual);
                return Assert.That(actual, a => !actualAsBool, comment??"ShouldBeFalse", args);
            }
            catch (Exception e)
            {
                return Assert.That(actual, a => BoolWithString.False(e.Message), comment ?? nameof(ShouldBeFalse), args);
            }
        }

        public static T ShouldBeGreaterThan<T,T2>(this T actual, T2 threshold, string comment=null, params object[] args) where T : IComparable<T2>
        {
            return Assert.That(actual, a => a.CompareTo(threshold) > 0, comment?? $"{nameof(ShouldBeGreaterThan)}\n\n{threshold}", args);
        }

        public static T ShouldBeGreaterThanOrEqualTo<T,T2>(this T actual, T2 threshold, string comment=null, params object[] args) where T : IComparable<T2>
        {
            return Assert.That(actual, a => a.CompareTo(threshold) >= 0, comment?? $"{nameof(ShouldBeGreaterThanOrEqualTo)}\n\n{threshold}", args);
        }

        public static T ShouldBeLessThan<T, T2>(this T actual, T2 threshold, string comment = null, params object[] args) where T : IComparable<T2>
        {
            return Assert.That(actual, a => a.CompareTo(threshold) < 0, comment?? $"{nameof(ShouldBeLessThan)}\n\n{threshold}", args);
        }

        public static T ShouldBeLessThanOrEqualTo<T, T2>(this T actual, T2 threshold, string comment = null, params object[] args) where T : IComparable<T2>
        {
            return Assert.That(actual, a => a.CompareTo(threshold) <= 0, comment?? $"{nameof(ShouldBeLessThanOrEqualTo)}\n\n{threshold}", args);
        }


        public static T ShouldBeOfType<T>(this object actual, string comment=null, params object[] args)
        {
            return (T) Assert.That(actual, a=> a is T, comment ?? $"Should Be Of Type: {typeof(T).FullName.TruncateTo(20)}\nWas: {actual.GetType().Name.TruncateTo(20)}", args);
        }

        public static T ShouldBeOfTypeEvenIfNull<T>(this T actual, Type type, string comment=null, params object[] args) where T : class
        {
            return Assert.That(actual, a => typeof(T)==type, comment ?? $"Should Be Of Type: {typeof(T).FullName.TruncateTo(20)} (event if null)\nWas: {actual.GetType().Name.TruncateTo(20)}", args);
        }

        public static T ShouldBeAssignableTo<T>(this object actual, string comment=null, params object[] args) where T : class
        {
            return Assert.That(actual, a=> a is T, comment??$"Should Be Assignable To: {typeof(T).FullName.TruncateTo(20)}\nWas: {actual.GetType().Name.TruncateTo(20)}", args) as T;
        }
        public static T ShouldBeCastableTo<T>(this object actual, string comment = null, params object[] args)
        {
            return (T)Assert.That(actual, a => a is T, comment ?? $"Should Be Castable To: {typeof(T).FullName.TruncateTo(20)}\nWas: {actual.GetType().Name.TruncateTo(20)}", args);
        }
        public static T As<T>(this object actual, string comment = null, params object[] args)
        {
            return (T)Assert.That(actual, a => a is T, comment ?? $"Should Be Castable To: {typeof(T).FullName.TruncateTo(20)}\nWas: {actual.GetType().Name.TruncateTo(20)}", args);
        }

        public static T ShouldSatisfy<T>(this T actual, Action<T> assertion) { assertion(actual); return actual; }

        public static T ShouldSatisfy<T>(this T actual, Func<T, bool> assertion, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => assertion(a), comment, args);
        }

        public static T ShouldNotSatisfy<T>(this T actual, Func<T, bool> predicate, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => !predicate(a), comment, args);
        }
    }
}