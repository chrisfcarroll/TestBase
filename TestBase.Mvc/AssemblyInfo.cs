using System.Reflection;
[assembly: AssemblyDescription(@"*TestBase* gives you a flying start with 
- fluent assertions that are easy to extend
- explicit error messages
- tools to help you test with “heavyweight” dependencies on 
    - AspNetCore.Mvc, AspNet.Mvc or WebApi Contexts
	- HttpClient
	- Ado.Net
	- Streams & Logging

TestBase.Shoulds
-------------------
Chainable fluent assertions get you to the point concisely:
```
ControllerUnderTest.Action()
  .ShouldbeViewResult()
  .ShouldHaveModel<TModel>()
  .ShouldEqualByValue(expected)
ControllerUnderTest.Action()
  .ShouldBeRedirectToRouteResult()
  .ShouldHaveRouteValue(""expectedKey"", [Optional] ""expectedValue"");

ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.
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
  .ShouldHaveRouteValue(""expectedKey"", [Optional] ""expectedValue"");

ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.
```

TestBase.Mvc.AspNetCore
-------------------------------------------

Quickly test controllers with zero setup using `controllerUnderTest.WithControllerContext()` :

```
[TestFixture]
public class WhenTestingControllersUsingFakeControllerContext
{
   [Test]
    public void ControllerUrlAndOtherPropertiesShouldWorkAsExpected__GivenControllerContext()
    {
        var uut = new FakeController().WithControllerContext();
        uut.Url.Action(""a"", ""b"").ShouldEqual(""/b/a"");
        uut.ControllerContext.ShouldNotBeNull();
        uut.HttpContext.ShouldBe(uut.ControllerContext.HttpContext);
        uut.ControllerContext.HttpContext.Request.ShouldNotBeNull();
        uut.Request.ShouldNotBeNull();
        uut.Response.ShouldNotBeNull();
        uut.Url.ShouldNotBeNull();
        uut.ViewData.ShouldNotBeNull();
        uut.TempData.ShouldNotBeNull();

        uut.MyAction(param)
            .ShouldBeViewResult()
            .ShouldHaveModel<YouSaidViewModel>()
            .YouSaid.ShouldBe(param);
    }

    [Test]
    public void ShouldBeViewWithModel_ShouldAssertViewResultAndNameAndModel_And_UrlHelper_ShouldWork()
    {
        var controllerUnderTest = 
            new AController()
                .WithControllerContext(virtualPathTemplate:""/{Action}/Before/{Controller}"");

        var result= controllerUnderTest
                .Action(""SomeController"",""SomeAction"")
                .ShouldBeViewWithModel<AClass>(""ViewName"");
                    .FooterLink
                    .ShouldBe(""/SomeAction/Before/SomeController"");
    }
}
```

... Or test against complex application dependencies using `HostedMvcTestFixtureBase` and specify your `Startup` class:

```
[TestFixture]
public class WhenTestingControllersUsingAspNetCoreTestTestServer : HostedMvcTestFixtureBase
{

    [TestCase(""/dummy/action?id={id}"")]
    public async Task Get_Should_ReturnActionResult(string url)
    {
        var id=Guid.NewGuid();
        var httpClient=GivenClientForRunningServer<Startup>();
        GivenRequestHeaders(httpClient, ""CustomHeader"", ""HeaderValue1"");
            
        var result= await httpClient.GetAsync(url.Formatz(new {id}));

        result
            .ShouldBe_200Ok()
            .Content.ReadAsStringAsync().Result
            .ShouldBe(""Content"");
    }

    [TestCase(""/dummy"")]
    public async Task Put_Should_ReturnA(string url)
    {
        var something= new Fixture().Create<Something>();
        var jsonBody= new StringContent(something.ToJSon(), Encoding.UTF8, ""application/json"");
        var httpClient=GivenClientForRunningServer<Startup>();
        GivenRequestHeaders(httpClient, ""CustomHeader"", ""HeaderValue1"");

        var result = await httpClient.PutAsync(url, jsonBody);

        result.ShouldBe_202Accepted();
        DummyController.Putted.ShouldEqualByValue( something );
    }
}
```

TestBase.Mvc for Mvc4 and Mvc 5
------------------
Use the `Controller.WithHttpContextAndRoutes()` extension methods to fake the 
http request &amp; context. And, by injecting the RegisterRoutes method of your
MvcApplication, you can use and test Controller.Url with your application's configured routes.

```
ControllerUnderTest
  .WithHttpContextAndRoutes(
    [Optional] Action&lt;RouteCollection&gt; mvcApplicationRoutesRegistration, 
    [optional] string requestUrl,
    [Optional] string query = """",
    [Optional] string appVirtualPath = ""/"",
    [Optional] HttpApplication applicationInstance)

ApiControllerUnderTest.WithWebApiHttpContext&lt;T&gt;(
    HttpMethod httpMethod, 
    [Optional] string requestUri,
    [Optional] string routeTemplate)
```
"
)]
