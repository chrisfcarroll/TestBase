TestBase
========
Aims to get you off to a flying start when unit testing projects with many dependencies by 
(1) reducing the amount of boilerplate mock management code you write
and (2) providing a rich extensible set of fluent assertions including
* Most NUnit assertions
* ShouldEqualByValue assertion for all kinds of types and collections
* Stream.ShouldCountain assertions
* Moq ShouldCall assertions
* Mvc assertions for controller action results

Works for both NUnit & MS UnitTestFramework test projects.

Examples included:
-----------------
The example projects under test are where possible the VS2012 Template Projects
* Asp.Net Mvc4 Web Application
* WebApi
And of course TestBase itself.


Dependencies in this version
------------
* .Net 4
* Moq for .Net 4
* Microsoft.VisualStudio.QualityTools.UnitTestFramewok v4 
* NUnit.framework
* System.Web.Mvc v4.0.0.0 is targeted

Less useful if
----------------
You don't need mocks and you already have a fluent assertions library 
