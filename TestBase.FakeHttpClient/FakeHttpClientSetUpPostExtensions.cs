using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace TestBase.HttpClient.Fake
{
    public static class FakeHttpClientSetUpPostExtensions
    {
        static readonly HttpMethod Post = HttpMethod.Post;

        /// <summary>Start to set up this Client to return a desired <see cref="HttpResponseMessage"/> in response to an
        /// incoming <see cref="HttpMethod.Post"/> <see cref="HttpRequestMessage"/> whose
        /// <see cref="HttpRequestMessage.RequestUri"/> matches <paramref name="urlPattern"/> and whose
        /// <see cref="HttpRequestMessage.Content"/> is <em>byte-for-byte identical</em> to
        /// <paramref name="exactContent"/>
        /// <example>
        ///    <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        /// </example>
        /// </summary>
        /// <param name="urlPattern"></param>
        /// <param name="exactContent">The content to recognise as a match for this setup.
        /// Posted content will be recognised as a match if it is <em>byte-for-byte identical</em>. For
        /// substring matching options, or regular expression matching, use an overload taking a <c>string</c>
        /// or <c>byte[]</c> instead:  
        /// see <seealso cref="SetupPost(TestBase.HttpClient.Fake.FakeHttpClient,string,string)"/>
        /// or <seealso cref="SetupPost(TestBase.HttpClient.Fake.FakeHttpClient,string,byte[])"/>
        /// </param>
        /// <returns>this, wrapped awaiting the
        /// <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)"/>
        /// call to specify the response</returns>
        public static FakeHttpClient.FakeHttpClientSetup 
                        SetupPost(this FakeHttpClient @this, string urlPattern, HttpContent exactContent)
        {
            bool Predicate(HttpRequestMessage m)
            {
                var uriMatches = Regex.IsMatch(m.RequestUri.ToString(), urlPattern);
                
                var setup = exactContent.ReadAsByteArrayAsync().ConfigureFalseGetResult();
                var actual= m.Content?.ReadAsByteArrayAsync().ConfigureFalseGetResult()??new byte[0];
                var contentMatches = 
                            setup.Length == actual.Length 
                         && Enumerable.Range(0, setup.Length).All(i => setup[i] == actual[i]);
                
                return m.Method == Post && uriMatches && contentMatches;
            }

            return @this.Setup(Predicate);
        }
        
        /// <summary>Start to set up this Client to return a desired <see cref="HttpResponseMessage"/> in response to an
        /// incoming <see cref="HttpMethod.Post"/> <see cref="HttpRequestMessage"/> whose
        /// <see cref="HttpRequestMessage.RequestUri"/> matches <paramref name="urlPattern"/> and whose
        /// <see cref="HttpRequestMessage.Content"/> matches the string pattern <paramref name="stringContentPattern"/>
        /// <example>
        ///    <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        /// </example>
        /// </summary>
        /// <param name="urlPattern"></param>
        /// <returns>this, wrapped awaiting the <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)"/>
        /// call to specify the response</returns>
        public static FakeHttpClient.FakeHttpClientSetup SetupPost(this FakeHttpClient @this, string urlPattern, string stringContentPattern)
        {
            bool Predicate(HttpRequestMessage m)
            {
                var urlMatched = Regex.IsMatch(m.RequestUri.ToString(), urlPattern);
                var actualContent = m.Content?.ReadAsStringAsync().ConfigureFalseGetResult() ?? "";
                var contentMatched = Regex.IsMatch(actualContent,stringContentPattern);
                
                return m.Method == Post && urlMatched && contentMatched;
            }

            return @this.Setup( Predicate );
        }

        /// <summary>Start to set up this Client to return a desired <see cref="HttpResponseMessage"/> in response to an
        /// incoming <see cref="HttpMethod.Post"/> <see cref="HttpRequestMessage"/> whose
        /// <see cref="HttpRequestMessage.RequestUri"/> matches <paramref name="urlPattern"/> and whose
        /// <see cref="HttpRequestMessage.Content"/> contains <paramref name="matchingsubarray"/> 
        /// <example>
        ///    <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        /// </example>
        /// </summary>
        /// <param name="urlPattern"></param>
        /// <returns>this, wrapped awaiting the <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)"/>
        /// call to specify the response</returns>
        public static FakeHttpClient.FakeHttpClientSetup SetupPost(this FakeHttpClient @this, string urlPattern, byte[] matchingsubarray)
        {
            bool Predicate(HttpRequestMessage m)
            {
                var uriMatches = Regex.IsMatch(m.RequestUri.ToString(), urlPattern);
                var actual = m.Content?.ReadAsByteArrayAsync().ConfigureFalseGetResult();
                var byteMatchesSubarray = actual?.HasSubArray(matchingsubarray) == true;
                
                return m.Method == Post && uriMatches && byteMatchesSubarray;
            }

            return @this.Setup(Predicate);
        }        
    }
}