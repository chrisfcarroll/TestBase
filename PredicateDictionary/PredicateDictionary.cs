using System;
using System.Collections.Generic;
using System.Linq;

namespace PredicateDictionary
{
    public class PredicateDictionary<T,TValue> : List<KeyValuePair<Func<T, bool>, TValue>>
    {
        public TValue this[T key]            => this.First(func=> func.Key(key)).Value;
        public TValue this[Func<T,bool> key] => this.First(kv=>kv.Key ==key).Value;

        public bool IsReadOnly => false;
        public ICollection<Func<T, bool>> Keys   => (ICollection<Func<T, bool>>) this.Select(kv=>kv.Key);
        public ICollection<TValue>        Values => (ICollection<TValue>) this.Select(kv => kv.Value);
        public bool ContainsKey(Func<T, bool> key) { return this.Any(kv=>kv.Key ==key); }        
        public bool TryGetValue(Func<T, bool> key, out TValue value)
        {
            value = this.FirstOrDefault(kv => kv.Key == key).Value;
            return value != null;
        }
       
    }
}
