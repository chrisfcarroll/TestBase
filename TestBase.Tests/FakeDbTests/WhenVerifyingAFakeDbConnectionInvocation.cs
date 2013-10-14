using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using NUnit.Framework;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace TestBase.Tests.FakeDbTests
{
    [TestFixture]
    public class WhenVerifyingAFakeDbConnectionInvocation
    {
        readonly IdAndName[] fakeData = new[]
            {
                    new IdAndName {Id = 11, Name = "cell 1,2"}, 
                    new IdAndName {Id = 21, Name = "cell 2,2"}
            };

        class IdAndName { public int Id { get; set; } public string Name { get; set; } }

        [Test]
        public void Should_verify_number_of_invocations_matching_predicate__Given__SetupForQuery()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForQuery(fakeData, new[] { "Id", "Name" });
            fakeConnection.Query<IdAndName>("Query @id, @name",new{id=1,name="pname"}).ShouldEqualByValue(fakeData);

            //A & A
            fakeConnection.Verify(x => x.Parameters["id"].Value.Equals(1) && x.Parameters["name"].Value.Equals("pname"));
            fakeConnection.Verify(x => x.Parameters["id"].Value.Equals(1) && x.Parameters["name"].Value.Equals("pname"),1);
            fakeConnection.Verify(x => x.Parameters["id"].Value.Equals(1) && x.Parameters["name"].Value.Equals("pname"),1,true);
            fakeConnection.Verify(x => x.CommandText=="Query @id, @name");

            Assert.Throws<AssertionException>(() => fakeConnection.Verify(x => x.Parameters["id"].Value.Equals(999)))
                  .Message.ShouldMatch("called .* times.*");

            Assert.Throws<AssertionException>(() => fakeConnection.Verify(x=>x.CommandText == "WrongCommandText"))
                  .Message.ShouldMatch("called .* times.*");
        }

        [Test]
        public void Should_verify_number_of_invocations_matching_predicate__Given__SetupForExecuteNonQuery()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123,2);

            var cmd=fakeConnection.CreateCommand();
            cmd.CommandText = "FakeCommandText";
            cmd.Parameters.Add(new FakeDbParameter{ParameterName = "pname", Value = "pvalue"});
            cmd.ExecuteNonQuery().ShouldEqual(123);

            var cmd2 = fakeConnection.CreateCommand();
            cmd2.CommandText = "FakeCommandText";
            cmd2.Parameters.Add(new FakeDbParameter { ParameterName = "pname", Value = "pvalue 2 is different" });
            cmd2.ExecuteNonQuery().ShouldEqual(123);

            //A & A
            fakeConnection.Verify(x => x.Parameters["pname"].Value.Equals("pvalue"));
            fakeConnection.Verify(x => x.Parameters["pname"].Value.ToString().StartsWith("pvalue"), 2);
            fakeConnection.Verify(x => x.Parameters["pname"].Value.ToString().StartsWith("pvalue"), 2, true);
            fakeConnection.Verify(x => x.CommandText == "FakeCommandText");

            Assert.Throws<AssertionException>(() => fakeConnection.Verify(x => x.Parameters["pname"].Value.Equals("pvalue"), 999, true))
                  .Message.ShouldMatch("called .* times.*");

            Assert.Throws<AssertionException>(() => fakeConnection.Verify(x => x.Parameters["pname"].Value.ToString().StartsWith("pvalue"), 1, true))
                  .Message.ShouldMatch("called .* times.*");

            Assert.Throws<AssertionException>(() => fakeConnection.Verify(x => x.Parameters["pname"].Value.Equals("wrongValue")))
                  .Message.ShouldMatch("called .* times.*");

            Assert.Throws<AssertionException>(() => fakeConnection.Verify(x => x.CommandText == "WrongCommandText"))
                  .Message.ShouldMatch("called .* times.*");
        }
    }
}
