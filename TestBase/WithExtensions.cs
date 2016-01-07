using System;

namespace TestBase
{
    public static class WithExtensions
    {
        public static T With<T>(this T @this, Action<T> with)
        {
            with(@this);
            return @this;
        }

        public static TResult WithTransform<TSource, TResult>(this TSource @this, Func<TSource, TResult> transformFunction)
        {
            return transformFunction(@this);
        }

        [Obsolete("Renamed to WithTransform")]
        public static TResult Transform<TSource, TResult>(this TSource @this, Func<TSource, TResult> transformFunction)
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