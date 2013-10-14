using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.FakeDb;
using TestBase.Shoulds;

namespace Example.Dapper.Tests
{
    [TestClass]
    public class WhenReadingAndWritingData : TestBase.TestBase<Repository>
    {
        private FakeDbConnection dbConnection;
        private List<Product> fakeProducts;

        [TestInitialize]
        public override void SetUp()
        {
            dbConnection = new FakeDbConnection();
            UnitUnderTest= new Repository(dbConnection);
        }

        [TestMethod]
        public void ShouldGetData__GivenDataInFakeDb()
        {
            //A
            fakeProducts = new List<Product> {new Product {Description = "Product 1", Id = 1}};
            dbConnection.SetUpFor(fakeProducts, new string[] {"Description", "Id"});

            //A&A
            UnitUnderTest.GetSomeData().ShouldEqualByValue(fakeProducts);
        }
    }
}
