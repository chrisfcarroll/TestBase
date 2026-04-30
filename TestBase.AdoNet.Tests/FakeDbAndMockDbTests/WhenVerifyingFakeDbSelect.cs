namespace TestBase.AdoNet.Tests.FakeDbAndMockDbTests;

[TestFixture]
public class WhenVerifyingFakeDbSelect
{
    [TestCase("ATableName")]
    [TestCase("namespaceisignored.ATableName")]
    [TestCase("namespace.isignored.ATableName")]
    [TestCase("otherTable o join ATableName a on o.something = a.something")]
    public void Should_Recognise_Select(string atablename)
    {
            using (var conn = new FakeDbConnection().SetUpForQuery(FakeData.GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("Select * from {0}", atablename);
                    var param = cmd.CreateParameter();
                    param.ParameterName = "PName";
                    param.Value         = "Boo";
                    cmd.Parameters.Add(param);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveSelected("ATableName");
                Should.Throw<Assertion>(() => { conn.ShouldHaveSelected("WrongTableName"); });
                Should.Throw<Assertion>(() => { conn.ShouldHaveUpdated("ATableName", "", ""); });
                Should.Throw<Assertion>(() => { conn.ShouldHaveDeleted("ATableName"); });
                Should.Throw<Assertion>(() => { conn.ShouldHaveInserted("ATableName", ""); });
            }
        }
}