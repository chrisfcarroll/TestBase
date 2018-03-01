using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests
{
    class AController : Controller
    {
        public IActionResult ActionName()
        {
            var model= new AClass();
            return View("ViewName",model);
        }
    }

    [TestFixture]
    public class ViewResultsShouldsTests
    {
        [Test]
        public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
        {
            var aController = new AController().WithControllerContext(nameof(AController.ActionName));
            //
            var result= aController.ActionName().ShouldBeViewWithModel<AClass>("ViewName");
            //
            result.ShouldBeOfType<AClass>();
        }
    }
}
