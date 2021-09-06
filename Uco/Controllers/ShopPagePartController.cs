using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco.Controllers
{
    public class ShopPagePartController : PagePartController
    {
        #region PagePart

        #region ChildAction

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid", VaryByParam = "id")]
        public ActionResult PrevAndNextOfProductPage(int id)
        {
            var currentItem = _db.ShopProductPages.Find(id);
            var parent = _db.ShopProductAllPages.Find(currentItem.ParentID);
            var childItems = _db.ShopProductPages.Where(s => s.ParentID == parent.ID && s.Visible).OrderBy(r => r.Order).ToList();

            ShopProductPage _PrevProduct;
            ShopProductPage _NextProduct;
            int counter = 0;

            foreach (var item in childItems)
            {
                if (currentItem.ID == item.ID)
                    break;
                else
                    counter++;
            }

            // prev
            if (counter > 0)
                _PrevProduct = childItems[counter - 1];
            else
                // the item is in the first place so prev is the last one
                _PrevProduct = childItems[childItems.Count - 1];


            // next
            if (counter + 1 < childItems.Count)
                _NextProduct = childItems[counter + 1];
            else
                // the item is in the last place so next is the first one
                _NextProduct = childItems[0];

            ViewBag.PrevProduct = _PrevProduct;
            ViewBag.NextProduct = _NextProduct;

            return View();
        }


        [ChildActionOnly]
        public ActionResult ProductSpecials(int id)
        {
            return View(_db.ShopProductPages.Where(p => p.Visible && p.ShowInProductsSpecials && p.ID != id).OrderBy(p => Guid.NewGuid()).Take(2).ToList());
        }

        [ChildActionOnly]
        public ActionResult _CategoryAllMenu(int CurentPageID)
        {
            CustomMenuItem CategoryAll = RP.GetDisplayMenuRepository().FirstOrDefault(r => r.RouteUrl == "ktl");
            if (CategoryAll == null) return View(new List<ShopCategoryPage>());
            ViewBag.CategoryAllID = CategoryAll.ID;

            CustomMenuItem CurentPage = RP.GetDisplayMenuRepository().FirstOrDefault(r => r.ID == CurentPageID);
            if (CurentPage == null) ViewBag.ParentPageID = 0;
            else
            {
                ViewBag.ParentPageID = CurentPage.ParentID;

                CustomMenuItem ParentPage = RP.GetDisplayMenuRepository().FirstOrDefault(r => r.ID == CurentPage.ParentID);
                if (ParentPage == null) ViewBag.GrandParentPageID = 0;
                else ViewBag.GrandParentPageID = ParentPage.ParentID;
            }

            ViewBag.CurentPageID = CurentPageID;

            return View(_db.ShopCategoryPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible).OrderBy(r => r.Order).ToList());
        }

        [ChildActionOnly]
        public ActionResult _BrandAllMenu(int CurentPageID)
        {
            CustomMenuItem parent = RP.GetDisplayMenuRepository().FirstOrDefault(r => r.RouteUrl == "brl");
            if (parent == null) return View(new List<ShopBrandPage>());
            ViewBag.ParentID = parent.ID;
            return View(_db.ShopBrandPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ParentID == parent.ID).OrderBy(r => r.Order).ToList());
        }

        [OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _NewProducts()
        { 
            
            return PartialView(_db.ShopProductPages.Where(r => r.Visible == true && r.ShowInProductsHomepage == true).OrderBy(r => r.Order).Take(4).ToList());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _HomeCategory()
        {
            return View(_db.ShopCategoryPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ShopShowInMainPage == true).OrderBy(r => r.Order).ToList());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _HomeBrands()
        {
            return View(_db.ShopBrandPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ShopShowInMainPage == true).OrderBy(r => r.Order).ToList());
        }

        public ActionResult _NextProduct(int ID)
        {
            ShopProductPage p = _db.ShopProductPages.OrderBy(r => r.ID).FirstOrDefault(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ID > ID);
            if (p != null) return Redirect(p.Url);
            else return Redirect("~/");
        }

        public ActionResult _PrevProduct(int ID)
        {
            ShopProductPage p = _db.ShopProductPages.OrderByDescending(r => r.ID).FirstOrDefault(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ID < ID);
            if (p != null) return Redirect(p.Url);
            else return Redirect("~/");
        }

        [ChildActionOnly]
        public ActionResult _SaleProduct(int ID)
        {
            ShopProductPage p = _db.ShopProductPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ShopShowInSale && r.ID != ID).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
            if (p != null) return View(p);
            else return Content("");
        }

        #endregion

        #region Ajax

        [HttpPost]
        public ActionResult _AjaxGrid(int GridPageID, int GridPageNum, int GridCount, int GridShow, int GridOrder, int GridType)
        {
            ViewData["GridPageID"] = GridPageID;
            ViewData["GridPageNum"] = GridPageNum;
            ViewData["GridShow"] = GridShow;
            ViewData["GridOrder"] = GridOrder;
            ViewData["GridType"] = GridType;
            ViewData["GridCount"] = GridCount;

            string toSearch = "," + GridPageID + ",";

            var l = _db.ShopProductPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible == true && r.ShowInMenu == true && (r.ShopCategoryIDs.Contains(toSearch)));

            if (GridOrder == 3) return View("_ProductGrid1", l.OrderBy(r => r.Title).Skip((GridPageNum - 1) * GridShow).Take(GridShow).ToList());
            else if (GridOrder == 2) return View("_ProductGrid1", l.OrderBy(r => r.ShopPrice).Skip((GridPageNum - 1) * GridShow).Take(GridShow).ToList());
            else return View("_ProductGrid1", l.OrderBy(r => r.Order).Skip((GridPageNum - 1) * GridShow).Take(GridShow).ToList());
        }

        #endregion

        #region Cache


        #endregion
        #endregion
    }
}