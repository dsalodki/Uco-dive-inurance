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
    
    public partial class Contacts
    {
        public int ID { get; set; }
        public int DomainID { get; set; }
        public System.DateTime ContactDate { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactData { get; set; }
        public string ContactUrl { get; set; }
        public string ContactReferal { get; set; }
        public string RoleDefault { get; set; }
        public string Rool { get; set; }
    }
}