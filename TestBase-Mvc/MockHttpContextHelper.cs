using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Moq;

namespace TestBase
{
    public static class MockHttpContextHelper
    {
        public static T WithMockHttpContext<T>(this T @this, string requestUrl=null, string query = "", string appVirtualDir = "/") where T : Controller
        {
            var httpContextBase = FakeHttpContextBase( requestUrl??@this.GetType().Name);
            @this.ControllerContext = new ControllerContext(httpContextBase, new RouteData(), @this);
            return @this;
        }

        public static T WithGlobalHttpContext<T>(this T @this, string requestUrl = null, string query = "", string appVirtualDir = "/") where T : Controller
        {
            HttpContext.Current = FakeHttpContext(requestUrl??@this.GetType().Name);
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

        public static HttpContextBase FakeHttpContextBase(string requestUrl, string query = "", string appVirtualDir = "/")
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var appState = new Mock<HttpApplicationStateBase>();

            server.Setup(s => s.MachineName).Returns(System.Environment.MachineName);
            server.Setup(s => s.MapPath(It.IsAny<string>()))
                  .Returns((string s) =>
                      {
                          var s1 = s.StartsWith("~")
                                           ? ".\\" + s.Substring(1) 
                                            : s.StartsWith(appVirtualDir) 
                                            ? ".\\" + s.Substring(appVirtualDir.Length) 
                                            : s;
                          return s1.Replace('/', '\\');
                      });

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.Application).Returns(appState.Object);

            return context.Object;
        }

        private static HttpContext FakeHttpContext(string requestUrl, string query = "", string appVirtualDir = "/")
        {

            //var httpRequest = new HttpRequest("", "http://localhost" + appVirtualDir + requestUrl, "");
            //var httpResponse = new HttpResponse(new StringWriter());

            var wr = new SimpleWorkerRequest(appVirtualDir, "..", requestUrl, query, new StringWriter());
            var httpContext = new HttpContext(wr);
            httpContext.User = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            httpContext.Items["AspSession"] = CreateSession();
            return httpContext;
        }

        private static HttpSessionState CreateSession()
        {
            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            return (HttpSessionState)typeof(HttpSessionState).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new[] { typeof(HttpSessionStateContainer) },
                null)
            .Invoke(new object[] { sessionContainer });
        }
    }
}
