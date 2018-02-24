using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mono.Linq.Expressions;

namespace TestBase
{
    public static class IEnumerableShoulds
    {
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> actual, Expression<Func<T,bool>> predicate,string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(predicate.Compile()), comment ?? $"Should contain {predicate.ToCSharpCode()}", args);
        }
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> actual, T expectedItem, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(i=>i.Equals(expectedItem) ), comment ?? $"Should contain {expectedItem}", args);
        }

        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(), comment??"Should not be empty but was.", args);
        }

        public static IEnumerable<T> ShouldNotBeNullOrEmpty<T>(this IEnumerable<T> @this, string message=null, params object[] args)
        {
            return @this.ShouldNotBeNull(message, args).ShouldNotBeEmpty(message, args);
        }

        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a.Any(), comment, args);
        }

        public static T ShouldNotBeEmpty<T>(this T @this, string message=null, params object[] args) where T : IEnumerable
        {
            Assert.That(@this, x=>x.HasAnyElements(), message, args);
            return @this;
        }

        public static T ShouldBeEmpty<T>(this T @this, string message=null, params object[] args) where T : IEnumerable
        {
            Assert.That(@this, Is.Empty, message, args);
            return @this;
        }

        public static IEnumerable<T> ShouldNotHaveAny<T>(this IEnumerable<T> actual, Func<T,bool> predicate, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a.Any(predicate), comment ??"ShouldNotHaveAny item satisfying", args);
        }
        
        public static IEnumerable<T> ShouldBeOfLength<T>(this IEnumerable<T> actual, int expected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => a.Count() == expected, comment, args);
        }
        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Count() == expected, comment, args);
        }

        public static T SingleOrAssertFail<T>(this IEnumerable<T> @this, string message = null, params object[] args)
        {
            try
            {
                return @this.Single();
            }
            catch
            {
                throw new Assertion<IEnumerable<T>>(@this, s=>s.Count()==1,message,args);
            }
        }
        public static T SingleOrAssertFail<T>(this IEnumerable<T> @this, Func<T, bool> predicate, string message = null, params object[] args)
        {
            try
            {
                return @this.Single(predicate);
            }
            catch
            {
                throw new Assertion<IEnumerable<T>>(@this, s => s.Count(predicate) == 1, message?? "SingleOrAssertFail expected exactly 1", args);
            }
        }
        
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

        public static IEnumerable<T> ShouldContainEachItemOf<T>(this IEnumerable<T> @this, IEnumerable<T> subset, string message=null, params object[] args)
        {
            foreach (var item in subset)
            {
                Assert.That(@this, x=>x.Contains(item), message, args);
            }
            return @this;
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
    }

    public static class IEnumerablePredicates
    {
        public static bool IsInList<T>(this T item, IEnumerable<T> list)
        {
            return list.Contains(item);
        }

        public static bool DoesNotContain<T>(this IEnumerable<T> list, T item)
        {
            return !list.Contains(item);
        }
    }
}