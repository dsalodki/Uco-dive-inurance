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
    public class BaseController : Controller
    {

        public Db _db;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.QueryString["agentid"] != null && !string.IsNullOrWhiteSpace(filterContext.HttpContext.Request.QueryString["agentid"].ToString()))
            {
                SF.SetCookie("InsuranceAgent", filterContext.HttpContext.Request.QueryString["agentid"].ToString(), 1);
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _db = LS.CurrentEntityContext;
        }

        private Settings _CurrentSettings = null;
        public Settings CurrentSettings
        {
            get
            {
                if (_CurrentSettings != null) return _CurrentSettings;

                _CurrentSettings = RP.GetCurrentSettings();
                return _CurrentSettings;
            }
        }
    }
}