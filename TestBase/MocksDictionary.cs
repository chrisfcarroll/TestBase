using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace TestBase
{
    public interface IMocksDictionary
    {
        Mock<T> Get<T>() where T : class;
        Mock<T> Get<T>(string key) where T : class;
        
        T Object<T>() where T : class;
        T Object<T>(string key) where T : class;

        IMocksDictionary Ensure<T>() where T : class;
        IMocksDictionary Ensure<T>(string key) where T : class;

        IMocksDictionary Add(Mock mock);
        IMocksDictionary Add(string key, Mock mock);
        IMocksDictionary Add<T>() where T : class;
        IMocksDictionary Add<T>(string key) where T : class;

        int Count();
    }

    public interface IMocksDictionaryReflectable
    {
        Mock Get(Type T);
        Mock Get(Type T, string key);

        object Object(Type T);
        object Object(Type T, string key);

        IMocksDictionaryReflectable EnsureMock(Type T);
        IMocksDictionaryReflectable EnsureMock(Type T, string key);
    }

    public class MocksDictionary : List<KeyValuePair<string, Mock>>, IMocksDictionary, IMocksDictionaryReflectable
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

        public IMocksDictionary Ensure<TMock>() where TMock : class
        {
            Ensure<TMock>(null);
            return this;
        }

        public IMocksDictionary Ensure<TMock>(string key) where TMock : class
        {
            if (GetOrNull(x=> x is TMock, key) == null)
            {
                Add(key, new Mock<TMock>());
            }
            return this;
        }

        public IMocksDictionary Add<TMock>() where TMock : class
        {
            Add(new Mock<TMock>());
            return this;
        }

        public IMocksDictionary Add<TMock>(string key) where TMock : class
        {
            Add(key, new Mock<TMock>());
            return this;
        }

        public new int Count()
        {
            return base.Count;
        }

        public IMocksDictionary Add(Mock mock)
        {
            Add(new KeyValuePair<string, Mock>(null, mock));
            return this;
        }

        public IMocksDictionary AddMock<TMock>(Mock<TMock> instance) where TMock : class
        {
            Add(instance);
            return this;
        }

        public IMocksDictionary Add(string key, Mock mock)
        {
            Add(new KeyValuePair<string, Mock>(key, mock));
            return this;
        }

        public IMocksDictionaryReflectable EnsureMock(Type T)
        {
            EnsureMock(T, null);
            return this;
        }

        public IMocksDictionaryReflectable EnsureMock(Type T, string key)
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