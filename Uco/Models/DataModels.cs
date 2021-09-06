using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web.Mvc;
using System.Xml.Serialization;
using Uco.Infrastructure;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;

namespace Uco.Models
{

    public partial class Settings
    {

        [Display(Name = "terminal Number", Prompt = "סליקה")]
        public string PaymentCreditGuardID1 { get; set; }


        [Display(Name = "Mid", Prompt = "סליקה")]
        public string PaymentCreditGuardID2 { get; set; }


        [Display(Name = "User", Prompt = "סליקה")]
        public string PaymentCreditGuardID3 { get; set; }


        [Display(Name = "Password", Prompt = "סליקה")]
        public string PaymentCreditGuardID4 { get; set; }

        [Display(Name = "Url", Prompt = "סליקה")]
        public string PaymentCreditGuardUrl { get; set; }

        [Display(Name = "שם משתמש", Prompt = "Sms")]
        public string SmsUsername { get; set; }

        [Display(Name = "סיסמא", Prompt = "Sms")]
        public string SmsPassword { get; set; }

        [Display(Name = "שם שולח לתצוגה", Prompt = "Sms")]
        public string SmsDisplaySender { get; set; }

        [Display(Name = "טקסט לפני הקישור", Prompt = "Sms")]
        public string SmsText { get; set; }

        [Display(Name = "הצג צור קשר בעמוד הבית בצד ימין", Prompt = "הגדרות")]
        [DefaultValue(true)]
        public bool ShowHomepageContactBox { get; set; }

        [Display(Name = "סיבות פניה", Prompt = "הגדרות")]
        [DataType(DataType.MultilineText)]
        public string ContactSubjects { get; set; }
    }

 
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {

            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", _resourceKey)
                    : displayName;
            }
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return enumValue.ToString();
        }
    }

  
    public enum CompetitionStatus
    {
        [Display(Name = "no status")]
        NoStatus = 0,
        [Display(Name = "רישום")]
        Registration = 1,
        [Display(Name = "הצבעה")]
        Voting = 2,
        [Display(Name = "הכרזה על הזוכה")]
        WinnerAnnouncement = 3,
        [Display(Name = "סגור")]
        Closed = 4,
    }








}