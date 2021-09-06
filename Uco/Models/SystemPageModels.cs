using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;
using System.Linq;
using System.Data.Entity;
using System.Web;
using Uco.Infrastructure.Livecycle;
using System.ComponentModel;

namespace Uco.Models
{
	public abstract class AbstractPage
	{


		[HiddenInput(DisplayValue = false)]
		[Key]
		public int ID { get; set; }
		[Display(Name = "Title", Order = 100, Prompt = "TabContent")]
		[Required(ErrorMessage = "TitleRequired")]
		public virtual string Title { get; set; }
		
        [Display(Name = "Pic", Order = 109, Prompt = "TabContent")]
        [UIHint("Image")]
        public virtual string Pic { get; set; }

        [Display(Name = "ShortDescription", Order = 115, Prompt = "TabContent")]
        [Column("ShortDescription", TypeName = "nvarchar(MAX)")]
        [DataType(DataType.MultilineText)]
        public virtual string ShortDescription { get; set; }
		
        [Display(Name = "Text", Order = 150, Prompt = "TabContent")]
        [UIHint("Html")]
        [AllowHtml]
        public virtual string Text { get; set; }

        [Display(Name = "Layout", Order = 200, Prompt = "TabSettings")]
        [UIHint("Layouts")]
        public virtual string Layout { get; set; }
        [Display(Name = "Order", Order = 210, Prompt = "TabSettings")]
        public int Order { get; set; }
        [Display(Name = "Visible", Order = 220, Prompt = "TabSettings")]
        public bool Visible { get; set; }

        [Display(Name = "ShowInMenu", Order = 230, Prompt = "TabSettings")]
        public bool ShowInMenu { get; set; }
        [Display(Name = "ShowInSitemap", Order = 240, Prompt = "TabSettings")]
        public bool ShowInSitemap { get; set; }
        [HiddenInput(DisplayValue = false)]

        public bool ShowInAdminMenu { get; set; }

        [Display(Name = "CreateTime", Order = 260, Prompt = "TabSettings")]
        [UIHint("DateTime")]
        [Column(TypeName = "datetime2")]
        public DateTime CreateTime { get; set; }
        [Display(Name = "ChangeTime", Order = 270, Prompt = "TabSettings")]
        [UIHint("DateTime")]
        [Column(TypeName = "datetime2")]
        public DateTime ChangeTime { get; set; }

        [Display(Name = "SeoUrlName", Order = 300, Prompt = "TabSeo")]
        [MaxLength(4000)]
        [RegularExpression("^[a-zA-Zא-ת0-9-]+$", ErrorMessage = "SeoUrlNameError")]
        public string SeoUrlName { get; set; }
        [Display(Name = "SeoTitle", Order = 310, Prompt = "TabSeo")]
        public string SeoTitle { get; set; }
        [Display(Name = "SeoH1", Order = 320, Prompt = "TabSeo")]
        public string SeoH1 { get; set; }
        [Display(Name = "SeoInLinkName", Order = 330, Prompt = "TabSeo")]
        public string SeoInLinkName { get; set; }
        [Display(Name = "SeoKywords", Order = 340, Prompt = "TabSeo")]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        public string SeoKywords { get; set; }
        [Display(Name = "SeoDescription", Order = 340, Prompt = "TabSeo")]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        public string SeoDescription { get; set; }

        [Display(Name = "PermissionsView", Order = 340, Prompt = "Permissions")]
        [UIHint("PermissionsView")]
        public string PermissionsView { get; set; }

        [Display(Name = "PermissionsEdit", Order = 340, Prompt = "Permissions")]
        [UIHint("PermissionsEdit")]
        public string PermissionsEdit { get; set; }

        [Display(Name = "PermissionsUpdateChildPages", Order = 340, Prompt = "Permissions")]
        public bool PermissionsUpdateChildPages { get; set; }

