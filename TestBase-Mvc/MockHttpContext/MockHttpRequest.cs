using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace TestBase.MockHttpContext
{
    public static class ControllerMockContextExtensions
    {
        public static void SetIsChildAction(this Controller @this)
        {
            @this.RouteData.DataTokens.Add("ParentActionViewContext", new Mock<ViewContext>().Object);
        }

        public static Mock<HttpContextBase> WithMockHttpContext<T>(this T @this) where T : Controller
        {
            return @this.WithMockHttpContext(req => req.Setup(x => x.Url).Returns(new Uri(@this.GetType().Name, UriKind.Relative)));
        }

        public static Mock<HttpContextBase> WithMockHttpContext(this ControllerBase @this, Action<Mock<HttpRequestBase>> requestMutator)
        {
            var mockHttpContext = new Mock<HttpContextBase>().WithSensibleDefaults();
            mockHttpContext.DefaultValue = DefaultValue.Mock;
            @this.ControllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), @this);
            
            requestMutator(mockHttpContext.Request());

            return mockHttpContext;
        }

        public static T WithApplicationRoutes<T>(this T @this, Action<RouteCollection> mvcApplicationRegisterRoutes) where T : Controller
        {
            var applicationRoutes = new RouteCollection();
            mvcApplicationRegisterRoutes(applicationRoutes);
            //Areas?
            var urlHelper = new UrlHelper(@this.ControllerContext.RequestContext, applicationRoutes);
            @this.Url = urlHelper;
            return @this;
        }

        public static Mock<HttpContextBase> MockHttpContextBase(this Controller @this)
        {
            return Mock.Get(@this.HttpContext);
        }

        public static T WithMockHttpContextAndRelativeUrlAndApplicationRoutes<T>(this T @this, Action<RouteCollection> mvcApplicationRegisterRoutes) where T : Controller
        {
            return WithMockHttpContextAndRelativeUrlAndApplicationRoutes(@this, @this.GetType().Name, new NameValueCollection(), mvcApplicationRegisterRoutes);
        }

        public static T WithMockHttpContextAndRelativeUrlAndApplicationRoutes<T>( this T @this, string url, NameValueCollection queryString, Action<RouteCollection> mvcApplicationRegisterRoutes ) where T : Controller
        {
            return WithMockHttpContextAndRequestUrl( @this, url, queryString, UriKind.Relative ).WithApplicationRoutes(mvcApplicationRegisterRoutes);
        }

        public static T WithMockHttpContextAndAbsoluteUrlAndApplicationRoutes<T>( this T @this, Action<RouteCollection> mvcApplicationRegisterRoutes ) where T : Controller
        {
            return WithMockHttpContextAndRequestUrl( @this, "http://localhost/" + @this.GetType().Name, new NameValueCollection(), UriKind.Absolute ).WithApplicationRoutes(mvcApplicationRegisterRoutes);
        }

        public static T WithMockHttpContextAndAbsoluteUrlAndApplicationRoutes<T>(this T @this, string url, NameValueCollection queryString, Action<RouteCollection> mvcApplicationRegisterRoutes) where T : Controller
        {
            return WithMockHttpContextAndRequestUrl(@this, url, queryString, UriKind.Absolute).WithApplicationRoutes(mvcApplicationRegisterRoutes);
        }

        public static T WithMockHttpContextAndRequestUrl<T>(this T @this, string url, NameValueCollection queryString, UriKind uriKind) where T : Controller
        {
            var mockContext = @this.WithMockHttpContext().WithSensibleDefaults();
            mockContext.Request().Setup(x => x.Url).Returns(new Uri(url, uriKind));
            mockContext.Request().Setup(x => x.QueryString).Returns(queryString);
            mockContext.WithServer().MapPathToApplicationWebProjectDirectory();
            
            return @this;
        }
    }
}
