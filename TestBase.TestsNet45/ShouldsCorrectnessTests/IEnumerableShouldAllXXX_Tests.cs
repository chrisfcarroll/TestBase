using System;
using NUnit.Framework;

namespace TestBase.TestsNet45.ShouldsCorrectnessTests
{
    [TestFixture]
    public class IEnumerableShouldAllXXX_VerboseMessageOnFailure_Tests
    {
        [TestCase(new[] {1, 2, 999})]
        public void IEnumerable_ShouldAllBeSuchThat_ShouldNameTheFailingElement(int[] value)
        {
            try { value.ShouldAllBeSuchThat(i => i < 3); } catch (Assertion e)
            {
                Console.WriteLine(e);
                e.Message.ShouldContain("999").ShouldNotContain("1");
            }
        }

        [TestCase(new[] {1, 2, 999})]
        public void IEnumerable_ShouldAllBeSuchThat_ShouldDisplayCustomMessage(int[] value)
        {
            try { value.ShouldAllBeSuchThat(i => i < 3, "Custom Message {0}", "And Params"); } catch (Assertion e)
            {
                Console.WriteLine(e);
                e.Message.ShouldContain("999").ShouldNotContain("1");
                e.Message.ShouldContain("Custom Message And Params");
            }
        }

        [TestCase(new[] {1, 2, 999})]
        public void IEnumerable_ShouldAllSatisfy_ShouldNameTheFailingElement(int[] value)
        {
            try { value.ShouldAllSatisfy(i => i * 2, Is.LessThanOrEqualTo(5)); } catch (Assertion e)
            {
                Console.WriteLine(e);
                e.Message.ShouldContain("999").ShouldNotContain("2");
            }
        }

        [TestCase(new[] {1, 2, 999})]
        public void IEnumerable_ShouldAllSatisfy_ShouldDisplayCustomMessage(int[] value)
        {
            try
            {
                value.ShouldAllSatisfy(i => i * 2, Is.LessThanOrEqualTo(5), "Custom Message {0}", "And Params");
            } catch (Assertion e)
            {
                Console.WriteLine(e);
                e.Message.ShouldContain("999").ShouldNotContain("2");
                e.Message.ShouldContain("Custom Message And Params");
            }
        }
    }

    [TestFixture]
    public class IEnumerableShouldAllXXX_Tests
    {
        [TestCase(new[] {1, 2, 3})]
        public void IEnumerable_ShouldAll_ShouldPass(int[] value)
        {
            value.ShouldAll(i => { i.ShouldBeGreaterThan(0); });
        }

        [TestCase(new[] {1, 2, 3})]
        public void IEnumerable_ShouldAll_ShouldFail(int[] value)
        {
            Assert.Throws<Assertion>(
                                     () => value.ShouldAll(i => { i.ShouldBeGreaterThan(2); })
                                    );
        }

        [TestCase(new[] {1, 2, 3})]
        public void IEnumerable_ShouldAllBeSuchThat_ShouldPass(int[] value) { value.ShouldAllBeSuchThat(i => i > 0); }

        [TestCase(new[] {1, 2, 3})]
        public void IEnumerable_ShouldAllBeSuchThat_ShouldFail(int[] value)
        {
            Assert.Throws<Assertion>(
                                     () => value.ShouldAllBeSuchThat(i => i > 2)
                                    );
        }

        [TestCase(new[] {1, 2, 3})]
        public void IEnumerable_ShouldAllSatisfy_ShouldPass(int[] value)
        {
            value.ShouldAllSatisfy(i => i * 2, Is.GreaterThan(0));
        }

        [TestCase(new[] {1, 2, 3})]
        public void IEnumerable_ShouldAllSatisfy_ShouldFail(int[] value)
        {
            Assert.Throws<Assertion>(
                                     () => value.ShouldAllSatisfy(i => i * 2, Is.GreaterThan(4))
                                    );
        }
    }
}
