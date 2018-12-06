using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace TestBase.HttpClient.Fake
{
    /// <summary>
    ///     A <c>FakeHttpClient</c> can be used to test code written for a <see cref="HttpClient" />. It matches incoming
    ///     <see cref="HttpRequestMessage" />s against the setup expectations. If a match  is found it returns the setup
    ///     <see cref="HttpResponseMessage" /> or,  more generally, applies a given
    ///     <see cref="Func{T,TResult}" /> to generate a <see cref="HttpRequestMessage" /> based on
    ///     the incoming <see cref="HttpRequestMessage" />.
    ///     Use overloads of <see cref="Setup" /> to setup expectations.
    ///     Set <see cref="OnNoMatchesReturn" /> to set up a response for when an incoming message matches no expectation.
    ///     Call <see cref="Verify" /> or <see cref="VerifyAll" /> for verifying whether an expectation was matched.
    ///     Examine <see cref="Invocations" /> to see what was actually invoked on the <c>FakeHttpClient</c>
    /// </summary>
    public class FakeHttpClient : System.Net.Http.HttpClient
    {
        /// <summary>Create a new <see cref="FakeHttpClient" /></summary>
        /// <param name="handler">Otional. Provide an existing <see cref="FakeHttpMessageHandler" /> to handle requests</param>
        public FakeHttpClient(FakeHttpMessageHandler handler = null) :
        base(handler = handler ?? new FakeHttpMessageHandler())
        {
            FakeHttpMessageHandler = handler;
        }

        /// <summary>A list of incoming messages received, paired with the response that was sent to them.</summary>
        public List<HttpRequestMessage> Invocations => FakeHttpMessageHandler.Invocations;

        /// <summary>
        ///     The "Predicate Dictionary" build up by <see cref="Setup" />, to match incoming requests and decide what
        ///     response to send.
        /// </summary>
        public Dictionary<HttpRequestMessage, HttpResponseMessage> InvocationResults
            => FakeHttpMessageHandler.InvocationResults;


        /// <summary>
        ///     What to return if no expectation matches the incoming request. Defaults to
        ///     <see cref="FakeHttpMessageHandler.InternalServerError" />
        /// </summary>
        public Func<HttpRequestMessage, HttpResponseMessage> OnNoMatchesReturn
        {
            get => FakeHttpMessageHandler.OnNoMatchesReturn;
            set => FakeHttpMessageHandler.OnNoMatchesReturn = value;
        }

        /// <summary>The instance of <see cref="HttpMessageHandler" /> to which all requests are passed.</summary>
        public FakeHttpMessageHandler FakeHttpMessageHandler { get; }

        /// <summary>
        ///     Setup this FakeHttpClient to return a desired <see cref="HttpResponseMessage" /> in response to a
        ///     <see cref="HttpRequestMessage" /> which satisfies the predicate <paramref name="messageMatchesPredicate" />
        ///     <example>
        ///         <c>fakehttpClient.Setup(m=>m.Uri="..." && m.Method=HttpMethod.Post).Returns(m=> fakeResponse(m) )</c>
        ///     </example>
        /// </summary>
        /// <param name="messageMatchesPredicate">The test to apply to incoming messages</param>
        /// <returns>
        ///     this, wrapped awaiting the <see cref="FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)" />
        ///     call to specify the response
        /// </returns>
        public FakeHttpClientSetup Setup(Func<HttpRequestMessage, bool> messageMatchesPredicate)
        {
            return new FakeHttpClientSetup(this, messageMatchesPredicate);
        }

        public FakeHttpClient VerifyAll()
        {
            FakeHttpMessageHandler.VerifyAll();
            return this;
        }

        public FakeHttpClient Verify(Func<HttpRequestMessage, bool> messageMatchesPredicate)
        {
            FakeHttpMessageHandler.Verify(messageMatchesPredicate);
            return this;
        }

        public FakeHttpClient Verify(
            Func<HttpRequestMessage, bool> messageMatchesPredicate,
            string                         failureMessage,
            params object[]                args)
        {
            FakeHttpMessageHandler.Verify(messageMatchesPredicate, failureMessage);
            return this;
        }


        public class FakeHttpClientSetup
        {
            readonly FakeHttpClient client;
            readonly Func<HttpRequestMessage, bool> messageMatchesPredicate;

            public FakeHttpClientSetup(FakeHttpClient client, Func<HttpRequestMessage, bool> messageMatchesPredicate)
            {
                this.client                  = client;
                this.messageMatchesPredicate = messageMatchesPredicate;
            }

            /// <summary>Specify the response to return for incoming messages matching the current setup</summary>
            /// <param name="desiredResponse">
            ///     Specify a function to create the resulting <see cref="HttpResponseMessage" /> from the
            ///     incoming <see cref="HttpRequestMessage" />
            /// </param>
            /// <returns>The <see cref="FakeHttpClient" /> being setup.</returns>
            public FakeHttpClient Returns(HttpResponseMessage desiredResponse)
            {
                client.FakeHttpMessageHandler.Expectations.Add(messageMatchesPredicate, rm => desiredResponse);
                return client;
            }

            /// <summary>Specify the response to return for incoming messages matching the current setup</summary>
            /// <param name="desiredResponse">Specify a single fixed response</param>
            /// <returns>The <see cref="FakeHttpClient" /> being setup.</returns>
            public FakeHttpClient Returns(Func<HttpRequestMessage, HttpResponseMessage> desiredResponse)
            {
                client.FakeHttpMessageHandler.Expectations.Add(messageMatchesPredicate, rm => desiredResponse(rm));
                return client;
            }

            /// <summary>Specify the response to return for incoming messages matching the current setup</summary>
            /// <param name="desiredResponse">
            ///     Specify a function of no parameters to create the resulting
            ///     <see cref="HttpResponseMessage" />
            /// </param>
            /// <returns>The <see cref="FakeHttpClient" /> being setup.</returns>
            public FakeHttpClient Returns(Func<HttpResponseMessage> desiredResponse)
            {
                client.FakeHttpMessageHandler.Expectations.Add(messageMatchesPredicate, rm => desiredResponse());
                return client;
            }

            /// <summary>Specify a <see cref="StringContent" /> response to return for incoming messages matching the current setup</summary>
            /// <param name="desiredResponseStringContent">
            ///     Specify a function of no parameters to create the resulting
            ///     <see cref="HttpResponseMessage" />
            /// </param>
            /// <param name="statusCode"></param>
            /// <returns>The <see cref="FakeHttpClient" /> being setup.</returns>
            public FakeHttpClient Returns(
                Func<HttpRequestMessage, string> desiredResponseStringContent,
                HttpStatusCode                   statusCode = HttpStatusCode.OK)
            {
                client.FakeHttpMessageHandler
                      .Expectations
                      .Add(messageMatchesPredicate,
                           rm => new HttpResponseMessage(statusCode)
                                 {
                                 Content = new StringContent(desiredResponseStringContent(rm))
                                 });
                return client;
            }

            public FakeHttpClient Returns(string desiredResponseStringContent)
            {
                return Returns(m => desiredResponseStringContent);
            }
        }
    }
}
