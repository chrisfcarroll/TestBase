using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;
using Tests.WebApi.TestFwk;

namespace TestBase.Tests.ShouldsCorrectnessTests
{
    class AController : Controller
    {
        public IActionResult ActionName()
        {
            var model= new AClass();
            var viewName = nameof(ActionName);
            return View(viewName,model);
        }
    }

    [TestFixture]
    public class ViewResultsShouldsTests
    {
        [Test]
        public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
        {
            var aController = new AController().WithControllerContext();
            //
            var result= aController.ActionName().ShouldBeViewWithModel<AClass>("ActionName");
            //
            result.ShouldBeOfType<AClass>();
        }
    }
}
