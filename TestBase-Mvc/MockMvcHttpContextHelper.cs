using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestBase
{
    public static class MockMvcHttpContextHelper
    {
        public static T WithMvcHttpContext<T>(this T @this, string requestUrl=null, string query = "", string appVirtualDir = "/") where T : Controller
        {
            var httpContextBase = MockHttpContextHelper.MockHttpContextBase(requestUrl ?? @this.GetType().Name);
            @this.ControllerContext = new ControllerContext(httpContextBase, new RouteData(), @this);
            return @this;
        }

        public static T WithHttpHeader<T>(
                            this T @this,
                            string header,
                            params string[] headerValues) where T : ApiController
        {
            var request = @this.ControllerContext.Request;
            request.Headers.Add(header, headerValues);
            return @this;
        }

    }
}
