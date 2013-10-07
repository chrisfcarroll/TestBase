using System;
using System.Collections.Specialized;
using System.Web;
using Moq;
//using MvcContrib.TestHelper;

namespace TestBase.MockHttpContext
{
    public static class HttpRequestBaseExtensions
    {
        public static void MakeItAnAjaxRequest(this Mock<HttpRequestBase> @this)
        {
            @this.Setup(x => x.Headers)
                .Returns(() => new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
        }

        public static Mock<HttpRequestBase> WithSensibleDefaults(this Mock<HttpRequestBase> @this)
        {
            @this.DefaultValue = DefaultValue.Mock;

            @this.Setup(x => x.ApplicationPath).Returns("/");
            @this.Setup(x => x.Url).Returns(new Uri("", UriKind.Relative));
            @this.Setup(x => x.ServerVariables).Returns(new NameValueCollection());
            @this.Setup(x => x.QueryString).Returns(new NameValueCollection());
            @this.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            @this.Setup(x => x.Form).Returns(new NameValueCollection());
            @this.Setup(x => x.Files).Returns(new Mock<HttpFileCollectionBase>().Object ) ; //(new WriteableHttpFileCollection());

            return @this;
        }

        public static Mock<HttpRequestBase> WithQueryParameter(this Mock<HttpRequestBase> @this, string key, string value)
        {
            if (@this.Object.QueryString[key] == null) {
                @this.Object.QueryString.Add( key, value );
            } else {
                @this.Object.QueryString.Set( key, value );
            }

            return @this;
        }
    }
}