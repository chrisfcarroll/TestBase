using System.Net.Http;
using System.Text.RegularExpressions;

namespace TestBase.HttpClient.Fake
{
    public static class FakeHttpClientSetupGet
    {
        /// <summary>Start to set up this Client to return a desired <see cref="HttpResponseMessage"/> in response to an
        /// incoming <see cref="HttpMethod.Get"/> <see cref="HttpRequestMessage"/> whose
        /// <see cref="HttpRequestMessage.RequestUri"/> matches <paramref name="urlPattern"/>
        /// <example>
        ///    <c>fakehttpClient.Setup("//server/expected/path").Returns(m=> fakeResponse(m) )</c>
        /// </example>
        /// </summary>
        /// <param name="urlPattern"></param>
        /// <returns>this, wrapped awaiting the <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)"/>
        /// call to specify the response</returns>
        public static FakeHttpClient.FakeHttpClientSetup SetupGetUrl(this FakeHttpClient @this, string urlPattern)
        {
            return @this.Setup(m=>m.Method ==HttpMethod.Get && Regex.IsMatch(m.RequestUri.ToString(),urlPattern) );
        }
        
        /// <summary>Start to set up this Client to return a desired <see cref="HttpResponseMessage"/> in response to an
        /// incoming <see cref="HttpMethod.Get"/> <see cref="HttpRequestMessage"/> whose <see cref="HttpRequestMessage.RequestUri"/>'s
        /// <see cref="Uri.PathAndQuery"/> matches <paramref name="urlPattern"/>
        /// <example>
        ///    <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        /// </example>
        /// </summary>
        /// <param name="urlPattern"></param>
        /// <returns>this, wrapped awaiting the <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)"/>
        /// call to specify the response</returns>
        public static FakeHttpClient.FakeHttpClientSetup SetupGetPath(this FakeHttpClient @this, string urlPathMatchingPattern)
        {
            return @this.Setup(m=>m.Method ==HttpMethod.Get && Regex.IsMatch(m.RequestUri.PathAndQuery.ToString(),urlPathMatchingPattern) );
        }
        
    }
}