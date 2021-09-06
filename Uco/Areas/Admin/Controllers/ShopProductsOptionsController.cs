using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Models;
using System.Data;
using Uco.Infrastructure;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data.Entity;

namespace Uco.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,DomainAdmin")]
    public class ShopProductsOptionsController : BaseAdminController
    {
        public ActionResult Index(int ParentID, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.ParentID = ParentID;
            return View();
        }

        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request, int ParentID)
        {
            IQueryable<ShopProductOption> items = _db.ShopProductOptions.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ShopParentID == ParentID).OrderBy(r => r.ID);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(ShopProductOption item, [DataSourceRequest]DataSourceRequest request, int ParentID)
        {
            _db.ShopProductOptions.Remove(_db.ShopProductOptions.First(r => r.ID == item.ID && r.ShopDomainID == AdminCurrentSettingsRepository.ID));
            _db.SaveChanges();

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Create(int ParentID, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View(new ShopProductOption() { ShopParentID = ParentID });
        }

        [HttpPost]
        public ActionResult Create(ShopProductOption ap, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                _db.ShopProductOptions.Add(ap);
                _db.SaveChanges();

                return RedirectToAction("Index", new { ParentID = ap.ShopParentID, ReturnUrl = ReturnUrl });
            }
            return View(ap);
        }

        public ActionResult Edit(int ID, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View(_db.ShopProductOptions.First(r => r.ID == ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, ShopProductOption ap, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                _db.Entry(ap).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Index", new { ParentID = ap.ShopParentID, ReturnUrl = ReturnUrl });
            }
            return View(ap);
        }

        public ActionResult Options(int ID, int ParentID, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.ParentID = ParentID;
            return View(ID);
        }

        public ActionResult _Item_AjaxIndex([DataSourceRequest]DataSourceRequest request, int ParentID)
        {
            ShopProductOption po = _db.ShopProductOptions.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ParentID);
            if (po == null)
            {
                //return View(new GridModel<ShopProductOptionItem> { Data = new List<ShopProductOptionItem>() });

                IQueryable<ShopProductOptionItem> items = new List<ShopProductOptionItem>().AsQueryable();
                DataSourceResult result = items.ToDataSourceResult(request);
                return Json(result);
            }
            else
            {  
                IQueryable<ShopProductOptionItem> items = po.ShopGetProductOptionItem().OrderBy(r => r.ShopOrder).AsQueryable();
                DataSourceResult result = items.ToDataSourceResult(request);
                return Json(result);
            }
        }

       
        public ActionResult _Item_AjaxInsert([DataSourceRequest] DataSourceRequest request, ShopProductOptionItem poi, int ParentID)
        {
            ShopProductOption po = _db.ShopProductOptions.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ParentID);
            if (po == null)
               return Json(new[] { new List<ShopProductOptionItem>() }.ToDataSourceResult(request, ModelState));

            List<ShopProductOptionItem> poil = po.ShopGetProductOptionItem().OrderBy(r => r.ShopOrder).ToList();

            if (poil.Count != 0)
            {
                poi.ID = poil.Max(r => r.ID) + 1;
                poi.ShopOrder = poil.Max(r => r.ShopOrder) + 1;
            }
            else
            {
                poi.ID = 1;
                poi.ShopOrder = 1;
            }
            poil.Add(poi);

            po.ShopSetProductOptionItem(poil);

            _db.Entry(po).State = EntityState.Modified;
            _db.SaveChanges();


            return Json(new[] { po.ShopGetProductOptionItem().OrderBy(r => r.ShopOrder) }.ToDataSourceResult(request, ModelState));
        }

        //[GridAction]
        public ActionResult _Item_AjaxSave([DataSourceRequest] DataSourceRequest request, ShopProductOptionItem poi, int ParentID)
        {
            ShopProductOption po = _db.ShopProductOptions.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ParentID);
            if (po == null)
                return Json(new[] { poi }.ToDataSourceResult(request, ModelState));

            List<ShopProductOptionItem> poil = po.ShopGetProductOptionItem().OrderBy(r => r.ShopOrder).ToList();

            poil.RemoveAll(r => r.ID == poi.ID);
            poi.ID = poi.ID;
            poil.Add(poi);

            po.ShopSetProductOptionItem(poil);

            _db.Entry(po).State = EntityState.Modified;
            _db.SaveChanges();

            //return View(new GridModel<ShopProductOptionItem>
            //{
            //    Data = po.ShopGetProductOptionItem().OrderBy(r => r.ShopOrder).ToList()
            //});
            return Json(new[] { poi }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult _Item_AjaxDelete([DataSourceRequest]DataSourceRequest request, ShopProductOptionItem poi, int ParentID)
        {
            ShopProductOption po = _db.ShopProductOptions.FirstOrDefault(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ParentID);
            if (po == null)
                return Json(new[] { poi }.ToDataSourceResult(request, ModelState));

            List<ShopProductOptionItem> poil = po.ShopGetProductOptionItem().OrderBy(r => r.ShopOrder).ToList();

            poil.RemoveAll(r => r.ID == poi.ID);

            po.ShopSetProductOptionItem(poil);

            _db.Entry(po).State = EntityState.Modified;
            _db.SaveChanges();

            return Json(new[] { poi }.ToDataSourceResult(request, ModelState));
        } 
    }
}
