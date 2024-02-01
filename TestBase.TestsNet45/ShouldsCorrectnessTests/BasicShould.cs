using System.Collections.Generic;
using NUnit.Framework;

namespace TestBase.TestsNet45.ShouldsCorrectnessTests
{
    [TestFixture]
    public class BasicShould
    {
        class ASubclass : AClass
        {
        }

        [Test]
        public void ShouldBeAssignableTo()
        {
            new AClass().ShouldBeAssignableTo<AClass>();
            new ASubclass().ShouldBeAssignableTo<AClass>();
        }

        [Test]
        public void ShouldHaveFails()
        {
            var uut = new {a = 1, b = "two"};

            Assert.Throws<Assertion>(() => uut.Should(u => u.a == 2));
            Assert.Throws<Assertion>(() => uut.Should(u => u.b == "1"));
            Assert.Throws<Assertion>(() => uut.Should(u => u.a.ShouldBe(2)));
            Assert.Throws<Assertion>(() => uut.Should(u => u.a.ShouldBeGreaterThan(2)));
            Assert.Throws<Assertion>(() =>
                                     new Dictionary<string, int>
                                     {
                                     {"a", 1},
                                     {"b", 2}
                                     }
                                    .Should(d => d["a"].ShouldBe(2))
                                    .Should(d => d["b"].ShouldBe(1))
                                    );
        }

        [Test]
        public void ShouldHavePasses()
        {
            var uut = new {a = 1, b = "two"};
            uut.Should(u => u.a == 1);
            uut.Should(u => u.a.ShouldBe(1));
            uut.ShouldHave(u => u.a == 1);
            uut.Should(u => u.b     == "two");
            uut.Should(u => u.b.ShouldBe("two"));
            uut.ShouldHave(u => u.b == "two");

            new Dictionary<string, int>
            {
            {"a", 1},
            {"b", 2}
            }
           .Should(d => d["a"].ShouldBe(1))
           .Should(d => d["b"].ShouldBe(2));
        }
    }
}
