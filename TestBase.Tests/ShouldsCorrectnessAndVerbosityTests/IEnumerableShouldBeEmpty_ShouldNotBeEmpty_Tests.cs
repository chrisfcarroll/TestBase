using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ShouldsCorrectnessAndVerbosityTests
{
    [TestFixture]
    public class IEnumerableShouldBeEmpty_ShouldNotBeEmpty_Tests
    {
        [TestCase( (new[] {1, 2, 3}))]
        public void IEnumerableGeneric_ShouldNotBeEmpty_ShouldPass(int[] value)
        {
            value.ShouldBeAssignableTo<IEnumerable<int>>().ShouldNotBeEmpty();
        }
        [TestCase((new[] { 1, 2, 3 }))]
        public void IEnumerableNonGeneric_ShouldNotBeEmpty_ShouldPass(int[] value)
        {
            value.ShouldBeAssignableTo<IEnumerable>().ShouldNotBeEmpty();
        }
        [TestCase((new int[0]))]
        public void IEnumerableGeneric_ShouldNotBeEmpty_ShouldFail(int[] value)
        {
            Assert.Throws<Assertion>(
                () => value.ShouldBeAssignableTo<IEnumerable<int>>().ShouldNotBeEmpty()
                );
        }
        [TestCase((new int[0]))]
        public void IEnumerableNonGeneric_ShouldNotBeEmpty_ShouldFail(int[] value)
        {
            Assert.Throws<Assertion>(
                () => value.ShouldBeAssignableTo<IEnumerable>().ShouldNotBeEmpty()
                );
        }

        [TestCase((new int[0]))]
        public void IEnumerableGeneric_ShouldBeEmpty_ShouldPass(int[] value)
        {
            value.ShouldBeAssignableTo<IEnumerable<int>>().ShouldBeEmpty();
        }
        [TestCase((new int[0]))]
        public void IEnumerableNonGeneric_ShouldBeEmpty_ShouldPass(int[] value)
        {
            value.ShouldBeAssignableTo<IEnumerable>().ShouldBeEmpty();
        }
        [TestCase((new[] { 1, 2, 3 }))]
        public void IEnumerableGeneric_ShouldBeEmpty_ShouldFail(int[] value)
        {
            Assert.Throws<Assertion>(
                () => value.ShouldBeAssignableTo<IEnumerable<int>>().ShouldBeEmpty()
                );
        }
        [TestCase((new[] { 1, 2, 3 }))]
        public void IEnumerableNonGeneric_ShouldBeEmpty_ShouldFail(int[] value)
        {
            Assert.Throws<Assertion>(
                () => value.ShouldBeAssignableTo<IEnumerable>().ShouldBeEmpty()
                );
        }

    }
}
