using System.Reflection;

#if ASPNETCORE
#endif
#if MVC45
#endif

[assembly: AssemblyDescription(@"*TestBase* gets you off to a flying start when unit testing projects with dependencies.
TestBase.Mvc adds a rich extensible set of fluent assertions for verifying Mvc ActionResults and for easy setup of ControllerContext and HttpContext for both Mvc and WebApi

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

Version 4 for netstandard20 & AspNetCore Mvc
-------------------------------------------

Test your controllers using by specifying your MVCApplications `Startup` class:

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


Version 3 for Net4
------------------
Use the `Controller.WithHttpContextAndRoutes()` extension methods to fake the 
http request &amp; context. By injecting the RegisterRoutes method of your
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

4.0.5.1 TestBase.Mvc partially ported to netstandard20 / AspNetCore
"
)]
