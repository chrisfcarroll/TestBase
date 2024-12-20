TestBase.FakeDb
------------------
Fake and verify AdoNet queries and commands
```
new fakeDbConnection()
    .SetupForQuery(IEnumerable<TFakeData> )
    .SetupForQuery(IEnumerable<Tuple<TFakeDataForTable1,TFakeDataForTable2>> )
    .SetupForQuery(fakeData, new[] {"FieldName1", "FieldName2"})
    .SetupForExecuteNonQuery(rowsAffected)
    .ShouldHaveUpdated("tableName", [Optional] fieldList, whereClauseField)
    .ShouldHaveSelected("tableName", [Optional] fieldList, whereClauseField)
    .ShouldHaveUpdated("tableName", [Optional] fieldList, whereClauseField)
    .ShouldHaveDeleted("tableName", whereClauseField)
    .ShouldHaveInvoked(cmd => predicate(cmd))
    .ShouldHaveExecutedStoredProcedure("name")
    .ShouldHaveXXX().ShouldHaveParameter("name", value)
    .Verify(x=>x.CommandText.Matches("Insert [case] .*") && x.Parameters["id"].Value==1)
```

TestBase.RecordingDb
--------------------
* `new RecordingDbConnection(IDbConnection)` helps you profile and assert Ado.Net Db calls for automated integration tests.

```
var recordingDbConnection = new RecordingDbConnection(RealDbConnection);

recordingDbConnection
    .ShouldHaveSelected(...)
    .ShouldHaveInsertedColumns(...)
    .ShouldHaveInserted(...)
    .ShouldHaveDeleted(...)
    .ShouldHaveUpdated(...)
    .ShouldHaveUpdatedNRows(...)
    .ShouldHaveInvoked(...)
        .ShouldHaveParameter(...)
        .ShouldHaveWhereClauseWithFieldEqualsExpected(...)
```


See also
 - TestBase
 - TestBase.Mvc
 - TestBase.AdoNet
 - Serilog.Sinks.ListOfString 
 - Extensions.Logging.ListOfString