using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TestBase;

/// <summary>
/// A fake HttpClient for testing. Set up expected requests with fluent API,
/// make HTTP calls, then verify what was called.
/// </summary>
public class FakeHttpClient : HttpClient
{
    readonly FakeHttpMessageHandler _handler;

    public FakeHttpClient() : this(new FakeHttpMessageHandler()) { }
    FakeHttpClient(FakeHttpMessageHandler handler) : base(handler) => _handler = handler;

    /// <summary>All requests that were sent through this client.</summary>
    public IReadOnlyList<HttpRequestMessage> Requests => _handler.Requests;

    /// <summary>All request/response pairs.</summary>
    public IReadOnlyList<(HttpRequestMessage Request, HttpResponseMessage Response)> Exchanges
        => _handler.Exchanges;

    // ── Setup ──────────────────────────────────────────────────────

    /// <summary>Match any request satisfying the predicate.</summary>
    public RequestSetup Setup(Func<HttpRequestMessage, bool> predicate, string? description = null)
        => new(this, _handler.AddExpectation(predicate, description));

    /// <summary>Match GET requests whose URI matches the regex pattern.</summary>
    public RequestSetup SetupGet(string urlPattern)
        => Setup(
            r => r.Method == HttpMethod.Get && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            $"GET {urlPattern}");

    /// <summary>Match POST requests whose URI matches the regex pattern.</summary>
    public RequestSetup SetupPost(string urlPattern)
        => Setup(
            r => r.Method == HttpMethod.Post && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            $"POST {urlPattern}");

    /// <summary>Match PUT requests whose URI matches the regex pattern.</summary>
    public RequestSetup SetupPut(string urlPattern)
        => Setup(
            r => r.Method == HttpMethod.Put && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            $"PUT {urlPattern}");

    /// <summary>Match DELETE requests whose URI matches the regex pattern.</summary>
    public RequestSetup SetupDelete(string urlPattern)
        => Setup(
            r => r.Method == HttpMethod.Delete && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            $"DELETE {urlPattern}");

    /// <summary>Match PATCH requests whose URI matches the regex pattern.</summary>
    public RequestSetup SetupPatch(string urlPattern)
        => Setup(
            r => r.Method.Method == "PATCH" && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            $"PATCH {urlPattern}");

    /// <summary>Match any request whose URI matches the regex pattern, regardless of method.</summary>
    public RequestSetup SetupAny(string urlPattern)
        => Setup(
            r => Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            $"ANY {urlPattern}");

    // ── Fallback ───────────────────────────────────────────────────

    /// <summary>Set the response returned when no setup matches a request. Default is 500.</summary>
    public FakeHttpClient WhenUnmatched(HttpStatusCode statusCode)
    {
        _handler.UnmatchedResponse = _ => new HttpResponseMessage(statusCode);
        return this;
    }

    /// <summary>Set a factory for the response when no setup matches.</summary>
    public FakeHttpClient WhenUnmatched(Func<HttpRequestMessage, HttpResponseMessage> factory)
    {
        _handler.UnmatchedResponse = factory;
        return this;
    }

    // ── Verification ───────────────────────────────────────────────

    /// <summary>Assert that a request matching the predicate was made.</summary>
    public HttpRequestMessage ShouldHaveReceived(
        Func<HttpRequestMessage, bool> predicate, string? message = null)
    {
        var match = _handler.Requests.FirstOrDefault(predicate);
        if (match is null)
            throw new AssertionException(
                message ?? "Expected a matching request, but none was found."
                + FormatInvocations());
        return match;
    }

    /// <summary>Assert that no request matching the predicate was made.</summary>
    public FakeHttpClient ShouldNotHaveReceived(
        Func<HttpRequestMessage, bool> predicate, string? message = null)
    {
        var match = _handler.Requests.FirstOrDefault(predicate);
        if (match is not null)
            throw new AssertionException(
                message ?? $"Expected no matching request, but found: {match.Method} {match.RequestUri}"
                + FormatInvocations());
        return this;
    }

    /// <summary>Assert that a GET to a URL matching the pattern was made.</summary>
    public HttpRequestMessage ShouldHaveReceivedGet(string urlPattern, string? message = null)
        => ShouldHaveReceived(
            r => r.Method == HttpMethod.Get && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            message ?? $"Expected GET matching '{urlPattern}'." + FormatInvocations());

    /// <summary>Assert that a POST to a URL matching the pattern was made.</summary>
    public HttpRequestMessage ShouldHaveReceivedPost(string urlPattern, string? message = null)
        => ShouldHaveReceived(
            r => r.Method == HttpMethod.Post && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            message ?? $"Expected POST matching '{urlPattern}'." + FormatInvocations());

