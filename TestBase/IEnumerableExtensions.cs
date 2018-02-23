using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

//
// code taken from Castle.Core.Internal
//
namespace TestBase
{
    public static class IEnumerableExtensions
    {
        public static T Find<T>(this T[] items, Predicate<T> predicate)
        {
            return Array.Find<T>(items, predicate);
        }

        public static T[] FindAll<T>(this T[] items, Predicate<T> predicate)
        {
            return Array.FindAll<T>(items, predicate);
        }

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

    public static class IDictionaryExtensions
    {
        public static IDictionary<Tkey, TValue> ShouldHaveKey<Tkey, TValue>(this IDictionary<Tkey, TValue> dict, Tkey key, TValue value, string comment=null, params object[] args)
        {
            Assert.That(dict, d => d.ContainsKey(key), comment ?? $"Should Contain {key} but didn't.", args);
            Assert.That(dict[key], v=>v.EqualsByValue(value), comment ?? $"[{key}] ShouldEqualByValue({value})", args);
            return dict;
        }
        public static TValue ShouldHaveKey<Tkey, TValue>(this IDictionary<Tkey, TValue> dict, Tkey key, string comment=null, params object[] args)
        {
            Assert.That(dict, d => d.ContainsKey(key), comment ?? $"Should Contain {key}", args);
            return dict[key];
        }
    }

    public static class AnonymousObjectInspector
    {
        public static Dictionary<string, object> ToPropertyDictionary(this object obj, ReferenceLoopHandling jsonReferenceLoopHandling=ReferenceLoopHandling.Ignore)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(
                        JsonConvert.SerializeObject(
                            obj,
                            new JsonSerializerSettings{ReferenceLoopHandling = jsonReferenceLoopHandling}));
        }
    }

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