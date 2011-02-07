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
using markashleybell.com.Web.Controllers;

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
                "", // URL with parameters
                new { controller = "Main", action = "Index" } // Parameter defaults
            );

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
                "article/{action}/{url}", // URL with parameters
                new { controller = "Article", action = "Index", url = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                null, // Route name
                "{*url}", // URL with parameters
                new { controller = "Main", action = "NotFoundRedirect" } // Parameter defaults
            );

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            var c = new HttpContextWrapper(Context);

            // Log the error here
            // ErrorLog.Logger.Log("NippyNormans", exception, c);

            if (Context.IsCustomErrorEnabled)
                ShowCustomErrorPage(exception);
        }

        private void ShowCustomErrorPage(Exception exception)
        {
            HttpException httpException = exception as HttpException;
            if (httpException == null)
                httpException = new HttpException(500, "Internal Server Error", exception);

            Response.Clear();

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("fromAppErrorEvent", true);

            var statusCode = httpException.GetHttpCode();

            switch (statusCode)
            {
                case 404:
                    routeData.Values.Add("action", "PageNotFound");
                    break;

                case 500:
                    routeData.Values.Add("action", "ServerError");
                    break;

                default:
                    routeData.Values.Add("action", "OtherHttpStatusCode");
                    routeData.Values.Add("httpStatusCode", httpException.GetHttpCode());
                    break;
            }

            Server.ClearError();

            var context = new RequestContext(new HttpContextWrapper(Context), routeData);
            context.HttpContext.Response.StatusCode = statusCode;

            IController controller = new ErrorController();
            controller.Execute(context);
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