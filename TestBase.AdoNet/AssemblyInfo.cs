using System.Reflection;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyDescription(@"*TestBase* gets you off to a flying start when unit testing projects with dependencies.
It has rich, but easily extensible, fluent assertions, including EqualsByValue, Regex, Stream Comparision, and Ado.Net assertions.

TestBase.Shoulds
------------------
Chainable fluent assertions get you to the point concisely
```
UnitUnderTest.Action()
		.ShouldNotBeNull()
  	.ShouldContain(expected);
UnitUnderTest.OtherAction()
  	.ShouldEqualByValue(new {Id=1, Payload=expected, Additional=new[]{ expected1, expected2 }} )
  	.Payload
  			.ShouldMatchIgnoringCase(""I expected this"");
```
* `ShouldBe(), ShouldMatch(), ShouldNotBe(), ShouldContain(), ShouldNotContain(), ShouldBeEmpty(), ShouldNotBeEmpty(), ShouldAll()` and many more
* `ShouldEqualByValue(), ShouldEqualByValueExceptForValues()` works with all kinds of object and collections
* `Stream.ShouldHaveSameStreamContentAs()` and `Stream.ShouldContain()`

# TestBase.AdoNet adds:

TestBase.FakeDb
------------------
Works with Ado.Net and technologies on top of it, including Dapper.
* `fakeDbConnection.SetupForQuery(IEnumerable&lt;TFakeData&gt; )`
* `fakeDbConnection.SetupForQuery(IEnumerable&lt;Tuple&lt;TFakeDataForTable1,TFakeDataForTable2&gt;&gt; )`
* `fakeDbConnection.SetupForQuery(fakeData, new[] {""FieldName1"", FieldName2""})`
* `fakeDbConnection.SetupForExecuteNonQuery(rowsAffected)`

* `fakeDbConnection.ShouldHaveUpdated(""tableName"", [Optional] fieldList, whereClauseField)`
* `fakeDbConnection.ShouldHaveSelected(""tableName"", [Optional] fieldList, whereClauseField)`
* `fakeDbConnection.ShouldHaveUpdated(""tableName"", [Optional] fieldList, whereClauseField)`
* `fakeDbConnection.ShouldHaveDeleted(""tableName"", whereClauseField)`
* `fakeDbConnection.ShouldHaveInvoked(cmd => predicate(cmd))`
* `fakeDbConnection.ShouldHaveXXX().ShouldHaveParameter(""name"", value)`

* `fakeDbConnection.Verify(x=>x.CommandText.Matches(""Insert [case] .*"") &amp;&amp; x.Parameters[""id""].Value==1)`

TestBase.RecordingDb
--------------------
`new RecordingDbConnection(IDbConnection)` helps you profile Ado.Net Db calls


ChangeLog
---------
4.0.4.0 StreamShoulds
4.0.3.0 StringListLogger as MS Logger and as SeriLogger
4.0.1.0 Port to NetCore
3.0.3.0 Improves FakeDb setup
3.0.x.0 adds and/or corrects missing Shoulds()
2.0.5.0 adds some intellisense and FakeDbConnection.Verify(..., message,args) overload
")]
