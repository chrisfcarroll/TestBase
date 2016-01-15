using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.TestBaseAutoMockTests.WhenRunningTests
{
    [TestFixture]
    public class Given_Initialize_has_not_been_overridden : TestBase<object>
    {
        [Test]
        public void For_the_first_test()
        {
            Mocks.ShouldNotBeNull().ShouldBeEmpty();
            Mocks.Add<object>();
        }

        [Test]
        public void For_the_second_test()
        {
            Mocks.ShouldNotBeNull().ShouldBeEmpty();
            Mocks.Add<object>();
        }
    }

    [TestFixture]
    public class Given_Initialize_has_been_overriden : TestBase<object>
    {
        [SetUp]
        public override void SetUp() { Mocks.Add<object>(); }

        [Test]
        public void For_the_first_test()
        {
            Mocks.Count().ShouldEqual(1);
        }

        [Test]
        public void For_the_second_test()
        {
            Mocks.Count().ShouldEqual(1);
        }
    }
}
