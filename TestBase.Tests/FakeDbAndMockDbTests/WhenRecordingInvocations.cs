using NUnit.Framework;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
    [TestFixture]
    public class WhenRecordingInvocations
    {
        FakeDbConnection UnitUnderTest;

        [SetUp]
        public void SetUp() { UnitUnderTest = new FakeDbConnection(); }

        [Test]
        public void Should_record_DbCommand()
        {
            var text = "Command 1";
            UnitUnderTest.SetUpForExecuteNonQuery(1);
            //
            var cmd= (FakeDbCommand) UnitUnderTest.CreateCommand();
            cmd.CommandText = text;
            cmd.ExecuteNonQuery();
            //
            UnitUnderTest.Invocations[0].CommandText.ShouldBe(text);
        }

        [Test]
        public void Should_record_DbParameters()
        {
            var text = "Command 1";
            UnitUnderTest.SetUpForExecuteNonQuery(1);
            //
            var cmd = (FakeDbCommand)UnitUnderTest.CreateCommand();
            cmd.CommandText = text;
            var p = new FakeDbParameter{ParameterName = "p1", Value = "p1"};
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            //
            UnitUnderTest.Invocations[0].CommandText.ShouldBe(text);
            UnitUnderTest.Invocations[0].Parameters[0].ShouldEqualByValue(p);
            UnitUnderTest.Invocations[0].Parameters[0].ShouldNotBe(p);
        }

        [Test]
        public void Should_DistinguishMultipleInvocationsOfOneCommand()
        {
            var text1 = "Command 1";
            var p1 = new FakeDbParameter { ParameterName = "p1", Value = "p1" };
            var p2 = new FakeDbParameter { ParameterName = "p2", Value = "p2" };
            UnitUnderTest.SetUpForExecuteNonQuery(1);
            //
            var cmd = (FakeDbCommand)UnitUnderTest.CreateCommand();
            cmd.CommandText = text1;
            cmd.Parameters.Add(p1);
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            cmd.Parameters.Add(p2);
            cmd.ExecuteNonQuery();
            //
            UnitUnderTest.Invocations[0].CommandText.ShouldBe(text1);
            UnitUnderTest.Invocations[0].Parameters[0].ShouldEqualByValue(p1);
            UnitUnderTest.Invocations[1].CommandText.ShouldBe(text1);
            UnitUnderTest.Invocations[1].Parameters[0].ShouldEqualByValue(p2);
        }


    }
}
