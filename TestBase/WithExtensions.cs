using System;
using System.Collections.Generic;

namespace TestBase
{
    public static class WithExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> ienumerable, Action<T> applyToEach)
        {
            foreach (var i in ienumerable) applyToEach(i);
            return ienumerable;
        }

        public static T With<T>(this T @this, Action<T> with)
        {
            with(@this);
            return @this;
        }

        public static TResult WithTransform<TSource, TResult>(
            this TSource           @this,
            Func<TSource, TResult> transformFunction)
        {
            return transformFunction(@this);
        }

        public static T Inspect<T>(this T @this)
        {
            //
            // Alas this breaks the resharper test runner both in VS and in TeamCity
            // System.Diagnostics.Debugger.Break();
            //
            Console.WriteLine(@this);
            return @this;
        }
    }
}
