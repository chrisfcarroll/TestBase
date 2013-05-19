using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Example.WebApi.Controllers;
using TestBase.Shoulds;

namespace TestBase.Example.WebApi.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest : TestBase<ValuesController>
    {
        private readonly ExampleDataSource fakeData= new ExampleDataSource();

        [TestInitialize]
        public override void Initialize()
        {
            Fakes.Add<ISimpleDataSource>(fakeData);
            base.Initialize();
        }

        [TestMethod]
        public void Get_should_return_correct_1_row_of_data()
        {
            UnitUnderTest.Get(1)
                .ShouldNotBeNull()
                .ShouldEqualByValue(Fakes.Get<ISimpleDataSource>().Get(1));
        }

        [TestMethod]
        public void GetAll_Should_return_all_source_data()
        {
            UnitUnderTest.Get().ShouldEqual( Fakes.Get<ISimpleDataSource>().GetAll() );
        }

        [TestMethod]
        public void Post_should_not_throw_WIP()
        {
            UnitUnderTest.Post("value");
        }

        [TestMethod]
        public void Put_Should_not_throw_WIP()
        {
            UnitUnderTest.Put(5, "value");
        }

        [TestMethod]
        public void Delete_Should_not_throw_WIP()
        {
            UnitUnderTest.Delete(5);
        }
    }
}
