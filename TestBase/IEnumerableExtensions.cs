using System;
using System.Collections;
using System.Collections.Generic;

//
// code taken from Castle.Core.Internal
//
namespace TestBase
{
    public static class EnumerableExtensions
    {
        /// <summary>Execute <paramref name="action"/> on each item in <paramref name="source"/></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns><paramref name="source"/></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source , Action<T> action)
        {
            foreach (var item in source) action(item);
            return source;
        }

        /// <summary>Find the first element of <paramref name="items"/> which satisfies <paramref name="predicate"/></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="predicate"></param>
        /// <returns><c>Array.Find{T}(<paramref name="items"/>, <paramref name="predicate"/>)</c> </returns>
        public static T Find<T>(this T[] items, Func<T,bool> predicate)
        {
            
            return Array.Find<T>(items, new Predicate<T>(predicate));
        }

        /// <summary>Find all elemente of <paramref name="items"/> which satisfy <paramref name="predicate"/></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="predicate"></param>
        /// <returns><c>Array.FindAll{T}(<paramref name="items"/>, <paramref name="predicate"/>)</c> </returns>
        public static T[] FindAll<T>(this T[] items, Func<T,bool> predicate)
        {
            return Array.FindAll<T>(items, new Predicate<T>(predicate));
        }

        /// <summary>Return true if <paramref name="@this"/> is null or has no elements</summary>
        /// <param name="this"></param>
        /// <returns><c>@this == null || !@this.GetEnumerator().MoveNext()</c>  </returns>
        public static bool IsNullOrEmpty(this IEnumerable @this)
        {
            return @this == null || !@this.GetEnumerator().MoveNext();
        }

        public static int GetContentsHashCode<T>(IList<T> list)
        {
            if (list == null)return 0;
            int num = 0;
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index] != null) {num += list[index].GetHashCode();}
            }
            return num;
        }
    }
}