using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable Mvc.ActionNotResolved
// ReSharper disable Mvc.ControllerNotResolved

namespace TestBase.Tests.AspNet6.AspNetCoreMVC
{
    [TestFixture]
    public class ControllerUrlShouldWork
    {
        [Test]
        public void UrlHelper_should_map_controller_and_action()
        {
            var uut = new StubController().WithControllerContext();
            uut.Url.Action("AView", "ATest").ShouldEqual("/ATest/AView");
        }

        [Test]
        public void UrlHelper_should_map_controller_and_action_and_other_values()
        {
            var uut = new StubController().WithControllerContext();
            uut.Url.Action("AView", "ATest", new {id = 1, otherparameter = "2"})
               .ShouldEqual("/ATest/AView?id=1&otherparameter=2");
        }

        [Test]
        public void UrlHelper_should_map_tilde_to_root()
        {
            var uut = new StubController().WithControllerContext();
            //
            uut.Url.Content("~/here.txt").ShouldBe("/here.txt");
        }

        [Test]
        public void UrlHelper_should_respect_virtualpathtemplate()
        {
            var uut =
            new StubController().WithControllerContext(virtualPathTemplate: "/some/{action}/{controller}/thing/");
            uut.Url.Action("AView", "ATest").ShouldEqual("/some/AView/ATest/thing/");
        }
    }
}
