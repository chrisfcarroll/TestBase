using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Moq;

namespace TestBase
{
    public static class MockHttpContextHelper
    {
        public static T WithHttpContextAndRoutes<T>(this T @this, Action<RouteCollection> mvcApplicationRoutesRegistration = null, string requestUrl = null, string query = "", string appVirtualPath = "/", HttpApplication applicationInstance = null) where T : Controller
        {
            string requestUrl1 = requestUrl ?? @this.GetType().Name;
            HttpApplication applicationInstance1 = applicationInstance??new HttpApplication();
            return @this.WithHttpContextAndRoutes(
                            MockHttpContextBase(
                                FakeHttpContextCurrent(appVirtualPath, requestUrl1, query, applicationInstance1), 
                                appVirtualPath),
                            mvcApplicationRoutesRegistration);
        }

        public static T WithHttpContextAndRoutes<T>(this T @this, HttpContextBase httpContextBase, Action<RouteCollection> mvcApplicationRoutesRegistration) where T : Controller
        {
            var routes = new RouteCollection();
            var routeData = routes.GetRouteData(httpContextBase) ?? new RouteData();
            mvcApplicationRoutesRegistration = mvcApplicationRoutesRegistration ?? TypicalMvcRouteConfig.RegisterRoutes;
            mvcApplicationRoutesRegistration(routes);
            @this.Url = new UrlHelper(new RequestContext(httpContextBase, routeData), routes);
            @this.ControllerContext = new ControllerContext(httpContextBase, routeData, @this);
            return @this;
        }

        public static HttpContextBase MockHttpContextBase(HttpContext httpContext, string appVirtualDir = "/")
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request).Returns(new HttpRequestWrapper(httpContext.Request));
            context.Setup(ctx => ctx.Response).Returns(new HttpResponseWrapper(httpContext.Response));
            context.Setup(ctx => ctx.User).Returns(httpContext.User);
            context.Setup(ctx => ctx.Session).Returns(new HttpSessionStateWrapper(httpContext.Items["AspSession"] as HttpSessionState));
            context.Setup(ctx => ctx.Items).Returns(httpContext.Items);
            context.Setup(ctx => ctx.Server).Returns(MockServerUtility(appVirtualDir).Object);
            context.Setup(ctx => ctx.Application).Returns(new Mock<HttpApplicationStateBase>().Object);
            return context.Object;
        }

        static HttpContext FakeHttpContextCurrent(string appVirtualPath, string requestUrl, string query,
                                                  HttpApplication applicationInstance)
        {
            var httpContext = FakeHttpContext(requestUrl, query, appVirtualPath, applicationInstance ?? new HttpApplication());
            HttpContext.Current = httpContext;
            return httpContext;
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
