//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrmMigration.NewDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Users
    {
        public System.Guid ID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ApplicationName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public System.DateTime LastActivityDate { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public System.DateTime LastPasswordChangedDate { get; set; }
        public bool IsOnline { get; set; }
        public bool IsLockedOut { get; set; }
        public System.DateTime LastLockedOutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public System.DateTime FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public System.DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
        public System.DateTime LastModified { get; set; }
        public string RoleDefault { get; set; }
        public string Roles { get; set; }
        public string Comment { get; set; }
    }
}
