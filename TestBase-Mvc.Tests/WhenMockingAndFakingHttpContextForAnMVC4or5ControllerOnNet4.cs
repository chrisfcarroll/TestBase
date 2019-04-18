using System.Web;
using NUnit.Framework;
using TestBase;

// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace TestBaseMvc.Tests
{
    [TestFixture]
    public class WhenMockingAndFakingHttpContextForAnMVC4or5ControllerOnNet4
    {
#if !MONO        
        [Test]
#endif
        public void Cookies_should_work()
        {
            var uut = new ATestController(new IDependency()).WithHttpContextAndRoutes();

            HttpContext.Current.Request.Cookies.Add(new HttpCookie("cookie1", "cookie!") {Path = "/"});
            var result = uut.SomethingWithCookies("cookie1", "cookie2", "changed!");

            result.ShouldBe("cookie!");
            HttpContext.Current.Response.Cookies["cookie1"].ShouldNotBeNull().Value.ShouldBe("changed!");
            HttpContext.Current.Response.Cookies["cookie2"].ShouldNotBeNull().Value.ShouldBe("changed!");
        }

        [Test]
        public void Should_get_a_context_an_httpcontext_and_a_request()
        {
            var uut = new StubController().WithHttpContextAndRoutes();
            uut.ControllerContext.ShouldNotBeNull();
            uut.ControllerContext.HttpContext.ShouldNotBeNull();
            uut.ControllerContext.HttpContext.Request.ShouldNotBeNull();
            uut.Request.ShouldNotBeNull();
        }

        [Test]
        public void Should_get_a_global_httpcontext_too()
        {
            var uut = new StubController().WithHttpContextAndRoutes();
            HttpContext.Current.ShouldNotBeNull();
            HttpContext.Current.Request.Url.ShouldEqual(uut.Request.Url);
        }
#if !MONO
        [Test]
#endif
        public void Should_get_a_working_urlhelper__Given_no_routeConfig()
        {
            var uut = new StubController().WithHttpContextAndRoutes();
            uut.Url.Action("a", "b").ShouldEqual("/b/a");
            uut.Url.Action("a", "b", new {id = 1, otherparameter = "2"}).ShouldEqual("/b/a/1?otherparameter=2");
            uut.Url.Action("Index", "Home").ShouldEqual("/"); //because RouteConfig defines Home/Index as the default
        }
    }
}
