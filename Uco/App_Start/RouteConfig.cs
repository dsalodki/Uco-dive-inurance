using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });

            routes.MapRoute("ImagesThumbnail", "Image", new { controller = "Images", action = "GetImage" });
            routes.MapRoute("Captcha", "Image", new { controller = "Captcha", action = "Get" });

            

                //main site page
            routes.MapRoute(
               name: "ShorterOrder",
               url: "ShopOrder/{id}",
               defaults: new { controller = "Insurance", action = "LinkShortOrder" },
               namespaces: new string[] { "Uco.Controllers" }
               );

            //main site page
            routes.MapRoute(
               name: "HomePage",
               url: "",
               defaults: new { controller = "Page", action = "DomainPage", name = "root" },
               namespaces: new string[] { "Uco.Controllers" }
           );

            //Uco.dll pages
            foreach (Type item in RP.GetPageTypesReprository())
            {
                routes.MapRoute(
                   name: item.Name,
                   url: SF.GetTypeRouteUrl(item) + "/{name}",
                   defaults: new { controller = "Page", action = item.Name },
                   namespaces: new string[] { "Uco.Controllers" }
               );

                routes.MapRoute(
                   name: item.Name + "_Lang",
                   url: "{lang}/" + SF.GetTypeRouteUrl(item) + "/{name}",
                   defaults: new { controller = "Page", action = item.Name },
                   namespaces: new string[] { "Uco.Controllers" }
                );
            }

            //if (SF.UsePlugins())
            //{

                //plugin dll pages
                //foreach (Type item in RP.GetPlugingsAbstractPageChildClasses())
                //{
                //    string PageTypeName = item.Name;
                //    string PluginNamespace = item.Namespace.Replace(".Models", "");

                //    routes.MapRoute(
                //       name: PageTypeName,
                //       url: SF.GetTypeRouteUrl(item) + "/{name}",
                //       defaults: new { controller = "Page", action = PageTypeName },
                //       namespaces: new string[] { PluginNamespace + ".Controllers" }
                //    );

                //    routes.MapRoute(
                //       name: PageTypeName + "_Lang",
                //       url: "{lang}/" + SF.GetTypeRouteUrl(item) + "/{name}",
                //       defaults: new { controller = "Page", action = PageTypeName },
                //       namespaces: new string[] { PluginNamespace + ".Controllers" }
                //    );
                //}

                ////plugin dll data
                //foreach (string item in RP.GetPluginsReprository())
                //{
                //    string Namespace = item + ".Controllers";
                //    routes.MapRoute(
                //       name: Namespace,
                //       url: item + "/{controller}/{action}/{id}",
                //       defaults: new { id = UrlParameter.Optional },
                //       namespaces: new string[] { Namespace }
                //    );

                //    routes.MapRoute(
                //       name: "lang_" + Namespace,
                //       url: "{lang}/" + item + "/{controller}/{action}/{id}",
                //       defaults: new { id = UrlParameter.Optional },
                //       namespaces: new string[] { Namespace }
                //    );
                //}
            //}

            //Uco.dll data
            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { id = UrlParameter.Optional },
               namespaces: new string[] { "Uco.Controllers" }
            );

            routes.MapRoute(
               name: "Default_Lang",
               url: "{lang}/{controller}/{action}/{id}",
               defaults: new { id = UrlParameter.Optional },
               namespaces: new string[] { "Uco.Controllers" }
            );
        }
    }
}
