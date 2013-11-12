using System.Web;
using Moq;

namespace TestBase.MockHttpContext
{
    public static class HttpContextBaseExtensions
    {
        public static Mock<HttpContextBase> WithSensibleDefaults(this Mock<HttpContextBase> @this)
        {
            @this.DefaultValue = DefaultValue.Mock;

            @this.Setup(x => x.Session)
                .Returns(new Mock<HttpSessionStateBase> { DefaultValue = DefaultValue.Empty }.Object);

            @this.WithRequest(new Mock<HttpRequestBase>().WithSensibleDefaults().Object);
            @this.WithResponse(new Mock<HttpResponseBase>().WithSensibleDefaults().Object);
            return @this;
        }

        public static Mock<HttpServerUtilityBase> WithServer(this Mock<HttpContextBase> @this)
        {
            var mock = new Mock<HttpServerUtilityBase>();
            @this.Setup(x => x.Server).Returns(mock.Object);
            return mock;
        }

        public static Mock<HttpRequestBase> Request(this Mock<HttpContextBase> @this)
        {
            return Mock.Get(@this.Object.Request);
        }

        public static Mock<HttpResponseBase> Response(this Mock<HttpContextBase> @this)
        {
            return Mock.Get(@this.Object.Response);
        }

        public static Mock<HttpSessionStateBase> Session(this Mock<HttpContextBase> @this)
        {
            return Mock.Get(@this.Object.Session);
        }

        public static Mock<HttpContextBase> WithRequest(this Mock<HttpContextBase> @this, HttpRequestBase value)
        {
            @this.Setup(x => x.Request).Returns(value);
            return @this;
        }

        public static Mock<HttpContextBase> WithResponse(this Mock<HttpContextBase> @this, HttpResponseBase value)
        {
            @this.Setup(x => x.Response).Returns(value);
            return @this;
        }
    }
}