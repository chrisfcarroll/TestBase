using System;
using System.Collections.Generic;
using System.Linq;

namespace PredicateDictionary
{
    /// <inheritdoc cref="List{T}" />
    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}" />
    /// <summary>
    ///     PredicateDictionary is something like a Dictionary, but with predicates for keys
    ///     rather than fixed values. So, <see cref="this[T]" /> will return a value if <c>T</c>
    ///     satisfies a predicate in the PredicateDictionary; and the value returned will be the
    ///     value associated with the first matching predicate.
    /// </summary>
    /// <example>
    ///     <c>
    ///         var dict= new PredicateDictionary
    ///         <string, string>
    ///             ();
    ///             dict.Add( s=> s.StartsWith("A"), "Here");
    ///             var value=dict["Abc"] ; // returns "Here";
    ///             var value=dict["A"] ; // returns "Here";
    ///             var value=dict["B"]} ; // throws KeyNotFoundException
    ///     </c>
    /// </example>
    public class PredicateDictionary<T, TValue> : List<KeyValuePair<Func<T, bool>, TValue>>,
                                                  IReadOnlyDictionary<T, TValue>
    {
        public IEnumerable<KeyValuePair<Func<T, bool>, TValue>> AsEnumerable => this;

        /// <summary>
        ///     If <paramref name="keyPredicate" /> is a predicate in the dictionary, returns
        ///     the associated value.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        ///     Thrown if <paramref name="key" /> does not satisfy any
        ///     predicate in the dictionary.
        /// </exception>
        public TValue this[Func<T, bool> keyPredicate]
        {
            get
            {
                try { return AsEnumerable.First(kv => kv.Key == keyPredicate).Value; } catch (InvalidOperationException
                    e) { throw new KeyNotFoundException("Sequence contains no matching element."); }
            }
        }

        /// <summary>
        ///     Returns the <c>predicates</c> in the <see cref="PredicateDictionary{T,TValue}" />
        /// </summary>
        public IEnumerable<Func<T, bool>> KeyPredicates
            => (ICollection<Func<T, bool>>)
            AsEnumerable.Select(kv => kv.Key);

        /// <inheritdoc />
        /// <summary>
        ///     Returns
        ///     <c>
        ///         true</> if <paramref name="key" /> satisfies any of the predicates in
        ///         the <see cref="PredicateDictionary{T,TValue}" />
        /// </summary>
        public bool ContainsKey(T key) { return this.Any<KeyValuePair<Func<T, bool>, TValue>>(kv => kv.Key(key)); }

        /// <inheritdoc />
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
            var rows = AsEnumerable.Where(kv => kv.Key(key)).Take(1).ToArray();
            if (rows.Length == 1)
            {
                value = rows[0].Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     <c>get</c>: If <paramref name="key" /> satisfies any of the predicates in the dictionary
        ///     then the value associated with the first matching predicate is returned.
        ///     <c>set:</c> If <paramref name="key" /> satisfies any of the predicates in the dictionary
        ///     then the value associated with each such predicate will be set.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        ///     Thrown if <paramref name="key" /> does not satisfy any
        ///     predicate in the dictionary.
        /// </exception>
        public TValue this[T key]
        {
            get
            {
                try { return AsEnumerable.First(kv => kv.Key(key)).Value; } catch (InvalidOperationException e)
                {
                    throw new KeyNotFoundException("Sequence contains no matching element.");
                }
            }
            set
            {
                var oldRows = this.Where<KeyValuePair<Func<T, bool>, TValue>>(kv => kv.Key(key));
                foreach (var kv in oldRows)
                {
                    Remove(kv);
                    Add(new KeyValuePair<Func<T, bool>, TValue>(kv.Key, value));
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<TValue> Values
            => (ICollection<TValue>)
            AsEnumerable.Select(kv => kv.Value);


        /// <exception cref="NotImplementedException">
        ///     PredicateDictionary cannot implement IEnumerable&lt;T&gt; Keys.
        ///     It is itself a List&lt;KeyValuePair&lt;Func&lt;T, bool&gt;, TValue&gt;&gt;.
        /// </exception>
        public IEnumerable<T> Keys => throw new NotImplementedException(
                                                                        "PredicateDictionary cannot Implement "
                                                                      + "IEnumerator<KeyValuePair<T, TValue>> GetEnumerator(). "
                                                                      + "It is itself a List<KeyValuePair<Func<T, bool>, TValue>>.");

        /// <exception cref="NotImplementedException">
        ///     PredicateDictionary cannot Implement IEnumerator&lt;KeyValuePair&lt;T, TValue&gt;&gt; GetEnumerator().
        ///     It is itself a List&lt;KeyValuePair&lt;Func&lt;T, bool&gt;, TValue&gt;&gt;.
        /// </exception>
        public new IEnumerator<KeyValuePair<T, TValue>> GetEnumerator()
        {
            throw new NotImplementedException(
                                              "PredicateDictionary cannot Implement "
                                            + "IEnumerator<KeyValuePair<T, TValue>> GetEnumerator(). "
                                            + "It is itself a List<KeyValuePair<Func<T, bool>, TValue>>.");
        }

        /// <summary>
        ///     <see cref="Add" /> a new <c>KeyValuePair(predicate, value)</c> to the PredicateDictionary.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public PredicateDictionary<T, TValue> Add(Func<T, bool> predicate, TValue value)
        {
            Add(new KeyValuePair<Func<T, bool>, TValue>(predicate, value));
            return this;
        }
    }
}
