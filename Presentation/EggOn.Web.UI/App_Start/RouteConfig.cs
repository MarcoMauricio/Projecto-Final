using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FlowOptions.EggOn.WebApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            if (HttpContext.Current.IsDebuggingEnabled)
            {
                // Routes module asset requests in DEBUG.
                // Preprocessor conditional isn't used since we want to be able to change this in web.config.
                routes.MapRoute(
                    name: "ModuleAsset",
                    url: "Modules/{*path}",
                    defaults: new { controller = "EggOn", action = "GetModuleAsset" }
                );
            }

            routes.MapRoute(
                name: "Default",
                url: "{*catchall}",
                defaults: new { controller = "EggOn", action = "GetDefaultPage", catchall = UrlParameter.Optional }
            );
        }
    }
}