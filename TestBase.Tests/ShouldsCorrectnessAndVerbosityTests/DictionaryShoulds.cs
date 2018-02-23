using System.Collections.Generic;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ShouldsCorrectnessAndVerbosityTests
{
    [TestFixture]
    public class DictionaryShoulds
    {
        [TestCase(1, "1", 2, "2")]
        public void ShouldContainKey_Should_Pass_when_true(int key1, string v1, int key2, string v2)
        {
            var uut = new Dictionary<int, string> {{key1, v1}, {key2, v2}};
            uut.ShouldContainKey(key1);
        }

        [TestCase(1, "1", 2, "2",3)]
        public void ShouldContainKey_Should_Fail_when_not_true(int key1, string v1, int key2, string v2, int fail)
        {
            var uut = new Dictionary<int, string> { { key1, v1 }, { key2, v2 } };
            Assert.Throws<AssertionException>(()=>uut.ShouldContainKey(fail),"Expected ShouldContainKey to fail");
        }

        [TestCase(1, "1", 2, "2")]
        public void ShouldNotContainKey_Should_Fail_when_false(int key1, string v1, int key2, string v2)
        {
            var uut = new Dictionary<int, string> { { key1, v1 }, { key2, v2 } };
            Assert.Throws<AssertionException>(() => uut.ShouldNotContainKey(key1),"Expected ShouldNotContainKey to fail");
        }

        [TestCase(1, "1", 2, "2", 3)]
        public void ShouldNotContainKey_Should_Pass_when_not_true(int key1, string v1, int key2, string v2, int other)
        {
            var uut = new Dictionary<int, string> { { key1, v1 }, { key2, v2 } };
            uut.ShouldNotContainKey(other);
        }
    }
}
