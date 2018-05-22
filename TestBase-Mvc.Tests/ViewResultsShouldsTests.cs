using System.Web.Mvc;
using NUnit.Framework;
using TestBase;
using TestBase.Shoulds;


// ReSharper disable InconsistentNaming
// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace TestBaseMvc.Tests
{
    public class MyViewModel
    {
        public string YouPassedIn { get; set; }
        public string LinkToSelf { get; set; }
        public string LinkToOther { get; set; }
    }

    public class IDependency{}

    public class AController : Controller
    {
        static string ViewName = "ViewName";
        
        public AController(IDependency dependency){}

        public ActionResult ActionName(string parameter, string other, string thing)
        {
            var model= new MyViewModel
            {
                YouPassedIn = parameter??"(null)",
                LinkToSelf = Url.Action("ActionName","AController"),
                LinkToOther= Url.Action(thing,other)
            };
            return View(ViewName,model);
        }
    }

    [TestFixture]
    public class ControllersShouldBeTestable__GivenDependencyOnUrlHelper
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
