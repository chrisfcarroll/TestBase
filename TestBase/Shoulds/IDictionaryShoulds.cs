using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace TestBase
{
    public static class IDictionaryShoulds
    {
        /// <summary>Assert that <paramref name="dict"/> contains the given <paramref name="key"/> and that it has value <paramref name="value"/></summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="comment"></param>
        /// <param name="args"></param>
        /// <returns><paramref name="dict"/></returns>
        public static IDictionary<Tkey, TValue> ShouldHaveKey<Tkey, TValue>(this IDictionary<Tkey, TValue> dict, Tkey key, TValue value, string comment=null, params object[] args)
        {
            Assert.That(dict, d => d.ContainsKey(key), comment ?? $"Should Contain {key} but didn't.", args);
            Assert.That(dict[key], v=>v.EqualsByValue(value), comment ?? $"[{key}] ShouldEqualByValue({value})", args);
            return dict;
        }

        /// <summary>Assert that <paramref name="dict"/> contains the given <paramref name="key"/></summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="comment"></param>
        /// <param name="args"></param>
        /// <returns><paramref name="dict"/></returns>
        public static TValue ShouldHaveKey<Tkey, TValue>(this IDictionary<Tkey, TValue> dict, Tkey key, string comment=null, params object[] args)
        {
            Assert.That(dict, d => d.ContainsKey(key), comment ?? $"Should Contain {key}", args);
            return dict[key];
        }
    }
}