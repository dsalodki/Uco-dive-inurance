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
using System.Collections;
using Uco.Infrastructure.Livecycle;
using System.Globalization;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class BannerController : BaseAdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Banner> items = _db.Banners.Where(r => r.DomainID == AdminCurrentSettingsRepository.ID);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult getstatistics(int bannnerID, string startDate, string endDate)
        {
            var _startDate = DateTime.Parse(startDate, new CultureInfo("he-IL", true));
            var _endDate = DateTime.Parse(endDate, new CultureInfo("he-IL", true));
            int clicks = SF.CountBannersClicks(bannnerID, _startDate, _endDate);
            int views = SF.CountBannersViews(bannnerID, _startDate, _endDate);

            string result = "<div><b>כמות צפיות:</b> " + views + "</div>" + "<div><b>כמות כניסות:</b> " + clicks + "</div>";
            return Content(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(Banner item, [DataSourceRequest]DataSourceRequest request)
        {
            if (ModelState.IsValid)
            {
                _db.Banners.Remove(_db.Banners.First(r => r.ID == item.ID && r.DomainID == AdminCurrentSettingsRepository.ID));
                _db.SaveChanges();
                CleanCache.CleanOutputCache();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }


        //NOT AJAX GRID

        public ActionResult Create()
        {
            return View(new Banner() { ShowDateMax = DateTime.Now.AddYears(1), ShowDateMin = DateTime.Now.AddDays(-1) });
        }

        [HttpPost]
        public ActionResult Create(Banner item)
        {
            if (ModelState.IsValid)
            {
                _db.Banners.Add(item);
                _db.SaveChanges();

                CleanCache.CleanCacheAfterPageEdit();
                //CleanBannerCache();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Edit(int ID)
        {
            return View(_db.Banners.Find(ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, Banner item)
        {
            if (ModelState.IsValid)
            {
                item.DomainID = AdminCurrentSettingsRepository.ID;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                CleanCache.CleanOutputCache();
                //CleanBannerCache();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        void CleanBannerCache()
        {
            IDictionaryEnumerator enumerator = LS.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = (string)enumerator.Key;
                if (key.IndexOf("BannersReprository_") >= 0)
                    LS.Cache.Remove(key);
                //object value = enumerator.Value;
               
            }
        }
    }
}
