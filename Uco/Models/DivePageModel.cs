using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Uco.Models
{
    public partial class Settings
    {
        [Display(Name = "TypesOfCertificate0", Prompt = "דף צולל")]
        public string TypesOfCertificate0 { get; set; }
        [Display(Name = "TypesOfCertificate1", Prompt = "דף צולל")]
        public string TypesOfCertificate1 { get; set; }
        [Display(Name = "TypesOfInsurance", Prompt = "דף צולל")]
        public string TypesOfInsurance { get; set; }
        [Display(Name = "InsuranceOrganizations", Prompt = "דף צולל")]
        public string InsuranceOrganizations { get; set; }
    }

    public class Certificate
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int ID { get; set; }

        [Display(Name = "UserID")]
        public Guid UserID { get; set; }
        [Display(Name = "CertificateImageBack")]
        [UIHint("Image")]
        public string CertificateImageBack { get; set; }
        [Display(Name = "CertificateImageFront")]
        [UIHint("Image")]
        public string CertificateImageFront { get; set; }
        [Display(Name = "TypeOfCertificate")]
        public string TypeOfCertificate { get; set; }
        [Display(Name = "CreateDate")]
        public DateTime CreateDate { get; set; }
    }

    public class Insurance
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int ID { get; set; }

        [Display(Name = "UserID")]
        [ForeignKey("User")]
        [Index(IsUnique = true)]
        public Guid UserID { get; set; }
        public User User { get; set; }
        [Display(Name = "ExternalInsurance")]
        public bool ExternalInsurance { get; set; }
        [Display(Name = "Organization")]
        public string Organization { get; set; }
        [Display(Name = "TypeOfInsurance")]
        public string TypeOfInsurance { get; set; }
        [Display(Name = "CreateDate")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "InsuranceEndDate")]
        public DateTime InsuranceEndDate { get; set; }
        [Display(Name = "InsuranceStartDate")]
        public DateTime InsuranceStartDate { get; set; }
        [Display(Name = "FileType")]
        public FileType FileType { get; set; }
        [Display(Name = "FilePath")]
        [UIHint("Image")]
        public string FilePath { get; set; }

        public bool SentNotificationExpired { get; set; }
        [NotMapped]
        public int OrderId { get; set; }
    }

    public enum FileType
    {
        None = 0,
        Image = 1,
        Pdf = 2
    }

    public class Dive
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int ID { get; set; }

        [Display(Name = "UserID")]
        public Guid UserID { get; set; }
        [Display(Name = "Organization")]
        public string Organization { get; set; }
        [Display(Name = "DivePlace")]
        public string DivePlace { get; set; }
        [Display(Name = "CreateDate")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "DateOfDive")]
        public DateTime DateOfDive { get; set; }

        [NotMapped]
        public DateTime DiveDate { get; set; }
        [NotMapped]
        public TimeSpan DiveTime { get; set; }

        [Display(Name = "IsRefreshingDive")]
        public bool IsRefreshingDive { get; set; }
        [Display(Name = "DivingSite")]
        public string DivingSite { get; set; }
        [Display(Name = "AirTank")]
        public AirTank AirTank { get; set; }
        [Display(Name = "AirDiveOrNitrox")]
        public AirDiveOrNitrox AirDiveOrNitrox { get; set; }
        [Display(Name = "MaximumDivingDepthMeter")]
        public int MaximumDivingDepthMeter { get; set; }
        [Display(Name = "DiveTimeMinutes")]
        public int DiveTimeMinutes { get; set; }
        [Display(Name = "DivingPartner")]
        public string DivingPartner { get; set; }
        [Display(Name = "ImpressionsFromDive")]
        public string ImpressionsFromDive { get; set; }
        [Display(Name = "DiveImagesGallery")]
        public int DiveImagesGallery { get; set; }
        [Display(Name = "DiveVideoLink")]
        public string DiveVideoLink { get; set; }
        [Display(Name = "Signature")]
        [UIHint("Image")]
        public string Signature { get; set; }
        [NotMapped]
        public string SignatureData { get; set; }
        [Display(Name = "AcceptTerms")]
        public bool AcceptTerms { get; set; }
        [Display(Name = "ValidateType")]
        public ValidateType ValidateType { get; set; }
        [Display(Name = "ValidateImage")]
        public string ValidateImage { get; set; }
    }

    public enum AirTank
    {
        None = 0,
        Ten = 1,
        Fifteen = 2,
        Twenty = 3
    }

    public enum ValidateType
    {
        None = 0,
        Receipt = 1,
        Selfie = 2,
        ExternalPage = 3
    }

    public enum AirDiveOrNitrox
    {
        None = 0,
        Air = 1,
        Nitrox = 2
    }
}