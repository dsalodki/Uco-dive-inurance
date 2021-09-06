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
    public class MagazineBanner2Controller : BaseAdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MagazineBanner2> items = _db.MagazineBanners2.Where(r => r.DomainID == AdminCurrentSettingsRepository.ID);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(Banner item, [DataSourceRequest]DataSourceRequest request)
        {
            if (ModelState.IsValid)
            {
                _db.MagazineBanners2.Remove(_db.MagazineBanners2.First(r => r.ID == item.ID && r.DomainID == AdminCurrentSettingsRepository.ID));
                _db.SaveChanges();
                CleanCache.CleanOutputCache();
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }


        //NOT AJAX GRID

        public ActionResult Create()
        {
            return View(new MagazineBanner2());
        }

        [HttpPost]
        public ActionResult Create(MagazineBanner2 item)
        {
            if (ModelState.IsValid)
            {
                _db.MagazineBanners2.Add(item);
                _db.SaveChanges();

                CleanBannerCache();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Edit(int ID)
        {
            return View(_db.MagazineBanners2.Find(ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, MagazineBanner2 item)
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
                if (key.IndexOf("MagazineBanners2Reprository_") >= 0)
                    LS.Cache.Remove(key);

            }
        }
    }
}