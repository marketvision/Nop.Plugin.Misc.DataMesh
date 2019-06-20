using Microsoft.AspNetCore.Routing;
using Nop.Services.Authentication.External;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
    public class Oauth2RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            Route route = null;

            foreach (Route item in routeBuilder.Routes)
            {
                if (item.Name == "Login")
                {
                    route = item;
                    break;
                }
            }

            if (route != null)
                routeBuilder.Routes.Remove(route);

            routeBuilder.MapLocalizedRoute("Login",
                  "login/",
                  new { controller = "OAuth2Authentication", action = "Login" },
                  new { },
                  new[] { "Nop.Plugin.Misc.DataMesh.Controllers" }
              );
        }

        public int Priority => -1;
    }
}