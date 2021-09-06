using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Serialization;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;

namespace Uco.Models
{
    public partial class Settings
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Display(Name = "AdminEmail", Order = 50, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabSettings")]
        [Required(ErrorMessageResourceName = "AdminEmailRequired", ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string AdminEmail { get; set; }

        [Display(Name = "HeaderHtml", Order = 60, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabSettings")]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        [AllowHtml]
        public string HeaderHtml { get; set; }
        [Display(Name = "FotterHtml", Order = 70, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabSettings")]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        [AllowHtml]
        public string FotterHtml { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainPageID { get; set; }

        [Display(Name = "LanguageRTL", Order = 100, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabDomain")]
        public bool LanguageRTL { get; set; }

        [Display(Name = "LanguageCode", Order = 100, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabDomain")]
        [UIHint("Languages")]
        public string LanguageCode { get; set; }

        [Display(Name = "Domain", Order = 100, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabDomain")]
        [Required]
        public virtual string Domain { get; set; }

        [Display(Name = "Themes", Order = 100, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabDomain")]
        [UIHint("Themes")]
        public virtual string Themes { get; set; }

        [Display(Name = "Error404", Prompt = "Error404")]
        [AllowHtml()]
        [DataType(DataType.MultilineText)]
        public string Error404 { get; set; }

        [Display(Name = "MenuGroups", Prompt = "MenuGroups")]
        public string MenuGroups { get; set; }


        [Display(Name = "Roles", Order = 300, ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabRoles")]
        [UIHint("RolesDomain")]
        public string Roles { get; set; }

        [NotMapped]
        public List<string> RolesList
        {
            get
            {
                return SF.RolesStringToList(this.Roles);
            }
        }
    }

    public class TextComponent
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Display(Name = "SystemName", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required()]
        public string SystemName { get; set; }

        [Display(Name = "DisplayName", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string DisplayName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "LangCode", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required()]
        [UIHint("Languages")]
        public string LangCode { get; set; }


        [Display(Name = "Visible", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [DefaultValue(true)]
        public bool Visible { get; set; }

        [Display(Name = "Text", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("Html")]
        [AllowHtml]
        public string Text { get; set; }

        public TextComponent()
        {
            if (RP.GetAdminCurrentSettingsRepository() != null)
            {
                this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
            }
        }
    }

    public class OutEmail
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "MailTo", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string MailTo { get; set; }
        [Display(Name = "Subject", Order = 30, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Subject { get; set; }
        [Display(Name = "Body", Order = 40, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("Html")]
        [AllowHtml]
        public string Body { get; set; }
        [Display(Name = "LastTry", Order = 50, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Column(TypeName = "datetime2")]
        public DateTime LastTry { get; set; }
        [Display(Name = "TimesSent", Order = 60, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public int TimesSent { get; set; }
        public List<string> NewsletterData { get; set; }
        public List<string> NewsletterAccountGroups { get; set; }
        public List<string> NewsletterAccountGroupsSelected { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public OutEmail()
        {
            if (RP.GetAdminCurrentSettingsRepository() == null) this.DomainID = 0;
            else this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
    }

    public class Error
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "Date", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }
        [Display(Name = "Message", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Message { get; set; }
        [Display(Name = "Host", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Host { get; set; }
        [Display(Name = "UserName", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string UserName { get; set; }
        [Display(Name = "PhysicalPath", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string PhysicalPath { get; set; }
        [Display(Name = "UserAgent", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string UserAgent { get; set; }
        [Display(Name = "UserHostAddress", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string UserHostAddress { get; set; }
        [Display(Name = "Url", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Url { get; set; }
        [Display(Name = "UrlReferrer", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string UrlReferrer { get; set; }
        [Display(Name = "InnerException", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string InnerException { get; set; }
        [Display(Name = "Source", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Source { get; set; }
        [Display(Name = "StackTrace", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string StackTrace { get; set; }
        [Display(Name = "TargetSite", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string TargetSite { get; set; }

        public Error()
        {
            this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
    }

    public class Contact
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "Date", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public DateTime ContactDate { get; set; }

        [Display(Name = "Name", Order = 20, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string ContactName { get; set; }

        [Display(Name = "Email", Order = 30, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string ContactEmail { get; set; }

        [NotMapped]
        [Display(Name = "קישור לאימייל", Order = 30)]
        [MaxLength(200)]
        [AllowHtml]
        [UIHint("Html")]
        public string EmailLink
        {

            get
            {

                if (string.IsNullOrEmpty(this.ContactEmail))
                { return ""; }
                else
                {
                    return "<a href=\"mailto:" + this.ContactEmail + "\">" + this.ContactEmail + "</a>";
                }


            }

        }

        [Display(Name = "Phone", Order = 40, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string ContactPhone { get; set; }

        [Display(Name = "Content", Order = 50, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("Html")]
        [AllowHtml]
        public string ContactData { get; set; }

        [Display(Name = "Url", Order = 60, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [AllowHtml]
        [UIHint("Html")]
        public string ContactUrl { get; set; }

        [Display(Name = "Referal", Order = 70, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string ContactReferal { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string RoleDefault { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Rool { get; set; }

        public Contact()
        {
            this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
    }

   
    public class Newsletter
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int DomainID { get; set; }

        [Display(Name = "NewsletterDate", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "NewsletterDateRequired")]
        [Column(TypeName = "datetime2")]
        public DateTime NewsletterDate { get; set; }
        [Display(Name = "NewsletterName", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "NewsletterNameRequired")]
        public string NewsletterName { get; set; }
        [Display(Name = "NewsletterEmail", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "NewsletterDataRequired")]
        [UcoEmail(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "NewsletterMailNotValid")]
        public string NewsletterEmail { get; set; }
        [Display(Name = "NewsletterData", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string NewsletterData { get; set; }

        [Display(Name = "תעודת זהות", Order = 11)]
        public string NewsletterIdNumber { get; set; }

        [Display(Name = "NewsletterAccept", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Mandatory(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "NewsletterAcceptMandatory")]
        public bool NewsletterAccept { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string RoleDefault { get; set; }

        public Newsletter()
        {
            this.DomainID = RP.GetAdminCurrentSettingsRepository().ID;
        }
        public Newsletter(int domainID)
        {
            this.DomainID = domainID;
        }
    }

    public class User
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public Guid ID { get; set; }

        [Display(Name = "CreationDate", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Column(TypeName = "datetime2")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "UserName", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        //[Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "UserNameRequired")]
        [MinLength(3)]
        public string UserName { get; set; }

        [Display(Name = "FirstName", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [DataType(DataType.ImageUrl)]
        public string LastName { get; set; }

        [Display(Name = "ApplicationName", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        public string ApplicationName { get; set; }

        [Display(Name = "Email", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "EmailRequired")]
        [UcoEmail()]
        public string Email { get; set; }

        [Display(Name = "Password", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PasswordRequired")]
        [MinLength(4)]
        public string Password { get; set; }

        [Display(Name = "PasswordQuestion", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        public string PasswordQuestion { get; set; }

        [Display(Name = "PasswordAnswer", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        public string PasswordAnswer { get; set; }

        [Display(Name = "IsApproved", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public bool IsApproved { get; set; }

        [Display(Name = "LastActivityDate", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        [Column(TypeName = "datetime2")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "LastLoginDate", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        [Column(TypeName = "datetime2")]
        public DateTime LastLoginDate { get; set; }

        [Display(Name = "LastPasswordChangedDate", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        [Column(TypeName = "datetime2")]
        public DateTime LastPasswordChangedDate { get; set; }

        [Display(Name = "IsOnline", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        public bool IsOnline { get; set; }

        [Display(Name = "IsLockedOut", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public bool IsLockedOut { get; set; }

        [Display(Name = "LastLockedOutDate", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        [Column(TypeName = "datetime2")]
        public DateTime LastLockedOutDate { get; set; }

        [Display(Name = "FailedPasswordAttemptCount", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        public int FailedPasswordAttemptCount { get; set; }

        [Display(Name = "FailedPasswordAttemptWindowStart", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        [Column(TypeName = "datetime2")]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        [Display(Name = "FailedPasswordAnswerAttemptCount", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        [Display(Name = "FailedPasswordAnswerAttemptWindowStart", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [HiddenInput(DisplayValue = false)]
        [Column(TypeName = "datetime2")]
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        [Display(Name = "LastModified", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Column(TypeName = "datetime2")]
        public DateTime LastModified { get; set; }

        [Display(Name = "RoleDefault", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("RoleDefault")]
        public string RoleDefault { get; set; }

        [Display(Name = "Roles", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("Roles")]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "RolesRequired")]
        public string Roles { get; set; }

        [Display(Name = "Comment", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [DataType(DataType.MultilineText)]
        [MaxLength(4000)]
        public string Comment { get; set; }

        //[Required(ErrorMessage = "Phone required")]
        [Display(Name = "Phone", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Phone { get; set; }

        [Display(Name = "City", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string City { get; set; }

        [Display(Name = "SendNewsletters", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public bool SendNewsletters { get; set; }

        /// <summary>
        /// תעודת זהות identify id number
        /// </summary>
        [Display(Name = "מספר ת.ז", Prompt = "פרטי משתמש")]
        [MaxLength(200)]
        public string IdNumber { get; set; }

        [Display(Name = "UserImage")]
        public string UserImage { get; set; }
        [Display(Name = "UserBanner")]
        public string UserBanner { get; set; }
        [Display(Name = "FullNameEnglish")]
        public string FullNameEnglish { get; set; }

        [NotMapped]
        public List<string> RolesList
        {
            get
            {
                return SF.RolesStringToList(this.Roles);
            }
        }

        public bool IsInRole(string Role)
        {
            if (RolesList.Contains(Role)) return true;
            else return false;
        }
    }

    public class Role
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabData")]
        public string Title { get; set; }

        [Display(Name = "IsSystem", ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "TabData")]
        [HiddenInput(DisplayValue = false)]
        public bool IsSystem { get; set; }

        [Display(Name = "MenuPermissions", ResourceType = typeof(Uco.Models.Resources.SystemModels), Prompt = "Hidden")]
        [HiddenInput(DisplayValue = false)]
        [UIHint("MenuPermissions")]
        public string MenuPermissions { get; set; }

        [NotMapped]
        public List<string> MenuPermissionsList
        {
            get
            {
                return SF.RolesStringToList(this.MenuPermissions);
            }
        }
    }

    [NotMapped]
    public class FormField
    {
        [Display(Name = "ID", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public int FormFieldID { get; set; }

        [Display(Name = "Order", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("Integer")]
        public int FormFieldOrder { get; set; }

        [Display(Name = "Title", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "TitleRequired")]
        public string FormFieldTitle { get; set; }

        [Display(Name = "Required", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public bool FormFieldRequired { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int FormFieldTypeValue { get; set; }

        [HiddenInput(DisplayValue = false)]
        [NotMapped]
        public string FormFieldTypeText
        {
            get
            {
                return ((FormFildType)FormFieldTypeValue).ToString();
            }
        }

        [Display(Name = "Type", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("Enum")]
        [XmlIgnore]
        public FormFildType FormFieldType
        {
            get { return (FormFildType)FormFieldTypeValue; }
            set { FormFieldTypeValue = (int)value; }
        }

        [Display(Name = "RequiredTitle", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FormFieldRequiredTitle { get; set; }

        public enum FormFildType
        {
            Name,
            Text,
            EmailAddress,
            PhoneNumber,
            MultilineText,
            DropDown,
            RadioBottonList,
            CheckboxList,
            Date
            //Html,
            //Date,
            //Time,
            //DateTime,
            //Currency,
            //Integer,
            //Number,
            //Url,
            //Duration,
            //Password,
            //UploadAnonim
        }

    }

    [NotMapped]
    public class FormRool
    {
        [Display(Name = "ID", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public int FormRoolID { get; set; }

        [Display(Name = "Order", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("Integer")]
        public int FormRoolOrder { get; set; }

        [Display(Name = "Title", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "TitleRequired")]
        public string FormRoolTitle { get; set; }

        [Display(Name = "Email", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "EmailRequired")]
        public string FormRoolEmail { get; set; }

        [Display(Name = "And", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public bool FormRoolAnd { get; set; }

        [Display(Name = "Item1", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FormRoolItem1 { get; set; }

        [Display(Name = "Value1", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FormRoolValue1 { get; set; }

        [Display(Name = "Item2", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FormRoolItem2 { get; set; }

        [Display(Name = "Value2", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FormRoolValue2 { get; set; }

        [Display(Name = "Value3", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FormRoolValue3 { get; set; }

        [Display(Name = "Item3", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string FormRoolItem3 { get; set; }

        [Display(Name = "RoleDefault", Order = 10, ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [UIHint("RoleDefault")]
        public string FormRoolRole { get; set; }

        public FormRool()
        {
            this.FormRoolRole = "Admin";
        }
    }

    [NotMapped]
    public class ImageGalleryItem
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string BigImageUrl { get; set; }
        public string SmallImageUrl { get; set; }
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.BigImageUrl)) return string.Empty;
                return this.BigImageUrl.Split('/').ToList().LastOrDefault();
            }
        }

        public ImageGalleryItem()
        {
            this.Order = 100;
        }
    }
}