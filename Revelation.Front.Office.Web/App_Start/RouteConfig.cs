using System.Web.Mvc;
using System.Web.Routing;

namespace Revelation.Front.Office.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // HotellingSystem lookup tables: /Setup/agents, /Setup/market-codes, ...
            routes.MapRoute(
                name: "MasterData",
                url: "Setup/{key}",
                defaults: new { controller = "MasterData", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
