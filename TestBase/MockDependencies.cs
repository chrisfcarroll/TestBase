using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace TestBase
{
    public interface IMockDictionary
    {
        Mock<T> Get<T>() where T : class;
        Mock<T> Get<T>(string key) where T : class;
        
        T Object<T>() where T : class;
        T Object<T>(string key) where T : class;

        IMockDictionary Ensure<T>() where T : class;
        IMockDictionary Ensure<T>(string key) where T : class;

        IMockDictionary Add(Mock mock);
        IMockDictionary Add(string key, Mock mock);
        IMockDictionary Add<T>() where T : class;
        IMockDictionary Add<T>(string key) where T : class;

        int Count();
    }

    public interface IMockDictionaryReflectable
    {
        Mock Get(Type T);
        Mock Get(Type T, string key);

        object Object(Type T);
        object Object(Type T, string key);

        IMockDictionaryReflectable EnsureMock(Type T);
        IMockDictionaryReflectable EnsureMock(Type T, string key);
    }

    public class MockDependencies : List<KeyValuePair<string, Mock>>, IMockDictionary, IMockDictionaryReflectable
    {
        public Mock<TMock> Get<TMock>() where TMock : class
        {
            return Get<TMock>(null);
        }

        public Mock<TMock> Get<TMock>(string key) where TMock : class
        {
            var mock = GetOrNull(x => x is TMock, key) as Mock<TMock>;

            if (mock == null)
                throw new InvalidOperationException(string.Format(
                    "Mock list does not contain a mock for {0}",
                    typeof(TMock)));

            return mock;
        }

        public TMock Object<TMock>() where TMock : class
        {
            Ensure<TMock>();
            return Get<TMock>().Object;
        }

        public TMock Object<TMock>(string key) where TMock : class
        {
            Ensure<TMock>(key);
            return Get<TMock>(key).Object;
        }

        public IMockDictionary Ensure<TMock>() where TMock : class
        {
            Ensure<TMock>(null);
            return this;
        }

        public IMockDictionary Ensure<TMock>(string key) where TMock : class
        {
            if (GetOrNull(x=> x is TMock, key) == null)
            {
                Add(key, new Mock<TMock>());
            }
            return this;
        }

        public IMockDictionary Add<TMock>() where TMock : class
        {
            Add(new Mock<TMock>());
            return this;
        }

        public IMockDictionary Add<TMock>(string key) where TMock : class
        {
            Add(key, new Mock<TMock>());
            return this;
        }

        public new int Count()
        {
            return base.Count;
        }

        public IMockDictionary Add(Mock mock)
        {
            Add(new KeyValuePair<string, Mock>(null, mock));
            return this;
        }

        public IMockDictionary AddMock<TMock>(Mock<TMock> instance) where TMock : class
        {
            Add(instance);
            return this;
        }

        public IMockDictionary Add(string key, Mock mock)
        {
            Add(new KeyValuePair<string, Mock>(key, mock));
            return this;
        }

        public IMockDictionaryReflectable EnsureMock(Type T)
        {
            EnsureMock(T, null);
            return this;
        }

        public IMockDictionaryReflectable EnsureMock(Type T, string key)
        {
            if (GetOrNull(T.IsInstanceOfType, key) ==null)
            {
                Add(key,
                    (Mock) typeof (Mock<>)
                               .MakeGenericType(new[] {T})
                               .GetConstructor(new Type[0])
                               .Invoke(new object[0]));
            }
            return this;
        }

        public Mock Get(Type T)
        {
            return Get(T, null);
        }

        public Mock Get(Type T, string key)
        {
            var mock = GetOrNull(T.IsInstanceOfType, key);

            if (mock == null)
                throw new InvalidOperationException(
                    string.Format("Mock list does not contain a mock for {0}", T));

            return mock;            
        }

        private Mock GetOrNull(Func<object,bool> isOfTypeTest, string key)
        {
            var mock = this
                .Where(x => string.IsNullOrEmpty(key) || key.Equals(x.Key))
                .Where(x => isOfTypeTest(x.Value.Object))
                .Select(x => x.Value)
                .FirstOrDefault();
            return mock;
        }


        public object Object(Type T)
        {
            EnsureMock(T);
            return Get(T).Object;
        }

        public object Object(Type T, string key)
        {
            EnsureMock(T, key);
            return Get(T, key).Object;
        }

    }
}