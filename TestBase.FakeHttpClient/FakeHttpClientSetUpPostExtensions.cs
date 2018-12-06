using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace TestBase.HttpClient.Fake
{
    public static class FakeHttpClientSetUpPostExtensions
    {
        static readonly HttpMethod Post = HttpMethod.Post;

        /// <summary>
        ///     Setup this Client to return a desired <see cref="HttpResponseMessage" /> in response to any
        ///     <see cref="HttpMethod.Post" /> <see cref="HttpRequestMessage" /> whose
        ///     <see cref="HttpRequestMessage.RequestUri" /> matches <paramref name="urlPattern" />.
        ///     <example>
        ///         <c>fakehttpClient.SetupPost("/expected/path").Returns(m=> fakeResponse(m) )</c>
        ///     </example>
        /// </summary>
        /// <param name="this">The <see cref="FakeHttpClient" /> being setup</param>
        /// <param name="urlPattern">The regex pattern to match incoming <see cref="HttpRequestMessage.RequestUri" /> against.</param>
        /// <returns>
        ///     this, wrapped awaiting the <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(HttpResponseMessage)" />
        ///     call to specify the response
        /// </returns>
        public static FakeHttpClient.FakeHttpClientSetup
        SetupPost(this FakeHttpClient @this, string urlPattern)
        {
            bool Predicate(HttpRequestMessage m)
            {
                return m.Method == Post && Regex.IsMatch(m.RequestUri.ToString(), urlPattern);
            }

            return @this.Setup(Predicate);
        }

        /// <summary>
        ///     Setup this Client to return a desired <see cref="HttpResponseMessage" /> in response to a
        ///     <see cref="HttpMethod.Post" /> <see cref="HttpRequestMessage" /> whose
        ///     <see cref="HttpRequestMessage.RequestUri" /> matches <paramref name="urlPattern" /> and whose
        ///     <see cref="HttpRequestMessage.Content" /> is <em>byte-for-byte identical</em> to
        ///     <paramref name="exactContent" />
        ///     <example>
        ///         <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        ///     </example>
        /// </summary>
        /// <param name="this">The <see cref="FakeHttpClient" /> being setup</param>
        /// <param name="urlPattern">The regex pattern to match incoming <see cref="HttpRequestMessage.RequestUri" /> against.</param>
        /// <param name="exactContent">
        ///     The content to recognise as a match for this setup.
        ///     Posted content will be recognised as a match if it is <em>byte-for-byte identical</em>. For
        ///     substring matching options, or regular expression matching, use an overload taking a <c>string</c>
        ///     or <c>byte[]</c> instead:
        ///     see <seealso cref="SetupPost(TestBase.HttpClient.Fake.FakeHttpClient,string,string)" />
        ///     or <seealso cref="SetupPost(TestBase.HttpClient.Fake.FakeHttpClient,string,byte[])" />
        /// </param>
        /// <returns>
        ///     this, wrapped awaiting the
        ///     <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)" />
        ///     call to specify the response
        /// </returns>
        public static FakeHttpClient.FakeHttpClientSetup
        SetupPost(this FakeHttpClient @this, string urlPattern, HttpContent exactContent)
        {
            bool Predicate(HttpRequestMessage m)
            {
                var uriMatches = Regex.IsMatch(m.RequestUri.ToString(), urlPattern);

                var setup  = exactContent.ReadAsByteArrayAsync().ConfigureFalseGetResult();
                var actual = m.Content?.ReadAsByteArrayAsync().ConfigureFalseGetResult() ?? new byte[0];
                var contentMatches =
                setup.Length == actual.Length
             && Enumerable.Range(0, setup.Length).All(i => setup[i] == actual[i]);

                return m.Method == Post && uriMatches && contentMatches;
            }

            return @this.Setup(Predicate);
        }

        /// <summary>
        ///     Setup this Client to return a desired <see cref="HttpResponseMessage" /> in response to a
        ///     incoming <see cref="HttpMethod.Post" /> <see cref="HttpRequestMessage" /> whose
        ///     <see cref="HttpRequestMessage.RequestUri" /> matches <paramref name="urlPattern" /> and whose
        ///     <see cref="HttpRequestMessage.Content" /> matches the regex pattern <paramref name="contentPattern" />
        ///     <example>
        ///         <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        ///     </example>
        /// </summary>
        /// <param name="this">The <see cref="FakeHttpClient" /> being setup</param>
        /// <param name="urlPattern">The regex pattern to match incoming <see cref="HttpRequestMessage.RequestUri" /> against.</param>
        /// <param name="contentPattern"></param>
        /// <returns>
        ///     this, wrapped awaiting the
        ///     <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)" />
        ///     call to specify the response
        /// </returns>
        public static FakeHttpClient.FakeHttpClientSetup SetupPost(
            this FakeHttpClient @this,
            string              urlPattern,
            string              contentPattern)
        {
            bool Predicate(HttpRequestMessage m)
            {
                var urlMatched     = Regex.IsMatch(m.RequestUri.ToString(), urlPattern);
                var actualContent  = m.Content?.ReadAsStringAsync().ConfigureFalseGetResult() ?? "";
                var contentMatched = Regex.IsMatch(actualContent, contentPattern);

                return m.Method == Post && urlMatched && contentMatched;
            }

            return @this.Setup(Predicate);
        }

        /// <summary>
        ///     Start to set up this Client to return a desired <see cref="HttpResponseMessage" /> in response to an
        ///     incoming <see cref="HttpMethod.Post" /> <see cref="HttpRequestMessage" /> whose
        ///     <see cref="HttpRequestMessage.RequestUri" /> matches <paramref name="urlPattern" /> and whose
        ///     <see cref="HttpRequestMessage.Content" /> contains <paramref name="subarrayToMatch" />
        ///     <example>
        ///         <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        ///     </example>
        /// </summary>
        /// <param name="this">The <see cref="FakeHttpClient" /> being setup</param>
        /// <param name="urlPattern">The regex pattern to match incoming <see cref="HttpRequestMessage.RequestUri" /> against.</param>
        /// <param name="subarrayToMatch">
        ///     The incoming <see cref="HttpRequestMessage.Content" /> will be read as a
        ///     <see cref="ByteArrayContent" /> and searched for whether the byte sequence <paramref name="subarrayToMatch" /> is
        ///     found within it.
        /// </param>
        /// <returns>
        ///     this, wrapped awaiting the <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(HttpResponseMessage)" />
        ///     call to specify the response
        /// </returns>
        public static FakeHttpClient.FakeHttpClientSetup SetupPost(
            this FakeHttpClient @this,
            string              urlPattern,
            byte[]              subarrayToMatch)
        {
            bool Predicate(HttpRequestMessage m)
            {
                var uriMatches          = Regex.IsMatch(m.RequestUri.ToString(), urlPattern);
                var actual              = m.Content?.ReadAsByteArrayAsync().ConfigureFalseGetResult();
                var byteMatchesSubarray = actual?.HasSubArray(subarrayToMatch) == true;

                return m.Method == Post && uriMatches && byteMatchesSubarray;
            }

            return @this.Setup(Predicate);
        }

        /// <summary>
        ///     Start to set up this Client to return a desired <see cref="HttpResponseMessage" /> in response to an
        ///     incoming <see cref="HttpMethod.Post" /> <see cref="HttpRequestMessage" /> whose
        ///     <see cref="HttpRequestMessage.RequestUri" /> matches <paramref name="urlPattern" /> and whose
        ///     <see cref="HttpRequestMessage.Content" /> satisfies the <paramref name="streamPredicate" />
        ///     <example>
        ///         <c>fakehttpClient.Setup("/expected/path").Returns(m=> fakeResponse(m) )</c>
        ///     </example>
        /// </summary>
        /// <param name="this">The <see cref="FakeHttpClient" /> being setup</param>
        /// <param name="urlPattern">The regex pattern to match incoming <see cref="HttpRequestMessage.RequestUri" /> against.</param>
        /// <param name="streamPredicate">
        ///     The incoming <see cref="HttpRequestMessage.Content" /> will be passed to this
        ///     to determine whether it counts as a match or not.
        /// </param>
        /// <returns>
        ///     this, wrapped awaiting the <see cref="FakeHttpClient.FakeHttpClientSetup.Returns(HttpResponseMessage)" />
        ///     call to specify the response
        /// </returns>
        public static FakeHttpClient.FakeHttpClientSetup SetupPost(
            this FakeHttpClient @this,
            string              urlPattern,
            Func<Stream, bool>  streamPredicate)
        {
            bool Predicate(HttpRequestMessage m)
            {
                var uriMatches    = Regex.IsMatch(m.RequestUri.ToString(), urlPattern);
                var streamMatches = streamPredicate(m.Content.ReadAsStreamAsync().ConfigureFalseGetResult());

                return m.Method == Post && uriMatches && streamMatches;
            }

            return @this.Setup(Predicate);
        }
    }
}
