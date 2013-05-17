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
        public void Get()
        {
            UnitUnderTest.Get(1)
                .ShouldNotBeNull()
                .ShouldEqualByValue(Fakes.Get<ISimpleDataSource>().Get(1));
        }

        [TestMethod]
        public void GetAll()
        {
            UnitUnderTest.Get().ShouldEqual( Fakes.Get<ISimpleDataSource>().GetAll() );
        }

        [TestMethod]
        public void Post()
        {
            UnitUnderTest.Post("value");
        }

        [TestMethod]
        public void Put()
        {
            UnitUnderTest.Put(5, "value");
        }

        [TestMethod]
        public void Delete()
        {
            UnitUnderTest.Delete(5);
        }
    }
}
