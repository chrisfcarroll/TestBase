using System.Collections.Generic;
using System.Linq;

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
    }
}
