using System.Web.Mvc;
using Uco.Models;
using Uco.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Uco.Infrastructure.Repositories;
using System;
using Uco.Infrastructure.Logger;

namespace Uco.Controllers
{
    public partial class PagePartController : BaseController
    {
        #region ChildAction

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _HomeProducts()
        {
            var data = _db.AbstractPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ShowInNewOnShelfHomepage)
                .OrderBy(r => r.Order).Take(3).ToList();
            return View(data);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _HomeProducts2()
        {
            var data = _db.AbstractPages.Where(r => r.DomainID == CurrentSettings.ID && r.Visible && r.ShowInNewOnShelfHomepage2)
                .OrderBy(r => r.Order).Take(3).ToList();
            return View(data);
        }


        [OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _NewsBox()
        {
            Logger.Information("News box action method entered", fileName: "idive.co.il_action_methods.txt");
            List<AbstractPage> pages = _db.AbstractPages.Where(r => r.Visible == true && r.ShowInNewsHomepage).OrderBy(r => r.Order).ToList();
            Logger.Information("News box pages count: " + pages.Count.ToString(), fileName: "idive.co.il_action_methods.txt");
            return PartialView("_NewsBox", pages);
        }


        //[OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _ArticleBox()
        {
            return PartialView(_db.AbstractPages.Where(r => r.Visible == true && r.ShowInArticlesHomepage).OrderBy(r => r.Order).Take(3).ToList());
        }
        
        //[OutputCache(Duration = 3600, VaryByCustom = "LangCodeDomainAndCleanCacheGuid")]
        public ActionResult _Gallerys()
        {
            return PartialView(_db.AbstractPages.Where(r => r.Visible == true && r.ShowInGalleryHomepage).OrderBy(r => r.Order).ToList());
        }

        public ActionResult Bannerlink(int id)
        {
            SF.BannersAddClick(id);
            var link = _db.Banners.First(b => b.ID == id).Link;
            return Redirect(link);
        }
        


        [ChildActionOnly]
        public ActionResult _Banner(int Num, string BannerGroup,int? pageId)
        {


            var banners = RP.GetBannersReprository(BannerGroup).Where(x => x.Visible).ToList();
            var resultBanners=new List<Banner>();
            if (pageId.HasValue)
            {
                foreach (var banner in banners)
                {
                         
                    if (string.IsNullOrEmpty(banner.ShowOnPageIds) || banner.ShowOnPageIds.Contains("," + pageId.Value.ToString() + ","))
                        resultBanners.Add(banner);
                }
            }else
                resultBanners=banners;
            

            resultBanners = resultBanners.OrderBy(r => r.ID).Take(Num).ToList();
            
            // add banners Views
            foreach (var item in resultBanners)
            {
                SF.BannersAddView(item.ID);
            }

            return View(resultBanners);
        }

        public ActionResult MagazineBanner2link(int id)
        {
            var link = _db.MagazineBanners2.First(b => b.ID == id).Link;
            return Redirect(link);
        }

        [ChildActionOnly]
        public ActionResult _MagazineBanner2(int Num, int? pageId)
        {
            var banners = RP.GetMagazineBanners2Reprository().Where(x => x.Visible).ToList();
            var resultBanners = new List<MagazineBanner2>();
            if (pageId.HasValue)
            {
                foreach (var banner in banners)
                {

                    if (string.IsNullOrEmpty(banner.ShowOnPageIds) || banner.ShowOnPageIds.Contains("," + pageId.Value.ToString() + ","))
                        resultBanners.Add(banner);
                }
            }
            else
                resultBanners = banners;


            resultBanners = resultBanners.OrderBy(r => r.ID).Take(Num).ToList();

            return View(resultBanners);
        }

        [ChildActionOnly]
        public ActionResult _MagazineBanner1(int Num, int? pageId)
        {
            var banners = RP.GetMagazineBanners1Reprository().Where(x => x.Visible).ToList();
            var resultBanners = new List<MagazineBanner1>();
            if (pageId.HasValue)
            {
                foreach (var banner in banners)
                {

                    if (string.IsNullOrEmpty(banner.ShowOnPageIds) || banner.ShowOnPageIds.Contains("," + pageId.Value.ToString() + ","))
                        resultBanners.Add(banner);
                }
            }
            else
                resultBanners = banners;


            resultBanners = resultBanners.OrderBy(r => r.ID).Take(Num).ToList();

            return View(resultBanners);
        }

        public ActionResult _HomeBanner()
        {
            var data = _db.HomeBanners.OrderBy(r => r.Order).ToList();

            return PartialView(data);
        }

        #endregion

        #region Ajax


        #endregion

        #region Cache


        #endregion
    }
}
