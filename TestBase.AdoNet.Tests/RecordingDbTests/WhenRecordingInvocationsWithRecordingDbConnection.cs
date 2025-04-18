﻿using System.Data;
using TestBase.AdoNet.RecordingDb;

namespace TestBase.AdoNet.Tests.RecordingDbTests;

[TestFixture]
public class WhenRecordingInvocationsWithRecordingDbConnection
{
    [SetUp]
    public void SetUp()
    {
            fakeDbConnection = new FakeDbConnection();
            UnitUnderTest    = new RecordingDbConnection(fakeDbConnection);
        }

    RecordingDbConnection UnitUnderTest;
    FakeDbConnection fakeDbConnection;

    [Test]
    public void Should_DistinguishMultipleInvocationsOfOneCommand()
    {
            var text1 = "Command 1";
            var p1    = new FakeDbParameter {ParameterName = "p1", Value = "p1"};
            var p2    = new FakeDbParameter {ParameterName = "p2", Value = "p2"};
            fakeDbConnection.SetUpForExecuteNonQuery(1);
            //
            var cmd = UnitUnderTest.CreateCommand();
            cmd.CommandText = text1;
            cmd.Parameters.Add(p1);
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            cmd.Parameters.Add(p2);
            cmd.ExecuteNonQuery();
            //
            fakeDbConnection.Invocations[0].CommandText.ShouldBe(text1);
            UnitUnderTest.Invocations[0].CommandText.ShouldBe(text1);
            fakeDbConnection.Invocations[0].Parameters[0].ShouldEqualByValue(p1);
            UnitUnderTest.Invocations[0].Parameters[0].ShouldEqualByValue(p1);
            fakeDbConnection.Invocations[1].CommandText.ShouldBe(text1);
            UnitUnderTest.Invocations[1].CommandText.ShouldBe(text1);
            fakeDbConnection.Invocations[1].Parameters[0].ShouldEqualByValue(p2);
            UnitUnderTest.Invocations[1].Parameters[0].ShouldEqualByValue(p2);
        }

    [Test]
    public void Should_record_DbCommand()
    {
            var text = "Command 1";
            fakeDbConnection.SetUpForExecuteNonQuery(1);
            //
            var cmd = UnitUnderTest.CreateCommand();
            cmd.CommandText = text;
            cmd.ExecuteNonQuery();
            //
            fakeDbConnection.Invocations[0].CommandText.ShouldBe(text);
            UnitUnderTest.Invocations[0].CommandText.ShouldBe(text);
        }

    [Test]
    public void Should_record_DbParameters()
    {
            var text = "Command 1";
            fakeDbConnection.SetUpForExecuteNonQuery(1);
            //
            var cmd = UnitUnderTest.CreateCommand();
            cmd.CommandText = text;
            var p = new FakeDbParameter {ParameterName = "p1", Value = "p1"};
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            //
            fakeDbConnection.Invocations[0].CommandText.ShouldBe(text);
            fakeDbConnection.Invocations[0].Parameters[0].ShouldEqualByValue(p);
            fakeDbConnection.Invocations[0].Parameters[0].ShouldNotBe(p);
            UnitUnderTest.Invocations[0].CommandText.ShouldBe(text);
            UnitUnderTest.Invocations[0].Parameters[0].ShouldEqualByValue(p);
            UnitUnderTest.Invocations[0].Parameters[0].ShouldNotBe(p);
    }

    [Test]
    public void Can_record_an_IDbConnection_if_it_is_a_DbConnection()
    {
        using var uut = new RecordingDbConnection((IDbConnection)fakeDbConnection);
        var text = "Command 1";
        fakeDbConnection.SetUpForExecuteNonQuery(1);
        //
        var cmd = uut.CreateCommand();
        cmd.CommandText = text;
        cmd.ExecuteNonQuery();
        //
        fakeDbConnection.Invocations[0].CommandText.ShouldBe(text);
        uut.Invocations[0].CommandText.ShouldBe(text);
        
    }

    [TearDown]
    public void TearDown()
    {
        fakeDbConnection?.Dispose();
        UnitUnderTest?.Dispose();
    }
}