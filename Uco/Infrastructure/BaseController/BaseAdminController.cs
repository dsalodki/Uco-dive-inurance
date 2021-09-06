using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;

namespace Uco.Models
{
    public class BaseAdminController : BaseController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (requestContext.HttpContext.Session["LangSelectList"] != null)
            {
                string LangSelectList = requestContext.HttpContext.Session["LangSelectList"] as string;
                if (!string.IsNullOrEmpty(LangSelectList))
                {
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(LangSelectList);
                    System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
        }

        public User CurrentUser
        {
            get
            {
                return LS.CurrentUser;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AdminCurrentSettingsRepository == null)
            {
                filterContext.Result = new RedirectResult("~/Error.html");
                return;
            }

            string Plugin = string.Empty;
            if (filterContext.RouteData.DataTokens["Namespaces"] != null)
            {
                string[] Namespaces = filterContext.RouteData.DataTokens["Namespaces"] as string[];
                string Namespace = Namespaces[0];
                if (!string.IsNullOrEmpty(Namespace))
                {
                    //Uco.LongTailArticles.Areas.Admin.Controllers
                    string pattern = "Uco.*.Areas.*.Controllers";

                    if (Regex.IsMatch(Namespace, pattern))
                    {
                        int RemoveStart = Namespace.IndexOf(".Areas");
                        int RemoveCount = Namespace.Count() - RemoveStart;

                        Namespace = Namespace.Remove(RemoveStart, RemoveCount);
                        filterContext.RouteData.Values.Add("plugin", Namespace);
                        Plugin = Namespace;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private Settings _AdminCurrentSettingsRepository = null;
        public Settings AdminCurrentSettingsRepository
        {
            get
            {
                if (_AdminCurrentSettingsRepository != null) return _AdminCurrentSettingsRepository;
                _AdminCurrentSettingsRepository = RP.GetAdminCurrentSettingsRepository();
                return _AdminCurrentSettingsRepository;
            }
        }
    }
}