        [Display(Name = "RedirectTo", Order = 270, Prompt = "TabSettings")]
        public virtual string RedirectTo { get; set; }

        [Display(Name = "ParentID")]
        [HiddenInput(DisplayValue = false)]
        public int ParentID { get; set; }

        [HiddenInput(DisplayValue = false)]
        [MaxLength(10)]
        public virtual string RouteUrl { get; set; }


        [Display(Name = "Author", Prompt = "Hidden")]
        //[HiddenInput(DisplayValue = false)]
        public virtual string Author { get; set; }

        //[Display(Name = "AddingFacebookComments", Order = 270, Prompt = "TabSettings")]
        [Display(Name = "AddingFacebookComments", Prompt = "Hidden")]
        public virtual bool AddingFacebookComments { get; set; }

        [NotMapped]
        [HiddenInput(DisplayValue = false)]
        public virtual string Url
        {
            get
            {
                return "~/" + SF.GetLangRoute(this.LanguageCode) + this.RouteUrl + "/" + this.SeoUrlName;
            }
        }

        [NotMapped]
        [HiddenInput(DisplayValue = false)]
        public virtual string UrlFix
        {
            get
            {
                return "/" + SF.GetLangRoute(this.LanguageCode) + this.RouteUrl + "/" + this.SeoUrlName;
            }
        }

        [Display(Name = "Link", Order = 200, Prompt = "TabSettings")]
        [UIHint("Link")]
        [NotMapped]
        public string Link
        {
            get
            {
                return this.Url;
            }
        }

        //Added Page Name and PageIcon by Jit
        [Display(Prompt = "Hidden")]
        public virtual string PageName { get; set; }

        [Display(Prompt = "Hidden")]
        public virtual string PageIcon { get; set; }

        [Display(Name = "Link", Order = 200, Prompt = "TabMoveText")]
        public virtual bool ShowInNewsHomepage { get; set; }

        [Display(Name = "Link", Order = 200, Prompt = "TabMoveText")]
        public virtual bool ShowInNewOnShelfHomepage { get; set; }

        [Display(Name = "Link", Order = 200, Prompt = "TabMoveText")]
        public virtual bool ShowInNewOnShelfHomepage2 { get; set; }

        [Display(Name = "Link", Order = 200, Prompt = "TabMoveText")]
        public virtual bool ShowInProductsHomepage { get; set; }

        [Display(Name = "Link", Order = 200, Prompt = "TabMoveText")]
        public virtual bool ShowInArticlesHomepage { get; set; }

        [Display(Name = "Link", Order = 200, Prompt = "TabMoveText")]
        public virtual bool ShowInGalleryHomepage { get; set; }

        [Display(Prompt = "Hidden")]
        public virtual string LanguageCode { get; set; }

        [Display(Prompt = "Hidden")]
        [NotMapped]
        public virtual string ShortDescriptionFix
        {
            get
            {
                if (!string.IsNullOrEmpty(ShortDescription))
                    return this.ShortDescription.Replace(Environment.NewLine, "<br />");
                else
                    return string.Empty;                
            }
        }

        [Display(Prompt = "Hidden")]
        public virtual string Text2 { get; set; }
        [Display(Prompt = "Hidden")]
        public virtual string Text3 { get; set; }
        //XML
        [HiddenInput(DisplayValue = false)]
        [AllowHtml]
        [Column(TypeName = "xml")]
        public virtual string XML1 { get; set; }

        public List<T> GetDataFromXML1<T>()
        {
            if (string.IsNullOrEmpty(this.XML1)) return new List<T>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
            return x.Deserialize(new System.IO.StringReader(this.XML1)) as List<T>;
        }
        public void SetDataToXML1<T>(List<T> DataToXML)
        {
            if (DataToXML == null) return;
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(DataToXML.GetType());
            System.IO.StringWriter sw = new System.IO.StringWriter();
            x.Serialize(sw, DataToXML);
            this.XML1 = sw.ToString();
        }
        [HiddenInput(DisplayValue = false)]
        public virtual int? MigrationOldID { get; set; }
        //XML
        [HiddenInput(DisplayValue = false)]
        [AllowHtml]
        [Column(TypeName = "xml")]
        public virtual string XML2 { get; set; }

