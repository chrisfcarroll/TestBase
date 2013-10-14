using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TestBase.Attributes;
using TestBase.Example.WebApi;
using TestBase.Example.WebApi.Controllers;
using TestBase.Shoulds;

namespace Example.WebApi.Windsor.Tests
{
    [TestClass]
    public class WhenWebApiApplicationBootStrapsWindsor : TestBase.TestBase<WebApiApplicationTestWrapper>
    {
        [TestMethod]public void Should_instantiate_Windsor_container()
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

        [TestMethod]
        public void Should_register_all_controllers()
        {
            var allControllers = typeof(WebApiApplication).Assembly.GetPublicClassesFromApplicationAssembly(c => c.Is<IController>());
            var registeredControllers = UnitUnderTest.Container.GetImplementationTypesFor(typeof(IController));
            allControllers.ShouldEqual(registeredControllers);
            // Is<TType>() is defined in Castle.Core.Internal namespace     
        }

        [TestMethod]
        public void Should_register_ExampleDataSource_for_ISimpleDataSource()
        {
            UnitUnderTest.Container.Resolve<ISimpleDataSource>()
                .ShouldNotBeNull()
                .ShouldBeOfType<ExampleDataSource>();
        }

        [TestMethod]
        public void Should_register_one_and_only_one_class_for_ISimpleDataSource()
        {
            var registeredDataSources = UnitUnderTest.Container.GetImplementationTypesFor(typeof(ISimpleDataSource));
            registeredDataSources
                .ShouldContain(typeof (ExampleDataSource))
                .Count().ShouldEqual(1);

            var candidates = typeof(WebApiApplication).Assembly.GetPublicClassesFromApplicationAssembly(c => c.Is<ISimpleDataSource>());
            candidates.ShouldEqual(registeredDataSources);
        }

        [NotCurrentlyTestableInCode]
        public void Should_install_WindsorControllerFactory() { }

        [NotCurrentlyTestableInCode]
        public void Should_install_WindsorHttpActivator() { }

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            UnitUnderTest.InitialiseWindsorContainer();
        }
    }

    public class WebApiApplicationTestWrapper : WebApiApplication
    {
        public IWindsorContainer Container { get { return container; } }
    }

    public static class WindsorExtensions
    {
        public static IHandler[] GetAllHandlers(this IWindsorContainer container)
        {
            return GetHandlersFor(container, typeof(object));
        }

        public static IHandler[] GetHandlersFor(this IWindsorContainer container, Type type)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }

        public static Type[] GetImplementationTypesFor(this IWindsorContainer container, Type type)
        {
            return
                container.GetHandlersFor(type)
                         .Select(h => h.ComponentModel.Implementation)
                         .OrderBy(t => t.Name)
                         .ToArray();
        }

        public static Type[] GetPublicClassesFromApplicationAssembly(this Assembly assembly, Predicate<Type> @where)
        {
            return
                assembly.GetExportedTypes()
                        .Where(t => t.IsClass)
                        .Where(t => t.IsAbstract == false)
                        .Where(@where.Invoke)
                        .OrderBy(t => t.Name)
                        .ToArray();
        }
    }
}


