using NUnit.Framework;
using TestBase.AdoNet.FakeDb;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
    [TestFixture]
    public class WhenVerifyingFakeDbInsert
    {
        [TestCase("ATableName")]
        [TestCase("namespaceisignored.ATableName")]
        [TestCase("namespace.isignored.ATableName")]
        [TestCase("into ATableName")]
        [TestCase("into namespace.isignored.ATableName")]
        public void Should_Recognise_Insert(string atablename)
        {
            using (var conn = new FakeDbConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("Insert {0} (Id, Name) Values(@id, @name)", atablename);
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "Id";
                    param1.Value = 1;

                    var param2 = cmd.CreateParameter();
                    param2.ParameterName = "Name";
                    param2.Value = "Boo1";

                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.ExecuteNonQuery();
                }

                conn.ShouldHaveInserted("ATableName", new AClass {Name = "Boo1", Id = 1});
                conn.ShouldHaveInserted("ATableName", new[] {"Name", "Id"});
                conn.ShouldHaveInserted("ATableName", "");
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", new AClass {Name = "Boo1", Id = 222}); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", new[] {"WrongCol", "Name"}); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("WrongTableName", new AClass {Name = "Boo1", Id = 1}); });

                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("ATableName"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", "", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName"); });
            }
        }
    }
}