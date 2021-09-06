using System.Globalization;
using System.Web.Mvc;

namespace Uco.Infrastructure
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.Values["lang"] != null && !string.IsNullOrWhiteSpace(filterContext.RouteData.Values["lang"].ToString()))
            {
                CultureInfo culture = new CultureInfo(filterContext.RouteData.Values["lang"].ToString());
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}