using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TestBase
{
    public static class IEnumerableExtensions
    {
        public static bool IsInList<T>(this T item, IEnumerable<T> list)
        {
            return list.Contains(item);
        }

        public static bool DoesNotContain<T>(this IEnumerable<T> list, T item)
        {
            return !list.Contains(item);
        }

        public static T SingleOrAssertFail<T>(this IEnumerable<T> @this, string message = null, params object[] args)
        {
            try
            {
                return @this.Single();
            }
            catch (InvalidOperationException e)
            {
                throw new AssertionException(string.Format(message ?? "Expected a single element but got " + e.Message, args));
            }
        }
        public static T SingleOrAssertFail<T>(this IEnumerable<T> @this, Func<T, bool> predicate, string message = null, params object[] args)
        {
            try
            {
                return @this.Single(predicate);
            }
            catch (InvalidOperationException e)
            {
                throw new AssertionException(string.Format(message ?? "Expected a single element but got " + e.Message, args));
            }
        }
    }
}
