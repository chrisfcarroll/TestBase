TestBase
========
*TestBase* gives you a jump-start when unit testing projects with dependencies, offering 
* a rich extensible set of fluent assertions 
* a set of Fake Ado.Net components, with easy setup and verification.

TestBase.Shoulds
------------------
Chainable fluent assertions get you to the point concisely.

    UnitUnderTest.Action()
      .ShouldNotBeNull()
      .ShouldContain(expected);
    UnitUnderTest.OtherAction()
      .ShouldEqualByValue(new {Id=1, Payload=new[]{ expected1, expected2 }});

* ShouldBe(), ShouldMatch(), ShouldNotBe(), ShouldContain(), ShouldNotContain(), ShouldBeEmpty(), ShouldNotBeEmpty(), ShouldAll() and many more
* ShouldEqualByValue() works with all kinds of object and collections
* Stream assertions include ShouldContain() and ShouldEqualByValue()

TestBase.FakeDb
------------------
Works with Ado.Net and technologies on top of it, including Dapper.

    fakeDbConnection.SetupForQuery(fakeData, new[] {"ColumnName1", ...})
    fakeDbConnection.SetupForExecuteNonQuery()
    fakeDbConnection.Verify(x=>x.CommandText.Matches("Insert .*") && x.Parameters["id"].Value==1)

TestBase-Mvc
------------

    ControllerUnderTest.Action()
      .ShouldbeViewResult()
      .ShouldHaveModel<TModel>()
      .ShouldEqualByValue(expected)
    ControllerUnderTest.Action()
      .ShouldBeRedirectToRouteResult()
      .ShouldHaveRouteValue("expectedKey", [Optional] "expectedValue");

* ShouldHaveViewDataContaining(), ShouldBeJsonResult() etc.

Can be used in both NUnit & MS UnitTestFramework test projects.

Building on Mono : define compile symbol NoMSTest to remove dependency on 
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