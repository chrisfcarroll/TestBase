using NUnit.Framework;
using TestBase;
using TestBase.Shoulds;


// ReSharper disable InconsistentNaming
// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace TestBaseMvc.Tests
{
    [TestFixture]
    public class ViewResultShoulds
    {
#if MONO
        [Test]
        public void SomeTestsDontYetRunOnMono(){}
#else        
        [Test]
        public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
        {
            var controllerUnderTest = new ATestController(new IDependency()).WithHttpContextAndRoutes();

            var viewModel =
            controllerUnderTest.AView("parameter", "Other", "Thing").ShouldBeViewWithModel<MyViewModel>("ViewName");

            viewModel.YouPassedIn.ShouldBe("parameter");
            viewModel.LinkToSelf.ShouldBe("/ATest/AView");
            viewModel.LinkToOther.ShouldBe("/Other/Thing");
        }
#endif
    }
}
