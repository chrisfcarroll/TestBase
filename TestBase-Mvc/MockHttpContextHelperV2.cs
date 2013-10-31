using System;
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
using Moq;
using TestBase.Shoulds;
using UrlHelper = System.Web.Mvc.UrlHelper;

namespace TestBase
{
    public static class MockHttpContextHelper
    {
        public static T WithHttpContextAndRoutes<T>(this T @this, string requestUrl = null, string query = "", string appVirtualDir = "/", HttpApplication applicationInstance = null, Action<RouteCollection> mvcApplicationRoutesRegistration = null) where T : Controller
        {
            return @this.WithHttpContextAndRoutes(
                            MockHttpContextBase(appVirtualDir: appVirtualDir, requestUrl: requestUrl ?? @this.GetType().Name, query: query, applicationInstance: applicationInstance??new HttpApplication()),
                            mvcApplicationRoutesRegistration);
        }

        public static T WithHttpContextAndRoutes<T>(this T @this, HttpContextBase httpContextBase, Action<RouteCollection> mvcApplicationRoutesRegistration) where T : Controller
        {
            var routes = RouteTable.Routes;
            var routeData = routes.GetRouteData(httpContextBase) ?? new RouteData();
            if (mvcApplicationRoutesRegistration != null)
            {
                mvcApplicationRoutesRegistration(routes);
            }
            @this.Url = new UrlHelper(new RequestContext(httpContextBase, routeData), routes);
            @this.ControllerContext = new ControllerContext(httpContextBase, routeData, @this);
            return @this;
        }

        public static HttpContextBase MockHttpContextBase(string appVirtualDir = "/", string requestUrl = "FakeUrl", string query = "", HttpApplication applicationInstance = null)
        {
            var httpContext = FakeHttpContext(requestUrl, query, appVirtualDir, applicationInstance??new HttpApplication());

            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request).Returns(new HttpRequestWrapper(httpContext.Request));
            context.Setup(ctx => ctx.Response).Returns(new HttpResponseWrapper(httpContext.Response));
            context.Setup(ctx => ctx.User).Returns(httpContext.User);
            context.Setup(ctx => ctx.Session).Returns(new HttpSessionStateWrapper(httpContext.Session));
            context.Setup(ctx => ctx.Items).Returns(httpContext.Items);
            context.Setup(ctx => ctx.Server).Returns(MockServerUtility(appVirtualDir).Object);
            context.Setup(ctx => ctx.Application).Returns(new Mock<HttpApplicationStateBase>().Object);
            return context.Object;
        }

        public static Mock<HttpServerUtilityBase> MockServerUtility(string appVirtualDir)
        {
            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MachineName).Returns(Environment.MachineName);
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
            return server;
        }

        public static HttpContext FakeHttpContext(string requestUrl, string query, string appVirtualDir, HttpApplication applicationInstance)
        {
            var request = new HttpRequest("",
                                          new UriBuilder("http", "localhost", 80, appVirtualDir + requestUrl).Uri.ToString(),
                                          query);

            //request.Headers.Add("Referer",referer);Can't do this, throws a System.PlatformNotSupportedException : Operation is not supported on this platform 

            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            httpContext.User = new WindowsPrincipal(WindowsIdentity.GetCurrent() ?? new WindowsIdentity("FakeWindowsIdentity"));
            var session = CreateSession();
            httpContext.Items["AspSession"] = session;
            httpContext.ApplicationInstance=applicationInstance;
            return httpContext;
        }

        public static HttpSessionState CreateSession()
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
