using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
#if NoMSTest
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Moq;
using TestBase.Shoulds;

namespace TestBase
{
    public abstract class TestBase<TClass> where TClass : class
    {
        public MocksDictionary Mocks { get; private set; }

        public FakesDictionary Fakes { get; private set; }

        protected TestBase()  {InitMocksAndFakes(); }

        protected void InitMocksAndFakes()
        {
            Fakes = new FakesDictionary();
            Mocks = new MocksDictionary();
        }

        public Mock<T> GetMock<T>() where T : class
        {
            return Mocks.Get<T>();
        }
#if NoMSTest
#else
        [TestCleanup]
#endif
		[NUnit.Framework.TearDown]
        public virtual void Cleanup()  { InitMocksAndFakes(); }

#if NoMSTest
#else
        [TestInitialize]
#endif
        [NUnit.Framework.SetUp]
        public virtual void SetUp()
        {
            var ctorInfoForClassUnderTest = typeof(TClass).GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            ctorInfoForClassUnderTest.ShouldNotBeNull(
                "The TestBase<{0}>.Init() base method couldn't create a UnitUnderTest of type {0} because no constructor was found for {0}." +
                "To test classes without a constructor, override the Initialize() method to construct your UnitUnderTest.",
                typeof(TClass).FullName);

            var tClassCtorParameters = CreateMocksOrFakesForConstructor(ctorInfoForClassUnderTest);

            UnitUnderTest = (TClass)ctorInfoForClassUnderTest.Invoke(tClassCtorParameters);
        }

        private object[] CreateMocksOrFakesForConstructor(ConstructorInfo constructorInfo)
        {
            var ctorParameters = constructorInfo.GetParameters();
            if (ctorParameters.Length == 0)
            {
                return new object[0];
            }

            var result = new List<object>();
            foreach (var paramInfo in constructorInfo.GetParameters())
            {
                if (Fakes.ContainsKey(paramInfo.ParameterType.Name))
                {
                    result.Add(Fakes.Get<object>(paramInfo.ParameterType.Name));
                }
                else if (Fakes.ContainsKey(paramInfo.Name))
                {
                    result.Add(Fakes.Get<object>(paramInfo.Name));
                }

                else if (paramInfo.ParameterType.IsSealed || paramInfo.ParameterType.IsValueType)
                {
                    result.Add(ConstructDefaultInstanceElseThrow(paramInfo.ParameterType));
                }
                else
                {
                    Mocks.EnsureMock(paramInfo.ParameterType);
                    result.Add(Mocks.Object(paramInfo.ParameterType));
                }
            }
            return result.ToArray();
        }

        private object ConstructDefaultInstanceElseThrow(Type type)
        {
            try
            {
                var defaultConstructor = type.GetConstructor(new Type[0]);
                return defaultConstructor != null ? defaultConstructor.Invoke(new object[0]) : GetDefault(type);
            }
            catch (TargetInvocationException e)
            {
                throw new InvalidOperationException(
                    string.Format("Failed to construct a default instance of (non-mockable) {0}, which is a constructor dependency for {1}.",
                                    type.FullName, typeof(TClass).FullName),
                    e);
            }
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public void Init<TMock1>() where TMock1 : Mock
        {
            Mocks.Ensure<TMock1>();
            var typeOfMock = typeof(TMock1).GetGenericArguments();
            var constructor = typeof(TClass).GetConstructor(typeOfMock);
            Debug.Assert(constructor != null,
                         "The TestBase<{0}>.Init<TMock1>() base method expects Type {0} to have a constructor taking 1 parameter of type <TMock1> (that is, {1})." +
                         "Have you called the wrong overload of Init()?",
                         typeof(TClass).FullName,
                         typeOfMock[0]);
            Debug.Assert(constructor != null, "constructor != null");
            UnitUnderTest = (TClass)constructor.Invoke(new object[] { Mocks.Object<TMock1>() });
        }

        public void Init(Func<IMocksDictionary, TClass> instantiateFromMockDependencies)
        {
            UnitUnderTest = instantiateFromMockDependencies(Mocks);
        }

        public void Init(Func<TClass> instantiateClassUnderTest)
        {
            UnitUnderTest = instantiateClassUnderTest();
        }

        public TClass UnitUnderTest { get; set; }

        public TestBase<TClass> AddMock<TMock>() where TMock : class
        {
            return AddMock<TMock>(null);
        }

        public TestBase<TClass> AddMock<TMock>(string key) where TMock : class
        {
            Mocks.Add(key, new Mock<TMock>());
            return this;
        }

        public TestBase<TClass> Extend(Action<TestBase<TClass>> extension)
        {
            extension(this);
            return this;
        }
    }
}