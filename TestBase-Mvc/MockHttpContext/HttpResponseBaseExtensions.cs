using System.Web;
using Moq;

namespace TestBase.MockHttpContext
{
    public static class HttpResponseBaseExtensions
    {
        public static Mock<HttpResponseBase> WithSensibleDefaults(this Mock<HttpResponseBase> @this)
        {
            @this.DefaultValue = DefaultValue.Mock;

            @this.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            @this.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            return @this;
        }
    }
}