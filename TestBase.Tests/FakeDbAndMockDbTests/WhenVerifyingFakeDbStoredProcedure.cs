using System.Data;
using NUnit.Framework;
using TestBase.AdoNet;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
    [TestFixture]
    public class WhenVerifyingFakeDbStoredProcedure
    {
        const string NoquerySproc = "NoquerySproc";

        static void ExecuteReader(FakeDbConnection conn, string procedureName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                var param1 = cmd.CreateParameter();
                param1.ParameterName = "Id";
                param1.Value         = 111;
                cmd.Parameters.Add(param1);
                cmd.ExecuteReader();
            }
        }

        static void ExecuteNonQuery(FakeDbConnection conn, string procedureName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                var param1 = cmd.CreateParameter();
                param1.ParameterName = "Id";
                param1.Value         = 111;
                cmd.Parameters.Add(param1);
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void Should_VerifyCalls()
        {
            using (var conn = new FakeDbConnection())
            {
                ExecuteNonQuery(conn, NoquerySproc);
                conn.ShouldHaveExecutedStoredProcedure(NoquerySproc);
                conn.ShouldHaveExecutedStoredProcedure(
                    NoquerySproc,
                    c=>c.CommandType==CommandType.StoredProcedure);
            }
        }

        [Test]
        public void Should_VerifyCallsWithParameter()
        {
            using (var conn = new FakeDbConnection())
            {
                ExecuteNonQuery(conn, NoquerySproc);
                conn.ShouldHaveExecutedStoredProcedure(NoquerySproc);
                conn.ShouldHaveExecutedStoredProcedureWithParameter(
                    NoquerySproc,
                    p => p.ParameterName == "Id" && 111.Equals(p.Value));
            }
        }
        [Test]
        public void Should_VerifyCallsWithParameterPredicateFailue()
        {
            using (var conn = new FakeDbConnection())
            {
                ExecuteNonQuery(conn, NoquerySproc);

                Assert.Throws<Assertion>(() =>
                {
                    conn.ShouldHaveExecutedStoredProcedureWithParameter(
                        NoquerySproc,
                        p => p.ParameterName == "Oops");
                });
            }
        }

        [Test]
        public void Should_VerifyPredicateFailure()
        {
            using (var conn = new FakeDbConnection())
            {
                ExecuteNonQuery(conn, NoquerySproc);
                Assert.Throws<Assertion>(() =>
                {
                    conn.ShouldHaveExecutedStoredProcedure(
                        NoquerySproc,
                        c=>c.CommandTimeout==9876);
                });
            }
        }

        [Test]
        public void Should_CatchSomeTyposInTheCommand()
        {
            using (var conn = new FakeDbConnection())
            {
                ExecuteNonQuery(conn, NoquerySproc);
                Assert.Throws<Assertion>(() =>
                {
                    conn.ShouldHaveExecutedStoredProcedure("Oops");
                });

                ExecuteReader(conn, "QuerySproc");
                Assert.Throws<Assertion>(() =>
                {
                    conn.ShouldHaveExecutedStoredProcedure("oops");
                });
            }
        }
    }
}
