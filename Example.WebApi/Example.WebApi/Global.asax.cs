using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using TestBase.Example.WebApi.App_Start;
using TestBase.Example.WebApi.Windsor;

namespace TestBase.Example.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected IWindsorContainer container;

        protected void Application_Start()
        {
            RegisterMvcBoilerplateConfigs();
            InitialiseWindsorContainer();
            SetWindsorAsWepApiControllerActivatorAndMvcControllerFactory();
        }

        private static void RegisterMvcBoilerplateConfigs()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void SetWindsorAsWepApiControllerActivatorAndMvcControllerFactory()
        {
            GlobalConfiguration
                .Configuration
                .Services.Replace(typeof (IHttpControllerActivator), new WindsorCompositionRoot(container));

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel)); 
        }

        public void InitialiseWindsorContainer()
        {
            container = new WindsorContainer().Install(new WindsorInstallerEverythingForThisApplication());
        }
    }
}