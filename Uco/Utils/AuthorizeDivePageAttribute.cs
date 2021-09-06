using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Uco.Infrastructure.Livecycle;

namespace Uco.Utils
{
    public class AuthorizeDivePageAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                RedirectToRouteResult routeData = null;
                var returnUrl = string.Empty;

                if (filterContext.HttpContext.Request.Url != null)
                    returnUrl = filterContext.HttpContext.Request.Url.LocalPath;

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "DivePage", action = "Login", returnUrl }));
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}