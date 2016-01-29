using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using TestBase.FakeDb;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
    [TestFixture]
    public class WhenVerifyingFakeDbCommandInvocations
    {
        [Test]
        public void Should_Recognise_Select() 
        {
            using (var conn = new FakeDbConnection().SetUpForQuery(GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from ATableName";
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

        [Test]
        public void Should_Recognise_Insert()
        {
            using (var conn = new FakeDbConnection().SetUpForQuery(GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Insert ATableName (Id, Name) Values(@id, @name)";
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "Id";
                    param1.Value = 1;

                    var param2 = cmd.CreateParameter();
                    param2.ParameterName = "Name";
                    param2.Value = "Boo1";

                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveInserted("ATableName", new AClass{Name= "Boo1", Id=1});
                conn.ShouldHaveInserted("ATableName", new[]{"Name","Id"});
                conn.ShouldHaveInserted("ATableName", "");
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", new AClass { Name = "Boo1", Id = 222 }); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", new []{ "WrongCol", "Name" }); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("WrongTableName", new AClass { Name = "Boo1", Id = 1 }); });
                
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("ATableName"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", "", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName"); });
            }
        }

        [Test]
        public void Should_Recognise_Update()
        {
            var source = new AClass {Name = "Boo1", Id = 111};
            using (var conn = new FakeDbConnection().SetUpForQuery(GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Update ATableName Set Name= @Name Where Id=@Id";
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
                conn.ShouldHaveUpdated("ATableName", new AClass { Name = "Boo1", Id = 111 }, "Id");
                conn.ShouldHaveUpdated("ATableName", new[] { "Name" }, "Id", 111);
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new BClass{More=111,EvenMore = ""}, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new AClass { Name = "Boo1", Id = 222 }, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new []{ "Col1", "Id"}, "Id", 22222); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new { Col1 = "WrongValue", Col2 = "Boo2" }, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("WrongTableName", new { Col1 = "Boo1", Id = 111 }, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("ATableName"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName"); });
            }
        }

        [Test]
        public void Should_Recognise_Delete()
        {
            using (var conn = new FakeDbConnection().SetUpForQuery(GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Delete ATableName Where Id=@Id";
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "Id";
                    param1.Value = 111;
                    cmd.Parameters.Add(param1);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveDeleted("ATableName");
                conn.ShouldHaveDeleted("ATableName", "Id", 111);
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName", "Id", 222222); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName", "Id2", 111); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("WrongTableName", "Id", 111); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("ATableName"); });
            }
        }
        [Test]
        public void Should_CatchSomeTyposInTheCommand()
        {
            using (var conn = new FakeDbConnection().SetUpForExecuteNonQuery(0).SetUpForQuery(GivenFakeDataInFakeDb()).SetUpForExecuteNonQuery(0))
            {
                ExecuteNonQuery(conn, "Delete ATableName Id=@Id ");
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName", "Id", 111); });

                ExecuteReader(conn, "Select ATableName Id=@Id ");
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("ATableName",whereClauseField:"Id"); });

                ExecuteNonQuery(conn, "Insert Int ATableName Id=@Id ");
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", new {Id=111}); });
            }
        }

        static void ExecuteReader(FakeDbConnection conn, string commandText)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = commandText;
                var param1 = cmd.CreateParameter();
                param1.ParameterName = "Id";
                param1.Value = 111;
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
                param1.Value = 111;
                cmd.Parameters.Add(param1);
                cmd.ExecuteNonQuery();
            }
        }

        static AClass[] GivenFakeDataInFakeDb()
        { 
            return new[]
            {
                new AClass {Id = 1, Name = "Name"},
                new AClass {Id = 2, Name = "Name2"},
            };
        }
    }
}