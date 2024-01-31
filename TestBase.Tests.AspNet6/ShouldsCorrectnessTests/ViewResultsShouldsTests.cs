using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestBase.Tests.AspNet6.ShouldsCorrectnessTests
{
    class AController : Controller
    {
        static readonly string Viewname = "ViewName";

        public IActionResult ActionName()
        {
            var model = new AClass();
            return View(Viewname, model);
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
            var result = aController.ActionName().ShouldBeViewWithModel<AClass>("ViewName");
            //
            result.ShouldBeOfType<AClass>();
        }
    }
}