    /// <summary>Assert that a PUT to a URL matching the pattern was made.</summary>
    public HttpRequestMessage ShouldHaveReceivedPut(string urlPattern, string? message = null)
        => ShouldHaveReceived(
            r => r.Method == HttpMethod.Put && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            message ?? $"Expected PUT matching '{urlPattern}'." + FormatInvocations());

    /// <summary>Assert that a DELETE to a URL matching the pattern was made.</summary>
    public HttpRequestMessage ShouldHaveReceivedDelete(string urlPattern, string? message = null)
        => ShouldHaveReceived(
            r => r.Method == HttpMethod.Delete && Regex.IsMatch(r.RequestUri!.ToString(), urlPattern),
            message ?? $"Expected DELETE matching '{urlPattern}'." + FormatInvocations());

    /// <summary>Assert that every setup was matched at least once.</summary>
    public FakeHttpClient ShouldHaveReceivedAll()
    {
        var unmatched = _handler.Expectations
            .Where(e => e.MatchCount == 0)
            .Select(e => e.Description ?? "(no description)")
            .ToList();
        if (unmatched.Any())
            throw new AssertionException(
                $"{unmatched.Count} expected request(s) were never received:\n"
                + string.Join("\n", unmatched.Select(u => $"  - {u}"))
                + FormatInvocations());
        return this;
    }

    /// <summary>Assert that exactly N requests were received in total.</summary>
    public FakeHttpClient ShouldHaveReceivedCount(int expected, string? message = null)
    {
        if (_handler.Requests.Count != expected)
            throw new AssertionException(
                message ?? $"Expected {expected} request(s) but received {_handler.Requests.Count}."
                + FormatInvocations());
        return this;
    }

    /// <summary>Assert no requests were made.</summary>
    public FakeHttpClient ShouldHaveReceivedNothing(string? message = null)
        => ShouldHaveReceivedCount(0, message);

    string FormatInvocations()
    {
        if (!_handler.Requests.Any()) return "\nNo requests were made.";
        var sb = new StringBuilder("\nRequests received:");
        foreach (var r in _handler.Requests)
            sb.Append($"\n  {r.Method} {r.RequestUri}");
        return sb.ToString();
    }
}

/// <summary>Fluent builder for setting up the response to a matched request.</summary>
public class RequestSetup
{
    readonly FakeHttpClient _client;
    readonly Expectation _expectation;

    internal RequestSetup(FakeHttpClient client, Expectation expectation)
    {
        _client = client;
        _expectation = expectation;
    }

    /// <summary>Return a static response message.</summary>
    public FakeHttpClient Returns(HttpResponseMessage response)
    {
        _expectation.ResponseFactory = _ => response;
        return _client;
    }

    /// <summary>Return a response built from the request.</summary>
    public FakeHttpClient Returns(Func<HttpRequestMessage, HttpResponseMessage> factory)
    {
        _expectation.ResponseFactory = factory;
        return _client;
    }

    /// <summary>Return a string content response with the given status code.</summary>
    public FakeHttpClient ReturnsString(
        string content,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string mediaType = "text/plain")
    {
        _expectation.ResponseFactory = _ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, mediaType)
        };
        return _client;
    }

    /// <summary>Return a JSON-serialized response with the given status code.</summary>
    public FakeHttpClient ReturnsJson(
        object value,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        JsonSerializerOptions? options = null)
    {
        var json = JsonSerializer.Serialize(value, options ?? JsonOptions.Default);
        _expectation.ResponseFactory = _ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return _client;
    }

    /// <summary>Return a JSON string response with the given status code.</summary>
    public FakeHttpClient ReturnsJsonString(
        string json,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _expectation.ResponseFactory = _ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return _client;
    }

    /// <summary>Return an empty response with the given status code.</summary>
    public FakeHttpClient ReturnsStatus(HttpStatusCode statusCode)
    {
        _expectation.ResponseFactory = _ => new HttpResponseMessage(statusCode);
        return _client;
    }

    /// <summary>Return byte array content.</summary>
    public FakeHttpClient ReturnsBytes(
        byte[] content,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string mediaType = "application/octet-stream")
    {
        _expectation.ResponseFactory = _ =>
        {
            var response = new HttpResponseMessage(statusCode);
            response.Content = new ByteArrayContent(content);
            response.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
            return response;
        };
        return _client;
    }

    /// <summary>Build a dynamic response from the request, returning string content.</summary>
    public FakeHttpClient Returns(
        Func<HttpRequestMessage, string> contentFactory,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string mediaType = "text/plain")
    {
        _expectation.ResponseFactory = request => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(contentFactory(request), Encoding.UTF8, mediaType)
        };
        return _client;
    }
}

internal static class JsonOptions
{
    internal static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}
