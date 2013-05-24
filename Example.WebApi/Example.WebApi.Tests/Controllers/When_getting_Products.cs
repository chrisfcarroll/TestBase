using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Example.WebApi.Controllers;
using TestBase.Example.WebApi.Models;
using TestBase.Shoulds;

namespace TestBase.Example.WebApi.Tests.Controllers
{
    [TestClass]
    public class When_getting_Products : TestBase.TestBase<ProductsController>
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public void GetAllProducts_should_get_all_products()
        {
            UnitUnderTest.GetAllProducts().ShouldEqualByValue(
                new[]
                    {
                        new Product(Id: 1, Name: "Tomato Soup", Category: "Groceries", Price: 1),
                        new Product(Id: 2, Name: "Yo-yo", Category: "Toys", Price: 3.75M),
                        new Product(Id: 3, Name: "Hammer", Category: "Hardware", Price: 16.99M),
                    });
        }

        [TestMethod]
        public void GetProductByValue_should_be_correct()
        {
            UnitUnderTest.GetProductById(2)
                .ShouldEqualByValue(new Product(Id: 2, Name: "Yo-yo", Category: "Toys", Price: 3.75M));
        }

        [TestMethod]
        public void GetProductByCategory_should_be_correct()
        {
            UnitUnderTest.GetProductsByCategory("Toys")
                         .ShouldBeSuchThat(x=>x.Count()==1)
                         .First()
                         .ShouldEqualByValue(new Product(Id: 2, Name: "Yo-yo", Category: "Toys", Price: 3.75M));
        }
    }
}