        public List<T> GetDataFromXML2<T>()
        {
            if (string.IsNullOrEmpty(this.XML2)) return new List<T>();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
            return x.Deserialize(new System.IO.StringReader(this.XML2)) as List<T>;
        }
        public void SetDataToXML2<T>(List<T> DataToXML)
        {
            if (DataToXML == null) return;
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(DataToXML.GetType());
            System.IO.StringWriter sw = new System.IO.StringWriter();
            x.Serialize(sw, DataToXML);
            this.XML2 = sw.ToString();
        }

        [NotMapped]
        public List<string> RolesViewList
        {
            get
            {
                return SF.RolesStringToList(this.PermissionsView);
            }
        }

        [NotMapped]
        public List<string> RolesEditList
        {
            get
            {
                return SF.RolesStringToList(this.PermissionsEdit);
            }
        }

        public virtual void BeforeCreate(int ParentID)
        {
            AbstractPage Parent = LS.CurrentEntityContext.AbstractPages.FirstOrDefault(r => r.ID == ParentID);
            if (Parent == null)
            {
                List<string> GetRoleListView = SF.GetRoleList();
                List<string> GetRoleListEdit = SF.GetRoleList();
                GetRoleListView.Remove("Admin");
                GetRoleListEdit.Remove("Admin");
                GetRoleListEdit.Remove("Anonymous");
                this.PermissionsView = SF.RolesListToString(GetRoleListView);
                this.PermissionsEdit = SF.RolesListToString(GetRoleListEdit);
            }
            else
            {
                this.PermissionsView = Parent.PermissionsView;
                this.PermissionsEdit = Parent.PermissionsEdit;
            }
        }

        public virtual void OnCreate()
        {
            if (SF.UsePermissions())
            {
                List<string> PermissionsView = new List<string>();
                PermissionsView.Add("Admin");
                if (LS.CurrentHttpContext.Request["PermissionsView"] != null)
                {
                    PermissionsView.AddRange(LS.CurrentHttpContext.Request.Form.GetValues("PermissionsView").ToList());
                }
                this.PermissionsView = SF.RolesListToString(PermissionsView);

                List<string> PermissionsEdit = new List<string>();
                PermissionsEdit.Add("Admin");
                if (LS.CurrentHttpContext.Request["PermissionsEdit"] != null)
                {
                    PermissionsEdit.AddRange(LS.CurrentHttpContext.Request.Form.GetValues("PermissionsEdit").ToList());
                }
                this.PermissionsEdit = SF.RolesListToString(PermissionsEdit);
            }
        }

        public virtual void OnCreated()
        {
            List<string> c = this.RolesEditList;

        }

        public virtual void OnCreateChild() { }

        public virtual void OnEdit()
        {

            if (SF.UsePermissions())
            {
                if (LS.CurrentUser.IsInRole("Admin"))
                {
                    List<string> PermissionsView = new List<string>();
                    PermissionsView.Add("Admin");
                    if (LS.CurrentHttpContext.Request["PermissionsView"] != null)
                    {
                        PermissionsView.AddRange(LS.CurrentHttpContext.Request.Form.GetValues("PermissionsView").ToList());
                    }
                    this.PermissionsView = SF.RolesListToString(PermissionsView);

                    List<string> PermissionsEdit = new List<string>();
                    PermissionsEdit.Add("Admin");
                    if (LS.CurrentHttpContext.Request["PermissionsEdit"] != null)
                    {
                        PermissionsEdit.AddRange(LS.CurrentHttpContext.Request.Form.GetValues("PermissionsEdit").ToList());
                    }
                    this.PermissionsEdit = SF.RolesListToString(PermissionsEdit);
                }
                else
                {
                    AbstractPage OldPage = LS.CurrentEntityContext.AbstractPages.FirstOrDefault(r => r.ID == ID);
                    LS.CurrentEntityContext.Entry(OldPage).State = EntityState.Detached;
                    this.PermissionsView = OldPage.PermissionsView;
                    this.PermissionsEdit = OldPage.PermissionsEdit;

                }

            }
        }

