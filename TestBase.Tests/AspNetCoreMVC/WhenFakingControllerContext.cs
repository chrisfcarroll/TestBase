using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestBase.Tests.AspNetCoreMVC
{
    public class StubController : Controller
    {
    }

    public class AnotherTestController : Controller
    {
        public IActionResult Action(string c, string a, int id)
        {
            return View("ViewName", new MyViewModel {LinkToOther = Url.Action(a, c, new {id})});
        }
    }

    [TestFixture]
    public class WhenFakingControllerContext
    {
        [Test]
        public void Cookies_should_work()
        {
            var controller = new ATestController().WithControllerContext();

            controller.HttpContext.Request.SetRequestCookies("name", "cookie!");
            var result = controller.SomethingWithCookies("name", "new!", "cookie2");

            result.ShouldBe("cookie!");
        }

        [Test]
        public void Should_get_dynamic_viewbag()
        {
            var controller = new StubController().WithControllerContext();
            controller.ViewBag.dynamicmember = "Here";
            (controller.ViewBag.dynamicmember as string).ShouldBe("Here");
            (controller.ViewBag as DynamicObject).ShouldBeOfType<DynamicObject>();
        }

        [Test]
        public void Should_get_nonnull_context_httpcontext_request_response_urlhelper_tempdata_viewdata()
        {
            var uut = new StubController().WithControllerContext();
            uut.ControllerContext.ShouldNotBeNull();
            uut.ControllerContext.HttpContext.ShouldNotBeNull();
            uut.ControllerContext.HttpContext.Request.ShouldNotBeNull();
            uut.Request.ShouldNotBeNull();
            uut.Response.ShouldNotBeNull();
            uut.Url.ShouldNotBeNull();
            uut.ViewData.ShouldNotBeNull();
            uut.TempData.ShouldNotBeNull();
            uut.HttpContext.ShouldBe(uut.ControllerContext.HttpContext);
        }
    }
}
