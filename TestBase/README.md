*TestBase* gives you a flying start with
- fluent assertions that are very easy to extend
- Object and Collection comparers
- Sharper error messages showing detailed Actual and Asserted
- tools to help you test with “heavyweight” dependencies on
    - AspNetCore.Mvc, AspNet.Mvc or WebApi Contexts
    - HttpClient
    - Ado.Net
    - Streams & Logging
- Mix & match with your favourite test runners & assertions.

```
UnitUnderTest.Action()
  .ShouldNotBeNull()
  .ShouldEqualByValueExceptFor(new {Id=1, Descr=expected}, ignoreList )
  .Payload
    .ShouldMatchIgnoringCase("I expected this")
    .Should(someOtherPredicate);
  .Items
    .ShouldAll(predicate)
    .ShouldContain(item)
    .ShouldNotContain(predicate)
    .Where(predicate)
    .SingleOrAssertFail()

# works with all kinds of object and collections, and report what differed.
objectOrCollection
 .ShouldEqualByValue().ShouldEqualByValueExceptFor(...).ShouldEqualByValueOnMembers() 
string
 .ShouldMatch(pattern).ShouldNotMatch().ShouldBeEmpty().ShouldNotBeEmpty()
 .ShouldNotBeNullOrEmptyOrWhiteSpace().ShouldEqualIgnoringCase()
 .ShouldContain().ShouldStartWith().ShouldEndWith().ShouldBeContainedIn(), ...
numeric
 .ShouldBeBetween().ShouldEqualWithTolerance()....GreaterThan....LessThan...GreaterOrEqualTo ...
ienumerable
 .ShouldAll().ShouldContain().ShouldNotContain().ShouldBeEmpty().ShouldNotBeEmpty() ...
stream
 .ShouldHaveSameStreamContentAs().ShouldContain()
value
 .ShouldBe().ShouldNotBe().ShouldBeOfType().ShouldBeAssignableTo()...
```

TestBase.HttpClient.Fake

```
new FakeHttpClient()
      .Setup(x=>x.RequestUri.PathAndQuery.StartsWith("/this"))
        .Returns(response)
      .Setup(x=>x.Method==HttpMethod.Put)
        .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
```

TestBase.AdoNet
------------------
`FakeDbConnection`

```
- db.SetupForQuery(…)
- db.SetupForExecuteNonQuery(…)
- db.ShouldHaveUpdated("tableName", …)
- db.ShouldHaveSelected("tableName", …)
- db.ShouldHaveDeleted("tableName", …)
- db.Verify( x=>x.CommandText.Matches("Insert [case] .*") 
             && x.Parameters["id"].Value==1 )
- db
    .ShouldHaveInvoked(cmd => predicate(cmd))
    .ShouldHaveParameter("name", value)
```

`RecordingDbConnection`

TestBase.Mvc.AspNetCore & TestBase.Mvc for Mvc 4 & Mvc 5
--------------------------------------------------------

```
ControllerUnderTest.WithControllerContext()
  .Action()
  .ShouldbeViewResult()
  .ShouldHaveModel<TModel>()
  .ShouldEqualByValue(expected)
ControllerUnderTest.Action()
  .ShouldBeRedirectToRouteResult()
  .ShouldHaveRouteValue("expectedKey", [Optional] "expectedValue");

ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.
```

- Test AspNetCore controllers with zero setup using
  `controllerUnderTest.WithControllerContext(actionUnderTest)`
- Test more complex AspNetCore controller/application dependencies using
  `HostedMvcTestFixtureBase` and specify your MVCApplications `Startup` class.

```
[TestCase("/dummy")]
public async Task Put_Should_ReturnA(string url)
{
    var httpClient=GivenClientForRunningServer<Startup>();
    GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");

    var result = await httpClient.PutAsync(url, json);

    result.ShouldBe_202Accepted();
}
```

For Mvc4 and Mvc 5, fake your http request &amp; context, and use the `RegisterRoutes` method
of your actual application to set up `Controller.Url`

```
ControllerUnderTest
  .WithHttpContextAndRoutes(
    RouteConfig.RegisterRoutes, 
    "/incomingurl"
  );

ApiControllerUnderTest.WithWebApiHttpContext<T>(
    httpMethod, 
    requestUri,
    routeTemplate)
```

Testable Logging

```
// Extensions.Logging.ListOfString
var log = new List<String>();
ILogger mslogger= new LoggerFactory().AddStringListLogger(log).CreateLogger("Test2");

// Serilog.Sinks.ListOfString
Serilog.Logger slogger= new LoggerConfiguration().WriteTo.StringList(log).CreateLogger();

### Release Notes

6.0.0   Better Assertion output for core asserts includes ActualValue,ActualExpression,Asserted,AssertedDetail