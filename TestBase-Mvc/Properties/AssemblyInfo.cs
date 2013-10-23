using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
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
Controller extensions to fake the http request &amp; context:
ControllerUnderTest
  .WithMvcHttpContext([Optional] requestUrl, [Optional] query, [Optional] appVirtualDir), 
  .WithHttpHeader(""headerName"",headerLines), 

WithWebApiHttpContext(httpMethod,[Optional] requestUri)"

)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8640aff7-2dc9-4b70-92ed-d24a574e56bb")]

