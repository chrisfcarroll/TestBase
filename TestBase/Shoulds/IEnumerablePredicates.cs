using System.Collections.Generic;
using System.Linq;

namespace TestBase
{
    public static class IEnumerablePredicates
    {
        /// <summary>Synonym for <c>list.Contains(item)</c></summary>
        /// <returns><c>true</c> iff <c>list.Contains(item)</c></returns>
        public static bool IsInList<T>(this T item, params T[] list) { return list.Contains(item); }

        /// <summary>Synonym for <c>list.Contains(item)</c></summary>
        /// <returns><c>true</c> iff <c>list.Contains(item)</c></returns>
        public static bool IsInList<T>(this T item, IEnumerable<T> list) { return list.Contains(item); }

        /// <summary>Synonym for <c>!list.Contains(item)</c></summary>
        /// <returns><c>true</c> iff not <c>list.Contains(item)</c></returns>
        public static bool IsNotInList<T>(this T item, params T[] list) { return !list.Contains(item); }

        /// <summary>Synonym for <c>!list.Contains(item)</c></summary>
        /// <returns><c>true</c> iff not <c>list.Contains(item)</c></returns>
        public static bool IsNotInList<T>(this T item, IEnumerable<T> list) { return !list.Contains(item); }

        /// <summary>Synonym for <c>!list.Contains(item)</c></summary>
        /// <returns><c>true</c> iff not <c>list.Contains(item)</c></returns>
        public static bool DoesNotContain<T>(this IEnumerable<T> list, T item) { return !list.Contains(item); }
    }
}
