using System.Collections.Generic;
using System.Linq;

#if !NET5_0_OR_GREATER
namespace TestBase
{
    static class EnumerablePolyfill
    {
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable,
                                                T item)
            => new List<T>() { item }.Union(enumerable);
    }
}
#endif