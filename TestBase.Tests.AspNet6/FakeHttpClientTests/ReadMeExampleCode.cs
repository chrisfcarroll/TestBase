using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using TestBase.HttpClient.Fake;

namespace TestBase.Tests.AspNet6.FakeHttpClientTests
{
    [TestFixture]
    public class ReadMeExampleCode
    {
        readonly HttpResponseMessage response
        = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent("response")};

        readonly HttpResponseMessage otherResponse
        = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent("otherresponse")};

        [Test]
        public async Task Examples()
        {
            //Arrange
            var httpClient = new FakeHttpClient()
                            .SetupGetUrl("https://host.*/")
                            .Returns(request => "Got:" + request.RequestUri)
                            .SetupGetPath("/uri[Pp]attern/")
                            .Returns("stringcontent")
                            .SetupPost(".*")
                            .Returns(response)
                            .SetupPost(".*", new byte[] {1, 2, 3})
                            .Returns(otherResponse)
                            .SetupPost(".*", "a=1&b=2")
                            .Returns(
                                     request => "You said : "
                                              + request.Content.ReadAsStringAsync().ConfigureFalseGetResult(),
                                     HttpStatusCode.Accepted)
                            .Setup(x => x.RequestUri.PathAndQuery.StartsWith("/this"))
                            .Returns(response)
                            .Setup(x => x.Method == HttpMethod.Put)
                            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));

            // Act
            var putResponse  = await httpClient.PutAsync("http://localhost/thing", new StringContent("{a=1,b=2}"));
            var postResponse = await httpClient.PostAsync("http://[::1]/", new StringContent("a=1&b=2"));

            //Debug
            httpClient.Invocations
                      .ForEach(async i => Console.WriteLine("{0} {1}",
                                                            i.RequestUri,
                                                            await i.Content.ReadAsStringAsync()));


            //Assert
            putResponse.StatusCode.ShouldBe(HttpStatusCode.Accepted);
            postResponse.ShouldBe(response); // ==> SetupPost(".*").Returns(response) was the first 
            // matched setup. Setups are tried in first-to-last order.                                            

            httpClient.Verify(x => x.Method == HttpMethod.Put, "Expected Put, but no matching invocations.");
            httpClient.Verify(
                              x => x.Method                                                == HttpMethod.Post
                                && x.Content.ReadAsStringAsync().ConfigureFalseGetResult() == "a=1&b=2",
                              "Expected Post a=1&b=2");


            Assert.Throws<Exception>(
                                     () => httpClient.VerifyAll() // ==> "Exception : 4 unmatched expectations")
                                    );
        }
    }
}
