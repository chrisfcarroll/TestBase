*TestBase* gives you a flying start with 
- fluent assertions that are easy to extend
- infrastructure for testing with dependencies on AspNetMvc, HttpClient, Ado.Net, Streams

Chainable fluent assertions get you to the point concisely
------------------
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
    .ShouldEqualByValue(new {Id=1, Payload=expected, Additional=new[]{ expected1, expected2 }} )
    .Payload
        .ShouldMatchIgnoringCase("I expected this");
```

TestBase.FakeDb
------------------
Works with Ado.Net and technologies on top of it, including Dapper.
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

ApiControllerUnderTest.WithWebApiHttpContext&lt;T&gt;(
    HttpMethod httpMethod, 
    [Optional] string requestUri,
    [Optional] string routeTemplate)
```

Can be used in both NUnit & MS UnitTestFramework test projects.


Testable Logging with `StringListLogger`:
```
MS Logging: ILoggerFactory factory=new LoggerFactory.AddProvider(new StringListLoggerProvider())
Serilogging: new LoggerConfiguration().WriteTo.StringList(stringList).CreateLogger()
//
var logger= factory.CreateLogger("Test1") ; ... ; StringListLogger.Instance.LoggedLines.ShouldContain(x=>x.Matches("kilroy was here")
```


- Building on Mono : define compile symbol NoMSTest to remove dependency on Microsoft.VisualStudio.QualityTools.UnitTestFramework

ChangeLog
---------
4.0.6.2 TestBase.Mvc can run controller actions on aspnetcore using controller.WithControllerContext()
4.0.5.2 TestBase.Mvc partially ported to AspNetcore
4.0.4.0 StreamShoulds
4.0.3.0 StringListLogger as MS Logger and as Serilogger
4.0.1.0 Port to NetCore
3.0.3.0 Improves FakeDb setup
3.0.x.0 adds and/or corrects missing Shoulds()
2.0.5.0 adds some intellisense and FakeDbConnection.Verify(..., message,args) overload
