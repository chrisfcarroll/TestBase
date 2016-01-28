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
                    cmd.CommandText = "Insert ATableName (Col1, Col2) Values(@col1, @col2)";
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "col1";
                    param1.Value = "Boo1";

                    var param2 = cmd.CreateParameter();
                    param2.ParameterName = "col2";
                    param2.Value = "Boo2";

                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveInserted("ATableName", new {Col1= "Boo1", Col2="Boo2"});
                conn.ShouldHaveInserted("ATableName", new[]{"Col1","Col2"});
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", new { Col1 = "WrongValue", Col2 = "Boo2" }); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", new []{ "WrongCol", "Col2" }); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("WrongTableName", new { Col1 = "Boo1", Col2 = "Boo2" }); });
                
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveInserted("ATableName", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveSelected("ATableName"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", "", ""); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName"); });
            }
        }

        [Test]
        public void Should_Recognise_Update()
        {
            using (var conn = new FakeDbConnection().SetUpForQuery(GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Update ATableName Set Col1= @Col1 Where Id=@Id";
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "col1";
                    param1.Value = "Boo1";

                    var param2 = cmd.CreateParameter();
                    param2.ParameterName = "Id";
                    param2.Value = 111;

                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);
                    cmd.ExecuteReader();
                }

                conn.ShouldHaveUpdated("ATableName", new { Col1 = "Boo1", Id = 111 }, "Id");
                conn.ShouldHaveUpdated("ATableName", new []{ "Col1"}, "Id", 111);
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new { WrongColumnName = "Col1", Id = 111 }, "Id"); });
                Assert.Throws<AssertionException>(() => { conn.ShouldHaveUpdated("ATableName", new { Col1 = "Col1", Id = 222222 }, "Id"); });
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
        public void Should_Catch_Some_Typos()
        {
            using (var conn = new FakeDbConnection().SetUpForQuery(GivenFakeDataInFakeDb()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Delete ATableName Id=@Id ";
                    var param1 = cmd.CreateParameter();
                    param1.ParameterName = "Id";
                    param1.Value = 111;
                    cmd.Parameters.Add(param1);
                    cmd.ExecuteReader();
                }

                Assert.Throws<AssertionException>(() => { conn.ShouldHaveDeleted("ATableName", "Id", 111); });
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