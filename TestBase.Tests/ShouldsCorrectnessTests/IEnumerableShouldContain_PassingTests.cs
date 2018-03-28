using System.Linq;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests
{
    [TestFixture]
    public class IEnumerableShouldContain_PassingTests
    {
        [TestCase(new[] { 1, 2, 999 })]
        public void IEnumerable_ShouldContain_ShouldPass(int[] value)
        {
            value.ShouldContain(i => i < value.Max()+1);
            value.ShouldContain( value.Last());
        }

        [TestCase(new[] { 1, 2, 999 })]
        public void IEnumerable_ShouldNotContain_ShouldPass(int[] value)
        {
            value.ShouldNotContain(i => i < -1);
            value.ShouldNotContain( value.Max()+1);
        }
    }
}