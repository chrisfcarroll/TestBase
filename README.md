TestBase
========
Aims to get you off to a flying start when unit testing projects with many dependencies by 
(1) reducing the amount of boilerplate mock management code you write
and (2) providing a rich extensible set of fluent assertions including
* ShouldEqualByValue assertion for all kinds of types and collections
* Stream assertions include ShouldContain and ShouldEqualByValue
* Moq ShouldCall assertions
* Mvc ActionResult, RedirectToRouteResult,ViewResult and Model assertions
* String Assertions

Works for both NUnit & MS UnitTestFramework test projects.

Work with Mono (define compile symbol NoMSTest to remove dependency on 
Microsoft.VisualStudio.QualityTools.UnitTestFramework)

Examples included:
-----------------
The example projects under test are where possible built from the VS2012 Template Projects
* Asp.Net Mvc4 Web Application
* WebApi
* Dapper
* Castle Windsor
And of course TestBase itself.


Dependencies in this version
------------
* .Net 4
* Moq for .Net 4
* Microsoft.VisualStudio.QualityTools.UnitTestFramework v4 
* NUnit.framework
* System.Web.Mvc v4.0.0.0 is targeted

Less useful if
----------------
You don't need mocks and you already have a full fluent assertions library 
