﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestBase.HttpClient.Fake
{
    /// <summary>
    ///     A low-level Fake for <see cref="HttpMessageHandler" />.
    ///     Setup expectations using
    ///     <see
    ///         cref="Setup(System.Func{System.Net.Http.HttpRequestMessage,bool},System.Func{System.Net.Http.HttpRequestMessage,System.Net.Http.HttpResponseMessage})" />
    ///     ,
    ///     Verify with <see cref="Verify(System.Func{System.Net.Http.HttpRequestMessage,bool})" />
    ///     Used by <see cref="FakeHttpClient" />
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        ///     An response for an unexpected incoming message: it returns a
        ///     <seealso cref="HttpStatusCode.InternalServerError" />
        /// </summary>
        /// <example>
        ///     <code>myFakeHandler.OnNoMatches=FakeHttpMessageHandler.NotFoundResponse</code>
        /// </example>
        public static readonly Func<FakeHttpMessageHandler, HttpRequestMessage, HttpResponseMessage>
        NotFoundResponse = (@this, msg) => new HttpResponseMessage(HttpStatusCode.NotFound)
                                           {
                                           ReasonPhrase   = ReasonPhraseFromMsgAndExpectations(@this, msg),
                                           RequestMessage = msg
                                           };

        /// <summary>
        ///     The default response for an unexpected incoming message: it returns a
        ///     <seealso cref="HttpStatusCode.InternalServerError" />
        /// </summary>
        public static readonly Func<FakeHttpMessageHandler, HttpRequestMessage, HttpResponseMessage>
        InternalServerError = (@this, msg) => new HttpResponseMessage(HttpStatusCode.InternalServerError)
                                              {
                                              ReasonPhrase   = ReasonPhraseFromMsgAndExpectations(@this, msg),
                                              RequestMessage = msg
                                              };


        /// <summary>A list of incoming messages received, paired with the response that was sent to them.</summary>
        public readonly Dictionary<HttpRequestMessage, HttpResponseMessage> InvocationResults =
        new Dictionary<HttpRequestMessage, HttpResponseMessage>();

        /// <summary>The list of incoming requests received.</summary>
        public readonly List<HttpRequestMessage> Invocations = new List<HttpRequestMessage>();

        /// <summary>
        ///     The "Predicate Dictionary" build up by <see cref="Setup" />, to match incoming requests and decide what
        ///     response to send.
        /// </summary>
        public Dictionary<Func<HttpRequestMessage, bool>, Func<HttpRequestMessage, HttpResponseMessage>>
        Expectations = new Dictionary<Func<HttpRequestMessage, bool>, Func<HttpRequestMessage, HttpResponseMessage>>();

        /// <summary>
        ///     What to return if no expectation matches the incoming request. Defaults to <see cref="InternalServerError" />
        /// </summary>
        public Func<HttpRequestMessage, HttpResponseMessage> OnNoMatchesReturn;

        /// <summary>Create a <see cref="FakeHttpMessageHandler" /></summary>
        /// <param name="name">Used in reporting and throw exceptions.</param>
        public FakeHttpMessageHandler(string name)
        {
            Name              = name;
            OnNoMatchesReturn = msg => InternalServerError(this, msg);
        }

        public FakeHttpMessageHandler() : this("Unnamed") { }
        public string Name { get; }

        public FakeHttpMessageHandler Setup(
            Func<HttpRequestMessage, bool>                messageMatchesPredicate,
            Func<HttpRequestMessage, HttpResponseMessage> desiredResponse)
        {
            Expectations.Add(messageMatchesPredicate, desiredResponse);
            return this;
        }

        public FakeHttpMessageHandler Setup(
            Func<HttpRequestMessage, bool> messageMatchesPredicate,
            HttpResponseMessage            desiredResponse)
        {
            Expectations.Add(messageMatchesPredicate, r => desiredResponse);
            return this;
        }

        /// <summary>
        ///     Find the first invocation that satisfies <paramref name="messageMatchesPredicate" />, or throw if none is
        ///     found.
        /// </summary>
        /// <param name="messageMatchesPredicate">
        ///     A predicate that the incoming <see cref="HttpRequestMessage" /> should have
        ///     satisfied.
        /// </param>
        /// <returns>Both the recorded incoming Request, and the outgoing response that returned for it.</returns>
        /// <exception cref="Exception">Thrown if no matching invocation is found.</exception>
        public KeyValuePair<HttpRequestMessage, HttpResponseMessage> Verify(
            Func<HttpRequestMessage, bool> messageMatchesPredicate)
        {
            try { return InvocationResults.First(i => messageMatchesPredicate(i.Key)); } catch
            {
                throw new Exception("No matching invocations were recorded");
            }
        }

        /// <summary>
        ///     Find the first invocation that satisfies <paramref name="messageMatchesPredicate" />, or throw if none is
        ///     found.
        /// </summary>
        /// <param name="messageMatchesPredicate">
        ///     A predicate that the incoming <see cref="HttpRequestMessage" /> should have
        ///     satisfied.
        /// </param>
        /// <param name="failureMessage">optional Exception message to show in case of failure</param>
        /// <param name="failureMessageArgs"></param>
        /// <returns>Both the recorded incoming Request, and the outgoing response that returned for it.</returns>
        /// <exception cref="Exception">Thrown if no matching invocation is found.</exception>
        public KeyValuePair<HttpRequestMessage, HttpResponseMessage> Verify(
            Func<HttpRequestMessage, bool> messageMatchesPredicate,
            string                         failureMessage = null,
            params object[]                failureMessageArgs)
        {
            try { return InvocationResults.First(i => messageMatchesPredicate(i.Key)); } catch
            {
                throw new Exception(string.Format(failureMessage ?? "No matching invocations were recorded",
                                                  failureMessageArgs));
            }
        }

        /// <summary>Throws if any <see cref="Expectations" /> were set but not met.</summary>
        /// <returns>this</returns>
        public FakeHttpMessageHandler VerifyAll()
        {
            var unmatched = Expectations.Where(e => !Invocations.Any(i => e.Key(i)));
            return !unmatched.Any()
                   ? this
                   : throw new Exception($"{unmatched.Count()} ummatched expectations.");
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken  cancellationToken)
        {
            Invocations.Add(request);
            var matchedExpectation = Expectations.FirstOrDefault(p => p.Key(request));
            if ( /*NoMatch*/ matchedExpectation.Value == null)
            {
                var noResult = OnNoMatchesReturn(request);
                noResult.RequestMessage = request;
                InvocationResults.Add(request, null);
                return Task.FromResult(noResult);
            }
            else
            {
                var result = matchedExpectation.Value(request);
                InvocationResults.Add(request, result);
                return Task.FromResult(result);
            }
        }

        static string ReasonPhraseFromMsgAndExpectations(FakeHttpMessageHandler @this, HttpRequestMessage msg)
        {
            return $"No stubbed expectations matched {msg.ToString().Replace('\n', ' ').Replace("\r", "")}";
        }
    }
}
