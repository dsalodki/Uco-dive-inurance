using System;
using System.Linq;
using System.Web.Mvc;
using Uco.Models;
using Uco.Infrastructure;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using Uco.Infrastructure.Repositories;
using System.Collections.Generic;
namespace Uco.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,DomainAdmin")]
    public class ShopOrderController : BaseAdminController
    {
        public ActionResult Index(string m)
        {
            ViewBag.M = m;
            return View();
        }
        public ActionResult _AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {           
            IQueryable<ShopOrder> items = _db.ShopOrders.Where(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID).OrderByDescending(r => r.ID);
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _AjaxDelete(ShopOrder item, [DataSourceRequest]DataSourceRequest request)
        {
            _db.ShopOrders.Remove(_db.ShopOrders.First(r => r.ID == item.ID && r.ShopDomainID == AdminCurrentSettingsRepository.ID));
            _db.SaveChanges();

            CleanCache.CleanOutputCache();

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Edit(int ID)
        {
            ViewBag.CurentSettings = AdminCurrentSettingsRepository;
            return View(_db.ShopOrders.First(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ID));
        }

        [HttpPost]
        public ActionResult Edit(int ID, ShopOrder tc)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tc).State = EntityState.Modified;
                _db.SaveChanges();

                CleanCache.CleanOutputCache();

                return RedirectToAction("Index");
            }
            ViewBag.CurentSettings = AdminCurrentSettingsRepository;
            return View(tc);
        }

        public ActionResult _AjaxOrderIndex([DataSourceRequest]DataSourceRequest request, int ID)
        {
            ShopOrder o = _db.ShopOrders.First(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == ID);
            List<ShopCartItem> l = o.ShopGetDataFromXML<ShopCartItem>();

            //return View(new GridModel<ShopCartItem>
            //{
            //    Data = l
            //});
            IQueryable<ShopCartItem> items = o.ShopGetDataFromXML<ShopCartItem>().AsQueryable();
            DataSourceResult result = items.ToDataSourceResult(request);
            return Json(result);
        }

        //public ActionResult EditCartItem(int ID, int OrderID)
        //{
        //    ViewBag.OrderID = OrderID;
        //    ShopOrder o = _db.ShopOrders.First(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == OrderID);
        //    List<ShopCartItem> cil = o.ShopGetDataFromXML<ShopCartItem>();
        //    return View(cil.FirstOrDefault(r => r.ID == ID));
        //}

        //[HttpPost]
        //public ActionResult EditCartItem(int ID, int OrderID, ShopCartItem tc)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ShopOrder o = _db.ShopOrders.First(r => r.ShopDomainID == AdminCurrentSettingsRepository.ID && r.ID == OrderID);
        //        List<ShopCartItem> cil = o.ShopGetDataFromXML<ShopCartItem>();
        //        ShopCartItem ci = cil.FirstOrDefault(r => r.ID == ID);
        //        ci = tc;
        //        o.ShopSetDataToXML<ShopCartItem>(cil);
        //        _db.Entry(o).State = EntityState.Modified;
        //        _db.SaveChanges();

        //        return RedirectToAction("Edit", new { ID = OrderID });
        //    }
        //    else
        //    {
        //        return View(tc);
        //    }
        //}


        public ActionResult SendPayData(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == ID);
            if (o == null) return Content("can't find order");
            if (o.ShopStatus == ShopOrderStatusEnum.Payed) return Redirect("/Admin/ShopOrder/Index?m=" + Server.UrlEncode("לא ניתן. הזמנה שולמה"));
            //return Redirect("/Admin/ShopOrder/Index?m=" + Server.UrlEncode(SF.SendOrderPlaced(o)));
            return Content(string.Empty);
        }

        public ActionResult SendInvoice(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == ID);
            if (o == null) return Content("can't find order");
            if (o.ShopStatus == ShopOrderStatusEnum.Placed || o.ShopStatus == ShopOrderStatusEnum.Canceled) return Redirect("/Admin/ShopOrder/Index?m=" + Server.UrlEncode("לא ניתן. הזמנה לא שולמה"));
            return Redirect("/Admin/ShopOrder/Index?m=" + Server.UrlEncode(SF.SendInvoice(o)));
        }
    }
}
