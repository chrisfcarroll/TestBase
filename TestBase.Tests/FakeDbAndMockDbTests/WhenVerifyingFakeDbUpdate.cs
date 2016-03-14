using NUnit.Framework;
using TestBase.FakeDb;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
    [TestFixture]
    public class WhenVerifyingFakeDbUpdate
    {

        [TestCase("ATableName")]
        [TestCase("namespaceisignored.ATableName")]
        [TestCase("namespace.isignored.ATableName")]
        public void Should_Recognise_Update(string atablename)
        {
            var source = new AClass {Name = "Boo1", Id = 111};
            using (var conn = new FakeDbConnection().SetUpForQuery(FakeData.GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("Update {0} Set Name= @Name Where Id=@Id", atablename);
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "Name";
                    param1.Value = "Boo1";

                    var param2 = cmd.CreateParameter();
                    param2.ParameterName = "Id";
                    param2.Value = 111;

                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveUpdated("ATableName", source, "Id");
                conn.ShouldHaveUpdated("ATableName", new AClass {Name = "Boo1", Id = 111}, "Id");
                conn.ShouldHaveUpdated("ATableName", new[] {"Name"}, "Id", 111);
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new BClass {More = 111, EvenMore = ""}, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new AClass {Name = "Boo1", Id = 222}, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new[] {"Col1", "Id"}, "Id", 22222); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new {Col1 = "WrongValue", Col2 = "Boo2"}, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("WrongTableName", new {Col1 = "Boo1", Id = 111}, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("ATableName"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName"); });
            }
        }
    }
}