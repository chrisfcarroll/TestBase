using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace TestBase.Shoulds
{
    // ReSharper disable PossibleMultipleEnumeration
    public static class IEnumerableShoulds
    {
        public static IDictionary<TKey,TValue> ShouldContainKey<TKey,TValue>(this IDictionary<TKey,TValue> @this, TKey key, string message=null, params object[] args)
        {
            message = message ?? string.Format("Expected IDictionary to contain key {0}", key);
            Assert.That(@this.ContainsKey(key), message, args);
            return @this;
        }

        public static IDictionary<TKey, TValue> ShouldNotContainKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, string message=null, params object[] args)
        {
            message = message ?? string.Format("Expected IDictionary to not contain key {0}", key);
            Assert.That(!@this.ContainsKey(key), message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> @this, T item, string message=null, params object[] args)
        {
            Assert.That(@this, x=>x.Contains(item), message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> @this, Func<T,bool> itemPredicate, string message=null, params object[] args)
        {
            @this.Where(itemPredicate).ShouldNotBeEmpty(message?? string.Format("No item was found satisfying the predicate {0}",itemPredicate.ToString()));
            return @this;
        }

        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> @this, Func<T,bool> itemPredicate, string message=null, params object[] args)
        {
            @this.Where(itemPredicate).ShouldBeEmpty(message ?? string.Format("Should not contain an item satisfying the predicate but did : {0}",itemPredicate.ToString()));
            return @this;
        }

        public static IEnumerable<T> ShouldContainEachItemOf<T>(this IEnumerable<T> @this, IEnumerable<T> subset, string message=null, params object[] args)
        {
            foreach (var item in subset)
            {
                Assert.That(@this, x=>x.Contains(item), message, args);
            }
            return @this;
        }

        public static T ShouldBeEmpty<T>(this T @this, string message=null, params object[] args) where T : IEnumerable
        {
            Assert.That(@this, Is.Empty, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.Empty, message, args);
            return @this;
        }

        public static T ShouldNotBeEmpty<T>(this T @this, string message=null, params object[] args) where T : IEnumerable
        {
            Assert.That(@this, x=>x.HasAnyElements(), message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> @this, string message=null, params object[] args)
        {
            Assert.That(@this, Is.NotEmpty, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldNotBeNullOrEmpty<T>(this IEnumerable<T> @this, string message=null, params object[] args)
        {
            return @this.ShouldNotBeNull(message, args).ShouldNotBeEmpty(message, args);
        }

        public static List<T> ShouldContainInOrder<T>(this List<T> @this, T expectedFirst, T expectedAfter, string message=null, params object[] args)
        {
            @this
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            @this.ShouldContain(expectedAfter, message, args);
            return @this;
        }

        public static T[] ShouldContainInOrder<T>(this T[] @this, T expectedFirst, T expectedAfter, string message=null, params object[] args)
        {
            @this
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            @this.ShouldContain(expectedAfter, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldAllSatisfy<T, TResult>(this IEnumerable<T> @this, Func<T, TResult> function, Expression<Func<TResult,bool>> constraintPerItem, string message=null, params object[] args)
        {
            foreach (var item in @this)
            {
                var r = function(item);
                try { Assert.That(r, constraintPerItem); }
                catch (Exception e)
                {
                    var innerMessage = e.Message;
                    Assert.That(
                            item,
                            i => false,
                            message + " : " + innerMessage,
                            args);
                }
            }
            return @this;
        }

        public static IEnumerable<T> ShouldAllBeSuchThat<T>(this IEnumerable<T> @this, Func<T,bool> function, string message=null, params object[] args)
        {
            foreach (var item in @this)
            {
                Assert.That(item, x=>function(x), message, args);
            }
            return @this;
        }

        public static IEnumerable<T> ShouldAll<T>(this IEnumerable<T> @this, Action<T> itemAssertion)
        {
            foreach (var item in @this)
            {
                itemAssertion(item);
            }
            return @this;
        }

        public static IEnumerable<T> ShouldBeOfLength<T>(this IEnumerable<T> @this, int expectedLength, string message=null, params object[] args)
        {
            @this.Count().ShouldEqual(expectedLength,message,args);
            return @this;
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> @this, int expectedLength, string message=null, params object[] args)
        {
            return ShouldBeOfLength(@this, expectedLength, message, args);
        }

    }
    // ReSharper restore PossibleMultipleEnumeration
}