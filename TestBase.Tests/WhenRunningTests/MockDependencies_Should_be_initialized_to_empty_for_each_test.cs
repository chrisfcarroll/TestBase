using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenRunningTests
{
    [TestClass]
    public class Given_Initialize_has_not_been_overridden : TestBase<object>
    {
        [TestMethod]
        public void For_the_first_test()
        {
            Mocks.ShouldNotBeNull().ShouldBeEmpty();
            Mocks.Add<object>();
        }

        [TestMethod]
        public void For_the_second_test()
        {
            Mocks.ShouldNotBeNull().ShouldBeEmpty();
            Mocks.Add<object>();
        }
    }

    [TestClass]
    public class Given_Initialize_has_been_overriden : TestBase<object>
    {
        [TestInitialize]
        public override void SetUp()
        {
            Mocks.Add<object>();
        }

        [TestMethod]
        public void For_the_first_test()
        {
            Mocks.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void For_the_second_test()
        {
            Mocks.Count().ShouldEqual(1);
        }
    }
}
