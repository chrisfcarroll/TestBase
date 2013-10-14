using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using Moq;

namespace TestBase
{
    public static class MockHttpContextHelper
    {
        public static HttpContextBase MockHttpContextBase(string requestUrl, string query = "", string appVirtualDir = "/")
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
        public static HttpContext MockHttpContext(string requestUrl, string query = "", string appVirtualDir = "/")
        {
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();
            var session = CreateSession();
            var server = new Mock<HttpServerUtility>();
            var appState = new Mock<HttpApplicationState>();

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

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.Application).Returns(appState.Object);

            return context.Object;
        }


        /// <summary>
        /// The problem with this approach is that I haven't found where to inject ServerUtility, so MapPath() etc don't work.
        /// </summary>
        internal static HttpContext FakeHttpContext(string requestUrl, string query = "", string appVirtualDir = "/")
        {
            var httpRequest = new HttpRequest("", new UriBuilder("http","http://localhost",80, appVirtualDir + requestUrl).Uri.ToString(), query);
            var httpResponse = new HttpResponse(new StringWriter());
            var httpContext= new HttpContext(httpRequest, httpResponse);
            httpContext.User = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            httpContext.Items["AspSession"] = CreateSession();
            return httpContext;

            //Alternative way to create httpcontext also doesn't have a space for ServerUtility
            //var wr = new SimpleWorkerRequest(appVirtualDir, "..", requestUrl, query, new StringWriter());
            //var httpContext = new HttpContext(wr);
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