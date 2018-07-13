using GIZ.API.Handlers;
using GIZ.API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Z.EntityFramework.Plus;

namespace GIZ.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Web API configuration and services
            AutoMapperConfig.Config();

            //
            EntityFilterConfig.Config();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //  Authentication handler
            config.MessageHandlers.Add(new AuthenticationHandler());

            //  Ensures a valid filter state
            config.Filters.Add(new Filters.UnhandledExceptionFilter());
            config.Filters.Add(new Filters.EnsureValidStateFilter());

            //  Remove the formatters
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //  Set database initializer
            Database.SetInitializer(new DbInitializer());


        }

    
    }
}
