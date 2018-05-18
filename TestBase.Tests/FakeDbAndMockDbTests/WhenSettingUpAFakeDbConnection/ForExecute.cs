using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using TestBase.AdoNet.FakeDb;

namespace TestBase.Tests.FakeDbAndMockDbTests.WhenSettingUpAFakeDbConnection
{
    [TestFixture]
    public class ForExecute
    {
        [Test]
        public void When_SetupForExecuteNonQuery__Should_return_the_setup_int()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123);

            //A 
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
        }
        [Test]
        public async Task When_SetupForExecuteNonQuery__Should_return_the_setup_int__GivenQueryAsync()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123);

            //A 
            (await fakeConnection.CreateCommand().ExecuteNonQueryAsync()).ShouldEqual(123);
        }

        [Test]
        public void When_SetupForExecuteNonQuery__Given__Using_Dapper__Should_return_the_setup_int()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123);

            //A 
            fakeConnection.Execute("").ShouldEqual(123);
        }

        [Test]
        public async Task When_SetupForExecuteNonQuery__Given__Using_Dapper__Should_return_the_setup_int__GivenExecuteAsync()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123);

            //A 
            (await fakeConnection.ExecuteAsync("")).ShouldEqual(123);
        }

        [Test]
        public void When_SetupForExecuteNonQueryNTimes__Should_return_an_int_each_time()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123, 3);

            //A 
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
        }

        [Test]
        public async Task When_SetupForExecuteNonQueryNTimes__Should_return_an_int_each_time__GivenExecuteAsync()
        {
            //A
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123, 3);

            //A 
            (await fakeConnection.CreateCommand().ExecuteNonQueryAsync()).ShouldEqual(123);
            fakeConnection.CreateCommand().ExecuteNonQuery().ShouldEqual(123);
            (await fakeConnection.CreateCommand().ExecuteNonQueryAsync()).ShouldEqual(123);
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