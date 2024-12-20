namespace TestBase.AdoNet.Tests.FakeDbAndMockDbTests;

[TestFixture]
public class WhenVerifyingFakeDbInvocations
{
    static void ExecuteReader(FakeDbConnection conn, string commandText)
    {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = commandText;
                var param1 = cmd.CreateParameter();
                param1.ParameterName = "Id";
                param1.Value         = 111;
                cmd.Parameters.Add(param1);
                cmd.ExecuteReader();
            }
        }

    static void ExecuteNonQuery(FakeDbConnection conn, string commandText)
    {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = commandText;
                var param1 = cmd.CreateParameter();
                param1.ParameterName = "Id";
                param1.Value         = 111;
                cmd.Parameters.Add(param1);
                cmd.ExecuteNonQuery();
            }
        }

    [Test]
    public void Should_CatchSomeTyposInTheCommand()
    {
            using (var conn = new FakeDbConnection()
                             .SetUpForExecuteNonQuery(0)
                             .SetUpForQuery(FakeData.GivenFakeDataInFakeDb())
                             .SetUpForExecuteNonQuery(0))
            {
                ExecuteNonQuery(conn, "Delete ATableName Id=@Id ");
                Assert.Throws<Assertion>(() => { conn.ShouldHaveDeleted("ATableName", "Id", 111); });

                ExecuteReader(conn, "Select ATableName Id=@Id ");
                Assert.Throws<Assertion>(() => { conn.ShouldHaveSelected("ATableName", whereClauseField: "Id"); });

                ExecuteNonQuery(conn, "Insert Int ATableName Id=@Id ");
                Assert.Throws<Assertion>(() => { conn.ShouldHaveInserted("ATableName", new {Id = 111}); });
            }
        }
}