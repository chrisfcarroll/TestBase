using NUnit.Framework;
using TestBase;
using TestBase.Shoulds;

// ReSharper disable InconsistentNaming
// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace TestBaseMvc.Tests
{
    [TestFixture]
    public class ShouldBeViewWithModelTests
    {
        [Test]
        public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
        {
            var controllerUnderTest = new AController(new IDependency()).WithHttpContextAndRoutes();

            var viewModel= 
                controllerUnderTest.ActionName("parameter", "Other", "Thing").ShouldBeViewWithModel<MyViewModel>("ViewName");

            viewModel.YouPassedIn.ShouldBe("parameter");
            viewModel.LinkToSelf.ShouldBe("/AController/ActionName");
            viewModel.LinkToOther.ShouldBe("/Other/Thing");
        }
    }
}
