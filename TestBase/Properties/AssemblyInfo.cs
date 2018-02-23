using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("TestBase")]
[assembly: AssemblyDescription(@"*TestBase* gets you off to a flying start when unit testing projects with dependencies.
It offers a rich extensible set of fluent assertions and a set of verifiable Fake Ado.Net components, with easy setup and verification.

TestBase.Shoulds
------------------
Chainable fluent assertions get you to the point concisely
UnitUnderTest.Action()
  .ShouldNotBeNull()
  .ShouldContain(expected);
UnitUnderTest.OtherAction()
  .ShouldEqualByValue( 
    new {Id=1, Payload=expectedPayload, Additional=new[]{ expected1, expected2 }}
);
* ShouldBe(), ShouldMatch(), ShouldNotBe(), ShouldContain(), ShouldNotContain(), ShouldBeEmpty(), ShouldNotBeEmpty(), ShouldAll() and many more
* ShouldEqualByValue() works with all kinds of object and collections
* Stream assertions include ShouldContain() and ShouldEqualByValue()

TestBase.FakeDb
------------------
Works with Ado.Net and technologies on top of it, including Dapper.
* fakeDbConnection.SetupForQuery(IEnumerable&lt;TFakeData&gt; )
* fakeDbConnection.SetupForQuery(IEnumerable&lt;Tuple&lt;TFakeDataForTable1,TFakeDataForTable2&gt;&gt; )
* fakeDbConnection.SetupForQuery(fakeData, new[] {""FieldName1"", FieldName2""})
* fakeDbConnection.SetupForExecuteNonQuery(rowsAffected)
* fakeDbConnection.ShouldHaveUpdated(""tableName"", [Optional] fieldList, whereClauseField)
* fakeDbConnection.ShouldHaveSelected(""tableName"", [Optional] fieldList, whereClauseField)
* fakeDbConnection.ShouldHaveUpdated(""tableName"", [Optional] fieldList, whereClauseField)
* fakeDbConnection.ShouldHaveDeleted(""tableName"", whereClauseField)
* fakeDbConnection.ShouldHaveInvoked(cmd => predicate(cmd))
* fakeDbConnection.ShouldHaveXXX().ShouldHaveParameter(""name"", value)
* fakeDbConnection.Verify(x=>x.CommandText.Matches(""Insert [case] .*"") &amp;&amp; x.Parameters[""id""].Value==1)

TestBase.TestBase&lt;T&gt; AutoMocksAndFakes
--------------------------------
is in development and currently works non-recursively. It auto-constructs the UnitUnderTest.
It identifies constructor dependencies by name and type, looking in the following places:
1) Fields in the TestFixture class 
2) Entries in the Mocks[] or Fake[] dictionaries
3) Finally it creates a Mock (if it's a mockable type) or a default instance (if its sealed, or value type) for anything that's missing

ChangeLog
---------
3.0.3.0 Improves FakeDb setup
3.0.x.0 adds and/or corrects missing Shoulds()
2.0.5.0 adds some intellisense and FakeDbConnection.Verify(..., message,args) overload
")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8640aff7-2dc9-4b70-92ed-d24a574e56bb")]
