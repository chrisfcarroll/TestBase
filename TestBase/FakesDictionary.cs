using System.Collections.Generic;

namespace TestBase
{
    public interface IMixedTypeDictionary<Key, TRoot>
    {
        T Get<T>() where T : TRoot;
        T Get<T>(Key key) where T : TRoot;
        IMixedTypeDictionary<Key, TRoot> Add<T>(TRoot value) where T : TRoot;
        IMixedTypeDictionary<Key, TRoot> Add<T>(T value) where T : TRoot; 
        IMixedTypeDictionary<Key,TRoot> Add(Key key, TRoot value);
        bool Remove(Key key);
    }

    public class FakesDictionary : Dictionary<string,object>, IMixedTypeDictionary<string,object>
    {
        public T Get<T>()
        {
            return Get<T>(typeof (T).Name);
        }

        public T Get<T>(string key)
        {
            return (T) this[key];
        }

        public new IMixedTypeDictionary<string, object> Add(string key, object value)
        {
            base.Add(key,value);
            return this;
        }

        public IMixedTypeDictionary<string, object> Add<T>(object value)
        {
            base.Add(typeof(T).Name, value);
            return this;
        }
        public IMixedTypeDictionary<string, object> Add<T>(T value)
        {
            base.Add(typeof (T).Name, value);
            return this;
        }
    }
}