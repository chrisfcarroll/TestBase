using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Shoulds;

namespace Example.Dapper.Tests.IntegrationTests
{
    [TestClass]
    public class GivenEmptyDb : ExampleIntegrationTestBase
    {

        [TestInitialize]
        public override void SetUp()
        {
            UnitUnderTest= new Repository(SqlConnection);
        }

        [TestMethod]
        public void GetSomeData_should_not_throw_and_should_not_return_null()
        {
            UnitUnderTest.GetSomeData().ShouldNotBeNull();
        }

        [TestMethod]
        public void SaveSomeData_followed_by_Get_Should_return_the_original_data()
        {
            //A
            UnitUnderTest.SaveSomeData("Product 1");
            UnitUnderTest.SaveSomeData("Product 2");
            //A
            UnitUnderTest.GetSomeData()
                         .ShouldEqualByValue(
                             new[]
                                 {
                                     new Product {Id = 1, Description = "Product 1"},
                                     new Product {Id = 2, Description = "Product 2"},
                                 }
                );
        }

        [ClassInitialize]
        public static void CreateExampleDapperTestsDb(TestContext context)
        {
            TryCreateDb();
        }

        [ClassCleanup]
        public static void TryDropExampleDapperTestsDb()
        {
            TryDropDbAndDisposeConnection(); 
        }
       
    }
}
