using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace TestBase
{
    public static class MockApiHttpContextHelper
    {
        public static T WithMockHttpContext<T>(
                            this T @this,
                            HttpMethod httpMethod,
                            string requestUri=null) where T : ApiController
        {
            requestUri = requestUri??string.Format("{0}/{1}", @this.GetType().Name, httpMethod.Method);
            HttpContext.Current = MockHttpContextHelper.FakeHttpContext(requestUri);
            var httpConfiguration = new HttpConfiguration(new HttpRouteCollection());
            var request = new HttpRequestMessage(httpMethod, requestUri);
            @this.ControllerContext = new HttpControllerContext(
                                            httpConfiguration,
                                            new HttpRouteData(new HttpRoute("api/{controller}/{action}")),
                                            request
                                      );
            @this.Request = request;
            return @this;
        }

        public static T WithHttpRequestHeaders<T>(
                            this T @this,
                            string header,
                            params string[] headerValues) where T : ApiController
        {
            @this.ControllerContext.Request.Headers.Add(header, headerValues);
            return @this;
        }
    }
}