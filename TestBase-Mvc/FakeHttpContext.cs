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
    public static class FakeHttpContextHelper
    {
        public static T WithHttpContext<T>(this T @this) where T : Controller
        {
            HttpContext.Current = FakeHttpContext(@this.GetType().Name);

            @this.ControllerContext = new ControllerContext(new HttpContextWrapper(HttpContext.Current), new RouteData(), @this);
            return @this;
        }

        public static T WithHttpRequestHeader<T>(
                            this T @this, 
                            HttpMethod httpMethod, 
                            string header,
                            params string[] headerValues) where T : ApiController
        {
            var requestUri = string.Format("{0}/{1}", @this.GetType().Name, httpMethod.Method);
            HttpContext.Current = FakeHttpContext(requestUri);
            var httpConfiguration = new HttpConfiguration(new HttpRouteCollection());
            var request = new HttpRequestMessage(httpMethod, requestUri);
            request.Headers.Add(header, headerValues);
            @this.ControllerContext = new HttpControllerContext(
                                            httpConfiguration,
                                            new HttpRouteData(new HttpRoute("api/{controller}/{action}")), 
                                            request
                                      );
            @this.Request = request;
            return @this;
        }

        public static HttpContext FakeHttpContext(string requestUrl)
        {
            var httpRequest = new HttpRequest("", "http://localhost/" + requestUrl, "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);
            httpContext.User = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }
    }
}
