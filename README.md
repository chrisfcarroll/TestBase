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

Work with Mono -- see below for how to build on Mono

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
* Optional (see below) Microsoft.VisualStudio.QualityTools.UnitTestFramework v4 
* NUnit.framework
* TestBase-Mvc targets System.Web.Mvc v4.0.0.0

Less useful if
----------------
You don't need mocks and you already have a full fluent assertions library 


Building on Mono
----------------
Several steps are required to build under Mono:

* In the TestBase and TestBase Mvc projects, define the compiler symbol NoMSTest to remove dependency on 
Microsoft.VisualStudio.QualityTools.UnitTestFramework.
* In Example.Mvc4 define the compiler symbol NoEF to remove the attempt to use an EntityFramework call not currently
supported on Mono. The consequence is that you can't auto-initialise the ASP.NET Simple Membership database
* run the NuGet commandline in each project file directory:
	nuget install "packages.config" -source ""   -RequireConsent -solutionDir "../ "
** except that for the Example projects the solutionDir is a level higher:
	nuget install "packages.config" -source ""   -RequireConsent -solutionDir "../../ "
* TODO: Recreate all of the projects that were created as MsTest projects to be ordinary C# library files.