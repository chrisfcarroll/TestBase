using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using TestBase.FixtureBase;

namespace TestBase.TestsNet45.FixtureBase
{
    [TestFixture]
    public class FixtureBaseSpecs : FixtureBaseWithDbAndHttpFor<AUseCase>
    {
        [Test,Ignore("WIP Next")]public void UUTShouldNotBeNull() => UnitUnderTest.ShouldNotBeNull();
        [Test]public void DbShouldNotBeNull() => Db.ShouldNotBeNull();
        [Test]public void HttpClientShouldNotBeNull() => HttpClient.ShouldNotBeNull();
    }

    public class AUseCase
    {
        readonly IServiceRequiringDBandHttp service;

        internal AUseCase(IServiceRequiringDBandHttp service)
        {
            this.service = service;
        }
    }

    interface IServiceRequiringDBandHttp
    {
        List<string> GetFromDb();
        Task<string> GetFromHttp();
    }

    class AServiceRequiringDBandHttp : IServiceRequiringDBandHttp
    {
        readonly IDbConnection db;
        readonly System.Net.Http.HttpClient httpClient;

        public AServiceRequiringDBandHttp(IDbConnection db, System.Net.Http.HttpClient httpClient)
        {
            this.db = db;
            this.httpClient = httpClient;
        }

        public List<string> GetFromDb() => db.Query<string>("Select * Fom AClass").ToList();

        public async Task<string> GetFromHttp() => await httpClient.GetStringAsync("/string");
    }
}
