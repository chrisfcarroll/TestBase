using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace TestBase.HttpClient.Fake
{
    /// <summary>
    /// A Fake HttpClient which matches incoming <see cref="HttpRequestMessage"/>s against 
    /// the expectations that were setup in <see cref="TestFwk.StubbedHttpMessageHandler.Expectations"/>
    /// 
    /// Use <see cref="SetupForSendAsync"/> to setup expectations
    /// Use <see cref="OnNoMatchesReturn"/> to set up a response for when an incoming message matches no expectation.
    /// </summary>
    public class FakeHttpClient : System.Net.Http.HttpClient
    {
        /// <summary>
        /// Start to set up this Client to return a desired <see cref="HttpResponseMessage"/> in response to an incoming
        /// <see cref="HttpRequestMessage"/> which satisfies the predicate <paramref name="messageMatchesPredicate"/>
        /// <example>
        ///    <c>fakehttpClient.Setup(m=>m.Uri="..." && m.Method=HttpMethod.Post).Returns(m=> fakeResponse(m) )</c>
        /// </example>
        /// </summary>
        /// <param name="messageMatchesPredicate">The test to apply to incoming messages</param>
        /// <returns>this, wrapped awaiting the <see cref="FakeHttpClientSetup.Returns(System.Net.Http.HttpResponseMessage)"/>
        /// call to specify the response</returns>
        public FakeHttpClientSetup Setup(Func<HttpRequestMessage, bool> messageMatchesPredicate)
        {
            return new FakeHttpClientSetup(this, messageMatchesPredicate);
        }

        /// <summary>A list of incoming messages received, paired with the response that was sent to them.</summary>
        public List<HttpRequestMessage> Invocations => FakeHttpMessageHandler.Invocations;

        /// <summary>The "Predicate Dictionary" build up by <see cref="Setup"/>, to match incoming requests and decide what response to send.</summary>
        public Dictionary<HttpRequestMessage, HttpResponseMessage> InvocationResults => FakeHttpMessageHandler.InvocationResults;


        /// <summary>What to return if no expectation matches the incoming request. Defaults to <see cref="FakeHttpMessageHandler.InternalServerError"/></summary>
        public Func<HttpRequestMessage, HttpResponseMessage> OnNoMatchesReturn
        {
            get => FakeHttpMessageHandler.OnNoMatchesReturn;
            set => FakeHttpMessageHandler.OnNoMatchesReturn = value;
        }

        /// <summary>The instance of <see cref="HttpMessageHandler"/> to which all requests are passed.</summary>
        public FakeHttpMessageHandler FakeHttpMessageHandler { get; }

        /// <summary>Create a new <see cref="FakeHttpClient"/></summary>
        /// <param name="handler">Otional. Provide an existing <see cref="FakeHttpMessageHandler"/> to handle requests</param>
        public FakeHttpClient(FakeHttpMessageHandler handler=null) : base(handler=handler??new FakeHttpMessageHandler()){FakeHttpMessageHandler = handler;}

        public class FakeHttpClientSetup
        {
            readonly FakeHttpClient client;
            readonly Func<HttpRequestMessage, bool> messageMatchesPredicate;

            public FakeHttpClientSetup(FakeHttpClient client, Func<HttpRequestMessage, bool> messageMatchesPredicate)
            {
                this.client = client;
                this.messageMatchesPredicate = messageMatchesPredicate;
            }

            /// <summary>Specify the response to return for incoming messages matching the current setup</summary>
            /// <param name="desiredResponse">Specify a function to create the resulting <see cref="HttpResponseMessage"/> from the incoming <see cref="HttpRequestMessage"/></param>
            /// <returns>The <see cref="FakeHttpClient"/> being setup.</returns>
            public FakeHttpClient Returns(HttpResponseMessage desiredResponse)
            {
                client.FakeHttpMessageHandler.Expectations.Add(messageMatchesPredicate, rm=>desiredResponse);
                return client;
            }
            /// <summary>Specify the response to return for incoming messages matching the current setup</summary>
            /// <param name="desiredResponse">Specify a single fixed response</param>
            /// <returns>The <see cref="FakeHttpClient"/> being setup.</returns>
            public FakeHttpClient Returns(Func<HttpRequestMessage,HttpResponseMessage> desiredResponse)
            {
                client.FakeHttpMessageHandler.Expectations.Add(messageMatchesPredicate, rm=>desiredResponse(rm));
                return client;
            }
            /// <summary>Specify the response to return for incoming messages matching the current setup</summary>
            /// <param name="desiredResponse">Specify a function of no parameters to create the resulting <see cref="HttpResponseMessage"/></param>
            /// <returns>The <see cref="FakeHttpClient"/> being setup.</returns>
            public FakeHttpClient Returns(Func<HttpResponseMessage> desiredResponse)
            {
                client.FakeHttpMessageHandler.Expectations.Add(messageMatchesPredicate, rm=>desiredResponse());
                return client;
            }

            /// <summary>Specify a <see cref="StringContent"/> response to return for incoming messages matching
            /// the current setup</summary>
            /// <param name="desiredResponse">Specify a function of no parameters to create the resulting <see cref="HttpResponseMessage"/></param>
            /// <param name="statusCode"></param>
            /// <returns>The <see cref="FakeHttpClient"/> being setup.</returns>
            public FakeHttpClient Returns(
                                    Func<HttpRequestMessage,string> desiredResponseStringContent,
                                    HttpStatusCode statusCode= HttpStatusCode.OK)
            {
                client.FakeHttpMessageHandler
                      .Expectations
                      .Add(messageMatchesPredicate, 
                           rm=>new HttpResponseMessage(statusCode)
                           {
                               Content =new StringContent(desiredResponseStringContent(rm)) 
                           });
                return client;
            }
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
    }
}