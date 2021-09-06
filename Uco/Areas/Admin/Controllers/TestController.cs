using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Uco.Models;
using System.Data;
using System.IO;
using System.Text;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure;
using System.Data.Entity;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class TestController : BaseAdminController
    {
        public ActionResult Index()
        {
            foreach (Translation item in _db.Translations.ToList())
            {
                if (item.ID > 4797)
                {
                    _db.Entry(item).State = EntityState.Deleted;
                }
            }
            _db.SaveChanges();

            return Content("");
        }
    }
}
