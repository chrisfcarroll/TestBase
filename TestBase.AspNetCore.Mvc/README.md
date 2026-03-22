*TestBase* gives you a flying start with
- fluent assertions that are easy to extend
- sharp error messages
- tools to help you test with “heavyweight” dependencies on
    -  AspNetCore.Mvc, AspNet.Mvc or WebApi Contexts
    -   HttpClient
    -   Ado.Net
    -   Streams & Logging
 
TestBase.AspNetCore.Mvc
-----------------------

Use the `Controller.WithControllerContext()` extension methods to stub the
http request &amp; context, enabling you to unit test IActionResults with
dependencies on `Controller.Url()`, `Request.Cookies`, `Response.Cookies`
and more.
 
Chainable fluent assertions get you to the point concisely:
 
```
var controllerUnderTest = new AController().WithControllerContext();
 
controllerUnderTest
.AView("parameter", "Thing")
.ShouldBeViewWithModel<MyViewModel>("ViewName")
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
.AProperty.ShouldBe("Thing");
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 
controllerUnderTest.Url.Action("AView", "ATest", new
{id=99}).ShouldEqual("/ATest/AView?id=99");
 
var fileResult=controllerUnderTest.AFileResult().ShouldBeFileContentResult();
fileResult.FileContents.ShouldEqualByValue( Encoding.UTF8.GetBytes("my words")
);
fileResult.ContentType.ShouldBe("text/plain");
fileResult.FileDownloadName.ShouldBe("filename.txt");
 
ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.
```
 
Or test against complex application dependencies using
`HostedMvcTestFixtureBase` and specify your `Startup` class:
 
```
[TestFixture]
public class WhenTestingControllersUsingAspNetCoreTestTestServer :
HostedMvcTestFixtureBase
{
 
[TestCase("/dummy/action?id={id}")]
public async Task Get_Should_ReturnActionResult(string url)
{
var id=Guid.NewGuid();
var httpClient=GivenClientForRunningServer<Startup>();
GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
var result= await httpClient.GetAsync(url.Formatz(new {id}));
 
result
.ShouldBe_200Ok()
.Content.ReadAsStringAsync().Result
.ShouldBe(""Content"");
}
 
[TestCase("/dummy")]
public async Task Put_Should_ReturnA(string url)
{
var something= new Fixture().Create<Something>();
var jsonBody= new StringContent(something.ToJSon(), Encoding.UTF8,
"application/json");
var httpClient=GivenClientForRunningServer<Startup>();
GivenRequestHeaders(httpClient, "CustomHeader", "HeaderValue1");
 
var result = await httpClient.PutAsync(url, jsonBody);
 
result.ShouldBe_202Accepted();
DummyController.Putted.ShouldEqualByValue( something );
}
}
```

Simple, easy-to-extend fluent assertions:
```
.ShouldEqualByValue().ShouldEqualByValueExceptFor(...).ShouldEqualByValueOnMembers()
  work with all kinds of object and collections, and report what differed.
string.ShouldMatch(pattern).ShouldNotMatch().ShouldBeEmpty().ShouldNotBeEmpty()
.ShouldNotBeNullOrEmptyOrWhiteSpace().ShouldEqualIgnoringCase()
.ShouldContain().ShouldStartWith().ShouldEndWith().ShouldBeContainedIn().ShouldBeOneOf().ShouldNotBeOneOf()
numeric.ShouldBeBetween().ShouldEqualWithTolerance()....GreaterThan....LessThan...GreaterOrEqualTo ...
ienumerable.ShouldAll().ShouldContain().ShouldNotContain().ShouldBeEmpty().ShouldNotBeEmpty() ...
stream.ShouldHaveSameStreamContentAs().ShouldContain()
value.ShouldBe().ShouldNotBe().ShouldBeOfType().ShouldBeAssignableTo()...
```

See also
 - [TestBase](https://www.nuget.org/packages/TestBase)
 - [TestBase.AspNetCore.Mvc](https://www.nuget.org/packages/TestBase.AspNetCore.Mvc)
 - [TestBase-Mvc](https://www.nuget.org/packages/TestBase-Mvc)
 - [TestBase.AdoNet](https://www.nuget.org/packages/TestBase.AdoNet)
 - [Serilog.Sinks.ListOfString](https://www.nuget.org/packages/Serilog.Sinks.Listofstring)
 - [Extensions.Logging.ListOfString](https://www.nuget.org/packages/Extensions.Logging.ListOfString)
