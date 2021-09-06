using System;
using System.Linq;
using System.Web.Mvc;
using Uco.Models;
using Uco.Infrastructure;
using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Uco.Infrastructure.Repositories;

namespace Uco.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,DomainAdmin")]
    public class ShopProductsController : BaseAdminController
    {
        public ActionResult Index(string SearchBox)
        {
            ViewData["SearchBox"] = SearchBox ?? string.Empty;
            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request, string SearchBox)
        {
            if (!string.IsNullOrEmpty(SearchBox))
            {
                IQueryable<ShopProductPage> items = _db.ShopProductPages.Where(r => r.DomainID == AdminCurrentSettingsRepository.ID && (r.Title.Contains(SearchBox) || r.ShopSKU.Contains(SearchBox)));
                DataSourceResult result = items.ToDataSourceResult(request);
                return Json(result);
            }
            else
            {               
                IQueryable<ShopProductPage> items = _db.ShopProductPages.Where(r => r.DomainID == AdminCurrentSettingsRepository.ID).OrderBy(r => r.ID);
                DataSourceResult result = items.ToDataSourceResult(request);
                return Json(result);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(ShopProductPage item, [DataSourceRequest]DataSourceRequest request)
        {
            _db.ShopProductPages.Remove(_db.ShopProductPages.First(r => r.ID == item.ID && r.DomainID == AdminCurrentSettingsRepository.ID));
            _db.SaveChanges();

            CleanCache.CleanOutputCache();
           
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Create()
        {
            return View(new ShopProductPage() { ParentID = SF.ShopGetProductAllPage().ID });
        }

        [HttpPost]
        public ActionResult Create(ShopProductPage ap)
        {
            if (ModelState.IsValid)
            {
                ap.ParentID = SF.ShopGetProductAllPage().ID;
                ap.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
                if (ap.SeoUrlName == null || ap.SeoUrlName == "") ap.SeoUrlName = Uco.Infrastructure.SF.CleanUrl(ap.Title);
                else ap.SeoUrlName = Uco.Infrastructure.SF.CleanUrl(ap.SeoUrlName);

                if (_db.AbstractPages.Count(r => r.DomainID == ap.DomainID && r.SeoUrlName == ap.SeoUrlName) != 0)
                {
                    ap.SeoUrlName = (_db.AbstractPages.Max(r => r.ID) + 1).ToString();
                }
                ap.CreateTime = DateTime.Now;
                if (_db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Count() == 0) ap.Order = 1;
                else ap.Order = _db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Max(r => r.Order) + 1;

                _db.ShopProductPages.Add(ap);
                _db.SaveChanges();

                CleanCache.CleanOutputCache();

                return RedirectToAction("Index");
            }
            return View(ap);
        }

        public ActionResult Edit(int ID)
        {
            return View(_db.ShopProductPages.First(r => r.ID == ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, ShopProductPage ap)
        {
            if (ModelState.IsValid)
            {
                ap.SeoUrlName = Uco.Infrastructure.SF.CleanUrl(ap.SeoUrlName);
                if (_db.AbstractPages.Count(r => r.ID != ap.ID && r.DomainID == ap.DomainID && r.SeoUrlName == ap.SeoUrlName) != 0)
                {
                    ModelState.AddModelError("SeoUrlName", "ShopUrl Name כבר קיים. שנה ShopUrl Name בלשונית קידום");
                    return View(ap);
                }
                ap.ChangeTime = DateTime.Now;
                _db.Entry(ap).State = EntityState.Modified;
                _db.SaveChanges();

                CleanCache.CleanOutputCache();

                return RedirectToAction("Index");
            }
            return View(ap);
        }
    }
}
