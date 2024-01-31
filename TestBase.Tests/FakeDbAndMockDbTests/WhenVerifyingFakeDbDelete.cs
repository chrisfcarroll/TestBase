using NUnit.Framework;
using TestBase.AdoNet;

namespace TestBase.Tests.FakeDbAndMockDbTests;

[TestFixture]
public class WhenVerifyingFakeDbDelete
{
    [TestCase("ATableName")]
    [TestCase("namespaceisignored.ATableName")]
    [TestCase("namespace.isignored.ATableName")]
    [TestCase("from ATableName")]
    [TestCase("from namespace.isignored.ATableName")]
    public void Should_Recognise_Delete(string tablename)
    {
            using (var conn = new FakeDbConnection().SetUpForQuery(FakeData.GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("Delete {0} Where Id=@Id", tablename);
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "Id";
                    param1.Value         = 111;
                    cmd.Parameters.Add(param1);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveDeleted("ATableName");
                conn.ShouldHaveDeleted("ATableName", "Id", 111);
                Assert.Throws<Assertion>(() => { conn.ShouldHaveDeleted("ATableName",     "Id",  222222); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveDeleted("ATableName",     "Id2", 111); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveDeleted("WrongTableName", "Id",  111); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveInserted("ATableName", ""); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveSelected("ATableName"); });
            }
        }

    [TestCase("ATableName",                     "Id=@Id")]
    [TestCase("ATableName",                     "a=b and Id=@Id")]
    [TestCase("namespace.isignored.ATableName", "a=b and Id=@Id")]
    [TestCase("namespace.isignored.ATableName", "a=b or Id=@Id")]
    public void Should_Recognise_Delete_WhereClause(string tablename, string whereClause)
    {
            using (var conn = new FakeDbConnection().SetUpForQuery(FakeData.GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("Delete {0} Where {1}", tablename, whereClause);
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "Id";
                    param1.Value         = 111;
                    cmd.Parameters.Add(param1);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveDeleted("ATableName");
                conn.ShouldHaveDeleted("ATableName", "Id", 111);
                Assert.Throws<Assertion>(() => { conn.ShouldHaveDeleted("ATableName",     "Id",  222222); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveDeleted("ATableName",     "Id2", 111); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveDeleted("WrongTableName", "Id",  111); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveInserted("ATableName", ""); });
                Assert.Throws<Assertion>(() => { conn.ShouldHaveSelected("ATableName"); });
            }
        }
}