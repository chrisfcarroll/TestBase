using System.Net;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using TestBase;

namespace TestBase.NetCore.FakeHttpClientTests;

[TestFixture]
public class SetupAndResponseTests
{
    [Test]
    public async Task SetupGet_ReturnsStringContent()
    {
        var http = new FakeHttpClient()
            .SetupGet("https://api\\.example\\.com/hello")
            .ReturnsString("world");

        var result = await http.GetStringAsync("https://api.example.com/hello");

        Assert.That(result, Is.EqualTo("world"));
    }

    [Test]
    public async Task SetupPost_ReturnsJsonObject()
    {
        var http = new FakeHttpClient()
            .SetupPost(".*")
            .ReturnsJson(new { id = 42, name = "created" });

        var response = await http.PostAsync(
            "https://api.example.com/items",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        var json = await response.Content.ReadAsStringAsync();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(json, Does.Contain("\"id\":42"));
        Assert.That(json, Does.Contain("\"name\":\"created\""));
    }

    [Test]
    public async Task SetupPut_ReturnsStatus()
    {
        var http = new FakeHttpClient()
            .SetupPut("/items/\\d+")
            .ReturnsStatus(HttpStatusCode.NoContent);

        var response = await http.PutAsync(
            "https://api.example.com/items/99",
            new StringContent("{}"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task SetupDelete_ReturnsStatus()
    {
        var http = new FakeHttpClient()
            .SetupDelete("/items/\\d+")
            .ReturnsStatus(HttpStatusCode.NoContent);

        var response = await http.DeleteAsync("https://api.example.com/items/5");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task SetupPatch_Works()
    {
        var http = new FakeHttpClient()
            .SetupPatch(".*")
            .ReturnsJson(new { patched = true });

        var response = await http.PatchAsync(
            "https://api.example.com/items/1",
            new StringContent("{\"name\":\"updated\"}", Encoding.UTF8, "application/json"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task SetupAny_MatchesAnyMethod()
    {
        var http = new FakeHttpClient()
            .SetupAny("/health")
            .ReturnsString("ok");

        var getResult = await http.GetStringAsync("https://host/health");
        var postResponse = await http.PostAsync("https://host/health", null);
        var postResult = await postResponse.Content.ReadAsStringAsync();

        Assert.That(getResult, Is.EqualTo("ok"));
        Assert.That(postResult, Is.EqualTo("ok"));
    }

    [Test]
    public async Task Returns_DynamicFromRequest()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*")
            .Returns(req => $"You asked for: {req.RequestUri!.PathAndQuery}");

        var result = await http.GetStringAsync("https://host/foo/bar");

        Assert.That(result, Is.EqualTo("You asked for: /foo/bar"));
    }

    [Test]
    public async Task Returns_HttpResponseMessage()
    {
        var customResponse = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent("accepted"),
            ReasonPhrase = "Custom Reason"
        };

        var http = new FakeHttpClient()
            .SetupPost(".*")
            .Returns(customResponse);

        var response = await http.PostAsync("https://host/", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
        Assert.That(response.ReasonPhrase, Is.EqualTo("Custom Reason"));
    }

    [Test]
    public async Task ReturnsJsonString_Works()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*")
            .ReturnsJsonString("{\"raw\":true}");

        var result = await http.GetStringAsync("https://host/");

        Assert.That(result, Does.Contain("\"raw\":true"));
    }

    [Test]
    public async Task ReturnsBytes_Works()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var http = new FakeHttpClient()
            .SetupGet(".*")
            .ReturnsBytes(data);

        var response = await http.GetAsync("https://host/file");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        Assert.That(bytes, Is.EqualTo(data));
    }
}

[TestFixture]
public class MatchingOrderTests
{
    [Test]
    public async Task FirstMatchingSetup_Wins()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("catch-all")
            .SetupGet("/specific").ReturnsString("specific");

        var result = await http.GetStringAsync("https://host/specific");

        Assert.That(result, Is.EqualTo("catch-all"), "First setup should win");
    }

    [Test]
    public async Task SpecificSetupFirst_MatchesCorrectly()
    {
        var http = new FakeHttpClient()
            .SetupGet("/specific").ReturnsString("specific")
            .SetupGet(".*").ReturnsString("catch-all");

        var specific = await http.GetStringAsync("https://host/specific");
        var other = await http.GetStringAsync("https://host/other");

        Assert.That(specific, Is.EqualTo("specific"));
        Assert.That(other, Is.EqualTo("catch-all"));
    }

    [Test]
    public async Task MultipleSetups_DifferentMethods()
    {
        var http = new FakeHttpClient()
            .SetupGet("/api").ReturnsString("got")
            .SetupPost("/api").ReturnsString("posted")
            .SetupPut("/api").ReturnsStatus(HttpStatusCode.NoContent);

        var getResult = await http.GetStringAsync("https://host/api");
        var postResponse = await http.PostAsync("https://host/api", null);
        var postResult = await postResponse.Content.ReadAsStringAsync();
        var putResponse = await http.PutAsync("https://host/api", null);

        Assert.That(getResult, Is.EqualTo("got"));
        Assert.That(postResult, Is.EqualTo("posted"));
        Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}

[TestFixture]
public class UnmatchedRequestTests
{
    [Test]
    public async Task Unmatched_Returns500_ByDefault()
    {
        var http = new FakeHttpClient();

        var response = await http.GetAsync("https://host/nothing");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task WhenUnmatched_CustomStatusCode()
    {
        var http = new FakeHttpClient()
            .WhenUnmatched(HttpStatusCode.NotFound);

        var response = await http.GetAsync("https://host/nothing");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task WhenUnmatched_CustomFactory()
    {
        var http = new FakeHttpClient()
            .WhenUnmatched(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent($"Unknown: {req.RequestUri}")
            });

        var response = await http.GetAsync("https://host/xyz");
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(body, Does.Contain("https://host/xyz"));
    }
}

[TestFixture]
public class VerificationTests
{
    [Test]
    public async Task ShouldHaveReceived_Passes_WhenMatched()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok");

        await http.GetStringAsync("https://host/hello");

        var request = http.ShouldHaveReceived(r => r.Method == HttpMethod.Get);
        Assert.That(request.RequestUri!.ToString(), Does.Contain("hello"));
    }

    [Test]
    public void ShouldHaveReceived_Throws_WhenNotMatched()
    {
        var http = new FakeHttpClient();

        Assert.Throws<AssertionException>(
            () => http.ShouldHaveReceived(r => r.Method == HttpMethod.Get));
    }

    [Test]
    public async Task ShouldNotHaveReceived_Passes_WhenNoMatch()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok");

        await http.GetStringAsync("https://host/");

        http.ShouldNotHaveReceived(r => r.Method == HttpMethod.Post);
    }

    [Test]
    public async Task ShouldNotHaveReceived_Throws_WhenMatched()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok");

        await http.GetStringAsync("https://host/");

        Assert.Throws<AssertionException>(
            () => http.ShouldNotHaveReceived(r => r.Method == HttpMethod.Get));
    }

    [Test]
    public async Task ShouldHaveReceivedGet_Passes()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok");

        await http.GetStringAsync("https://host/api/data");

        http.ShouldHaveReceivedGet("/api/data");
    }

    [Test]
    public void ShouldHaveReceivedGet_Throws_WhenNoGet()
    {
        var http = new FakeHttpClient();

        Assert.Throws<AssertionException>(() => http.ShouldHaveReceivedGet("/anything"));
    }

    [Test]
    public async Task ShouldHaveReceivedPost_Passes()
    {
        var http = new FakeHttpClient()
            .SetupPost(".*").ReturnsStatus(HttpStatusCode.Created);

        await http.PostAsync("https://host/items", new StringContent("{}"));

        http.ShouldHaveReceivedPost("/items");
    }

    [Test]
    public async Task ShouldHaveReceivedPut_Passes()
    {
        var http = new FakeHttpClient()
            .SetupPut(".*").ReturnsStatus(HttpStatusCode.OK);

        await http.PutAsync("https://host/items/1", new StringContent("{}"));

        http.ShouldHaveReceivedPut("/items/1");
    }

    [Test]
    public async Task ShouldHaveReceivedDelete_Passes()
    {
        var http = new FakeHttpClient()
            .SetupDelete(".*").ReturnsStatus(HttpStatusCode.OK);

        await http.DeleteAsync("https://host/items/1");

        http.ShouldHaveReceivedDelete("/items/1");
    }

    [Test]
    public async Task ShouldHaveReceivedAll_Passes_WhenAllMatched()
    {
        var http = new FakeHttpClient()
            .SetupGet("/a").ReturnsString("a")
            .SetupPost("/b").ReturnsString("b");

        await http.GetStringAsync("https://host/a");
        await http.PostAsync("https://host/b", null);

        http.ShouldHaveReceivedAll();
    }

    [Test]
    public async Task ShouldHaveReceivedAll_Throws_WhenSomeUnmatched()
    {
        var http = new FakeHttpClient()
            .SetupGet("/a").ReturnsString("a")
            .SetupPost("/b").ReturnsString("b");

        await http.GetStringAsync("https://host/a");
        // never POST /b

        var ex = Assert.Throws<AssertionException>(() => http.ShouldHaveReceivedAll());
        Assert.That(ex!.Message, Does.Contain("POST /b"));
    }

    [Test]
    public async Task ShouldHaveReceivedCount_Passes()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok");

        await http.GetStringAsync("https://host/1");
        await http.GetStringAsync("https://host/2");
        await http.GetStringAsync("https://host/3");

        http.ShouldHaveReceivedCount(3);
    }

    [Test]
    public void ShouldHaveReceivedCount_Throws_WhenWrong()
    {
        var http = new FakeHttpClient();

        Assert.Throws<AssertionException>(() => http.ShouldHaveReceivedCount(1));
    }

    [Test]
    public void ShouldHaveReceivedNothing_Passes_WhenEmpty()
    {
        var http = new FakeHttpClient();
        http.ShouldHaveReceivedNothing();
    }

    [Test]
    public async Task ShouldHaveReceivedNothing_Throws_WhenNotEmpty()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok");

        await http.GetStringAsync("https://host/");

        Assert.Throws<AssertionException>(() => http.ShouldHaveReceivedNothing());
    }
}

