using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using TestBase;
using TestBase.Shoulds;

namespace TestBase.Tests.WhenTestingMvcControllerWithMockContext
{
    [TestFixture]
    public class WhenMockContextIsApplied
    {
        public class FakeController : Controller{}

        [Test]
        public void Should_create_a_mock_context()
        {
            new FakeController().WithMvcHttpContext()
                .ControllerContext.ShouldNotBeNull()
                .HttpContext.ShouldNotBeNull();
        }

        [Test]
        public void Mock_context_should_have_request_properties()
        {
            var request=new FakeController()
                .WithMvcHttpContext("FakeUrl", "fakequery=1", null, "fakeAppVirtualDirectory")
                .ControllerContext.ShouldNotBeNull()
                .HttpContext.ShouldNotBeNull()
                .Request;

            request.QueryString.AllKeys.ShouldEqual(new []{"fakequery"});
            request.RawUrl.ShouldEqual("http://localhost:80/fakeAppVirtualDirectory/FakeUrl?fakequery=1");
            request.Url.ShouldEqual(new Uri("http://localhost:80/fakeAppVirtualDirectory/FakeUrl?fakequery=1"));
        }
    }
}
