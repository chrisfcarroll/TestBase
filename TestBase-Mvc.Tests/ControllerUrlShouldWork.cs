using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using TestBase;

// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved
// ReSharper disable Html.PathError

namespace TestBaseMvc.Tests
{
    [TestFixture]
    public class ControllerUrlShouldWork
    {
        public static void RegisterFakeRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                            "Default",
                            "custom/{controller}-{action}/{id}",
                            new {controller = "CustomC", action = "CustomA", id = UrlParameter.Optional}
                           );
        }

        [Test]
        public void Should_get_a_urlHelper_which_respects_appVirtualPath()
        {
            var appVirtualPath = "/OverriddenAppVirtualPath";
            //
            var uut = new StubController().WithHttpContextAndRoutes(RegisterFakeRoutes, appVirtualPath: appVirtualPath);
            //
            uut.Url.Content("~/here.txt").ShouldBe(appVirtualPath + "/here.txt");
        }

        [Test]
        public void Should_get_a_working_urlhelper__Given_custom_route_config()
        {
            var uut = new StubController().WithHttpContextAndRoutes(RegisterFakeRoutes);
            uut.Url.Action("a", "c").ShouldEqual("/custom/c-a");
            uut.Url.Action("a", "c", new {id = 1, otherparameter = "2"}).ShouldEqual("/custom/c-a/1?otherparameter=2");
            uut.Url.Action("Index", "Home").ShouldEqual("/custom/Home-Index");
        }
    }
}
