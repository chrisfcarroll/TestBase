using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ExpressionToCodeLib;

// ReSharper disable InconsistentNaming

namespace TestBase
{
    public static class IEnumerableShoulds
    {
        static void ThrowCollectionAssertion<T>(IEnumerable<T> actual, string assertionName, string assertedDetail, string message, object[] args,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
            string actualStr;
            try { actualStr = actual != null ? $"[{string.Join(", ", actual)}]" : "null"; }
            catch { actualStr = actual?.ToString() ?? "null"; }
            throw new Assertion<IEnumerable<T>>(
                actualStr,
                actualExpression,
                assertionName,
                assertedDetail,
                comment,
                false);
        }

        static void ThrowEnumerableAssertion(IEnumerable actual, string assertionName, string assertedDetail, string message, object[] args,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
            throw new Assertion<IEnumerable>(
                actual?.ToString() ?? "null",
                actualExpression,
                assertionName,
                assertedDetail,
                comment,
                false);
        }

        /// <summary>Asserts that <paramref name="actual" /> contains an element satisfying <paramref name="predicate" /></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldContain<T>(
            this IEnumerable<T>       actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           args)
        {
            return Assert.That(actual,
                               a => a.Any(predicate.Compile()),
                               comment ?? $"Should contain {ExpressionToCode.ToCode((Expression) predicate)}",
                               args);
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> does not contain an element satisfying <paramref name="predicate" />
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldNotContain<T>(
            this IEnumerable<T>       actual,
            Expression<Func<T, bool>> predicate,
            string                    comment = null,
            params object[]           args)
        {
            return Assert.That(actual,
                               a => !a.Any(predicate.Compile()),
                               comment ?? $"Should not contain {ExpressionToCode.ToCode((Expression) predicate)}",
                               args);
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> contains an element such that element.Equals(
        ///     <paramref name="expectedItem" />)
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldContain<T>(
            this IEnumerable<T> actual,
            T                   expectedItem,
            string              comment = null,
            params object[]     args)
        {
            if (!actual.Any(i => i.Equals(expectedItem)))
                ThrowCollectionAssertion(actual, nameof(ShouldContain),
                    $"Expected: collection containing {expectedItem}", comment, args);
            return actual;
        }

        /// <summary>
        ///     Asserts that <paramref name="actual" /> does not contains an element such that element.Equals(
        ///     <paramref name="unexpectedItem" />)
        /// </summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldNotContain<T>(
            this IEnumerable<T> actual,
            T                   unexpectedItem,
            string              comment = null,
            params object[]     args)
        {
            if (actual.Any(i => i.Equals(unexpectedItem)))
                ThrowCollectionAssertion(actual, nameof(ShouldNotContain),
                    $"Expected: collection not containing {unexpectedItem}", comment, args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> is not empty</summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldNotBeEmpty<T>(
            this IEnumerable<T> actual,
            string              comment = null,
            params object[]     args)
        {
            if (!actual.Any())
                ThrowCollectionAssertion(actual, nameof(ShouldNotBeEmpty),
                    "Expected: non-empty collection, Actual: empty", comment, args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> is not null or empty</summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        [return:NotNull]
        public static IEnumerable<T> ShouldNotBeNullOrEmpty<T>(
            this IEnumerable<T> actual,
            string              message = null,
            params object[]     args)
        {
            return actual.ShouldNotBeNull(message, args).ShouldNotBeEmpty(message, args);
        }

        /// <summary>Asserts that <paramref name="actual" /> is not empty</summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static T ShouldNotBeEmpty<T>(this T actual, string message = null, params object[] args)
        where T : IEnumerable
        {
            if (!actual.HasAnyElements())
                ThrowEnumerableAssertion(actual, nameof(ShouldNotBeEmpty),
                    "Expected: non-empty collection, Actual: empty", message, args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> is empty</summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldBeEmpty<T>(
            this IEnumerable<T> actual,
            string              comment = null,
            params object[]     args)
        {
            if (actual.Any())
                ThrowCollectionAssertion(actual, nameof(ShouldBeEmpty),
                    "Expected: empty collection, Actual: collection has elements", comment, args);
            return actual;
        }

        public static T ShouldBeEmpty<T>(this T actual, string message = null, params object[] args)
        where T : IEnumerable
        {
            if (actual.HasAnyElements())
                ThrowEnumerableAssertion(actual, nameof(ShouldBeEmpty),
                    "Expected: empty collection, Actual: collection has elements", message, args);
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> has no elements satisfying <paramref name="predicate" /></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldNotHaveAny<T>(
            this IEnumerable<T> actual,
            Func<T, bool>       predicate,
            string              comment = null,
            params object[]     args)
        {
            if (actual.Any(predicate))
                ThrowCollectionAssertion(actual, nameof(ShouldNotHaveAny),
                    "Expected: no items satisfying predicate, but found at least one", comment, args);
            return actual;
        }

        /// <summary>Synonym for <see cref="ShouldBeOfLength{T}" /></summary>
        /// <param name="actual">The object whose property is to be asserted</param>
        /// <param name="expected">expected length</param>
        /// <param name="comment">[Optional] override the default message if the assertion fails</param>
        /// <param name="args">[Optional] string.format() arguments for <paramref name="comment"/> in the case that the assertion fails</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldBeOfLength<T>(
            this IEnumerable<T> actual,
            int                 expected,
            string              comment = null,
            params object[]     args)
        {
            var count = actual.Count();
            if (count != expected)
                ThrowCollectionAssertion(actual, nameof(ShouldBeOfLength),
                    $"Expected: length {expected}, Actual: length {count}", comment, args);
            return actual;
        }

        /// <summary>Assert that <paramref name="actual" /> has <paramref name="expected" /> elements</summary>
        /// <param name="actual">The object whose property is to be asserted</param>
        /// <param name="expected">expected length</param>
        /// <param name="comment">[Optional] override the default message if the assertion fails</param>
        /// <param name="args">[Optional] string.format() arguments for <paramref name="comment"/> in the case that the assertion fails</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldHaveCount<T>(
            this IEnumerable<T> actual,
            int                 expected,
            string              comment = null,
            params object[]     args)
        {
            var count = actual.Count();
            if (count != expected)
                ThrowCollectionAssertion(actual, nameof(ShouldHaveCount),
                    $"Expected: count {expected}, Actual: count {count}", comment, args);
            return actual;
        }

        /// <summary>
        ///     returns the Single element of <paramref name="actual" /> or throws an <see cref="Assertion" /> if there isn't
        ///     exactly one element
        /// </summary>
        /// <returns>the single element</returns>
        public static T SingleOrAssertFail<T>(this IEnumerable<T> actual, string message = null, params object[] args)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            try { return actual.Single(); } catch
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var count = actual.Count();
                ThrowCollectionAssertion(actual, nameof(SingleOrAssertFail),
                    $"Expected: exactly 1 element, Actual: {count} elements",
                    message ?? "SingleOrAssertFail expected exactly 1", args);
                return default; // unreachable
            }
        }

        /// <summary>
        ///     returns the Single element of <paramref name="actual" /> which satisfies <paramref name="predicate" />
        ///     or throws an <see cref="Assertion" /> if there isn't exactly one element
        /// </summary>
        /// <returns>the single matched element</returns>
        public static T SingleOrAssertFail<T>(
            this IEnumerable<T> actual,
            Func<T, bool>       predicate,
            string              message = null,
            params object[]     args)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            try { return actual.Single(predicate); } catch
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var count = actual.Count(predicate);
                ThrowCollectionAssertion(actual, nameof(SingleOrAssertFail),
                    $"Expected: exactly 1 element matching predicate, Actual: {count} matching",
                    message ?? "SingleOrAssertFail expected exactly 1", args);
                return default; // unreachable
            }
        }

        /// <summary>Asserts that <paramref name="actual" /> contains the key <paramref name="key" /></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IDictionary<TKey, TValue> ShouldContainKey<TKey, TValue>(
            this IDictionary<TKey, TValue> actual,
            TKey                           key,
            string                         message = null,
            params object[]                args)
        {
            if (!actual.ContainsKey(key))
            {
                var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
                throw new Assertion<IDictionary<TKey, TValue>>(
                    actual?.ToString() ?? "null",
                    null,
                    nameof(ShouldContainKey),
                    $"Expected: dictionary containing key \"{key}\"",
                    comment ?? $"Expected IDictionary to contain key {key}",
                    false);
            }
            return actual;
        }

        /// <summary>Asserts that <paramref name="actual" /> does not contain the key <paramref name="key" /></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IDictionary<TKey, TValue> ShouldNotContainKey<TKey, TValue>(
            this IDictionary<TKey, TValue> actual,
            TKey                           key,
            string                         message = null,
            params object[]                args)
        {
            if (actual.ContainsKey(key))
            {
                var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
                throw new Assertion<IDictionary<TKey, TValue>>(
                    actual?.ToString() ?? "null",
                    null,
                    nameof(ShouldNotContainKey),
                    $"Expected: dictionary not containing key \"{key}\"",
                    comment ?? $"Expected IDictionary to not contain key {key}",
                    false);
            }
            return actual;
        }

        /// <summary>
        ///     Synonym for <see cref="ShouldContainEachOf{T}" />.
        ///     Assert that <paramref name="actual" /> contains each item of <paramref name="subset" />
        /// </summary>
        /// <param name="actual">The object whose property is to be asserted</param>
        /// <param name="subset">the expected subset</param>
        /// <param name="message">[Optional] override the default message if the assertion fails</param>
        /// <param name="args">[Optional] string.format() arguments for <paramref name="message"/> in the case that the assertion fails</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldContainEachItemOf<T>(
            this IEnumerable<T> actual,
            IEnumerable<T>      subset,
            string              message = null,
            params object[]     args)
        {
            foreach (var item in subset)
                if (!actual.Contains(item))
                    ThrowCollectionAssertion(actual, nameof(ShouldContainEachItemOf),
                        $"Expected: collection containing {item}", message, args);
            return actual;
        }

