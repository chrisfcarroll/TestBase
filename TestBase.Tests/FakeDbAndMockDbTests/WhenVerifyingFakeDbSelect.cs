using NUnit.Framework;
using TestBase.AdoNet.FakeDb;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
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
                    param.Value = "Boo";
                    cmd.Parameters.Add(param);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveSelected("ATableName");
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("WrongTableName"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", "", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", ""); });
            }
        }
    }
}