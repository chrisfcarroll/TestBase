using System;
using System.Collections.Generic;
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
        public static T WithMvcHttpContext<T>(this T @this, string requestUrl = null, string query = "", Action<RouteCollection> mvcApplicationRoutesRegistration = null, string appVirtualDir = "/") where T : Controller
        {
            var httpContextBase = MockHttpContextBase(requestUrl: requestUrl ?? @this.GetType().Name, scheme: "http", hostname: "localhost", port: 80);

            var routes = RouteTable.Routes;
            var routeData = routes.GetRouteData(httpContextBase) ?? new RouteData();
            if (mvcApplicationRoutesRegistration != null) { mvcApplicationRoutesRegistration(routes); }
            @this.Url = new UrlHelper(new RequestContext(httpContextBase, routeData), routes);
            @this.ControllerContext = new ControllerContext(httpContextBase, routeData, @this);
            return @this;
        }

        public static HttpContextBase MockHttpContextBase(string requestUrl = "FakeUrl", string query = "", string scheme = "http", string hostname = "localhost", int port = 80, string appVirtualDir = "/")
        {
            var httpContext = FakeHttpContext(requestUrl, query, scheme, hostname, port, appVirtualDir);
            HttpContext.Current = httpContext;

            var appState = new Mock<HttpApplicationStateBase>();
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

            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request).Returns(new HttpRequestWrapper(httpContext.Request));
            context.Setup(ctx => ctx.Response).Returns(new HttpResponseWrapper(httpContext.Response));
            context.Setup(ctx => ctx.Session).Returns(new HttpSessionStateWrapper(httpContext.Session));
            context.Setup(ctx => ctx.User).Returns(httpContext.User);
            context.Setup(ctx => ctx.Items).Returns(httpContext.Items);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.Application).Returns(appState.Object);
            return context.Object;
        }

        public static HttpContext FakeHttpContext(string requestUrl, string query, string scheme, string hostname, int port,
                                           string appVirtualDir)
        {
            var request = new HttpRequest("",
                                          new UriBuilder(scheme, hostname, port, appVirtualDir + requestUrl).Uri.ToString(),
                                          query);
            var response = new HttpResponse(new StringWriter());
            var user = new WindowsPrincipal(WindowsIdentity.GetCurrent() ?? new WindowsIdentity("FakeWindowsIdentity"));
            var session = CreateSession();
            var httpItems = new Dictionary<string, object> {{"AspSession", session}};
            var httpContext = new HttpContext(request, response) {User = user};
            httpContext.Items["AspSession"] = session;
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