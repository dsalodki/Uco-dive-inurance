using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Uco.Infrastructure
{
    public static class CssBundleHelper
    {
        public static HtmlString CssBundle(this HtmlHelper htmlHelper, params string[] Files)
        {
            string OutString = string.Empty;
            foreach (string item in Files)
            {
                OutString = string.Format("{0}\r\n<link href=\"{1}\" rel=\"stylesheet\" type=\"text/css\" />", OutString, item.Replace("~/", "/"));
            }
            return new HtmlString(OutString);
        }
    }
}
