using System;
using System.Collections.Generic;
using System.Linq;

namespace PredicateDictionary
{
    public class PredicateToFuncDictionary<T, TValue> : PredicateDictionary<T, Func<T, TValue>>
    {
        public new IEnumerable<KeyValuePair<Func<T, bool>, Func<T, TValue>>> AsEnumerable => base.AsEnumerable;

        /// <inheritdoc cref="PredicateDictionary{T,TValue}" />
        /// <summary>
        ///     <c>get</c>: If <paramref name="key" /> satisfies any of the predicates in the dictionary,
        ///     then let func be the function associated with the first matching predicate.
        ///     Then <c>func(key)</c> is returned.
        ///     <c>set:</c> throws <exception cref="InvalidOperationException"></exception>
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        ///     Thrown if <paramref name="key" /> does not satisfy any
        ///     predicate in the dictionary.
        /// </exception>
        /// <exception cref="InvalidOperationException"> if you call the setter.</exception>
        public new TValue this[T key]
        {
            get
            {
                Func<T, TValue> func;
                try { func = AsEnumerable.First(kv => kv.Key(key)).Value; } catch (InvalidOperationException e)
                {
                    throw new KeyNotFoundException("Sequence contains no matching element.");
                }

                return func(key);
            }
            set => throw new InvalidOperationException("You cannot set values on a PredicateToFuncDictionary. "
                                                     + "Instead, use PredicateToFuncDictionary.Add(predicate,function) to"
                                                     + "add a predicate,function pair.");
        }

        /// <inheritdoc cref="PredicateDictionary{T,TValue}.TryGetValue" />
        /// <summary>
        ///     Returns
        ///     <c>
        ///         true</> if <paramref name="key" /> satisfies any of the predicates in
        ///         the <see cref="PredicateDictionary{T,TValue}" />
        /// </summary>
        /// <param name="value">
        ///     Contains the value associated with the first matched predicate, if any,
        ///     or <c>default(TValue)</c> if not.
        /// </param>
        public bool TryGetValue(T key, out TValue value)
        {
            if (base.TryGetValue(key, out var funcValue))
            {
                value = funcValue(key);
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }
    }
}
