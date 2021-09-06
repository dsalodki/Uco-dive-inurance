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

    #region Shop pages

    [RestrictParents(new string[] { "DomainPage" })]
    [RouteUrl("scl")]
    public class ShopCategoryAllPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "scl"; } set { } }
    }

    [RestrictParents(new string[] { "ShopCategoryAllPage", "ShopCategoryPage" })]
    [RouteUrl("scp")]
    public class ShopCategoryPage : AbstractPage
    {
        [Display(Prompt = "TabContent", Order = 110)]
        [UIHint("Image")]
        [Required]
        public override string Pic { get; set; }

        [Display(Prompt = "TabContent", Order = 110)]
        [UIHint("Image")]
        public string ShopBanner { get; set; }

        [Display(Prompt = "TabContent", Order = 110)]
        [DataType(DataType.MultilineText)]
        public override string ShortDescription { get; set; }

        [Display(Prompt = "Hidden")]
        public bool ShopShowInMainPage { get; set; }

        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "scp"; } set { } }

        public ShopCategoryPage()
        {
            Pic = "/Content/Shop/default.jpg";
        }

        public override void OnCreated()
        {
            base.OnCreated();
            RP.CleanCategoryPageRepository();
        }

        public override void OnEdited()
        {
            base.OnEdited();
            RP.CleanCategoryPageRepository();
        }

        public override void OnDeleted()
        {
            base.OnDeleted();
            RP.CleanCategoryPageRepository();
        }

        public override void OnMoved()
        {
            base.OnMoved();
            RP.CleanCategoryPageRepository();
        }
    }

    [RestrictParents(new string[] { "DomainPage" })]
    [RouteUrl("sbl")]
    public class ShopBrandAllPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "sbl"; } set { } }
    }

    [RestrictParents(new string[] { "ShopBrandAllPage", "ShopBrandPage" })]
    [RouteUrl("sbp")]
    public class ShopBrandPage : AbstractPage
    {
        [Display(Prompt = "TabContent")]
        public bool ShopShowInMainPage { get; set; }

        [UIHint("Image")]
        [Display(Prompt = "TabContent", Order = 110)]
        [Required]
        public override string Pic { get; set; }

        [Display(Prompt = "TabContent", Order = 110)]
        [DataType(DataType.MultilineText)]
        public override string ShortDescription { get; set; }

        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "sbp"; } set { } }

        public ShopBrandPage()
        {
            Pic = "/Content/DesignFiles/Shop/default.jpg";
        }
    }

    [RestrictParents(new string[] { "DomainPage" })]
    [RouteUrl("spl")]
    public class ShopProductAllPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "spl"; } set { } }
    }

    [RestrictParents(new string[] { "ShopProductAllPage" })]
    [RouteUrl("spp")]
    public class ShopProductPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "spp"; } set { } }

        [Display(Prompt = "TabContent", Order = 110)]
        [Required]
        [UIHint("Image")]
        public override string Pic { get; set; }

        [Display(Prompt = "TabContent", Order = 110)]
        [DataType(DataType.MultilineText)]
        public override string ShortDescription { get; set; }

         [Display(Prompt = "Hidden")]
        public bool ShopShowInMainPage { get; set; }

        [Display(Prompt = "Hidden")]
        public bool ShopShowInBest { get; set; }

        [Display(Prompt = "Hidden")]
        public bool ShopIsVisibleOnMirror { get; set; }

        [Display(Prompt = "Hidden")]
        public bool ShopShowInSale { get; set; }

         [Display(Prompt = "Hidden")]
        public bool ShopShowInStock { get; set; }

         [Display(Name = "ShowInProductsSpecials", Order = 201, Prompt = "TabSettings")]
         public bool ShowInProductsSpecials { get; set; }

        [Display(Prompt = "TabData", Order = 250)]
        [UIHint("Currency")]
        public decimal ShopPrice { get; set; }

        [Display(Prompt = "TabData", Order = 260)]
        [UIHint("Currency")]
        public decimal ShopOldPrice { get; set; }

        [Display(Prompt = "TabData", Order = 270)]
        [UIHint("Currency")]
        public decimal ShopAddShippingPrice { get; set; }

        [Display(Prompt = "TabData", Order = 280)]
        public bool ShopAddShippingPriceEach { get; set; }

        [Display(Prompt = "TabData", Order = 290)]
        public string ShopSKU { get; set; }

        [Display(Prompt = "TabCategory")]
        [UIHint("ShopBrand")]
        public string ShopBrand { get; set; }

        [Display(Prompt = "TabCategory")]
        [UIHint("ShopRelatedProducts")]
        public string ShopRelatedProducts { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ShopCategoryNames { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ShopCategoryIDs { get; set; }

        [Display(Prompt = "TabCategory")]
        [UIHint("ShopCategory")]
        [NotMapped]
        public string ShopCategoryData
        {
            get
            {
                return "Area" + "|" + this.ShopCategoryIDs;
            }
            set
            {
                this.ShopCategoryIDs = SF.SetTreeViewItemModel(value);
                this.ShopCategoryNames = SF.SetTreeViewItemModelNames(value);
            }
        }

        [Display(Prompt = "TabGallery")]
        [UIHint("ShopGallery")]
        [NotMapped]
        public string ShopGallery { get { return "ShopGallery/product_" + this.ID; } }

        //[Display(Prompt = "TabVideo")]
        //[UIHint("ShopYoutube")]
        //[DataType(DataType.MultilineText)]
        //public string ShopVideoGallery { get; set; }

        //[Display(Prompt = "TabOptions")]
        //[UIHint("ShopProductOptionEditor")]
        //[NotMapped]
        //public int ShopProductOption { get { return this.ID; } }

        [HiddenInput(DisplayValue = false)]
        public bool? ShopImported { get; set; }

        public ShopProductPage()
        {
            ShopShowInMainPage = false;
            ShopShowInStock = true;
            Pic = "/Content/DesignFiles/Shop/default.jpg";
        }
    }

    [RestrictParents(new string[] { "DomainPage" })]
    [RouteUrl("sps")]
    public class ShopProductSalePage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "sps"; } set { } }
    }

    #endregion

}