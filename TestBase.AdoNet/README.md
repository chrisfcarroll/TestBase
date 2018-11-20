*TestBase* gives you a flying start with 
- fluent assertions that are easy to extend
- sharp error messages
- tools to help you test with "heavyweight" dependencies on 
    - AspNet.Mvc or AspNetCore Contexts
	- HttpClient
	- Ado.Net
	- Streams & Logging
- Mix & match with your favourite test runners and assertions.

TestBase.AdoNet

TestBase.FakeDb
------------------
Fake and verify AdoNet queries and commands
```
- fakeDbConnection.SetupForQuery(IEnumerable<TFakeData>; )
- fakeDbConnection.SetupForQuery(IEnumerable<Tuple<TFakeDataForTable1,TFakeDataForTable2>> )
- fakeDbConnection.SetupForQuery(fakeData, new[] {"FieldName1", FieldName2"})
- fakeDbConnection.SetupForExecuteNonQuery(rowsAffected)
- fakeDbConnection.ShouldHaveUpdated("tableName", [Optional] fieldList, whereClauseField)
- fakeDbConnection.ShouldHaveSelected("tableName", [Optional] fieldList, whereClauseField)
- fakeDbConnection.ShouldHaveUpdated("tableName", [Optional] fieldList, whereClauseField)
- fakeDbConnection.ShouldHaveDeleted("tableName", whereClauseField)
- fakeDbConnection.ShouldHaveInvoked(cmd => predicate(cmd))
- fakeDbConnection.ShouldHaveXXX().ShouldHaveParameter("name", value)
- fakeDbConnection.Verify(x=>x.CommandText.Matches("Insert [case] .*") && x.Parameters["id"].Value==1)
```

TestBase.RecordingDb
--------------------
* `new RecordingDbConnection(IDbConnection)` helps you profile Ado.Net Db calls


Chainable fluent assertions get you to the point concisely:
```
UnitUnderTest.Action()
  .ShouldNotBeNull()
  .ShouldEqualByValueExceptFor(new {Id=1, Descr=expected}, ignoreList )
  .Payload
    .ShouldMatchIgnoringCase("I expected this")
	.Should(someOtherPredicate);
	.Items
      .ShouldAll(predicate)
	  .ShouldContain(item)
	  .ShouldNotContain(predicate)
	  .Where(predicate)
	  .SingleOrAssertFail()

.ShouldEqualByValue().ShouldEqualByValueExceptFor(...).ShouldEqualByValueOnMembers()
  work with all kinds of object and collections, and report what differed.
string.ShouldMatch(pattern).ShouldNotMatch().ShouldBeEmpty().ShouldNotBeEmpty()
.ShouldNotBeNullOrEmptyOrWhiteSpace().ShouldEqualIgnoringCase()
.ShouldContain().ShouldStartWith().ShouldEndWith().ShouldBeContainedIn().ShouldBeOneOf().ShouldNotBeOneOf()
numeric.ShouldBeBetween().ShouldEqualWithTolerance()....GreaterThan....LessThan...GreaterOrEqualTo ...
ienumerable.ShouldAll().ShouldContain().ShouldNotContain().ShouldBeEmpty().ShouldNotBeEmpty() ...
stream.ShouldHaveSameStreamContentAs().ShouldContain()
value.ShouldBe().ShouldNotBe().ShouldBeOfType().ShouldBeAssignableTo()...
```

See also
 - [TestBase](https://www.nuget.org/packages/TestBase)
 - [TestBase.AspNetCore.Mvc](https://www.nuget.org/packages/TestBase.AspNetCore.Mvc)
 - [TestBase-Mvc](https://www.nuget.org/packages/TestBase-Mvc)
 - [TestBase.AdoNet](https://www.nuget.org/packages/TestBase.AdoNet)
 - [TestBase.HttpClient.Fake](https://www.nuget.org/packages/TestBase.HttpClient.Fake)
 - [Serilog.Sinks.ListOfString](https://www.nuget.org/packages/Serilog.Sinks.Listofstring)
 - [Extensions.Logging.ListOfString](https://www.nuget.org/packages/Extensions.Logging.ListOfString)
