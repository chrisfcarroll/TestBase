using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace TestBase.Shoulds
{
    // ReSharper disable PossibleMultipleEnumeration
    public static class IEnumerableShoulds
    {
        public static IDictionary<TKey,TValue> ShouldContainKey<TKey,TValue>(this IDictionary<TKey,TValue> @this, TKey key, [Optional] string message, params object[] args)
        {
            message = message ?? string.Format("Expected IDictionary to contain key {0}", key);
            Assert.True(@this.ContainsKey(key), message, args);
            return @this;
        }

        public static IDictionary<TKey, TValue> ShouldNotContainKey<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, [Optional] string message, params object[] args)
        {
            message = message ?? string.Format("Expected IDictionary to not contain key {0}", key);
            Assert.False(@this.ContainsKey(key), message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> @this, T item, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Contains.Item(item), message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> @this, Func<T,bool> itemPredicate, [Optional] string message, params object[] args)
        {
            @this.Where(itemPredicate).ShouldNotBeEmpty(message?? string.Format("No item was found satisfying the predicate {0}",itemPredicate.ToString()));
            return @this;
        }

        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> @this, Func<T,bool> itemAssertion, [Optional] string message, params object[] args)
        {
            foreach (var item in @this)
            {
                Assert.That(itemAssertion(item), Is.Not.True, message?? "Shouldn't contain an item satisfying the condition but did : {0}", args.Length>0?args:new object[]{item});
            }
            return @this;
        }

        public static IEnumerable<T> ShouldContainEachItemOf<T>(this IEnumerable<T> @this, IEnumerable<T> subset, [Optional] string message, params object[] args)
        {
            foreach (var item in subset)
            {
                Assert.That(@this, Contains.Item(item), message, args);
            }
            return @this;
        }

        public static T ShouldBeEmpty<T>(this T @this, [Optional] string message, params object[] args) where T : IEnumerable
        {
            Assert.That(@this, Is.Empty, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Empty, message, args);
            return @this;
        }

        public static T ShouldNotBeEmpty<T>(this T @this, [Optional] string message, params object[] args) where T : IEnumerable
        {
            Assert.That(@this, Is.Not.Empty, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> @this, [Optional] string message, params object[] args)
        {
            Assert.That(@this, Is.Not.Empty, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldNotBeNullOrEmpty<T>(this IEnumerable<T> @this, [Optional] string message, params object[] args)
        {
            return @this.ShouldNotBeNull(message, args).ShouldNotBeEmpty(message, args);
        }

        public static List<T> ShouldContainInOrder<T>(this List<T> @this, T expectedFirst, T expectedAfter, [Optional] string message, params object[] args)
        {
            @this
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            @this.ShouldContain(expectedAfter, message, args);
            return @this;
        }

        public static T[] ShouldContainInOrder<T>(this T[] @this, T expectedFirst, T expectedAfter, [Optional] string message, params object[] args)
        {
            @this
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            @this.ShouldContain(expectedAfter, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldAllSatisfy<T, TResult>(this IEnumerable<T> @this, Func<T, TResult> function, IResolveConstraint constraintPerItem, [Optional] string message, params object[] args)
        {
            foreach (var item in @this)
            {
                var r = function(item);
                try { Assert.That(r, constraintPerItem); }
                catch (AssertionException e)
                {
                    var innerMessage = e.Message;
                    Assert.That(
                            item,
                            new PredicateConstraint<T>(i => false),
                            message + " : " + innerMessage,
                            args);
                }
            }
            return @this;
        }

        public static IEnumerable<T> ShouldAllBeSuchThat<T>(this IEnumerable<T> @this, Predicate<T> function, [Optional] string message, params object[] args)
        {
            foreach (var item in @this)
            {
                Assert.That(item, new PredicateConstraint<T>(function), message, args);
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

        public static IEnumerable<T> ShouldBeOfLength<T>(this IEnumerable<T> @this, int expectedLength, [Optional] string message, params object[] args)
        {
            @this.Count().ShouldEqual(expectedLength,message,args);
            return @this;
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> @this, int expectedLength, [Optional] string message, params object[] args)
        {
            return ShouldBeOfLength(@this, expectedLength, message, args);
        }

    }
    // ReSharper restore PossibleMultipleEnumeration
}