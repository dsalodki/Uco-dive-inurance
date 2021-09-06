using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Uco.Infrastructure;
using Uco.Models;
using System.Data.Entity;
using Uco.Infrastructure.Repositories;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class TranslationController : BaseAdminController
    {
        public ActionResult Index(string lang)
        {
            ViewBag.MessageRed = TempData["MessageRed"];
            ViewBag.MessageYellow = TempData["MessageYellow"];
            ViewBag.MessageGreen = TempData["MessageGreen"];

            if (string.IsNullOrEmpty(lang)) ViewBag.Lang = SF.GetLangCodeThreading();
            else ViewBag.Lang = lang;

            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request, string Lang)
        {
            IQueryable<Translation> items = _db.Translations.Where(r => r.LangCode == Lang);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(Translation item, [DataSourceRequest]DataSourceRequest request, string Lang)
        {
            if (ModelState.IsValid)
            {
                _db.Translations.Remove(_db.Translations.First(r => r.ID == item.ID && r.LangCode == Lang));
                _db.SaveChanges();
                RP.CleanTranslationsRepository();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxInsert([DataSourceRequest] DataSourceRequest request, Translation item, string Lang)
        {
            if (ModelState.IsValid)
            {
                item.SystemName = item.SystemName.Trim();

                if (RP.GetTranslationsReprository().Count(r => r.SystemName == item.SystemName) > 0)
                {
                    ModelState.AddModelError("SystemName", RP.TC("SystemNameDublicate"));
                    return Json(new[] { item }.ToDataSourceResult(request, ModelState));
                }

                item.LangCode = Lang;
                _db.Translations.Add(item);
                _db.SaveChanges();

                RP.CleanTranslationsRepository();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxSave([DataSourceRequest] DataSourceRequest request, Translation item, string Lang)
        {
            if (ModelState.IsValid)
            {
                item.SystemName = item.SystemName.Trim();

                Translation NewItem = _db.Translations.Find(item.ID);

                if (NewItem.SystemName != item.SystemName && RP.GetTranslationsReprository().Count(r => r.SystemName == item.SystemName) > 0)
                {
                    ModelState.AddModelError("SystemName", RP.TC("SystemNameDublicate"));
                    return Json(new[] { item }.ToDataSourceResult(request, ModelState));
                }
                
                NewItem.LangCode = Lang;
                NewItem.SystemName = item.SystemName;
                NewItem.Text = item.Text;

                _db.Entry(NewItem).State = EntityState.Modified;
                _db.SaveChanges();

                RP.CleanTranslationsRepository();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
    }
}
