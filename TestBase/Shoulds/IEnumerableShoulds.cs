using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestBase.Mono.Linq.Expressions;

namespace TestBase
{
    public static class IEnumerableShoulds
    {
        /// <summary>Asserts that <paramref name="actual"/> contains an element satisfying <paramref name="predicate"/></summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> actual, Expression<Func<T,bool>> predicate,string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(predicate.Compile()), comment ?? $"Should contain {predicate.ToCSharpCode()}", args);
        }

        /// <summary>Asserts that <paramref name="actual"/> does not contain an element satisfying <paramref name="predicate"/></summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> actual, Expression<Func<T,bool>> predicate,string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a.Any(predicate.Compile()), comment ?? $"Should not contain {predicate.ToCSharpCode()}", args);
        }

        /// <summary>Asserts that <paramref name="actual"/> contains an element such that element.Equals(<paramref name="expectedItem"/>)</summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> actual, T expectedItem, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(i=>i.Equals(expectedItem) ), comment ?? $"Should contain {expectedItem}", args);
        }

        /// <summary>Asserts that <paramref name="actual"/> does not contains an element such that element.Equals(<paramref name="unexpectedItem"/>)</summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> actual, T unexpectedItem, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a.Any(i=>i.Equals(unexpectedItem) ), comment ?? $"Should not contain {unexpectedItem}", args);
        }

        /// <summary>Asserts that <paramref name="actual"/> is not empty</summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldNotBeEmpty<T>(this IEnumerable<T> actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Any(), comment??"Should not be empty but was.", args);
        }

        /// <summary>Asserts that <paramref name="actual"/> is not null or empty</summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldNotBeNullOrEmpty<T>(this IEnumerable<T> actual, string message=null, params object[] args)
        {
            return actual.ShouldNotBeNull(message, args).ShouldNotBeEmpty(message, args);
        }

        /// <summary>Asserts that <paramref name="actual"/> is not empty</summary>
        /// <returns><paramref name="actual"/></returns>
        public static T ShouldNotBeEmpty<T>(this T actual, string message=null, params object[] args) where T : IEnumerable
        {
            Assert.That(actual, x=>x.HasAnyElements(), message, args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual"/> is empty</summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> actual, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a.Any(), comment, args);
        }
        public static T ShouldBeEmpty<T>(this T actual, string message=null, params object[] args) where T : IEnumerable
        {
            Assert.That(actual, Is.Empty, message, args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual"/> has no elements satisfying <paramref name="predicate"/></summary>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldNotHaveAny<T>(this IEnumerable<T> actual, Func<T,bool> predicate, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => !a.Any(predicate), comment ??"ShouldNotHaveAny item satisfying", args);
        }
        
        /// <summary>Synonym for <see cref="ShouldBeOfLength{T}"/></summary>
        /// <param name="expected">expected length</param>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldBeOfLength<T>(this IEnumerable<T> actual, int expected, string comment=null, params object[] args)
        {
            return Assert.That(actual, a => a.Count() == expected, comment??$"ShouldBeOfLength({expected})", args);
        }

        /// <summary>Assert that <paramref name="actual"/> has <paramref name="expected"/> elements</summary>
        /// <param name="expected">expected length</param>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected, string comment = null, params object[] args)
        {
            return Assert.That(actual, a => a.Count() == expected, comment??$"ShouldHaveCount({expected})", args);
        }

        /// <summary>returns the Single element of <paramref name="actual"/> or throws an <see cref="Assertion"/> if there isn't exactly one element</summary>
        /// <returns>the single element</returns>
        public static T SingleOrAssertFail<T>(this IEnumerable<T> actual, string message = null, params object[] args)
        {
            try
            {
                return actual.Single();
            }
            catch
            {
                throw new Assertion<IEnumerable<T>>(actual, s=>s.Count()==1,message?? "SingleOrAssertFail expected exactly 1", args);
            }
        }

        /// <summary>returns the Single element of <paramref name="actual"/> which satisfies <paramref name="predicate"/> 
        /// or throws an <see cref="Assertion"/> if there isn't exactly one element</summary>
        ///<returns>the single matched element</returns>
        public static T SingleOrAssertFail<T>(this IEnumerable<T> actual, Func<T, bool> predicate, string message = null, params object[] args)
        {
            try
            {
                return actual.Single(predicate);
            }
            catch
            {
                throw new Assertion<IEnumerable<T>>(actual, s => s.Count(predicate) == 1, message?? "SingleOrAssertFail expected exactly 1", args);
            }
        }
        
        /// <summary>Asserts that <paramref name="actual"/> contains the key <paramref name="key"/></summary>
        /// <returns><paramref name="actual"/></returns>
        public static IDictionary<TKey,TValue> ShouldContainKey<TKey,TValue>(this IDictionary<TKey,TValue> actual, TKey key, string message=null, params object[] args)
        {
            message = message ?? string.Format("Expected IDictionary to contain key {0}", key);
            Assert.That(actual.ContainsKey(key), message, args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual"/> does not contain the key <paramref name="key"/></summary>
        /// <returns><paramref name="actual"/></returns>
        public static IDictionary<TKey, TValue> ShouldNotContainKey<TKey, TValue>(this IDictionary<TKey, TValue> actual, TKey key, string message=null, params object[] args)
        {
            message = message ?? string.Format("Expected IDictionary to not contain key {0}", key);
            Assert.That(!actual.ContainsKey(key), message, args);
            return actual;
        }

        public static IEnumerable<T> ShouldContainEachItemOf<T>(this IEnumerable<T> actual, IEnumerable<T> subset, string message=null, params object[] args)
        {
            foreach (var item in subset)
            {
                Assert.That(actual, x=>x.Contains(item), message, args);
            }
            return actual;
        }

        public static List<T> ShouldContainInOrder<T>(this List<T> actual, T expectedFirst, T expectedAfter, string message=null, params object[] args)
        {
            actual
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            actual.ShouldContain(expectedAfter, message, args);
            return actual;
        }

        public static T[] ShouldContainInOrder<T>(this T[] actual, T expectedFirst, T expectedAfter, string message=null, params object[] args)
        {
            actual
                .TakeWhile(t => !t.Equals(expectedAfter))
                .ShouldContain(expectedFirst, message, args);

            actual.ShouldContain(expectedAfter, message, args);
            return actual;
        }

        /// <summary>
        /// Assert that <paramref name="itemAssertion"/> is true of each item in <paramref name="actual"/>
        /// </summary>
        /// <param name="itemAssertion">The assertion to apply</param>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldAll<T>(this IEnumerable<T> actual, Action<T> itemAssertion)
        {
            foreach (var item in actual)
            {
                itemAssertion(item);
            }
            return actual;
        }

        /// <summary>
        /// Assert that <paramref name="constraintPerItem"/> is true of each item in <paramref name="actual"/>
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns><paramref name="actual"/></returns>
        static IEnumerable<T> ShouldAll<T>(IEnumerable<T> actual, Expression<Func<T, bool>> constraintPerItem, string message, object[] args)
        {
            foreach (var item in actual)
            {
                try
                {
                    Assert.That(item, constraintPerItem);
                }
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
            return actual;
        }

        /// <summary>
        /// Apply <paramref name="transformation"/>  to each item of <paramref name="actual"/>,
        /// and then Assert that <paramref name="constraintPerItem"/> is true of each transformed item in <paramref name="actual"/>
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns><paramref name="actual"/></returns>
        static IEnumerable<T> ShouldAll<T, TResult>(IEnumerable<T> actual, Func<T, TResult> transformBeforeAsserting, Expression<Func<TResult, bool>> constraintPerItem, string message, object[] args)
        {
            foreach (var item in actual)
            {
                var r = transformBeforeAsserting(item);
                try
                {
                    Assert.That(r, constraintPerItem);
                }
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

            return actual;
        }


        /// <summary>
        /// Synonym for <see cref="ShouldAll{T}"/>
        /// Assert that <paramref name="constraintPerItem"/> is true of each item in <paramref name="actual"/>
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldAllBeSuchThat<T>(this IEnumerable<T> actual, Expression<Func<T, bool>> function,string message = null, params object[] args) => ShouldAll(actual,function,message,args);

        /// <summary>
        /// Synonym for <see cref="ShouldAll{T}"/>
        /// Assert that <paramref name="constraintPerItem"/> is true of each item in <paramref name="actual"/>
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldAllSatisfy<T>(this IEnumerable<T> actual, Expression<Func<T,bool>> constraintPerItem, string message=null, params object[] args) => ShouldAll(actual, constraintPerItem, message, args);

        /// <summary>
        /// Synonym for <see cref="ShouldAll{T}"/>
        /// Apply <paramref name="transformBeforeAsserting"/>  to each item of <paramref name="actual"/>,
        /// and then Assert that <paramref name="constraintPerItem"/> is true of each transformed item in <paramref name="actual"/>
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns><paramref name="actual"/></returns>
        public static IEnumerable<T> ShouldAllSatisfy<T, TResult>(this IEnumerable<T> actual, Func<T, TResult> transformBeforeAsserting, Expression<Func<TResult,bool>> constraintPerItem, string message=null, params object[] args)
        {
            return ShouldAll(actual, transformBeforeAsserting, constraintPerItem, message, args);
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