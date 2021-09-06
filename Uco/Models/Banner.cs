using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;

namespace Uco.Models
{
    public class Banner
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public virtual int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "Title", Order = 10, Prompt = "Data")]
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; }

        [Display(Name = "BannerGroup", Order = 10, Prompt = "Data")]
        [Required(ErrorMessage = "BannerGroup Required")]
        [UIHint("BannerGroup")]
        public string BannerGroup { get; set; }

        [Display(Name = "LangCode", Order = 15, Prompt = "Data")]
        [UIHint("Languages")]
        public string LangCode { get; set; }

        [Display(Name = "NewWindow", Order = 15, Prompt = "Data")]
        public bool NewWindow { get; set; }

        [Display(Name = "Visible", Order = 15, Prompt = "Data")]
        public bool Visible { get; set; }

        [Display(Name = "ShowDateMin", Order = 15, Prompt = "Data")]
        public DateTime ShowDateMin { get; set; }

        [Display(Name = "ShowDateMax", Order = 15, Prompt = "Data")]
        public DateTime ShowDateMax { get; set; }

        [Display(Name = "Width", Order = 15, Prompt = "Data")]
        public int Width { get; set; }

        [Display(Name = "Height", Order = 15, Prompt = "Data")]
        public int Height { get; set; }


        [HiddenInput(DisplayValue = false)]
        public string ShowOnPageIds { get; set; }


        [Display(Name = "ShowOnPageIds", Order = 15, Prompt = "ShowOnPage")]
        [UIHint("AllPagesTree")]
        [NotMapped]
        public string ShowOnPageData
        {
            get
            {
                return "Area" + "|" + this.ShowOnPageIds;
            }
            set
            {
                this.ShowOnPageIds = SF.SetTreeViewItemModelForAllPages(value);
                //this.ShopCategoryNames = SF.SetTreeViewItemModelNames(value);
            }
        }


        [HiddenInput(DisplayValue = false)]
        public int Type { get; set; }
        [Display(Name = "Type", Order = 25, Prompt = "Data")]
        [UIHint("Enum")]
        [NotMapped]
        public BannerType BannerTypeName
        {
            get { return (BannerType)Type; }
            set { Type = (int)value; }
        }
        public enum BannerType
        {
            Image,
            Flash,
            FlashAndBackground,
            Html,
            Text
        }

        [Display(Name = "Image or flash", Order = 30, Prompt = "Data")]
        [UIHint("File")]
        public string MainFile { get; set; }

        [NotMapped]
        [HiddenInput(DisplayValue = false)]
        public string Output { get; set; }

        [Display(Name = "Flash background", Order = 40, Prompt = "Data")]
        [UIHint("File")]
        public string OtherFile { get; set; }

        [Display(Name = "Link", Order = 50, Prompt = "Data")]
        public string Link { get; set; }

        [NotMapped]
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "PreLink", Order = 50, Prompt = "Data")]
        public string PreLink
        {
            get
            {
                if (ID != 0 && !string.IsNullOrEmpty(this.Link))
                    return "/pagepart/bannerlink?id=" + ID.ToString();
                else
                    return string.Empty;
            }

        }



        [Display(Name = "Html", Order = 60, Prompt = "Data")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(HtmlEncode = false, ApplyFormatInEditMode = true)]
        [AllowHtml()]
        public string Html { get; set; }

        [Display(Name = "Text", Order = 120, Prompt = "Data")]
        [DataType(DataType.Html)]
        [DisplayFormat(HtmlEncode = false, ApplyFormatInEditMode = true)]
        [AllowHtml()]
        public virtual string Text { get; set; }


        [Display(Name = "BanerStatistics", Order = 120, Prompt = "Statistics")]
        [UIHint("BanerStatistics")]
        [NotMapped]
        public virtual string BanerStatistics { get; set; }



        public Banner()
        {
            if (RP.GetAdminCurrentSettingsRepository() != null)
                this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
    }

    public class BannersStatistic
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public virtual int ID { get; set; }

        [Index]
        [HiddenInput(DisplayValue = false)]
        public int BannerID { get; set; }

        [Index]
        [HiddenInput(DisplayValue = false)]
        public virtual DateTime Date { get; set; }

        public int CountViews { get; set; }

        public int CountClicks { get; set; }
    }

}