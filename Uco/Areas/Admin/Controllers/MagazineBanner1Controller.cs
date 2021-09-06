using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Models;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class MagazineBanner1Controller : BaseAdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MagazineBanner1> items = _db.MagazineBanners1.Where(r => r.DomainID == AdminCurrentSettingsRepository.ID);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(Banner item, [DataSourceRequest]DataSourceRequest request)
        {
            if (ModelState.IsValid)
            {
                _db.MagazineBanners1.Remove(_db.MagazineBanners1.First(r => r.ID == item.ID && r.DomainID == AdminCurrentSettingsRepository.ID));
                _db.SaveChanges();
                CleanBannerCache();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }


        //NOT AJAX GRID

        public ActionResult Create()
        {
            return View(new MagazineBanner1());
        }

        [HttpPost]
        public ActionResult Create(MagazineBanner1 item)
        {
            if (ModelState.IsValid)
            {
                _db.MagazineBanners1.Add(item);
                _db.SaveChanges();

                CleanBannerCache();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Edit(int ID)
        {
            return View(_db.MagazineBanners1.Find(ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, MagazineBanner1 item)
        {
            if (ModelState.IsValid)
            {
                item.DomainID = AdminCurrentSettingsRepository.ID;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                CleanBannerCache();
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
                if (key.IndexOf("MagazineBanners1Reprository_") >= 0)
                    LS.Cache.Remove(key);
            }
        }
    }
}