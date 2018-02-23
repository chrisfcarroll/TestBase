using System.Web.Mvc;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.Tests.ShouldsCorrectnessAndVerbosityTests
{
    class AController : Controller
    {
        public ActionResult ActionName()
        {
            var model= new AClass();
            return View("ActionName",model);
        }
    }

    [TestFixture]
    public class ViewResultsShouldsTests
    {
        [Test]
        public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
        {
            var aController = new AController().WithHttpContextAndRoutes();
            //
            var result= aController.ActionName().ShouldBeViewWithModel<AClass>("ActionName");
            //
            result.ShouldBeOfType<AClass>();
        }
    }
}
