using System.Reflection;
[assembly: AssemblyDescription(@"*TestBase* gets you off to a flying start when unit testing projects with dependencies.
TestBase.Mvc adds a rich extensible set of fluent assertions for verifying Mvc ActionResults and for easy setup of ControllerContext and HttpContext for both Mvc and WebApi

TestBase.Shoulds
-------------------
Chainable fluent assertions get you to the point concisely:
```
ControllerUnderTest.Action()
  .ShouldbeViewResult()
  .ShouldHaveModel&lt;TModel&gt;()
  .ShouldEqualByValue(expected)
ControllerUnderTest.Action()
  .ShouldBeRedirectToRouteResult()
  .ShouldHaveRouteValue(""expectedKey"", [Optional] ""expectedValue"");

ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.
```


Version 3 for Net4
------------------
Controller extensions to fake the http request &amp; context. By injecting the RegisterRoutes method of your
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


4.0.5.0 TestBase.Mvc partially ported to netstandard20 / AspNetCore
"
)]
