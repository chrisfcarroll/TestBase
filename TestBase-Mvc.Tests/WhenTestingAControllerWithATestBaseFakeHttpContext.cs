using System;
using NUnit.Framework;
using System.Web.Mvc;
using TestBase.MockHttpContext;
using System.Web.Routing;
using TestBase.Shoulds;

namespace TestBaseMvc.Tests
{
    public class TestController : System.Web.Mvc.Controller
    {
        public string Index()
        {
            string s = this.Url.Action( "Index" ).ShouldNotBeNullOrEmptyOrWhitespace();
            return s;
        }
    }

    [TestFixture]
    public class WhenTestingAControllerWithATestBaseFakeHttpContext
    {

        [Test]
        public void ShouldBeAbleToReferenceContollerContext()
        {
            Action<RouteCollection> mvcAppRegisterRoutes= r=>{} ;
            var uut = new TestController().WithMockHttpContextAndRelativeUrlAndApplicationRoutes(mvcAppRegisterRoutes);
            uut.Index();
        }

    }
}

