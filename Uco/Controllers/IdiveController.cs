using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Models;

namespace Uco.Controllers
{
    [Localization]
    public class IdiveController : BaseController
    {

        #region Search

        public ActionResult Search(string key, int? page)
        { 
            var items = LS.CurrentEntityContext.AbstractPages.AsNoTracking()
                .Where(x => x.Title.Contains(key) || x.ShortDescription.Contains(key) || x.Text.Contains(key)).ToList();
            //Paging
            Pagination paging = new Uco.Models.Pagination();
            paging.PageTotal = items.Count();
            paging.PageItems = 10;
            paging.param = key;
            paging.Url = Url.Content("/Idive/Search");
            if (page == null || page < 1) paging.PageNumber = 1;
            else paging.PageNumber = (int)page;
            ViewBag.Pagination = paging;
            var result = items.Skip((paging.PageNumber - 1) * paging.PageItems).Take(paging.PageItems).ToList();


            //
            //ViewBag.Title = CurrentPage.SeoTitle != null && CurrentPage.SeoTitle != "" ? CurrentPage.SeoTitle : CurrentPage.Title;
            //ViewBag.H1 = CurrentPage.SeoH1 != null && CurrentPage.SeoH1 != "" ? CurrentPage.SeoH1 : CurrentPage.Title;
            //ViewBag.Keywords = CurrentPage.SeoKywords;
            //ViewBag.Description = CurrentPage.SeoDescription;
            //ViewBag.ID = CurrentPage.ID;
            //ViewBag.Url = CurrentPage.Url;
            //ViewBag.LanguageRTL = CurrentSettings.LanguageRTL;
            //

            return View("SearchResultPage", result);
        }
        #endregion
    }
}