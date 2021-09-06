using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Models;

namespace Uco.Controllers
{
    public partial class PageController : BasePageController
    {
        #region Pages

        public ActionResult ShopCategoryAllPage(string name, string xml, int? page)
        {
            if (xml == "1")
            {
                ViewBag.Items = _db.ShopCategoryPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible).OrderBy(r => r.Order).ToList();
                return View("CategoryAllPageZap", CurrentPage);
            }
            else
            {
                var Items = _db.ShopCategoryPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ParentID == CurrentPage.ID).OrderBy(r => r.Order).ToList();
                //Paging
                Pagination paging = new Uco.Models.Pagination();
                paging.PageTotal = Items.Count();
                paging.PageItems = 6;
                paging.Url = Url.Content(CurrentPage.Url);
                if (page == null || page < 1) paging.PageNumber = 1;
                else paging.PageNumber = (int)page;
                ViewBag.Pagination = paging;
                ViewBag.Items = Items.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();
               
                return View(CurrentPage);
            }
        }

        public ActionResult ShopCategoryPage(string name, string xml, int? page)
        {
            string toSearch = "," + CurrentPage.ID + ",";

            if (xml == "1")
            {
                HttpContext.Response.Clear();
                string xmlString = "";

                xmlString = xmlString + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
                xmlString = xmlString + "<STORE url=\"http://" + Request.ServerVariables["HTTP_HOST"] + "\" date=\"" + DateTime.Now.Date + "\" name=\"\" status=\"ONLINE\" id=\"\">\r\n";
                xmlString = xmlString + "<PRODUCTS>\r\n";

                foreach (ShopProductPage item in _db.ShopProductPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible == true && r.ShowInMenu == true && r.ShopIsVisibleOnMirror == true && (r.ShopCategoryIDs.Contains(toSearch)))
                    .OrderBy(r => r.Order)
                    .ToList())
                {
                    xmlString = xmlString + "<PRODUCT>\r\n";
                    xmlString = xmlString + "<PRODUCT_NAME>" + item.Title + "</PRODUCT_NAME>\r\n";
                    xmlString = xmlString + "<DETAILS>" + item.ShortDescription + "</DETAILS>\r\n";
                    xmlString = xmlString + "<PRODUCT_URL>http://" + Request.ServerVariables["HTTP_HOST"] + "/" + item.RouteUrl + "/" + item.SeoUrlName + "</PRODUCT_URL>\r\n";
                    xmlString = xmlString + "<PRICE>" + item.ShopPrice + "</PRICE>\r\n";
                    xmlString = xmlString + "<CATALOG_NUMBER>" + item.ShopSKU + "</CATALOG_NUMBER>\r\n";
                    xmlString = xmlString + "<IMAGE>http://" + Request.ServerVariables["HTTP_HOST"] + Url.Content(item.Pic) + "</IMAGE>\r\n";
                    xmlString = xmlString + "<CURRENCY>ILS</CURRENCY>\r\n";
                    xmlString = xmlString + "<SHIPMENT_COST>" + (CurrentSettings.ShopShippingPrice + item.ShopAddShippingPrice) + " </SHIPMENT_COST>\r\n";
                    xmlString = xmlString + "<DELIVERY_TIME>" + CurrentSettings.ShopShippingDays + "</DELIVERY_TIME>\r\n";
                    xmlString = xmlString + "<MANUFACTURER/>\r\n";
                    xmlString = xmlString + "<WARRANTY>http://" + Request.ServerVariables["HTTP_HOST"] + "/c/warranty</WARRANTY>\r\n";
                    xmlString = xmlString + "<PRODUCT_TYPE>מוצר חדש</PRODUCT_TYPE>\r\n";
                    xmlString = xmlString + "<MODEL/>\r\n";
                    xmlString = xmlString + "</PRODUCT>\r\n";
                }

                xmlString = xmlString + "</PRODUCTS>\r\n";
                xmlString = xmlString + "</STORE>\r\n";

                return Content(xmlString, "text/xml");
            }

            ViewBag.Items1 = _db.ShopCategoryPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ParentID == CurrentPage.ID).OrderBy(r => r.Order).ToList();

            List<ShopProductPage> l = _db.ShopProductPages
                .Where(r => r.DomainID == CurrentSettings.ID && r.Visible == true && (r.ShopCategoryIDs.Contains(toSearch)))
                .OrderBy(r => r.Order)
                .ToList();

            //Paging
            Pagination paging = new Uco.Models.Pagination();
            paging.PageTotal = l.Count();
            paging.PageItems = 6;
            paging.Url = Url.Content(CurrentPage.Url);
            if (page == null || page < 1) paging.PageNumber = 1;
            else paging.PageNumber = (int)page;
            ViewBag.Pagination = paging;
            ViewBag.Items2 = l.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();

            return View(CurrentPage);
        }

        public ActionResult ShopBrandAllPage(string name, int? page)
        {
            var Items = _db.ShopBrandPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ParentID == CurrentPage.ID).OrderBy(r => r.Order).ToList();

            //Paging
            Pagination paging = new Uco.Models.Pagination();
            paging.PageTotal = Items.Count();
            paging.PageItems = 6;
            paging.Url = Url.Content(CurrentPage.Url);
            if (page == null || page < 1) paging.PageNumber = 1;
            else paging.PageNumber = (int)page;
            ViewBag.Pagination = paging;
            ViewBag.Items = Items.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();
            return View(CurrentPage);
        }

        public ActionResult ShopBrandPage(string name, string xml)
        {
            ViewBag.Items = _db.ShopProductPages
                .Where(r => r.DomainID == CurrentSettings.ID && r.Visible == true && r.ShowInMenu == true && (r.ShopBrand == CurrentPage.Title))
                .OrderBy(r => r.Order)
                .ToList();

            return View(CurrentPage);
        }

        public ActionResult ShopProductPage(string name)
        {
            ShopProductPage CurrentProduct = (ShopProductPage)CurrentPage;
            List<string> RelatedProducts = new List<string>();
            if (!string.IsNullOrEmpty(CurrentProduct.ShopRelatedProducts))
            {
                string[] RelatedProductsArray = CurrentProduct.ShopRelatedProducts.Split(',');
                foreach (string item in RelatedProductsArray)
                {
                    if (!string.IsNullOrEmpty(item.Trim())) RelatedProducts.Add(item.Trim());
                }
            }
            ViewBag.Items1 = _db.ShopProductPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ID != CurrentPage.ID && RelatedProducts.Contains(r.Title)).ToList();
            return View(CurrentPage);
        }

        public ActionResult ShopProductSalePage(string name)
        {
            ViewBag.Items = _db.ShopProductPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ShopShowInSale).OrderBy(r => r.Order).ToList();
            return View(CurrentPage);
        }

        public ActionResult ShopProductAllPage(string name)
        {
            ViewBag.Items = _db.ShopProductPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ParentID == CurrentPage.ID).OrderBy(r => r.Order).ToList();
            return View(CurrentPage);
        }

        #endregion
	}
}