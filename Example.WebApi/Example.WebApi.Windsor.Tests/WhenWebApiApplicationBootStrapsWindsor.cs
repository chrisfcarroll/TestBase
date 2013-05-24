using System;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Attributes;
using TestBase.Example.WebApi;
using TestBase.Example.WebApi.Controllers;
using TestBase.Shoulds;

namespace Example.WebApi.Windsor.Tests
{
    [TestClass]
    public class WhenWebApiApplicationBootStrapsWindsor : TestBase.TestBase<WebApiApplicationTestWrapper>
    {
        [TestMethod]
        public void Should_instantiate_Windsor_container()
        {
            UnitUnderTest.Container.ShouldNotBeNull();
        }

        [TestMethod]
        public void Should_resolve_WebApi_Controllers()
        {
            UnitUnderTest.Container.Resolve<ProductsController>().ShouldNotBeNull();
        }

        [TestMethod]
        public void Should_resolve_Controllers_With_Constructor_Dependency()
        {
            UnitUnderTest.Container.Resolve<ValuesController>().ShouldNotBeNull();
        }

        [TestMethod]
        public void Should_resolve_MvcControllers()
        {
            UnitUnderTest.Container.Resolve<HomeController>().ShouldNotBeNull();
        }

        [NotCurrentlyTestableInCode]
        public void Should_install_WindsorControllerFactory(){}

        [NotCurrentlyTestableInCode]
        public void Should_install_WindsorHttpActivator() { }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            UnitUnderTest.InitialiseWindsorContainer();
        }
    }

    public class WebApiApplicationTestWrapper : WebApiApplication
    {
        public IWindsorContainer Container { get { return container; } }
    }
}
