*TestBase* gives you a flying start with 
- fluent assertions that are easy to extend
- tools to help you test with dependencies on AspNetMvc, HttpClient, Ado.Net, Streams and Logging

Chainable fluent assertions get you to the point concisely:
```
- ShouldEqualByValue(), ShouldEqualByValueExceptFor() 
  work with all kinds of object and collections, and report what differed.
- string.ShouldMatch() ShouldNotBeNullOrEmptyOrWhiteSpace(), ShouldEqualIgnoringCase(), ShouldBeContainedIn(), ...
- numeric.ShouldBeBetween(), ShouldEqualWithTolerance(), GreaterThan, LessThan, GreaterOrEqualTo ...
- IEnumerable.ShouldAll(), ShouldContain(), ShouldNotContain(), ShouldBeEmpty(), ShouldNotBeEmpty() ...
- Stream.ShouldHaveSameStreamContentAs() , Stream.ShouldContain()
- ShouldBe(), ShouldNotBe(), ShouldBeOfType(), ...

UnitUnderTest.Action()
    .ShouldNotBeNull()
    .ShouldEqualByValueExceptFor(new {Id=1, Payload=expected}, ignoreList )
    .Payload
        .ShouldMatchIgnoringCase("I expected this")
		.Should(someOtherPredicate);
```

TestBase.HttpClient.Fake
------------------------

```
[Test]
public async Task Should_MatchTheRightExpectationAndReturnTheSetupResponse__GivenMultipleSetups()
{
    var httpClient = new FakeHttpClient()
        .Setup(x=>x.Method==HttpMethod.Put).Returns(new HttpResponseMessage(HttpStatusCode.Accepted))
        .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/this")).Returns(thisResponse)
        .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/that")).Returns(thatResponse)
        .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/forbidden")).Returns(new HttpResponseMessage(HttpStatusCode.Forbidden));

    (await httpClient.GetAsync("http://localhost/that")).ShouldEqualByValue(thatResponse);
    (await httpClient.GetAsync("http://localhost/forbidden")).StatusCode.ShouldBe(HttpStatusCode.Forbidden);


    httpClient.Verify(x=>x.Method==HttpMethod.Put);
    httClient.VerifyAll();     
}
```

TestBase.FakeDb
------------------
Works with Ado.Net 
```
- fakeDbConnection.SetupForQuery(IEnumerable<TFakeData>; )
- fakeDbConnection.SetupForQuery(IEnumerable<Tuple<TFakeDataForTable1,TFakeDataForTable2>> )
- fakeDbConnection.SetupForQuery(fakeData, new[] {"FieldName1", FieldName2"})
- fakeDbConnection.SetupForExecuteNonQuery(rowsAffected)
- fakeDbConnection.ShouldHaveUpdated("tableName", [Optional] fieldList, whereClauseField)
- fakeDbConnection.ShouldHaveSelected("tableName", [Optional] fieldList, whereClauseField)
- fakeDbConnection.ShouldHaveUpdated("tableName", [Optional] fieldList, whereClauseField)
- fakeDbConnection.ShouldHaveDeleted("tableName", whereClauseField)
- fakeDbConnection.ShouldHaveInvoked(cmd => predicate(cmd))
- fakeDbConnection.ShouldHaveXXX().ShouldHaveParameter("name", value)
- fakeDbConnection.Verify(x=>x.CommandText.Matches("Insert [case] .*") && x.Parameters["id"].Value==1)
```

TestBase.RecordingDb
--------------------
* `new RecordingDbConnection(IDbConnection)` helps you profile Ado.Net Db calls

TestBase.Mvc
------------
```
ControllerUnderTest.Action()
  .ShouldbeViewResult()
  .ShouldHaveModel<TModel>()
  .ShouldEqualByValue(expected)
ControllerUnderTest.Action()
  .ShouldBeRedirectToRouteResult()
  .ShouldHaveRouteValue("expectedKey", [Optional] "expectedValue");

ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.
```

TestBase.Mvc Version 4 for netstandard20 & AspNetCore Mvc
---------------------------------------------------------

- Test most controllers with zero setup using `controllerUnderTest.WithControllerContext(actionUnderTest)` :

```
[Test]
public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel()
{
    var controllerUnderTest = new AController().WithControllerContext("Action");
    
    var result= controllerUnderTest.ActionName().ShouldBeViewWithModel<AClass>("ViewName");
    
    result.ShouldBeOfType<AClass>().FooterLink.ShouldBe("/AController/ActionName");
}

```

- Test controllers with complex application dependencies using `HostedMvcTestFixtureBase` and specify your MVCApplications `Startup` class:

```
[TestFixture]
public class WhenTestingControllersUsingAspNetCoreTestTestServer : HostedMvcTestFixtureBase
{
    [TestCase("/dummy/action?id={id}")]
    public async Task Get_Should_ReturnActionResult(string url)
    {
        var httpClient=GivenClientForRunningServer<Startup>();
        GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
            
        var result= await httpClient.GetAsync(url.Formatz(new {Guid.NewGuid()}));

        result
            .ShouldBe_200Ok()
            .Content.ReadAsStringAsync().Result
            .ShouldBe("Content");
    }

    [TestCase("/dummy")]
    public async Task Put_Should_ReturnA(string url)
    {
        var something= new Fixture().Create<Something>();
        var jsonBody= new StringContent(something.ToJSon(), Encoding.UTF8, "application/json");
        var httpClient=GivenClientForRunningServer<Startup>();
        GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");

        var result = await httpClient.PutAsync(url, jsonBody);

        result.ShouldBe_202Accepted();
        DummyController.Putted.ShouldEqualByValue( something );
    }
}
```

TestBase.Mvc Version 3 for Net4
-------------------------------

Use the `Controller.WithHttpContextAndRoutes()` extension methods to fake the 
http request &amp; context. By injecting the RegisterRoutes method of your
MvcApplication, you can use and test Controller.Url with your application's configured routes.

```
ControllerUnderTest
  .WithHttpContextAndRoutes(
    [Optional] Action&lt;RouteCollection&gt; mvcApplicationRoutesRegistration, 
    [optional] string requestUrl,
    [Optional] string query = "",
    [Optional] string appVirtualPath = "/",
    [Optional] HttpApplication applicationInstance)

ApiControllerUnderTest.WithWebApiHttpContext<T>(
    HttpMethod httpMethod, 
    [Optional] string requestUri,
    [Optional] string routeTemplate)
```

Testable Logging with StringListLogger
--------------------------------------
`Extensions.Logging.ListOfString` for Microsoft.Extensions.Logging.Abstractions:
```
var logger= new LoggerFactory.AddProvider(new StringListLoggerProvider()).CreateLogger("Test1");
// or
var loggedLines = new List<string>();
var logger= new LoggerFactory().AddStringListLogger(loggedLines).CreateLogger("Test2");

 ... ;
StringListLogger.Instance
	.LoggedLines
	.ShouldContain(x=>x.Matches("kilroy was here"));
```
`Serilog.Sinks.ListOfString` for Serilog:
```
var loglines= new List<String>();
var logger=new LoggerConfiguration().WriteTo.StringList(loglines).CreateLogger();
... ;
logLines.ShouldContain(x=>x.Matches("kilroy was here"));
```

- Mix and match with your favourite test runners and assertions
- Building on Mono : define compile symbol NoMSTest to remove dependency on Microsoft.VisualStudio.QualityTools.UnitTestFramework
