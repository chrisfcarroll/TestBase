using NUnit.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace TestBase.Tests.AspNetCoreMVC
{
    [TestFixture]
    public class ViewResultShoulds
    {
        [Test]
        public void ShouldBeViewResult_ShouldAssertViewResultAndNameAndModel()
        {
            var controllerUnderTest = new ATestController().WithControllerContext();

            var viewResult = controllerUnderTest
                            .AView("parameter", "Other", "Thing")
                            .ShouldBeViewResult();

            viewResult.ShouldNotBeNull().ShouldBeViewNamed("ViewName");
        }

        [Test]
        public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
        {
            var controllerUnderTest = new ATestController().WithControllerContext();

            var viewModel =
            controllerUnderTest
           .AView("parameter", "Other", "Thing")
           .ShouldBeViewWithModel<MyViewModel>("ViewName");

            viewModel.YouPassedIn.ShouldBe("parameter");
            viewModel.LinkToSelf.ShouldBe("/ATest/AView");
            viewModel.LinkToOther.ShouldBe("/Other/Thing");
        }
    }
}
