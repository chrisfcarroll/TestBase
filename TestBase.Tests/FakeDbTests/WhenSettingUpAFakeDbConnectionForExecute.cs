using NUnit.Framework;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace TestBase.Tests.FakeDbTests
{
    [TestFixture]
    public class WhenSettingUpAFakeDbConnectionForExecute
    {
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
            var fakeConnection = new FakeDbConnection().SetUpForExecuteNonQuery(123, 3);

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