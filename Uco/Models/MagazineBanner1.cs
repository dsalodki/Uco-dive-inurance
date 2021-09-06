﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;

namespace Uco.Models
{
    [Table("MagazineBanners1")]
    public class MagazineBanner1
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public virtual int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "Title", Order = 10, Prompt = "Data")]
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; }

        [Display(Name = "LangCode", Order = 15, Prompt = "Data")]
        [UIHint("Languages")]
        public string LangCode { get; set; }

        [Display(Name = "Visible", Order = 15, Prompt = "Data")]
        public bool Visible { get; set; }


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
            }
        }

        [Display(Name = "Image", Order = 30, Prompt = "Data")]
        [UIHint("Image")]
        public string Image { get; set; }

        [Display(Name = "Description", Order = 120, Prompt = "Data")]
        [DataType(DataType.Html)]
        [DisplayFormat(HtmlEncode = false, ApplyFormatInEditMode = true)]
        [AllowHtml()]
        public string Description { get; set; }


        public MagazineBanner1()
        {
            if (RP.GetAdminCurrentSettingsRepository() != null)
                this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
    }
}