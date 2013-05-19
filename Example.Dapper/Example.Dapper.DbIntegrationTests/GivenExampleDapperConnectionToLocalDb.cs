using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Shoulds;

namespace Example.Dapper.DbIntegrationTests
{
    [TestClass]
    public class GivenEmptyDb : IntegrationTestBaseForExampleDapper
    {
        [TestInitialize]
        public override void Initialize()
        {
            Init(() => new Program(ConnectionString));

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
            CreateExampleDapperTestsDb();
        }

        [ClassCleanup]
        new public static void TryDropExampleDapperTestsDb()
        {
            IntegrationTestBaseForExampleDapper.TryDropExampleDapperTestsDb(); 
        }
       
    }
}
