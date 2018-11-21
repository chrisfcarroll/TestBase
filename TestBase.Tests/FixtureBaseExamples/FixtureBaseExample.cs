using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using TestBase.AdoNet;
using TestBase.Tests.FakeDbAndMockDbTests.WhenSettingUpAFakeDbConnection;

namespace TestBase.Tests.FixtureBaseExamples
{
    [TestFixture]
    public class FixtureBaseExample : FixtureBaseWithDbAndHttpFor<AUseCase>
    {
        [Test]
        public void UUTreturnsDataFromDbQuerySingleColumn()
        {
            var dbData = new[] { "row1", "row2", "row3", "row4"};
            Db.SetUpForQuerySingleColumn(dbData);
            UnitUnderTest.FromDbStrings().ShouldEqualByValue(dbData);
        }

        [Test]
        public void UUTreturnsDataFromDbQuery()
        {
            var dataToReturn = new[]
            {
                new IdAndName {Id = 11, Name = "cell 1,2"}, 
                new IdAndName {Id = 21, Name = "cell 2,2"}
            };
            Db.SetUpForQuery(dataToReturn,new[] {"Id", "Name"});
            UnitUnderTest
                .FromDbIdAndNames()
                .ShouldEqualByValue(dataToReturn);
        }

        [Test]
        public void UUTreturnsDataFromDbQueryScalar()
        {
            Db.SetUpForQueryScalar(999);
            UnitUnderTest.FromDb().ShouldBeOfLength(1).First().ShouldBe(999);
        }

        [Test]
        public async Task UUTreturnsDataFromService()
        {
            var fromService = "FromService";
            HttpClient
                .Setup(m => true)
                .Returns(new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(fromService)});

            var result= await UnitUnderTest.FromHttpService();
            result.ShouldBe(fromService);
        }

    }
    public class AUseCase
    {
        readonly IServiceRequiringDBandHttp service;

        public AUseCase(IServiceRequiringDBandHttp service)
        {
            this.service = service;
        }

        public List<int> FromDb() => service.GetFromDb();
        public List<IdAndName> FromDbIdAndNames() => service.GetFromDbIdAndNames();
        public async Task<string> FromHttpService() => await service.GetFromHttp();
        public List<string> FromDbStrings() => service.GetFromDbStrings();
    }

    public interface IServiceRequiringDBandHttp
    {
        List<IdAndName> GetFromDbIdAndNames();
        List<int> GetFromDb();
        Task<string> GetFromHttp();
        List<string> GetFromDbStrings();
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

        public List<IdAndName> GetFromDbIdAndNames()
        {
            return db.Query<IdAndName>("Select * Fom AClass").ToList();
        }

        public List<int> GetFromDb()
        {
            return db.Query<int>("Select * Fom AClass").ToList();
        }

        public async Task<string> GetFromHttp()
        {
            return await httpClient.GetStringAsync("http://localhost/string");
        }

        public List<string> GetFromDbStrings()
        {
            return db.Query<string>("Select * Fom AClass").ToList();
        }
    }

}