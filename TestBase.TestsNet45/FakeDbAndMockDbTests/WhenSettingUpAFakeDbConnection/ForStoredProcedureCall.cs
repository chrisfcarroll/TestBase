using System.Data;
using Dapper;
using NUnit.Framework;
using TestBase.AdoNet;

namespace TestBase.Tests.FakeDbAndMockDbTests.WhenSettingUpAFakeDbConnection
{
    [TestFixture]
    public class ForStoredProcedureCall
    {
        [Test]
        public void ShouldWorkWithDapperDynamicParameters()
        {
            var parms = new
                        {
                        id      = 1,
                        name    = "FakeName",
                        dooble  = 123d,
                        dekimal = 123m,
                        boool   = true
                        };
            var parameters = new DynamicParameters(parms);

            var connection = new FakeDbConnection();

            connection.Open();
            connection.Execute("StoredProcedureName", parameters, commandType: CommandType.StoredProcedure);

            var invokedCommand =
            connection.VerifySingle(c => c.CommandType == CommandType.StoredProcedure
                                      && c.CommandText == "StoredProcedureName");

            var pars = invokedCommand.Parameters;
            pars["id"].Value.ShouldBe(1);
            pars["name"].Value.ShouldBe("FakeName");
            pars["boool"].Value.ShouldBe(true);
            pars["dekimal"].Value.ShouldBe(123m);
            pars["dooble"].Value.ShouldBe(123d);
        }
    }
}
