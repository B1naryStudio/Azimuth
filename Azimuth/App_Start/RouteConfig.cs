using System.Web.Mvc;
using System.Web.Routing;

namespace Azimuth
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SearchRoute",
                url: "Search/{action}/{searchParam}",
                defaults: new { action = "Index", searchParam = UrlParameter.Optional }
            );
        }
    }
}
