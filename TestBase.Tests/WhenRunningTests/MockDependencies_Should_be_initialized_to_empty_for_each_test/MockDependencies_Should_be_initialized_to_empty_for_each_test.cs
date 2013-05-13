using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenRunningTests.MockDependencies_Should_be_initialized_to_empty_for_each_test
{
    [TestClass]
    public class Given_Initialize_has_not_been_overridden : TestBase<object>
    {
        [TestMethod]
        public void For_the_first_test()
        {
            MockDependencies.ShouldNotBeNull().ShouldBeEmpty();
            MockDependencies.Add<object>();
        }

        [TestMethod]
        public void For_the_second_test()
        {
            MockDependencies.ShouldNotBeNull().ShouldBeEmpty();
            MockDependencies.Add<object>();
        }
    }

    [TestClass]
    public class Given_Initialize_has_been_overriden : TestBase<object>
    {
        [TestInitialize]
        public override void Initialize()
        {
            MockDependencies.Add<object>();
        }

        [TestMethod]
        public void For_the_first_test()
        {
            MockDependencies.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void For_the_second_test()
        {
            MockDependencies.Count().ShouldEqual(1);
        }
    }
}
