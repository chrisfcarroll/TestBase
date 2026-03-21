using System.Net;

namespace TestBase;

internal class FakeHttpMessageHandler : HttpMessageHandler
{
    readonly List<Expectation> _expectations = new();
    readonly List<HttpRequestMessage> _requests = new();
    readonly List<(HttpRequestMessage, HttpResponseMessage)> _exchanges = new();

    internal IReadOnlyList<HttpRequestMessage> Requests => _requests;
    internal IReadOnlyList<(HttpRequestMessage Request, HttpResponseMessage Response)> Exchanges => _exchanges;
    internal IReadOnlyList<Expectation> Expectations => _expectations;

    internal Func<HttpRequestMessage, HttpResponseMessage> UnmatchedResponse { get; set; }
        = _ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("FakeHttpClient: no matching setup for this request.")
        };

    internal Expectation AddExpectation(Func<HttpRequestMessage, bool> predicate, string? description)
    {
        var expectation = new Expectation(predicate, description);
        _expectations.Add(expectation);
        return expectation;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _requests.Add(request);

        var matched = _expectations.FirstOrDefault(e => e.Predicate(request));
        var response = matched is not null
            ? matched.GetResponse(request)
            : UnmatchedResponse(request);

        _exchanges.Add((request, response));
        return Task.FromResult(response);
    }
}

internal class Expectation
{
    internal Func<HttpRequestMessage, bool> Predicate { get; }
    internal string? Description { get; }
    internal Func<HttpRequestMessage, HttpResponseMessage> ResponseFactory { get; set; }
    internal int MatchCount { get; private set; }

    internal Expectation(Func<HttpRequestMessage, bool> predicate, string? description)
    {
        Predicate = predicate;
        Description = description;
        ResponseFactory = _ => new HttpResponseMessage(HttpStatusCode.OK);
    }

    internal HttpResponseMessage GetResponse(HttpRequestMessage request)
    {
        MatchCount++;
        return ResponseFactory(request);
    }
}
