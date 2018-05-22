using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("TestBase-Mvc")]
[assembly: AssemblyDescription(@"*TestBase* gives you a flying start with 
- fluent assertions that are easy to extend
- sharp error messages
- tools to help you test with “heavyweight” dependencies on 
    - AspNet.Mvc & AspNetCore.Mvc Contexts
	- HttpClient
	- Ado.Net
	- Streams & Logging

TestBase-Mvc adds a rich extensible set of fluent assertions for verifying Mvc4 ActionResults, and for easy setup of ControllerContext and HttpContext for both Mvc and WebApi

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

TestBase
----------
Controller extensions to fake the http request &amp; context. By injecting the RegisterRoutes method of your
MvcApplication, you can use and test Controller.Url with your application's configured routes.

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


See Also
--------

For AspNetCore: https://www.nuget.org/packages/TestBase.Mvc.AspNetCore

For NetStandard2
-----------------
https://www.nuget.org/Packages/TestBase 
https://www.nuget.org/Packages/TestBase.HttpClient.Fake
https://www.nuget.org/packages/TestBase.AdoNet
"
)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8640aff7-2dc9-4b70-92ed-d24a574e56bb")]

