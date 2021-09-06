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

    //Add to Models Db.cs
    //public DbSet<ShopCategoryAllPage> ShopCategoryAllPages { get; set; }
    //public DbSet<ShopCategoryPage> ShopCategoryPages { get; set; }
    //public DbSet<ShopBrandAllPage> ShopBrandAllPages { get; set; }
    //public DbSet<ShopBrandPage> ShopBrandPages { get; set; }
    //public DbSet<ShopProductAllPage> ShopProductAllPages { get; set; }
    //public DbSet<ShopProductPage> ShopProductPages { get; set; }
    //public DbSet<ShopProductSalePage> ShopProductSalePages { get; set; }
    //public DbSet<ShopOrder> ShopOrders { get; set; }
    //public DbSet<ShopAnaliticsData> ShopAnaliticsDatas { get; set; }
    //public DbSet<ShopProductOption> ShopProductOptions { get; set; }


    #region Settings

    public partial class Settings
    {
        [Display(Prompt = "TabDomain")]
        public string ShopSiteMainCollors { get; set; }

        [Display(Prompt = "TabPayment")]
        public bool ShopPaymentUse { get; set; }


        [Display(Prompt = "TabPayment")]
        public bool ShopPaymentPhoneUse { get; set; }


        [Display(Prompt = "TabPayment")]
        public bool ShopPaymentInstoreUse { get; set; }


        [Display(Prompt = "TabPayment")]
        public bool ShopPaymentPayPalUse { get; set; }


        [Display(Prompt = "TabPayment")]
        public string ShopPaymentPayPalEmail { get; set; }


        [Display(Prompt = "TabPayment")]
        public bool ShopPaymentCreditGuardUse { get; set; }



        // ****** There is another tab for it *******/

        //[Display(Prompt = "TabPayment")]
        //public string ShopPaymentCreditGuardUser { get; set; }


        //[Display(Prompt = "TabPayment")]
        //public string ShopPaymentCreditGuardPass { get; set; }


        //[Display(Prompt = "TabPayment")]
        //public string ShopPaymentCreditGuardMod { get; set; }


        //[Display(Prompt = "TabPayment")]
        //public string ShopPaymentCreditGuardMod2 { get; set; }


        [Display(Prompt = "TabPayment")]
        public bool ShopPaymentZCreditUse { get; set; }


        [Display(Prompt = "TabPayment")]
        public string ShopPaymentZCreditTerminalNumber { get; set; }


        [Display(Prompt = "TabPayment")]
        public string ShopPaymentZCreditUserName { get; set; }


        [Display(Prompt = "TabShipment")]
        public bool ShopShippingUse { get; set; }


        [Display(Prompt = "TabShipment")]
        public bool ShopShippingSelfUse { get; set; }


        [Display(Prompt = "TabShipment")]
        public string ShopShippingSelfText { get; set; }


        [Display(Prompt = "TabShipment")]
        public decimal ShopShippingPrice { get; set; }


        [Display(Prompt = "TabShipment")]
        public int ShopShippingDays { get; set; }


        [Display(Prompt = "TabShipment")]
        public string ShopShippingName { get; set; }


        [Display(Prompt = "TabShipment")]
        public string ShopShippingText { get; set; }


        [Display(Prompt = "TabInvoice")]
        public bool ShopInvoiceUse { get; set; }


        [Display(Prompt = "TabInvoice")]
        public string ShopInvoiceUser { get; set; }


        [Display(Prompt = "TabInvoice")]
        public string ShopInvoiceKey { get; set; }

        [Display(Prompt = "TabTaks")]
        public string TaksPercent { get; set; }
    }

    #endregion

}