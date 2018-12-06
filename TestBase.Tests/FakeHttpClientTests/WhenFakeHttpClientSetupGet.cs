using System.Net;
using System.Net.Http;
using NUnit.Framework;
using TestBase.HttpClient.Fake;

namespace TestBase.Tests.FakeHttpClientTests
{
    public class WhenFakeHttpClientSetupGet
    {
        static readonly HttpResponseMessage ExpectedResponse = new HttpResponseMessage(HttpStatusCode.Accepted)
                                                               {Content = new StringContent("I Expected this")};

        static readonly HttpResponseMessage NotFoundResult = new HttpResponseMessage(HttpStatusCode.NotFound);

        [Test]
        public void Should_MatchAGetUrlAndReturnTheSetupResponse()
        {
            var uut = new FakeHttpClient()
                     .SetupGetUrl("//host/")
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            uut.GetAsync("https://host/iexpectedthis")
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult()
               .ShouldEqualByValue(ExpectedResponse);
        }

        [Test]
        public void Should_MatchAGetPathAndReturnTheSetupResponse()
        {
            var uut = new FakeHttpClient()
                     .SetupGetPath("/iexpectedthis/")
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            uut.GetAsync("http://anyhostatall/iexpectedthis/too/")
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult()
               .ShouldEqualByValue(ExpectedResponse);
        }

        [Test]
        public void Should_NotMatchGetUrl_GivenAMismatch()
        {
            var uut = new FakeHttpClient()
                     .SetupGetUrl("//host/")
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            uut.GetAsync("https://otherhost/")
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult()
               .ShouldBe(NotFoundResult);

            uut.GetAsync("https://host/")
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult()
               .ShouldBe(ExpectedResponse);
        }

        [Test]
        public void Should_NotMatchGetPath_GivenAMismatch()
        {
            var uut = new FakeHttpClient()
                     .SetupGetPath("/iexpectedthis")
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            uut.GetAsync("http://anyhostatall/ididntexpectthis")
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult()
               .ShouldBe(NotFoundResult);

            uut.GetAsync("http://anyhostatall/iexpectedthis")
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult()
               .ShouldBe(ExpectedResponse);
        }
    }
}
