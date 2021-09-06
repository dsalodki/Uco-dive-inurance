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

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class TextComponentController : BaseAdminController
    {
        public ActionResult Index()
        {
            ViewBag.MessageRed = TempData["MessageRed"];
            ViewBag.MessageYellow = TempData["MessageYellow"];
            ViewBag.MessageGreen = TempData["MessageGreen"];
            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TextComponent> items = _db.TextComponents.Where(r => r.DomainID == CurrentSettings.ID);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(TextComponent item, [DataSourceRequest]DataSourceRequest request)
        {
            if (ModelState.IsValid)
            {
                _db.TextComponents.Remove(_db.TextComponents.First(r => r.ID == item.ID && r.DomainID == CurrentSettings.ID));
                _db.SaveChanges();
                CleanCache.CleanOutputCache();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Create()
        {
            return View(new TextComponent());
        }

        [HttpPost]
        public ActionResult Create(TextComponent item)
        {
            if (ModelState.IsValid)
            {
                item.DomainID = CurrentSettings.ID;
                _db.TextComponents.Add(item);
                _db.SaveChanges();

                CleanCache.CleanOutputCache();

                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Edit(int ID)
        {
            return View(_db.TextComponents.First(r => r.ID == ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, TextComponent item)
        {
            if (ModelState.IsValid)
            {
                item.DomainID = CurrentSettings.ID;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                CleanCache.CleanOutputCache();

                return RedirectToAction("Index");
            }
            return View(item);
        }
    }
}
