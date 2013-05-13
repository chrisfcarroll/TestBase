using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenAsserting
{
    [TestClass]
    public class Using_Moq_verification
    {
        [TestMethod]
        public void The_ShouldCall_method_shouldnt_throw__Given_mocked_method_was_called()
        {
            var dependencyMock = new Mock<SomeMockableClass>();
            Action actionUnderTest = () => dependencyMock.Object.SomeMockableMethod();
            actionUnderTest.ShouldCall(dependencyMock, x=>x.SomeMockableMethod());
        }
        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void The_ShouldCall_method_should_throw_a_Moq_Exception__Given_mocked_method_wasnt_called()
        {
            var dependencyMock = new Mock<SomeMockableClass>();
            Action actionUnderTest = () => { };
            actionUnderTest.ShouldCall(dependencyMock, x => x.SomeMockableMethod());
        }
    }

    public class SomeMockableClass
    {
        public virtual SomeMockableClass SomeMockableMethod() { return new SomeMockableClass(); }
    }
}