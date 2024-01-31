using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TestBase.HttpClient.Fake;

namespace TestBase.Tests.AspNet6.FakeHttpClientTests
{
    public class WhenFakeHttpClientSetupPost
    {
        static readonly HttpResponseMessage ExpectedResponse = new HttpResponseMessage(HttpStatusCode.Accepted)
                                                               {Content = new StringContent("I Expected this")};

        static readonly HttpResponseMessage NotFoundResult = new HttpResponseMessage(HttpStatusCode.NotFound);

        [Test]
        public async Task Should_MatchPostUrlAndStringContentAndReturnTheSetupResponse()
        {
            var form = "a=1&b=2";
            var uut = new FakeHttpClient()
                     .SetupPost("//host/", form)
                     .Returns(ExpectedResponse);

            //
            var result = await uut.PostAsync("https://host/iexpectedthis", new StringContent(form));

            //
            result.ShouldEqualByValue(ExpectedResponse);
        }

        [Test]
        public async Task Should_MatchPostUrlAndStringContentContainingSetupAndReturnTheSetupResponse()
        {
            var form           = "a=1&b=2";
            var containingform = "more&" + form + "&evenmore";
            var uut = new FakeHttpClient()
                     .SetupPost("//host/", form)
                     .Returns(ExpectedResponse);

            //
            var result =
            await uut.PostAsync("https://host/iexpectedthis", new StringContent(containingform));

            //
            result.ShouldEqualByValue(ExpectedResponse);
        }

        [Test]
        public void Should_MatchPostUrlAndByteArrayContentContainingSetupAndReturnTheSetupResponse()
        {
            var setupBytes = new byte[100];
            new Random().NextBytes(setupBytes);
            var bytesincludingsetup = new byte[] {1, 2, 3}
                                     .Concat(setupBytes)
                                     .Concat(new byte[] {4, 5, 6})
                                     .ToArray();
            var uut = new FakeHttpClient()
                     .SetupPost("//host/", setupBytes)
                     .Returns(ExpectedResponse);

            //
            uut.PostAsync("https://host/iexpectedthis", new ByteArrayContent(bytesincludingsetup))
               .ConfigureFalseGetResult()
               .ShouldEqualByValue(ExpectedResponse);
        }

        [Test]
        public void Should_MatchPostDotStarUrlAndStringContentAndReturnTheSetupResponse()
        {
            var bytes = new byte[100];
            new Random().NextBytes(bytes);
            var setup  = new StreamContent(new MemoryStream(bytes));
            var posted = new StreamContent(new MemoryStream(bytes.Clone() as byte[]));
            var uut = new FakeHttpClient()
                     .SetupPost(".*", setup)
                     .Returns(ExpectedResponse);

            uut.PostAsync("http://anyhostatall/iexpectedthis/too/", posted)
               .ConfigureFalseGetResult()
               .ShouldEqualByValue(ExpectedResponse);
        }

        [Test]
        public void Should_MatchPostUrlAndStreamContentAndReturnTheSetupResponse()
        {
            var bytes = new byte[100];
            new Random().NextBytes(bytes);
            var setup  = new StreamContent(new MemoryStream(bytes));
            var posted = new StreamContent(new MemoryStream(bytes.Clone() as byte[]));

            var uut = new FakeHttpClient()
                     .SetupPost("//host/", setup)
                     .Returns(ExpectedResponse);

            uut.PostAsync("https://host/iexpectedthis", posted)
               .ConfigureFalseGetResult()
               .ShouldEqualByValue(ExpectedResponse);
        }

        [Test]
        public void Should_NotMatchPostMismatchedUrlWithStringContent()
        {
            var form = "a=1&b=2";

            var uut = new FakeHttpClient()
                     .SetupPost("//host/", form)
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            uut.PostAsync("https://notthishost/", new StringContent(form))
               .ConfigureFalseGetResult()
               .ShouldBe(NotFoundResult);

            uut.PostAsync("https://host/yesthishost", new StringContent(form))
               .ConfigureFalseGetResult()
               .ShouldBe(ExpectedResponse);
        }

        [Test]
        public void Should_NotMatchPostUrlAndMismatchedStringContent()
        {
            var form = "a=1&b=2";
            var uut = new FakeHttpClient()
                     .SetupPost("//host/", form)
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            uut.PostAsync("https://host/iexpectedthis", new StringContent("butididntexpect=this"))
               .ConfigureFalseGetResult()
               .ShouldBe(NotFoundResult);

            uut.PostAsync("https://host/iexpectedthis", new StringContent(form))
               .ConfigureFalseGetResult()
               .ShouldBe(ExpectedResponse);
        }

        [Test]
        public void Should_NotMatchPostUrlAndMismatchedStreamContent()
        {
            var bytes = new byte[100];
            new Random().NextBytes(bytes);
            var setup       = new StreamContent(new MemoryStream(bytes));
            var sameAsSetup = new StreamContent(new MemoryStream(bytes));
            var mismatch    = new StreamContent(new MemoryStream(Encoding.Unicode.GetBytes("NotTheSame")));

            var uut = new FakeHttpClient()
                     .SetupPost("//host/", setup)
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            uut.PostAsync("https://host/iexpectedthis", mismatch)
               .ConfigureFalseGetResult()
               .ShouldBe(NotFoundResult);

            uut.PostAsync("https://host/iexpectedthis", sameAsSetup)
               .ConfigureFalseGetResult()
               .ShouldBe(ExpectedResponse);
        }

        [Test]
        public void Should_NotMatchPostUrlAndMismatchedByteArray()
        {
            var setupBytes = new byte[100];
            new Random().NextBytes(setupBytes);
            var different = new ByteArrayContent(setupBytes.Reverse().ToArray());

            var uut = new FakeHttpClient()
                     .SetupPost("//host/", setupBytes)
                     .Returns(ExpectedResponse)
                     .With(u => u.OnNoMatchesReturn = _ => NotFoundResult);

            var result = uut.PostAsync("https://host/", different)
                            .ConfigureFalseGetResult();
        }
    }
}
