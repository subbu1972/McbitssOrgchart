using REORGCHART.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.WebHost;

namespace REORGCHART
{
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {

            var httpControllerRouteHandler = typeof(HttpControllerRouteHandler).GetField("_instance",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            if (httpControllerRouteHandler != null)
            {
                httpControllerRouteHandler.SetValue(null,
                    new Lazy<HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
            }

            //Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            EnableCorsAttribute Cors= new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(Cors);
        }
    }
}