[TestFixture]
public class RequestInspectionTests
{
    [Test]
    public async Task Requests_RecordsAllRequests()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok")
            .SetupPost(".*").ReturnsStatus(HttpStatusCode.OK);

        await http.GetStringAsync("https://host/a");
        await http.PostAsync("https://host/b", new StringContent("body"));
        await http.GetStringAsync("https://host/c");

        Assert.That(http.Requests, Has.Count.EqualTo(3));
        Assert.That(http.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
        Assert.That(http.Requests[1].Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(http.Requests[2].RequestUri!.PathAndQuery, Is.EqualTo("/c"));
    }

    [Test]
    public async Task Exchanges_PairsRequestsWithResponses()
    {
        var http = new FakeHttpClient()
            .SetupGet("/ok").ReturnsStatus(HttpStatusCode.OK)
            .SetupGet("/not").ReturnsStatus(HttpStatusCode.NotFound);

        await http.GetAsync("https://host/ok");
        await http.GetAsync("https://host/not");

        Assert.That(http.Exchanges, Has.Count.EqualTo(2));
        Assert.That(http.Exchanges[0].Response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(http.Exchanges[1].Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task ReadContentAsString_Works()
    {
        var http = new FakeHttpClient()
            .SetupPost(".*").ReturnsStatus(HttpStatusCode.OK);

        await http.PostAsync("https://host/", new StringContent("hello world"));

        var body = http.Requests[0].ReadContentAsString();
        Assert.That(body, Is.EqualTo("hello world"));
    }

    [Test]
    public async Task ReadContentAsJson_Deserializes()
    {
        var http = new FakeHttpClient()
            .SetupPost(".*").ReturnsStatus(HttpStatusCode.OK);

        await http.PostAsync("https://host/",
            new StringContent("{\"Name\":\"test\",\"Value\":42}", Encoding.UTF8, "application/json"));

        var obj = http.Requests[0].ReadContentAsJson<TestPayload>();
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.Name, Is.EqualTo("test"));
        Assert.That(obj.Value, Is.EqualTo(42));
    }

    [Test]
    public async Task HasHeader_ChecksRequestHeaders()
    {
        var http = new FakeHttpClient()
            .SetupGet(".*").ReturnsString("ok");

        var request = new HttpRequestMessage(HttpMethod.Get, "https://host/");
        request.Headers.Add("X-Custom", "myvalue");
        await http.SendAsync(request);

        Assert.That(http.Requests[0].HasHeader("X-Custom", "myvalue"), Is.True);
        Assert.That(http.Requests[0].HasHeader("X-Custom"), Is.True);
        Assert.That(http.Requests[0].HasHeader("X-Missing"), Is.False);
    }

    record TestPayload(string Name, int Value);
}

[TestFixture]
public class CustomPredicateSetupTests
{
    [Test]
    public async Task Setup_WithCustomPredicate_MatchesHeaders()
    {
        var http = new FakeHttpClient()
            .Setup(
                r => r.HasHeader("Authorization", "Bearer token123"),
                "Authorized request")
            .ReturnsJson(new { data = "secret" })
            .SetupGet(".*").ReturnsStatus(HttpStatusCode.Unauthorized);

        // Authorized request
        var authReq = new HttpRequestMessage(HttpMethod.Get, "https://host/secret");
        authReq.Headers.Add("Authorization", "Bearer token123");
        var authResponse = await http.SendAsync(authReq);

        // Unauthorized request
        var noAuthResponse = await http.GetAsync("https://host/secret");

        Assert.That(authResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(noAuthResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Setup_WithCustomPredicate_MatchesContent()
    {
        var http = new FakeHttpClient()
            .Setup(
                r => r.Method == HttpMethod.Post
                     && (r.ReadContentAsString() ?? "").Contains("important"),
                "POST with 'important' in body")
            .ReturnsString("matched important")
            .SetupPost(".*").ReturnsString("generic post");

        var importantResponse = await http.PostAsync("https://host/",
            new StringContent("this is important data"));
        var genericResponse = await http.PostAsync("https://host/",
            new StringContent("boring data"));

        Assert.That(await importantResponse.Content.ReadAsStringAsync(), Is.EqualTo("matched important"));
        Assert.That(await genericResponse.Content.ReadAsStringAsync(), Is.EqualTo("generic post"));
    }
}

[TestFixture]
public class RealWorldScenarioTests
{
    [Test]
    public async Task TypicalRestApi_CrudOperations()
    {
        // Arrange - set up a typical REST API
        var http = new FakeHttpClient()
            .SetupGet("/api/items$")
            .ReturnsJson(new[] { new { id = 1, name = "Item 1" }, new { id = 2, name = "Item 2" } })

            .SetupGet("/api/items/1$")
            .ReturnsJson(new { id = 1, name = "Item 1" })

            .SetupPost("/api/items$")
            .ReturnsJson(new { id = 3, name = "New Item" }, HttpStatusCode.Created)

            .SetupPut("/api/items/1$")
            .ReturnsStatus(HttpStatusCode.NoContent)

            .SetupDelete("/api/items/1$")
            .ReturnsStatus(HttpStatusCode.NoContent);

        // Act - simulate a client using the API
        var listResponse = await http.GetAsync("https://api.example.com/api/items");
        var getResponse = await http.GetAsync("https://api.example.com/api/items/1");
        var createResponse = await http.PostAsync("https://api.example.com/api/items",
            new StringContent("{\"name\":\"New Item\"}", Encoding.UTF8, "application/json"));
        var updateResponse = await http.PutAsync("https://api.example.com/api/items/1",
            new StringContent("{\"name\":\"Updated\"}", Encoding.UTF8, "application/json"));
        var deleteResponse = await http.DeleteAsync("https://api.example.com/api/items/1");

        // Assert
        Assert.That(listResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        http.ShouldHaveReceivedCount(5);
        http.ShouldHaveReceivedAll();
    }

    [Test]
    public async Task ServiceWithRetries_TracksAllAttempts()
    {
        var callCount = 0;
        var http = new FakeHttpClient()
            .SetupGet("/flaky")
            .Returns(_ =>
            {
                callCount++;
                return callCount < 3
                    ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    : new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("finally!")
                    };
            });

        // Simulate 3 retries
        HttpResponseMessage response;
        do
        {
            response = await http.GetAsync("https://host/flaky");
        } while (response.StatusCode == HttpStatusCode.ServiceUnavailable);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("finally!"));
        http.ShouldHaveReceivedCount(3);
    }

    [Test]
    public async Task VerifyRequestBodies_JsonPayloads()
    {
        var http = new FakeHttpClient()
            .SetupPost("/api/orders")
            .ReturnsJson(new { orderId = "ORD-001" }, HttpStatusCode.Created);

        await http.PostAsync("https://host/api/orders",
            new StringContent(
                JsonSerializer.Serialize(new { product = "Widget", quantity = 5 }),
                Encoding.UTF8, "application/json"));

        // Verify the request was made and inspect the body
        var request = http.ShouldHaveReceivedPost("/api/orders");
        var body = request.ReadContentAsJson<OrderRequest>();
        Assert.That(body, Is.Not.Null);
        Assert.That(body!.Product, Is.EqualTo("Widget"));
        Assert.That(body.Quantity, Is.EqualTo(5));
    }

    record OrderRequest(string Product, int Quantity);
}
