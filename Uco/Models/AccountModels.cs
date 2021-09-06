using System;
using System.ComponentModel.DataAnnotations;
using Uco.Infrastructure;

namespace Uco.Models
{

    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "OldPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "NewPasswordRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PasswordMinimumLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "ConfirmPasswordCompare")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "UserNameRequired")]
        [Display(Name = "UserName", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "שם פרטי חובה")]
        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "שם משפחה חובה")]
        [Display(Name = "שם משפחה")]
        public string LastName { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PasswordMinimumLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "ConfirmPasswordCompare")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "שם מוצר")]
        public string PayText { get; set; }

        [Display(Name = "מספר משתמש")]
        public Guid UserID { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PhoneRequired")]
        [UcoPhoneAttribute(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PhoneNotValid")]
        public string Phone { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "CityRequired")]
        public string City { get; set; }

        [Display(Name = "SendNewsletters")]
        public bool SendNewsletters { get; set; }
    }


    public class ProfileEditModel
    {
        [Required(ErrorMessage = "שם פרטי חובה")]
        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "שם משפחה חובה")]
        [Display(Name = "שם משפחה")]
        public string LastName { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(Uco.Models.Resources.SystemModels))]
        public string Email { get; set; }

        [Display(Name = "מספר משתמש")]
        public Guid UserID { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PhoneRequired")]
        [UcoPhoneAttribute(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "PhoneNotValid")]
        public string Phone { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessageResourceType = typeof(Uco.Models.Resources.SystemModels), ErrorMessageResourceName = "CityRequired")]
        public string City { get; set; }

        [Display(Name = "SendNewsletters")]
        public bool SendNewsletters { get; set; }
    }
}
