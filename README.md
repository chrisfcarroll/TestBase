TestBase
========
Aims to get you off to a flying start when unit testing projects with many dependencies by 
(1) Reducing the amount of boilerplate mock management code you right
and (2) Providing a rich extensible set of fluent assertions including
* Most NUnit assertions
* ShouldEqualByValue assertion for all kinds of types and collections
* Stream.ShouldCountain assertions
* Moq ShouldCall assertions
* Mvc assertions for controller action results

Works for both NUnit & MS UnitTestFramework test projects.

Dependencies in this version
------------
* .Net 4
* Moq for .Net 4
* Microsoft.VisualStudio.QualityTools.UnitTestFramewok v4 
* NUnit.framework
* System.Web.Mvc v4.0.0.0 is targeted

Less useful when
----------------
You don't need mocks and you already have a fluent assertions library