        public virtual void OnEdited()
        {
            if (SF.UsePermissions() && this.PermissionsUpdateChildPages)
            {
                if (LS.CurrentUser.IsInRole("Admin"))
                {
                    SF.UpdateChildPermissions(this.PermissionsView, this.PermissionsEdit, this.ID);
                }
            }
        }

        public virtual void OnDelete() { }

        public virtual void OnDeleted() { }

        public virtual void OnMove() { }

        public virtual void OnMoved() { }

        public virtual void OnCopy() { }

        public virtual void OnCopyed() { }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        public AbstractPage()
        {
            Order = 100;
            Layout = "_Layout.cshtml";
            Visible = true;
            ShowInMenu = true;
            ShowInSitemap = true;
            ShowInAdminMenu = true;
            CreateTime = DateTime.Now;
            ChangeTime = DateTime.Now;
            PermissionsUpdateChildPages = true;
            LanguageCode = SF.GetLangCodeWebconfig();
        }
    }

    [RestrictParents(new string[] { })]
    [RouteUrl("d")]
    [PageIcon("~/Areas/Admin/Content/pages/route_d.png")]
    [PageName("DomainPage")]
    public class DomainPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "d"; } set { } }

        [NotMapped]
        [HiddenInput(DisplayValue = false)]
        public override string Url
        {
            get
            {
                return "~/";
            }
        }

        public DomainPage()
        {

        }
    }

    [RestrictParents(new string[] { "DomainPage" })]
    [RouteUrl("l")]
    [PageIcon("~/Areas/Admin/Content/pages/route_l.png")]
    [PageName("LanguagePage")]
    public class LanguagePage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "l"; } set { } }

        [Display(Name = "LanguageCode", Order = 900, Prompt = "TabDomain")]
        [UIHint("Languages")]
        public override string LanguageCode { get; set; }

        [Display(Name = "HeaderHtml", Order = 900, Prompt = "TabDomain")]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        [AllowHtml]
        public override string Text2 { get; set; }

        [Display(Name = "FotterHtml", Order = 900, Prompt = "TabDomain")]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        [AllowHtml]
        public override string Text3 { get; set; }

        public override void OnEdited()
        {
            SF.SetLanguageCode(this, 100, this.LanguageCode);

            base.OnEdited();
        }

        public LanguagePage()
        {

        }
    }

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage" })]
    [RouteUrl("m")]
    [PageIcon("~/Areas/Admin/Content/pages/route_m.png")]
    [PageName("SiteMapPage")]
    public class SiteMapPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "m"; } set { } }
    }

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage", "ArticleListPage" })]
    [RouteUrl("arl")]
    [PageIcon("~/Areas/Admin/Content/pages/route_arl.png")]
    [PageName("ArticleListPage")]
    public class ArticleListPage : AbstractPage
    {
        [UIHint("ArticleTemplate")]
        [Display(Name = "PageTemplate", Order = 205, Prompt = "TabSettings")]

        public string PageTemplate { get; set; }

        [Display(Name = "ShortDescription", Order = 115, Prompt = "TabContent")]
        [Column("ShortDescription", TypeName = "nvarchar(MAX)")]
        [DataType(DataType.MultilineText)]
        public override string ShortDescription { get; set; }

        [Display(Name = "Pic", Order = 109, Prompt = "TabContent")]
        [UIHint("Image")]
        public override string Pic { get; set; }

        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "arl"; } set { } }
       
    }


    [RestrictParents(new string[] { "ArticleListPage", "ContentPage" })]
    [RouteUrl("a")]
    [PageIcon("~/Areas/Admin/Content/pages/route_a.png")]
    [PageName("ArticlePage")]
    public class ArticlePage : AbstractPage
    {
        [Display(Name = "ShortDescription", Order = 115, Prompt = "TabContent")]
        [Column("ShortDescription", TypeName = "nvarchar(MAX)")]
        [DataType(DataType.MultilineText)]
        public override string ShortDescription { get; set; }

        [Display(Name = "Pic", Order = 109, Prompt = "TabContent")]
        [UIHint("Image")]
        public override string Pic { get; set; }

        //[Display(Name = "MainImg", Order = 109, Prompt = "TabContent")]
        //[UIHint("Image")]
        //public string MainImg { get; set; }

        [Display(Name = "Author", Order = 108, Prompt = "TabContent")]
        public override string Author { get; set; }

        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "a"; } set { } }

        [Display(Name = "RelatedArticles", Order = 120, Prompt = "TabContent")]
        [DataType(DataType.MultilineText)]
        public string RelatedArticles { get; set; }

        [Display(Name = "Photographer", Prompt = "TabContent")]
        [HiddenInput(DisplayValue = false)]
        public string Photographer { get; set; }

        [Display(Name = "AddingFacebookComments", Order = 255, Prompt = "TabSettings")]
        [DefaultValue(false)]
        public override bool AddingFacebookComments { get; set; }

        [Display(Name = "Pictures", Order = 400, Prompt = "TabData")]
        [UIHint("Gallery")]
        public string Gallery { get { return "Gallery/gallery_" + this.ID; } }


        public ArticlePage()
        {
            //Pic = "/Content/DesignFiles/default.png";
        }
      
    }


    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage", "ArticlePage", "ArticleListPage" })]
    [RouteUrl("f")]
    [PageIcon("~/Areas/Admin/Content/pages/route_arl.png")]
    [PageName("FormPage")]
    public class FormPage : AbstractPage
    {
        [Display(Name = "RoleDefault", Order = 110, Prompt = "TabContent")]
        [UIHint("RoleDefault")]
        public string RoleDefault { get; set; }

        [Display(Name = "FormTextAfter", Order = 300, Prompt = "TabTextAfter")]
        [UIHint("Html")]
        [AllowHtml]
        [Column(TypeName = "nvarchar(MAX)")]
        public override string Text2 { get; set; }

        [Display(Name = "FormTextEmail", Order = 300, Prompt = "TabTextAfter")]
        [UIHint("Html")]
        [AllowHtml]
        [Column(TypeName = "nvarchar(MAX)")]
        public override string Text3 { get; set; }

        [NotMapped]
        public IEnumerable<FormField> Fields { get; set; }

        [NotMapped]
        public IEnumerable<FormRool> Rools { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();


            if (Fields != null)
            {
                foreach (FormField item in Fields) { if (item.FormFieldID <= 0) item.FormFieldID = Fields.Max(r => r.FormFieldID) + 1; }
                this.SetDataToXML1<FormField>(Fields.OrderBy(r => r.FormFieldOrder).ToList());
            }
            else this.SetDataToXML1<FormField>(new List<FormField>());

            if (Rools != null)
            {
                foreach (FormRool item in Rools) { if (item.FormRoolID <= 0) item.FormRoolID = Rools.Max(r => r.FormRoolID) + 1; }
                this.SetDataToXML2<FormRool>(Rools.OrderBy(r => r.FormRoolOrder).ToList());
            }
            else this.SetDataToXML2<FormRool>(new List<FormRool>());
        }

        public override void OnEdit()
        {
            base.OnEdit();

            if (Fields != null)
            {
                foreach (FormField item in Fields) { if (item.FormFieldID <= 0) item.FormFieldID = Fields.Max(r => r.FormFieldID) + 1; }
                this.SetDataToXML1<FormField>(Fields.OrderBy(r => r.FormFieldOrder).ToList());
            }
            else this.SetDataToXML1<FormField>(new List<FormField>());

            if (Rools != null)
            {
                foreach (FormRool item in Rools) { if (item.FormRoolID <= 0) item.FormRoolID = Rools.Max(r => r.FormRoolID) + 1; }
                this.SetDataToXML2<FormRool>(Rools.OrderBy(r => r.FormRoolOrder).ToList());
            }
            else this.SetDataToXML2<FormRool>(new List<FormRool>());
        }

        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "f"; } set { } }
    }

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage", "ArticleListPage" })]
    [RouteUrl("gal")]
    [PageIcon("~/Areas/Admin/Content/pages/route_arl.png")]
    [PageName("GalleryListPage")]
    public class GalleryListPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "gal"; } set { } }
    }

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "ContentPage", "GalleryListPage", "ArticleListPage" })]
    [RouteUrl("g")]
    [PageIcon("~/Areas/Admin/Content/pages/route_g.png")]
    [PageName("GalleryPage")]
    public class GalleryPage : AbstractPage
    {
        [Display(Name = "Pictures", Order = 400, Prompt = "TabData")]
        [UIHint("Gallery")]
        public string Gallery { get { return "Gallery/gallery_" + this.ID; } }

        [Display(Name = "Author", Order = 104, Prompt = "TabContent")]
        public override string Author { get; set; }


        [UIHint("Image")]
        [Display(Name = "Pic", Order = 109, Prompt = "TabContent")]
        [Required]
        public override string Pic { get; set; }

        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "g"; } set { } }
    }

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage", "NewsListPage" })]
    [RouteUrl("nwl")]
    [PageIcon("~/Areas/Admin/Content/pages/route_arl.png")]
    [PageName("NewsListPage")]
    public class NewsListPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "nwl"; } set { } }
    }

    [RestrictParents(new string[] { "NewsListPage" })]
    [RouteUrl("n")]
    [PageIcon("~/Areas/Admin/Content/pages/route_n.png")]
    [PageName("NewsPage")]
    public class NewsPage : AbstractPage
    {
        [Display(Name = "Pic", Order = 115, Prompt = "TabContent")]
        [UIHint("Image")]
        public override string Pic { get; set; }
        [Display(Name = "ShortDescription", Order = 105, Prompt = "TabContent")]
        [Column("ShortDescription", TypeName = "nvarchar(MAX)")]
        [DataType(DataType.MultilineText)]
        public override string ShortDescription { get; set; }



        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "n"; } set { } }

        public NewsPage()
        {
            //Pic = "/Content/DesignFiles/default.png";
        }
    }

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage" })]
    [RouteUrl("r")]
    [PageIcon("~/Areas/Admin/Content/pages/route_r.png")]
    [PageName("RedirectPage")]
    public class RedirectPage : AbstractPage
    {
        [Display(Prompt = "Hidden")]
        public override string Text { get; set; }

        [Display(Name = "RedirectTo", Order = 104, Prompt = "Content")]
        [Required()]
        public override string RedirectTo { get; set; }

        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "r"; } set { } }
    }

    [RestrictParents(new string[] { "DomainPage", "LanguagePage", "RedirectPage", "ContentPage" })]
    [RouteUrl("c")]
    [PageIcon("~/Areas/Admin/Content/pages/route_arl.png")]
    [PageName("ContentPage")]
    public class ContentPage : AbstractPage
    {
        [HiddenInput(DisplayValue = false)]
        public override string RouteUrl { get { return "c"; } set { } }


        [Display(Name = "Pictures", Order = 400, Prompt = "TabData")]
        [UIHint("Gallery")]
        public string Gallery { get { return "Gallery/gallery_" + this.ID; } }
    }

}

