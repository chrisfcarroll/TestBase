using System.Web.Mvc;
using System.Web.Routing;

namespace TestBase
{
    public class TypicalMvcRouteConfig
    {
        /// <summary>
        /// Creates the typical route mappings supplied out of the box by Visual Studio MVC Project Wizards:
        /// <code>
        /// routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        /// routes.MapRoute(
        ///    name: "Default",
        ///     url: "{controller}/{action}/{id}",
        ///     defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
        ///     );
        /// </code>
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}