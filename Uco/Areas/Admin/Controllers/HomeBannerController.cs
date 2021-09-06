using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Uco.Infrastructure;
using Uco.Models;
using System.Data;
using System.Data.Entity;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class HomeBannerController : BaseAdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<HomeBanner> items = _db.HomeBanners.Where(r => r.DomainID == AdminCurrentSettingsRepository.ID).OrderBy(x => x.Order);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(HomeBanner item, [DataSourceRequest]DataSourceRequest request)
        {
            if (ModelState.IsValid)
            {
                _db.HomeBanners.Remove(_db.HomeBanners.First(r => r.ID == item.ID && r.DomainID == AdminCurrentSettingsRepository.ID));
                _db.SaveChanges();
                CleanCache.CleanOutputCache();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }


        //NOT AJAX GRID

        public ActionResult Create()
        {
            return View(new HomeBanner() { });
        }

        [HttpPost]
        public ActionResult Create(HomeBanner item)
        {
            if (ModelState.IsValid)
            {
                _db.HomeBanners.Add(item);
                _db.SaveChanges();

                CleanCache.CleanCacheAfterPageEdit();

                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Edit(int ID)
        {
            return View(_db.HomeBanners.Find(ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, HomeBanner item)
        {
            if (ModelState.IsValid)
            {
                item.DomainID = AdminCurrentSettingsRepository.ID;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                CleanCache.CleanOutputCache();

                return RedirectToAction("Index");
            }
            return View(item);
        }
    }
}
