using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using TestBase.HttpClient.Fake;

namespace TestBase.Tests.FakeHttpClientTests
{
    [TestFixture]
    public class WhenUsingFakeHttpClient
    {
        [Test]
        public void Should_MatchAnExpectationAndReturnTheSetupResponse__GivenCannedResponse()
        {
            var cannedResponse = new HttpResponseMessage(HttpStatusCode.Accepted){Content = new StringContent("I Expected this")};
            var uut = new FakeHttpClient()
                            .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/iexpectedthis"))
                            .Returns(cannedResponse);

            uut.GetAsync("http://localhost/iexpectedthis")
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .ShouldEqualByValue(cannedResponse);
        }

        [TestCase("heresapath")]
        [TestCase("heresapath/and/a?query=")]
        public void Should_MatchAnExpectationAndReturnTheSetupResponse__GivenResponseFunction(string pathandquery)
        {
            var uut = new FakeHttpClient();

            uut
                .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/here"))
                .Returns(rm=> new HttpResponseMessage(HttpStatusCode.OK){Content=new StringContent(rm.RequestUri.PathAndQuery)});

            uut.GetAsync("http://localhost/" + pathandquery)
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .ShouldEqualByValue(new HttpResponseMessage(HttpStatusCode.OK){Content=new StringContent(pathandquery)});
        }

        [TestCase("theredifferentpath")]
        [TestCase("theredifferentpath/and/a?query=")]
        public void Should_NotMatch(string pathandquery)
        {
            var uut = new FakeHttpClient()
                        .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/here"))
                        .Returns(rm=> new HttpResponseMessage(HttpStatusCode.OK){Content=new StringContent(rm.RequestUri.PathAndQuery)});

            uut.GetAsync("http://localhost/" + pathandquery)
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }

        [Test]
        public void Should_MatchTheRightExpectationAndReturnTheSetupResponse__GivenMultipleSetups()
        {
            var thisResponse = new HttpResponseMessage(HttpStatusCode.OK){Content = new StringContent("I Expected This")};
            var thatResponse = new HttpResponseMessage(HttpStatusCode.Accepted){Headers = { {"That-Header","Value"}}};

            var uut = new FakeHttpClient()
                        .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/this") && x.Method==HttpMethod.Put).Returns(thisResponse)
                        .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/that")).Returns(thatResponse)
                        .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/forbidden")).Returns(new HttpResponseMessage(HttpStatusCode.Forbidden));

            uut.GetAsync("http://localhost/that")
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .ShouldEqualByValue(thatResponse,"/that should have matched That");

            uut.PostAsync("http://localhost/forbidden",new StringContent(@"{""postedContent"":""here""}"))
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .StatusCode.ShouldBe(HttpStatusCode.Forbidden, "/forbidden should be Forbidden");

            uut.PutAsync("http://localhost/this", new FormUrlEncodedContent(new []{new KeyValuePair<string, string>("a","b") }))
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .ShouldEqualByValue(thisResponse,"/this should have matched This");

            uut.GetAsync("http://other/")
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .StatusCode.ShouldBe(HttpStatusCode.InternalServerError, "Other should have fallen back to InternalServerError");
        }

        [Test]
        public void Should_UseOnNoMatch__GivenNoMatch()
        {
            var uut = new FakeHttpClient()
                .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/nowhere"))
                .Returns(rm=> new HttpResponseMessage(HttpStatusCode.OK){Content=new StringContent(rm.RequestUri.PathAndQuery)});

            HttpResponseMessage NoMatchesReturn(HttpRequestMessage rm) => 
                new HttpResponseMessage(HttpStatusCode.Ambiguous) {Headers = {{"Custom", rm.RequestUri.ToString()}}};
            uut.OnNoMatchesReturn = NoMatchesReturn;

            uut.GetAsync("http://localhost/unexpected")
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .Should( r=>r.StatusCode==HttpStatusCode.Ambiguous)
                .Should(r=> r.Headers.GetValues("Custom").Single() == "http://localhost/unexpected");
        }

        [Test]
        public async Task Should_BeVerifiable__GivenMultipleSetups()
        {
            var cannedResponseThis = new HttpResponseMessage(HttpStatusCode.OK){Content = new StringContent("I Expected This")};
            var cannedResponseThat = new HttpResponseMessage(HttpStatusCode.Accepted){Headers = { {"That-Header","Value"}}};

            var uut = new FakeHttpClient()
                .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/this")).Returns(cannedResponseThis)
                .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/that")).Returns(cannedResponseThat)
                .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/forbidden")).Returns(new HttpResponseMessage(HttpStatusCode.Forbidden));
            var that=      await uut.GetAsync("http://localhost/that");
            var forbidden= await uut.GetAsync("http://localhost/forbidden");
            var @this=     await uut.GetAsync("http://localhost/this");
            var other =    await uut.GetAsync("http://other/");

            uut.FakeHttpMessageHandler.VerifyAll();

            uut.FakeHttpMessageHandler.Verify(x => x.RequestUri.ToString() == "http://localhost/that");

            Assert.Throws<Exception>(
                () => uut.FakeHttpMessageHandler.Verify(x => x.Method==HttpMethod.Delete)
            );
        }
    }
}
