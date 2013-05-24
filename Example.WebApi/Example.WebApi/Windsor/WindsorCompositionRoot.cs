using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.Windsor;

namespace TestBase.Example.WebApi.Windsor
{
    internal class WindsorCompositionRoot : IHttpControllerActivator
    {
        private readonly IWindsorContainer container;

        public WindsorCompositionRoot(IWindsorContainer container)
        {
            this.container = container;
        }

        public IHttpController Create(HttpRequestMessage request,
                                      HttpControllerDescriptor controllerDescriptor,
                                      Type controllerType)
        {
            var controller = (IHttpController)container.Resolve(controllerType);
            request.RegisterForDispose( new Releaseable(() => container.Release(controller)));
            return controller;
        }

        private class Releaseable : IDisposable
        {
            private readonly Action release;

            public Releaseable(Action release) { this.release = release; }

            public void Dispose() { release(); }
        }
    }
}