using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using markashleybell.com.Web.Infrastructure;
using markashleybell.com.Domain.Concrete;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Database;

namespace markashleybell.com.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                null, // Route name
                "about", // URL with parameters
                new { controller = "Main", action = "About" } // Parameter defaults
            );

            routes.MapRoute(
                null, // Route name
                "jquery", // URL with parameters
                new { controller = "Main", action = "JQuery" } // Parameter defaults
            );

            routes.MapRoute(
                null, // Route name
                "sitemap-xml", // URL with parameters
                new { controller = "Main", action = "SiteMapXml" } // Parameter defaults
            );

            routes.MapRoute(
                null, // Route name
                "articles", // URL with parameters
                new { controller = "Article", action = "Index" } // Parameter defaults
            );

            routes.MapRoute(
                null, // Route name
                "articles/{url}", // URL with parameters
                new { controller = "Article", action = "Article", url = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                null, // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Main", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            DbDatabase.SetInitializer<Db>(new DbInitializer());

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            AutoMapperConfiguration.Configure();
        }
    }
}