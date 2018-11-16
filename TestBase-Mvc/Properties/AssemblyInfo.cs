using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Chris F Carroll @ https://github.com/chrisfcarroll/TestBase and https://www.nuget.org/profiles/chrisfcarroll")]
[assembly: AssemblyProduct("TestBase")]
[assembly: AssemblyCopyright("(c) Chris F. Carroll, 2013-2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("fdedc6a4-d54b-4d9c-80a9-20a91c07dbbd")]

[assembly: AssemblyVersion("4.0.0.0-preview")]
[assembly: AssemblyFileVersion("4.0.0.0-preview")]

[assembly: AssemblyTitle("TestBase-Mvc")]
[assembly: AssemblyDescription(@"*TestBase* gets you off to a flying start when unit testing projects with dependencies.
TestBase-Mvc adds a rich extensible set of fluent assertions for verifying Mvc ActionResults and for easy setup of ControllerContext and HttpContext for both Mvc and WebApi
TestBase.Shoulds
-------------------
Chainable fluent assertions get you to the point concisely
ControllerUnderTest.Action()
  .ShouldbeViewResult()
  .ShouldHaveModel&lt;TModel&gt;()
  .ShouldEqualByValue(expected)
ControllerUnderTest.Action()
  .ShouldBeRedirectToRouteResult()
  .ShouldHaveRouteValue(""expectedKey"", [Optional] ""expectedValue"");

ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.

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
    [Optional] string routeTemplate)"
)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8640aff7-2dc9-4b70-92ed-d24a574e56bb")]