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

    #region Shop data

    public class ShopCartItem
    {

        [Display(Prompt = "TabDetails")]
        public int ID { get; set; }

        [Display(Prompt = "TabDetails")]
        public string ShopProductTitle { get; set; }

        [Display(Prompt = "TabDetails")]
        public int ShopQuantity { get; set; }

        [Display(Prompt = "TabDetails")]
        [UIHint("Currency")]
        public decimal ShopPrice { get; set; }

        [Display(Prompt = "TabDetails")]
        [UIHint("Currency")]
        public decimal ShopPriceShipping { get; set; }

        [Display(Prompt = "TabDetails")]
        [HiddenInput(DisplayValue = false)]
        public string ShopPic { get; set; }

        [Display(Prompt = "TabDetails")]
        [HiddenInput(DisplayValue = false)]
        public string ShopUrl { get; set; }
    }

    public class ShopCart
    {

        public int? ShopPayType { get; set; }

        public int? ShopShippingType { get; set; }

        [UcoEmail(ErrorMessage = "Wrong email")]
        public string ShopEmail { get; set; }


        [Required(ErrorMessage = "Required")]
        public string ShopFirstName { get; set; }


        [Required(ErrorMessage = "Required")]
        public string ShopLastName { get; set; }


        [Required(ErrorMessage = "Required")]
        [UcoPhone(ErrorMessage = "Phone invalid")]
        public string ShopPhone1 { get; set; }


        [UcoPhone(ErrorMessage = "Phone invalid")]
        public string ShopPhone2 { get; set; }


        [Required(ErrorMessage = "Required")]
        public string ShopAdressStreet { get; set; }


        [Required(ErrorMessage = "Required")]
        public string ShopAdressCity { get; set; }


        public string ShopCompanyName { get; set; }


        [DataType(DataType.MultilineText)]
        public string ShopOther { get; set; }


        [Mandatory]
        public bool ShopTearm { get; set; }
    }

    public class ShopOrder
    {
        [Key]
        [Display(Prompt = "TabDetails")]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }


        [Display(Prompt = "TabDetails")]
        [UIHint("Enum")]
        public ShopOrderStatusEnum ShopStatus { get; set; }

        [Display(Prompt = "TabDetails")]
        [Required(ErrorMessage = "שם פרטי חובה")]
        public string ShopFirstName { get; set; }


        [Display(Prompt = "TabDetails")]
        [Required]
        public string ShopLastName { get; set; }


        [Display(Prompt = "TabDetails")]
        public string ShopCompanyName { get; set; }


        [Display(Prompt = "TabDetails")]
        [Required]
        [UcoPhone]
        public string ShopPhone1 { get; set; }


        [Display(Prompt = "TabDetails")]
        [UcoPhone]
        public string ShopPhone2 { get; set; }


        [Display(Prompt = "TabDetails")]
        [UcoEmail]
        public string ShopEmail { get; set; }


        [Display(Prompt = "TabDetails")]
        [Required]
        public string ShopAdressStreet { get; set; }


        [Display(Prompt = "TabDetails")]
        [Required]
        public string ShopAdressCity { get; set; }


        [Display(Prompt = "TabDetails")]
        [DataType(DataType.MultilineText)]
        public string ShopOther { get; set; }


        [Display(Prompt = "TabPayment")]
        [UIHint("Enum")]
        public ShopPayTypeEnum ShopPayType { get; set; }


        [Display(Prompt = "TabPayment")]
        [UIHint("Currency")]
        public decimal ShopTotal { get; set; }


        [Display(Prompt = "TabPayment")]
        [UIHint("ReadOnly")]
        public string ShopApprovedToken { get; set; }


        [Display(Prompt = "TabPayment")]
        [UIHint("ReadOnly")]
        public string ShopApprovedGuid { get; set; }


        [Display(Prompt = "TabPayment")]
        [UIHint("ReadOnly")]
        public DateTime ShopApprovedDate { get; set; }


        [Display(Prompt = "TabInvoice")]
        public bool ShopInvoiceSent { get; set; }


        [Display(Prompt = "TabInvoice")]
        public string ShopInvoiceID { get; set; }


        [Display(Prompt = "TabInvoice")]
        public string ShopInvoiceURL { get; set; }


        [Display(Prompt = "TabShipment")]
        [UIHint("Enum")]
        public ShopShippingTypeEnum ShopShippingType { get; set; }

        [Display(Prompt = "TabShipment")]
        [UIHint("Currency")]
        public decimal ShopShippingPrice { get; set; }

        [Display(Prompt = "TabShipment")]
        [UIHint("ShopDate")]
        public DateTime ShopDeliveryDate { get; set; }

        [Display(Prompt = "TabLog")]
        [DataType(DataType.MultilineText)]
        public string ShopLog { get; set; }

        [HiddenInput(DisplayValue = false)]
        public Guid OrderGuid { get; set; }

        //ShopXML
        [HiddenInput(DisplayValue = false)]
        [AllowHtml]
        [Column(TypeName = "xml")]
        public virtual string ShopXML { get; set; }

        public List<T> ShopGetDataFromXML<T>()
        {
            if (string.IsNullOrEmpty(this.ShopXML)) return new List<T>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
            return x.Deserialize(new System.IO.StringReader(this.ShopXML)) as List<T>;
        }
        public void ShopSetDataToXML<T>(List<T> DataToXML)
        {
            if (DataToXML == null) return;
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(DataToXML.GetType());
            System.IO.StringWriter sw = new System.IO.StringWriter();
            x.Serialize(sw, DataToXML);
            this.ShopXML = sw.ToString();
        }

        [HiddenInput(DisplayValue = false)]
        public int ShopDomainID { get; set; }

        public ShopOrder()
        {
            this.ShopStatus = ShopOrderStatusEnum.Placed;
            //this.ShopDomainID = SF.GetAdminCurrentSettingsRepository().ID;
        }
    }

    public class ShopAnaliticsData
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        public DateTime ShopDate { get; set; }

        public int ShopUnic { get; set; }

        public int ShopShow { get; set; }

        public int ShopPhone { get; set; }

        public int ShopContact { get; set; }

        public int ShopBuy { get; set; }

        public string ShopReferalUrl { get; set; }

        public string ShopReferalSearch { get; set; }

        public int ShopPageID { get; set; }

        public string ShopPageTitle { get; set; }

        public string ShopPageRouteUrl { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ShopDomainID { get; set; }

        public ShopAnaliticsData()
        {
            this.ShopDate = DateTime.Now.Date;
            this.ShopUnic = 0;
            this.ShopShow = 0;
            this.ShopPhone = 0;
            this.ShopContact = 0;
            this.ShopBuy = 0;
            this.ShopPageID = 0;
            this.ShopDomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
    }

    public class ShopProductOption
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Display(Prompt = "TabData")]
        [HiddenInput(DisplayValue = false)]
        public int ShopParentID { get; set; }

        [Display(Prompt = "TabData")]
        [Required]
        public int ShopOrder { get; set; }

        [Display(Prompt = "TabData")]
        [Required]
        public string ShopTitle { get; set; }

        [Display(Prompt = "TabData")]
        [UIHint("Enum"), Required]
        public ShopProductOptionTypeEnum ShopProductOptionTypeValue { get; set; }

        [HiddenInput(DisplayValue = false)]
        [AllowHtml]
        [Column(TypeName = "xml")]
        public virtual string ShopXML { get; set; }

        public List<ShopProductOptionItem> ShopGetProductOptionItem()
        {
            if (string.IsNullOrEmpty(this.ShopXML)) return new List<ShopProductOptionItem>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<ShopProductOptionItem>));
            return x.Deserialize(new System.IO.StringReader(this.ShopXML)) as List<ShopProductOptionItem>;
        }
        public void ShopSetProductOptionItem(List<ShopProductOptionItem> DataToXML)
        {
            if (DataToXML == null) return;
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(DataToXML.GetType());
            System.IO.StringWriter sw = new System.IO.StringWriter();
            x.Serialize(sw, DataToXML);
            this.ShopXML = sw.ToString();
        }

        [HiddenInput(DisplayValue = false)]
        public int ShopDomainID { get; set; }

        public ShopProductOption()
        {
            this.ShopDomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }

    }

    [NotMapped]
    public class ShopProductImport
    {
        public string ShopProductID { get; set; }
        public string ShopTitle { get; set; }
        public string ShopShortDescription { get; set; }
        public string ShopText { get; set; }
        public string ShopShowInMainPage { get; set; }
        public string ShopShowInBest { get; set; }
        public string ShopIsVisibleOnMirror { get; set; }
        public string ShopShowInSale { get; set; }
        public string ShopShowInSlider { get; set; }
        public string ShopUseStock { get; set; }
        public string ShopShowInStockNumber { get; set; }
        public string ShopPrice { get; set; }
        public string ShopOldPrice { get; set; }
        public string ShopAddShippingPrice { get; set; }
        public string ShopAddShippingPriceEach { get; set; }
        public string ShopBrand { get; set; }
        public string ShopTags { get; set; }
        public string ShopRelatedProducts { get; set; }
        public string ShopCategoryNames { get; set; }
        public string ShopVideoGallery { get; set; }

        public string ShopSeoH1 { get; set; }
        public string ShopSeoTitle { get; set; }
        public string ShopSeoDescription { get; set; }
        public string ShopSeoKywords { get; set; }
        public string ShopSeoInLinkName { get; set; }

        public string ShopPic { get; set; }
    }

    [NotMapped]
    public class ShopProductOptionItem
    {
        [Key]
        [Display(Name = "ID")]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Display(Prompt = "TabData")]
        [Required()]
        public int ShopOrder { get; set; }

        [Display(Prompt = "TabData")]
        [Required()]
        public string ShopTitle { get; set; }

        [Display(Prompt = "TabData")]
        [Required()]
        [UIHint("Currency")]
        public decimal ShopPrice { get; set; }
    }

    #endregion

    #region Shop Enum

    public enum ShopProductOptionTypeEnum
    {
        SingleSelect = 0,
    }

    public enum ShopOrderStatusEnum
    {
         [Display(Name = "לא שולם")]
        Placed = 0,
         [Display(Name = "שולם")]
        Payed = 1,
         [Display(Name = "נשלח")]
        Shipped = 2,
         [Display(Name = "התקרבל")]
        Received = 3,
         [Display(Name = "בוטל")]
        Canceled = 4
    }

    public enum ShopPayTypeEnum
    {
        [Display(Name = "לא שולם")]
        NoPayment = 0,
        [Display(Name = "איסוף עצמי")]
        Instore = 1,
        [Display(Name = "דרך הטלפון")]
        Phone = 2,
        [Display(Name = "פייפל")]
        Paypal = 3,
        [Display(Name = "קרדיט-גארד")]
        CreditGuard = 4,
        [Display(Name = "ZCredit")]
        ZCredit = 5
    }

    public enum ShopShippingTypeEnum
    {
         [Display(Name = "לא נשלח")]
        NoShipment = 0,
         [Display(Name = "איסוף עצמי")]
        Self = 1,
         [Display(Name = "נשלח")]
        Shipment = 2
    }

    #endregion

}