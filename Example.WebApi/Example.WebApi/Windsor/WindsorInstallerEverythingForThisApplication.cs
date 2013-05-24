using System.Web.Http;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TestBase.Example.WebApi.Controllers;
using TestBase.Example.WebApi.Windsor.Windsor3HybridWebLifeStyleManager;

namespace TestBase.Example.WebApi.Windsor
{
    public class WindsorInstallerEverythingForThisApplication : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<ApiController>().Configure(c=>c.LifeStyle.HybridPerWebRequestTransient()),
                Classes.FromThisAssembly().BasedOn<Controller>().Configure(c=>c.LifeStyle.HybridPerWebRequestTransient())
                );

            container.Register(
                Types.FromThisAssembly()
                     .Pick()
                     .WithServiceFirstInterface()
                     .Configure(c => c.LifeStyle.HybridPerWebRequestTransient())
                );
        }
    }
}