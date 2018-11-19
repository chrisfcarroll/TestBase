using NUnit.Framework;
using TestBase.AdoNet.FakeDb;

namespace TestBase.Tests.FakeDbAndMockDbTests
{
    [TestFixture]
    public class WhenVerifyingFakeDbInvocationMultiples
    {
        [TestCase("Update",  "Select","Insert","Delete")]
        [TestCase("Select",  "Insert", "Delete","Update")]
        [TestCase("Insert",  "Delete", "Update", "Select")]
        [TestCase("Delete",  "Update", "Select","Insert")]
        public void Should_VerifyInvocationCount(string verb, params string[] verbsNotExecuted)
        {
            using (var conn = new FakeDbConnection().SetUpForExecuteNonQuery(0))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = verb + " ATableName Set Field=@field where Id=@Id ";
                var param1 = cmd.CreateParameter();
                cmd.Parameters.Add(param1);
                param1.ParameterName = "Id";

                param1.Value = 111;
                cmd.ExecuteNonQuery();
                param1.Value = 222;
                cmd.ExecuteNonQuery();
                param1.Value = 333;
                cmd.ExecuteNonQuery();

                cmd.ShouldHaveExecutedNTimes(3);
                conn.ShouldHaveExecutedNTimes(verb,"",new{Field=111}, times:3);
                conn.ShouldHaveExecutedNTimes(verb, "ATableName", "Field".Split(), 3);

                foreach (var otherVerb in verbsNotExecuted)
                {
                    Assert.Throws<Assertion>(() => { conn.ShouldHaveExecutedNTimes(otherVerb, "", new{Field=111}, 3); });
                    Assert.Throws<Assertion>(() => { conn.ShouldHaveExecutedNTimes(otherVerb, "", "Field".Split(), 3); });
                    Assert.Throws<Assertion>(() => { conn.ShouldHaveExecutedNTimes(otherVerb, "ATableName", "Field".Split(), 3); });
                    
                }
            }
        }
    }
}