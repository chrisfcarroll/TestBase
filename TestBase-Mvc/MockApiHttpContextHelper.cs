//using System.Net.Http;
//using System.Web;
//using System.Web.Http;
//using System.Web.Http.Controllers;
//using System.Web.Http.Routing;
//
//namespace TestBase
//{
//    public static class MockApiHttpContextHelper
//    {
//        public static T WithWebApiHttpContext<T>(this T @this, HttpMethod httpMethod, string requestUri=null, string routeTemplate=null) where T : ApiController
//        {
//            requestUri = requestUri??string.Format("{0}/{1}", @this.GetType().Name, httpMethod.Method);
//            routeTemplate = routeTemplate ?? "api/{controller}/{action}";
//
//            @this.Request = new HttpRequestMessage(httpMethod, requestUri);
//            @this.ControllerContext = new HttpControllerContext(
//                                            new HttpConfiguration(new HttpRouteCollection()),
//                                            new HttpRouteData(new HttpRoute(routeTemplate)),
//                                            new HttpRequestMessage(httpMethod, requestUri)
//                                      );
//            HttpContext.Current = MockHttpContextHelper.FakeHttpContext(requestUri,"","/",new HttpApplication());
//            return @this;
//        }
//
//        public static T WithHttpRequestHeaders<T>(
//                            this T @this,
//                            string header,
//                            params string[] headerValues) where T : ApiController
//        {
//            @this.ControllerContext.Request.Headers.Add(header, headerValues);
//            return @this;
//        }
//    }
//}