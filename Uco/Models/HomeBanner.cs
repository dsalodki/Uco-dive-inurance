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
    public class HomeBanner
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public virtual int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "Title", Order = 10, Prompt = "Data")]
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; }

        [Display(Name = "ShortDescription", Order = 20, Prompt = "Data")]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [Display(Name = "Credit", Order = 25, Prompt = "Data")]
        public string Credit { get; set; }

        [Display(Name = "Order", Order = 30,  Prompt = "Data")]
        public int Order { get; set; }
        [Display(Name = "Pic", Order = 40, Prompt = "Data")]
        [UIHint("Image")]
        public string Pic { get; set; }


        //[Display(Name = "Width", Order = 50, Prompt = "Data")]
        //public int Width { get; set; }

        //[Display(Name = "Height", Order = 60, Prompt = "Data")]
        //public int Height { get; set; }

        //[Display(Name = "Html", Order = 70, Prompt = "Data")]
        //[DataType(DataType.MultilineText)]
        //[DisplayFormat(HtmlEncode = false, ApplyFormatInEditMode = true)]
        //[AllowHtml()]
        //public string Html { get; set; }  

        [Display(Name = "Url", Order = 60, Prompt = "Data")]
        public string Url { get; set; }

        public HomeBanner()
        {
            this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
    }
}