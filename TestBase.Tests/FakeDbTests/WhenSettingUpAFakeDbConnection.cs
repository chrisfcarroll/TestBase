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
    public class WhenSettingUpAFakeDbConnection
    {
        internal class IdAndName { public int Id { get; set; } public string Name { get; set; } }
        internal class WithJoin { public int Id { get; set; } public IdAndName IdAndName { get; set; } }

        [Test]
        public void Should_return_the_setup_data__Given_an_array_of_fakedata()
        {
            //A
            var dataToReturn = new[]
                {
                    new IdAndName {Id = 11, Name = "cell 1,2"}, 
                    new IdAndName {Id = 21, Name = "cell 2,2"}
                };

            //A
            var fakeConnection = new FakeDbConnection().SetUpForQuery(dataToReturn,new[] {"Id", "Name"});

            //A 
            //Dapper -- the easy way to read a DbDataReader.
            fakeConnection.Query<IdAndName>("").ShouldEqualByValue(dataToReturn);
        }

        [Test, Ignore("WIP - doesn't yet handle nested classes")]
        public void Should_return_the_setup_data__Given_an_array_of_fakedata_with_joins()
        {
            //A
            var dataToReturn = new[]
                {
                    new WithJoin{ Id=100, IdAndName = new IdAndName {Id = 11, Name = "cell 1,2"}}, 
                    new WithJoin{ Id=200, IdAndName = new IdAndName {Id = 21, Name = "cell 2,2"}}
                };

            //A
            var fakeConnection = new FakeDbConnection().SetUpForQuery(dataToReturn, new[] { "Id", "IdAndName.Id", "IdAndName.Name" });

            //A 
            //Dapper -- the easy way to read a DbDataReader.
            fakeConnection.Query<WithJoin>("").ShouldEqualByValue(dataToReturn);
        }

        [Test]
        public void When_SetupForExecuteNonQuery__Should_return_an_int()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123);

            //A 
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
        }

        [Test]
        public void When_SetupForExecuteNonQueryNTimes__Should_return_an_int_each_time()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123,3);

            //A 
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
        }

        [Test]
        public void When_SetupForExecuteScalar__Should_return_scalar()
        {
            //A
            const string value = "fake hello";
            var fakeConnection = new FakeDbConnection().SetUpForExecuteScalar(value);

            //A 
            fakeConnection.CreateCommand().ExecuteScalar().ShouldEqualByValue(value);
        }
    }
}