        /// <summary>Assert that <paramref name="actual" /> contains each item of <paramref name="subset" /></summary>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldContainEachOf<T>(
            this IEnumerable<T> actual,
            IEnumerable<T>      subset,
            string              message = null,
            params object[]     args)
        {
            foreach (var item in subset)
                if (!actual.Contains(item))
                    ThrowCollectionAssertion(actual, nameof(ShouldContainEachOf),
                        $"Expected: collection containing {item}", message, args);
            return actual;
        }


        public static List<T> ShouldContainInOrder<T>(
            this List<T>    actual,
            T               expectedFirst,
            T               expectedAfter,
            string          message = null,
            params object[] args)
        {
            actual
           .TakeWhile(t => !t.Equals(expectedAfter))
           .ShouldContain(expectedFirst, message, args);

            actual.ShouldContain(expectedAfter, message, args);
            return actual;
        }

        public static T[] ShouldContainInOrder<T>(
            this T[]        actual,
            T               expectedFirst,
            T               expectedAfter,
            string          message = null,
            params object[] args)
        {
            actual
           .TakeWhile(t => !t.Equals(expectedAfter))
           .ShouldContain(expectedFirst, message, args);

            actual.ShouldContain(expectedAfter, message, args);
            return actual;
        }

        /// <summary>
        ///     Assert that <paramref name="itemAssertion" /> is true of each item in <paramref name="actual" />
        /// </summary>
        /// <param name="actual">The enumerable on whose items <paramref name="itemAssertion"/> is to be asserted</param>
        /// <param name="itemAssertion">The assertion to apply</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldAll<T>(this IEnumerable<T> actual, Action<T> itemAssertion)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in actual) itemAssertion(item);
            return actual;
        }

        /// <summary>
        ///     Assert that <paramref name="constraintPerItem" /> is true of each item in <paramref name="actual" />
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        static IEnumerable<T> ShouldAll<T>(
            IEnumerable<T>            actual,
            Expression<Func<T, bool>> constraintPerItem,
            string                    message,
            object[]                  args)
        {
            foreach (var item in actual)
                try { Assert.That(item, constraintPerItem); } catch (Exception e)
                {
                    var innerMessage = e.Message;
                    Assert.That(
                                item,
                                i => false,
                                message + " : " + innerMessage,
                                args);
                }

            return actual;
        }

        /// <summary>
        ///     Apply <paramref name="transformation" />  to each item of <paramref name="actual" />,
        ///     and then Assert that <paramref name="constraintPerItem" /> is true of each transformed item in
        ///     <paramref name="actual" />
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        static IEnumerable<T> ShouldAll<T, TResult>(
            IEnumerable<T>                  actual,
            Func<T, TResult>                transformBeforeAsserting,
            Expression<Func<TResult, bool>> constraintPerItem,
            string                          message,
            object[]                        args)
        {
            foreach (var item in actual)
            {
                var r = transformBeforeAsserting(item);
                try { Assert.That(r, constraintPerItem); } catch (Exception e)
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
        ///     Synonym for <see cref="ShouldAll{T}" />
        ///     Assert that <paramref name="constraintPerItem" /> is true of each item in <paramref name="actual" />
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldAllBeSuchThat<T>(
            this IEnumerable<T>       actual,
            Expression<Func<T, bool>> function,
            string                    message = null,
            params object[]           args)
        {
            return ShouldAll(actual, function, message, args);
        }

        /// <summary>
        ///     Synonym for <see cref="ShouldAll{T}" />
        ///     Assert that <paramref name="constraintPerItem" /> is true of each item in <paramref name="actual" />
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldAllSatisfy<T>(
            this IEnumerable<T>       actual,
            Expression<Func<T, bool>> constraintPerItem,
            string                    message = null,
            params object[]           args)
        {
            return ShouldAll(actual, constraintPerItem, message, args);
        }

        /// <summary>
        ///     Synonym for <see cref="ShouldAll{T}" />
        ///     Apply <paramref name="transformBeforeAsserting" />  to each item of <paramref name="actual" />,
        ///     and then Assert that <paramref name="constraintPerItem" /> is true of each transformed item in
        ///     <paramref name="actual" />
        /// </summary>
        /// <param name="constraintPerItem">The constraint to test</param>
        /// <returns>
        ///     <paramref name="actual" />
        /// </returns>
        public static IEnumerable<T> ShouldAllSatisfy<T, TResult>(
            this IEnumerable<T>             actual,
            Func<T, TResult>                transformBeforeAsserting,
            Expression<Func<TResult, bool>> constraintPerItem,
            string                          message = null,
            params object[]                 args)
        {
            return ShouldAll(actual, transformBeforeAsserting, constraintPerItem, message, args);
        }
    }
}
