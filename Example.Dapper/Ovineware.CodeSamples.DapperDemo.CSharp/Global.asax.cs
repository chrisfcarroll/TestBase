﻿using System.Web.Mvc;
using System.Web.Routing;

namespace Ovineware.CodeSamples.DapperDemo.CSharp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Managers",
                "Managers",
                new { controller = "Manager", action = "Index" }
            );

            routes.MapRoute(
                "AllProductsWithSubCategories",
                "Products/WithSubCategories",
                new { controller = "Product", action = "WithSubCategories" }
            );

            routes.MapRoute(
                "AllProducts",
                "Products",
                new { controller = "Product", action = "Index" }
            );

            routes.MapRoute(
                "AllSubCategories",
                "SubCategories",
                new { controller = "SubCategory", action = "Index" }
            );

            routes.MapRoute(
                "Image",
                "{controller}/{action}/{id}/{size}",
                new { controller = "Product", action = "Image" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Product", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}