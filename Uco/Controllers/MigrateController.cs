using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Migrate;
using Uco.Models;

namespace Uco.Controllers
{
    public class MigrateController : BaseController
    {
        public ActionResult Index()
        {
           var content = MigrateLib.Migrate();
           return Content("<pre>"+content);
        }
    }
}
