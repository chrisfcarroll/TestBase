using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestBase.Tests.AspNetCoreMVC
{
    class AController : Controller
    {
        public IActionResult ActionName()
        {
            var link = Url.Action("ActionName","AController");
            var model= new AClass{Name= link};
            return View("ViewName",model);
        }
    }

    [TestFixture]
    public class ControllersShouldBeTestable__GivenDependencyOnUrlHelper
    {
        [Test]
        public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
        {
            var aController = new AController().WithControllerContext(nameof(AController.ActionName));
            //
            var result= aController.ActionName().ShouldBeViewWithModel<AClass>("ViewName");
            //
            result.ShouldBeOfType<AClass>().Name.ShouldBe("/AController/ActionName");
        }
    }
}
