using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using System.ComponentModel;

namespace Uco.Models
{

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage" })]
    [RouteUrl("mgs")]
    [PageIcon("~/Areas/Admin/Content/pages/route_mg.png")]
    [PageName("MagazinesPage", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
    public class MagazinesPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "mgs"; } set { } }
    }

    [RestrictParents(new string[] { "MagazinesPage" })]
    [RouteUrl("mg")]
    [PageIcon("~/Areas/Admin/Content/pages/route_mg.png")]
    [PageName("MagazinePage", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
    public class MagazinePage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "mg"; } set { } }

        [Display(Name = "MagazineArticlesMapping", Order = 500, Prompt = "TabData")]
        [UIHint("MagazineArticleGroupMapping")]
        [NotMapped]
        public string MagazineArticleGroupMapping { get { return "MagazineArticleGroupMapping_" + this.ID; } }
    }

    [RestrictParents(new string[] { "MagazinePage" })]
    [RouteUrl("mga")]
    [PageIcon("~/Areas/Admin/Content/pages/route_mg.png")]
    [PageName("MagazineArticlePage", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
    public class MagazineArticlePage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "mga"; } set { } }

        [Display(Name = "Pictures", Order = 400, Prompt = "TabData")]
        [UIHint("Gallery")]
        public string Gallery { get { return "Gallery/gallery_" + this.ID; } }

        [UIHint("Image")]
        [Display(Name = "Pic", Order = 110, Prompt = "TabContent")]
        public override string Pic { get; set; }

        [Display(Name = "Author", Order = 108, Prompt = "TabContent")]
        public override string Author { get; set; }

        [Display(Name = "Text2", Order = 300, Prompt = "TabData")]
        [UIHint("Html")]
        [AllowHtml]
        public override string Text2 { get; set; }
    }

    //[RestrictParents(new string[] { "DomainPage" })]
    //[RouteUrl("inp")]
    //[PageIcon("~/Areas/Admin/Content/pages/route_ti.png")]
    //[PageName("InsurancePage", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
    //public class InsurancePage : AbstractPage
    //{
    //    [HiddenInput(DisplayValue = false)]
    //    public override string RouteUrl { get { return "inp"; } set { } }

    //    public InsurancePage()
    //    {

    //    }
    //}
}