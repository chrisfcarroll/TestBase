using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace TestBase
{
    public static class IDictionaryShoulds
    {
        static void ThrowDictionaryAssertion<TKey, TValue>(IDictionary<TKey, TValue> actual, string assertionName, string assertedDetail, string message, object[] args,
            [CallerArgumentExpression("actual")] string actualExpression = null)
        {
            var comment = message != null && args?.Length > 0 ? string.Format(message, args) : message;
            throw new Assertion<IDictionary<TKey, TValue>>(
                actual?.ToString() ?? "null",
                actualExpression,
                assertionName,
                assertedDetail,
                comment,
                false);
        }

        /// <summary>
        ///     Assert that <paramref name="dict" /> contains the given <paramref name="key" /> and that it has value
        ///     <paramref name="value" />
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="comment"></param>
        /// <param name="args"></param>
        /// <returns>
        ///     <paramref name="dict" />
        /// </returns>
        public static IDictionary<Tkey, TValue> ShouldHaveKey<Tkey, TValue>(
            this IDictionary<Tkey, TValue> dict,
            Tkey                           key,
            TValue                         value,
            string                         comment = null,
            params object[]                args)
        {
            if (!dict.ContainsKey(key))
                ThrowDictionaryAssertion(dict, nameof(ShouldHaveKey),
                    $"Expected: dictionary containing key \"{key}\", but key was not found", comment ?? $"Should Contain {key} but didn't.", args);
            if (!dict[key].EqualsByValue(value))
                ThrowDictionaryAssertion(dict, nameof(ShouldHaveKey),
                    $"Expected: [{key}] equals {value}, Actual: [{key}] equals {dict[key]}", comment ?? $"[{key}] ShouldEqualByValue({value})", args);
            return dict;
        }

        /// <summary>Assert that <paramref name="dict" /> contains the given <paramref name="key" /></summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="comment"></param>
        /// <param name="args"></param>
        /// <returns>
        ///     <paramref name="dict" />
        /// </returns>
        public static TValue ShouldHaveKey<Tkey, TValue>(
            this IDictionary<Tkey, TValue> dict,
            Tkey                           key,
            string                         comment = null,
            params object[]                args)
        {
            if (!dict.ContainsKey(key))
                ThrowDictionaryAssertion(dict, nameof(ShouldHaveKey),
                    $"Expected: dictionary containing key \"{key}\", but key was not found", comment ?? $"Should Contain {key}", args);
            return dict[key];
        }
    }
}
