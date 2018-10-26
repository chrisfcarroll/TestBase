using System;
using System.Collections.Generic;
using System.Linq;

namespace TestBase
{
    public static class Generate
    {
        public static IEnumerable<T> Times<T>(int count, Func<int, T> generator)
        {
            return Enumerable.Range(1, count).Select(generator);
        }
        public static IEnumerable<T> Times<T>(this Func<int, T> generator, int count)
        {
            return Times(count, generator);
        }
    }
}